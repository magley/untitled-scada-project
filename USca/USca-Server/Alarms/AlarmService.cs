using Microsoft.EntityFrameworkCore;
using USca_Server.Shared;

namespace USca_Server.Alarms
{
    public class AlarmService : IAlarmService
    {

        public List<Alarm> GetAll()
        {
            using var db = new ServerDbContext();
            return db.Alarms.Include(a => a.Tag).ToList();
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
    }
}