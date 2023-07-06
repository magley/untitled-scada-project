using System;
using System.ComponentModel;

namespace USca_AlarmDisplay.Alarm
{
    public enum AlarmThresholdType
    {
        BELOW, // Trigger alarm when value drops below threshold
        ABOVE, // Trigger alarm when value rises above threshold
    }

    public enum AlarmPriority
    {
        LOW,
        MEDIUM,
        HIGH,
    }

    public static class ExtensionMethods
    {
        public static string ToSign(this AlarmThresholdType type)
        {
            return type switch
            {
                AlarmThresholdType.ABOVE => ">",
                AlarmThresholdType.BELOW => "<",
                _ => throw new NotImplementedException()
            };
        }
    }

    public partial class AlarmLogDTO : INotifyPropertyChanged
    {
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

        public static string LogEntry(AlarmLogDTO log)
        {
            return $"[{log.Priority} {log.TimeStamp}] Tag {log.TagId} ({log.TagName}) recorded alarm {log.AlarmId} at address {log.Address}: {log.RecordedValue:#.####} {log.ThresholdType.ToSign()} {log.Threshold:#.####}";
        }
    }
}
