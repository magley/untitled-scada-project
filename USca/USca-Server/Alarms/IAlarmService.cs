namespace USca_Server.Alarms
{
    public interface IAlarmService
    {
        public List<Alarm> GetAll();
        public void Add(AlarmAddDTO alarmAddDTO);
        public void Update(AlarmUpdateDTO alarmUpdateDTO);
        public void Delete(int alarmId);
    }
}