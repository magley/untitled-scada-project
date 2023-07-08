using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using USca_Server.Shared;
using USca_Server.Tags;
using USca_Server.Util;

namespace USca_Server.Alarms
{
    public class AlarmService : IAlarmService
    {

        public List<Alarm> GetAll()
        {
            using var db = new ServerDbContext();
            return db.Alarms.Include(a => a.Tag).ToList();
        }

        public List<ActiveAlarmDTO> GetActive()
        {
            using var db = new ServerDbContext();
            return db.Alarms.Where(a => a.IsActive).Include(a => a.Tag).Select(a => new ActiveAlarmDTO()
            {
                AlarmId = a.Id,
                TagId = a.TagId,
                TagName = a.Tag.Name,
                Priority = a.Priority,
            }).ToList();
        }

        public void Delete(int alarmId)
        {
            using var db = new ServerDbContext();
            var alarm = db.Alarms.Find(alarmId);
            if (alarm == null)
            {
                return;
            }
            db.Alarms.Remove(alarm);
            db.SaveChanges();
        }

        public void Add(AlarmAddDTO alarmAddDTO)
        {
            using var db = new ServerDbContext();

            Alarm alarm = new()
            {
                ThresholdType = alarmAddDTO.ThresholdType,
                Priority = alarmAddDTO.Priority,
                Threshold = alarmAddDTO.Threshold,
                Tag = db.Tags.Find(alarmAddDTO.TagId) ?? new(), // FIXME: Dumb
            };
            db.Alarms.Add(alarm);
            db.SaveChanges();
        }

        public void Update(AlarmUpdateDTO alarmUpdateDTO)
        {
            using var db = new ServerDbContext();
            var alarm = db.Alarms.Find(alarmUpdateDTO.Id);
            if (alarm == null)
            {
                return;
            }
            db.Alarms.Entry(alarm).CurrentValues.SetValues(alarmUpdateDTO);
            db.SaveChanges();
        }

        public async Task StartAlarmValuesListener(WebSocket ws)
        {
            List<SocketMessageType> supportedMessageTypes = new()
            {
                SocketMessageType.ALARM_TRIGGERED,
            };
            SocketWorker listener = new(ws, supportedMessageTypes);
            await listener.Start();
        }
    }
}