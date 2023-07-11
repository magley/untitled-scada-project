using System;
using System.Text.Json.Serialization;

namespace USca_ReportManager.Util
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AlarmThresholdType
    {
        BELOW, // Trigger alarm when value drops below threshold
        ABOVE, // Trigger alarm when value rises above threshold
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AlarmPriority
    {
        LOW,
        MEDIUM,
        HIGH,
    }

    public class AlarmLogDTO
    {
        public int Id { get; set; }
        public int AlarmId { get; set; }
        public AlarmThresholdType ThresholdType { get; set; }
        public AlarmPriority Priority { get; set; }
        public double Threshold { get; set; }
        public bool IsActive { get; set; }
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public int Address { get; set; }
        public DateTime Timestamp { get; set; }
        public double RecordedValue { get; set; }
        public string Unit { get; set; } = "";
    }
}
