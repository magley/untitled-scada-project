using System.Net.WebSockets;

namespace USca_Server.Alarms
{
    public interface IAlarmService
    {
        public List<Alarm> GetAll();
        public List<ActiveAlarmDTO> GetActive();
        public void Add(AlarmAddDTO alarmAddDTO);
        public void Update(AlarmUpdateDTO alarmUpdateDTO);
        public void Delete(int alarmId);
        public Task StartAlarmValuesListener(WebSocket ws);
    }
}