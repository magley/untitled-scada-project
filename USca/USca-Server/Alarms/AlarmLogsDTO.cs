namespace USca_Server.Alarms
{
    public class AlarmLogsDTO
    {
        public List<AlarmLog> Logs { get; set; }

        public AlarmLogsDTO(List<AlarmLog> logs)
        {
            this.Logs = logs;
        }
    }
}
