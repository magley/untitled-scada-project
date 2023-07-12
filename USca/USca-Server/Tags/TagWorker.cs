using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using USca_Server.Alarms;
using USca_Server.Measures;
using USca_Server.Shared;
using USca_Server.TagLogs;
using USca_Server.Util;
using USca_Server.Util.Socket;

namespace USca_Server.Tags
{

    /// <summary>
    /// TagWorker encapsulates all logic for running tag threads that read measures.
    /// </summary>
    public class TagWorker : INotifySocket
    {
        private readonly Dictionary<int, TagThreadWrapper> _threads = new();
        private LoopThread? _tagSyncThread;
        private readonly object _lock = new();
        private ScadaConfig config = ScadaConfig.Instance;
        public event EventHandler<SocketMessageDTO>? RaiseSocketEvent;
        private List<Tuple<Tag, DateTime>> localLogs = new();
        private static object localLogsLock = new();
        private ITagLogService _tagLogService = new TagLogService();

        private static readonly TagWorker _instance = new();
        public static TagWorker Instance { get { return _instance; } }

        private void WriteTagLog(Tag tag, DateTime timestamp)
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

        private void OnRaiseSocketEvent(SocketMessageDTO e)
        {
            RaiseSocketEvent?.Invoke(this, e);
        }

        private TagWorker()
        {
            StartTagSync();
        }

        private void StartTagSync()
        {
            _tagSyncThread = new(new ThreadStart(TagSync));
            _tagSyncThread.Start();
        }

        /// <summary>
        /// This method syncs the tags from the database onto _threads.
        /// This method should run in the tag sync thread.
        /// </summary>
        private void TagSync()
        {
            Thread.Sleep(config.SyncThreadTimerInMs);

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
                LogHelper.GeneralLog($"[{DateTime.Now}] Removed thread for tag {tagId}.", ConsoleColor.Cyan);
            }

            // Create threads for new tags.

            foreach (var tag in currentTags)
            {
                if (!_threads.ContainsKey(tag.Id))
                {
                    _threads[tag.Id] = new(this, tag);
                    _threads[tag.Id].LoopThread.Start();
                    LogHelper.GeneralLog($"Added thread for tag {tag.Id}.", ConsoleColor.Cyan);
                }
                else
                {
                    _threads[tag.Id].Tag = tag;
                }
            }
        }
        public class TagThreadWrapper
        {
            public TagWorker TagWorker { get; set; }
            public Tag Tag { get; set; }
            public LoopThread LoopThread { get; set; }

            public TagThreadWrapper(TagWorker tagWorker, Tag tag)
            {
                TagWorker = tagWorker;
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
                    TagWorker.WriteTagLog(Tag, measure.Timestamp);
                    SendData(measure);
                    try
                    {
                        CheckAlarms(measure);
                    } catch (DbUpdateConcurrencyException)
                    {
                        // Because TagWorker doesn't have concurrency safety CheckAlarms can find itself in a situation where it's trying
                        // to update (activate/deactivate) an alarm that has previously been deleted. This is because TagWorker syncs its
                        // worker threads (by default) only once a second (potentionally having a stale representation of current state
                        // of tags and alarms), and also because database updates/deletes aren't synchronized behind locks.
                        // This is an ad-hoc fix for this particular concurrency exception.
                        // FIXME: Make TagWorker receive tag and alarm updates through events instead of through polling and use locks
                        // FIXME: Make *all* database updates/deletes concurrently safe
                    }
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
                TagWorker.OnRaiseSocketEvent(message);
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
                            AddAlarmLog(db, alarm, measure, Tag.Unit, logs);
                        }
                    } else
                    {
                        if (alarm.IsActive)
                        {
                            alarm.IsActive = false;
                            AddAlarmLog(db, alarm, measure, Tag.Unit, logs);
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
                    TagWorker.OnRaiseSocketEvent(message);
                }
                SaveAlarmLogToFile(logs);
                logs.ForEach(log => LogHelper.GeneralLog(AlarmLog.LogEntry(log), ConsoleColor.Magenta));
            }

            private void SaveAlarmLogToFile(List<AlarmLog> logs)
            {
                lock (TagWorker._lock)
                {
                    try
                    {
                        File.AppendAllLines(TagWorker.config.AlarmLogPath, logs.Select(AlarmLog.LogEntry));
                    }
                    // Probably had an invalid path in config
                    catch
                    {
                        TagWorker.config.AlarmLogPath = ScadaConfig.DefaultAlarmLogPath;
                        TagWorker.config.Save();
                        // If it fails this time, let it throw
                        File.AppendAllLines(TagWorker.config.AlarmLogPath, logs.Select(AlarmLog.LogEntry));
                    }
                }
            }

            private void AddAlarmLog(ServerDbContext db, Alarm alarm, Measure measure, string unitOfMeasurement, List<AlarmLog> logs)
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
                    Unit = unitOfMeasurement,
                };
                db.AlarmLogs.Add(log);
                logs.Add(log);
            }
        }
    }
}
