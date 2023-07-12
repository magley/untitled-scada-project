namespace USca_Server.Alarms
{
    public class AlarmAddDTO
    {
        public AlarmThresholdType ThresholdType { get; set; }
        public AlarmPriority Priority { get; set; }
        public double Threshold { get; set; }
        public int TagId { get; set; }
    }
}