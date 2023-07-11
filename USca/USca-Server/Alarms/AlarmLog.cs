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
        public bool IsActive { get; set; }
        public string? TagName { get; set; }
        public int Address { get; set; }
        public DateTime Timestamp { get; set; }
        public double RecordedValue { get; set; }
        public string? Unit { get; set; }

        public static string ActiveStatus(bool status)
        {
            return status ? "activated" : "ceased";
        }

        public static string SignStatus(AlarmThresholdType Type, bool IsActive)
        {
            if (IsActive)
            {
                return Type.ToSign();
            }
            else
            {
                // Alarm has been deactivated, therefore the type/sign to display is reversed
                return Type switch
                {
                    AlarmThresholdType.ABOVE => AlarmThresholdType.BELOW.ToSign(),
                    AlarmThresholdType.BELOW => AlarmThresholdType.ABOVE.ToSign(),
                    _ => throw new NotImplementedException()
                };
            }
        }

        public static string LogEntry(AlarmLog log)
        {
            return $"[{log.Timestamp}] ({log.Priority}) Tag {log.TagId} ({log.TagName}) {ActiveStatus(log.IsActive)} alarm {log.AlarmId} at address {log.Address}: {log.RecordedValue:0.0000} {SignStatus(log.ThresholdType, log.IsActive)} {log.Threshold:0.0000}";
        }
    }
}