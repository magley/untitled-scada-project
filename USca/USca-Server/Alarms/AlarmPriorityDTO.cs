namespace USca_Server.Alarms
{
    public class AlarmPriorityDTO
    {
        public AlarmPriority Priority { get; set; }

        public AlarmPriorityDTO(AlarmPriority priority)
        {
            this.Priority = priority;
        }
    }
}
