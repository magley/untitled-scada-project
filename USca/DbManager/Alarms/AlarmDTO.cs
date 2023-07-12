using System.ComponentModel;
using System.Text.Json.Serialization;

namespace USca_DbManager.Alarms
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

    public partial class AlarmDTO : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public AlarmThresholdType ThresholdType { get; set; }
        public AlarmPriority Priority { get; set; }
        public double Threshold { get; set; }
        public int TagId { get; set; }

        public override string ToString()
        {
            return $"Alarm[Id={Id}, ThresholdType={ThresholdType}, Priority={Priority}, Threshold={Threshold}, TagId={TagId}]";
        }
    }
}
