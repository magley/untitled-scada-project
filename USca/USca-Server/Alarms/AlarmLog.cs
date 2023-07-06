using System.ComponentModel.DataAnnotations;

namespace USca_Server.Alarms
{
    public class AlarmLog
    {
        [Key]
        public int Id { get; set; }
        public int AlarmId { get; set; }
        public AlarmThresholdType ThresholdType { get; set; }
        public AlarmPriority Priority { get; set; }
        public double Threshold { get; set; }
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public int Address { get; set; }
        public DateTime TimeStamp { get; set; }
        public double RecordedValue { get; set; }

        public static string LogEntry(AlarmLog log)
        {
            return $"[{log.Priority} {log.TimeStamp}] Tag {log.TagId} ({log.TagName}) recorded alarm {log.AlarmId} at address {log.Address}: {log.RecordedValue:#.####} {log.ThresholdType.ToSign()} {log.Threshold:#.####}";
        }
    }
}