namespace USca_Server.Alarms
{
    public interface IAlarmService
    {
        public List<Alarm> GetAll();
        public void Delete(int alarmId);
    }
}