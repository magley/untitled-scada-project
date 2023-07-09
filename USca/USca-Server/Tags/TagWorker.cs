using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using USca_Server.Alarms;
using USca_Server.Measures;
using USca_Server.Shared;
using USca_Server.TagLogs;
using USca_Server.Util;

namespace USca_Server.Tags
{

    /// <summary>
    /// TagWorker encapsulates all logic for running tag threads that read measures.
    /// </summary>
    public static class TagWorker
    {
        private static readonly Dictionary<int, TagThreadWrapper> _threads = new();
        private static LoopThread? _tagSyncThread;
        private static readonly object _lock = new();
        private const string alarmLogPath = "./alarmLog.txt";
        public static event EventHandler<SocketMessageDTO>? RaiseWorkerEvent;
        private static List<Tuple<Tag, DateTime>> localLogs = new();
        private static object localLogsLock = new();
        private static ITagLogService _tagLogService = new TagLogService();

        private static void WriteTagLog(Tag tag, DateTime timestamp)
        {
            lock (localLogsLock)
            {
                localLogs.Add(new(tag, timestamp));

                if (localLogs.Count > 50)
                {
                    _tagLogService.AddBatch(localLogs);
                    localLogs.Clear();
                }
            }
        }

        private static void OnRaiseWorkerEvent(SocketMessageDTO e)
        {
            RaiseWorkerEvent?.Invoke(null, e);
        }

        static TagWorker()
        {
            StartTagSync();
        }

        private static void StartTagSync()
        {
            _tagSyncThread = new(new ThreadStart(TagSync));
            _tagSyncThread.Start();
        }

        /// <summary>
        /// This method syncs the tags from the database onto _threads.
        /// This method should run in the tag sync thread.
        /// </summary>
        private static void TagSync()
        {
            Thread.Sleep(1000);

            using var db = new ServerDbContext();
            var currentTags = db.Tags.Include(t => t.Alarms).ToList()
                .Where(t => t.Mode == TagMode.Input)
                .ToList();
            var staleTagIds = _threads.Keys.ToList().Except(currentTags.Select(t => t.Id));

            // Remove threads for tags that don't exist anymore.

            foreach (var tagId in staleTagIds)
            {
                _threads[tagId].LoopThread.Abort();
                _threads.Remove(tagId);
                SocketMessageDTO message = new()
                {
                    Type = SocketMessageType.DELETE_TAG_READING,
                    Message = JsonSerializer.Serialize(tagId),
                };
                OnRaiseWorkerEvent(message);
                LogHelper.ServiceLog($"Removed thread for tag {tagId}.");
            }

            // Create threads for new tags.

            foreach (var tag in currentTags)
            {
                if (!_threads.ContainsKey(tag.Id))
                {
                    _threads[tag.Id] = new(tag);
                    _threads[tag.Id].LoopThread.Start();
                    LogHelper.ServiceLog($"Added thread for tag {tag.Id}.");
                }
                else
                {
                    _threads[tag.Id].Tag = tag;
                }
            }
        }
        public class TagThreadWrapper
        {
            public Tag Tag { get; set; }
            public LoopThread LoopThread { get; set; }

            public TagThreadWrapper(Tag tag)
            {
                Tag = tag;
                LoopThread = new(() => UpdateTag());
            }

            ~TagThreadWrapper()
            {
                LoopThread.Abort();
            }

            /// <summary>
            /// This method tries to send current data from the tag <c>t</c> across the WebSocket,
            /// and also checks if any of <c>t</c>'s alarms have triggered.
            /// This method should run in a <c>LoopThread</c> assigned to the given tag.
            /// </summary>
            private void UpdateTag()
            {
                Thread.Sleep(Tag.ScanTime);
                // make sure the wrapper's IsRunning hasn't become false since we put the underlying thread to sleep
                if (!LoopThread.IsRunning)
                {
                    return;
                }

                using var db = new ServerDbContext();
                var measure = db.Measures.Find(Tag.Address);
                if (measure == null)
                {
                    // The tag points to a measure that doesn't exist.
                    return;
                }
                Tag.Value = measure.Value;
                LogHelper.GeneralLog(TagLog.LogEntry(Tag, measure.Timestamp), ConsoleColor.Blue);
                if (Tag.IsScanning)
                {
                    WriteTagLog(Tag, measure.Timestamp);
                    SendData(measure);
                    CheckAlarms(measure);
                }
            }

            private void SendData(Measure measure)
            {
                var tagReading = new
                {
                    Tag.Id,
                    Tag.Name,
                    Tag.Address,
                    Tag.ScanTime,
                    Tag.Type,
                    Tag.Min,
                    Tag.Max,
                    Tag.Unit,
                    Tag.Value,
                    measure.Timestamp,
                };
                SocketMessageDTO message = new()
                {
                    Type = SocketMessageType.UPDATE_TAG_READING,
                    Message = JsonSerializer.Serialize(tagReading),
                };
                OnRaiseWorkerEvent(message);
            }

            private void CheckAlarms(Measure measure)
            {
                using var db = new ServerDbContext();
                List<AlarmLog> logs = new();
                foreach (var alarm in Tag.Alarms)
                {
                    if (alarm.ThresholdCrossed(Tag.Value))
                    {
                        if (!alarm.IsActive)
                        {
                            alarm.IsActive = true;
                            AddAlarmLog(db, alarm, measure, logs);
                        }
                    } else
                    {
                        if (alarm.IsActive)
                        {
                            alarm.IsActive = false;
                            AddAlarmLog(db, alarm, measure, logs);
                        }
                    }
                }
                db.SaveChanges();
                foreach (var log in logs)
                {
                    SocketMessageDTO message = new()
                    {
                        Type = SocketMessageType.ALARM_TRIGGERED,
                        Message = JsonSerializer.Serialize(log),
                    };
                    OnRaiseWorkerEvent(message);
                }
                lock (_lock)
                {
                    File.AppendAllLines(alarmLogPath, logs.Select(AlarmLog.LogEntry));
                }
                logs.ForEach(log => LogHelper.GeneralLog(AlarmLog.LogEntry(log), ConsoleColor.Magenta));
            }

            private void AddAlarmLog(ServerDbContext db, Alarm alarm, Measure measure, List<AlarmLog> logs)
            {
                db.Alarms.Update(alarm);
                AlarmLog log = new()
                {
                    AlarmId = alarm.Id,
                    ThresholdType = alarm.ThresholdType,
                    Priority = alarm.Priority,
                    Threshold = alarm.Threshold,
                    IsActive = alarm.IsActive,
                    TagId = Tag.Id,
                    TagName = Tag.Name,
                    Address = Tag.Address,
                    RecordedValue = Tag.Value,
                    Timestamp = measure.Timestamp,
                };
                db.AlarmLogs.Add(log);
                logs.Add(log);
            }
        }
    }
}
