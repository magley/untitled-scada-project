using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using USca_Server.Tags;

namespace USca_Server.Alarms
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

    public class Alarm
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public AlarmThresholdType ThresholdType { get; set; }
        [Required]
        public AlarmPriority Priority { get; set; }
        [Required]
        public double Threshold { get; set; }
        [Required]
        public int TagId { get; set; }
        [JsonIgnore]
        public virtual Tag Tag { get; set; } = new();
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return $"Alarm[Id={Id}, ThresholdType={ThresholdType}, Priority={Priority}, Threshold={Threshold}, TagId={TagId}, IsActive={IsActive}]";
        }

        public bool ThresholdCrossed(double value)
        {
            return ThresholdType switch
            {
                AlarmThresholdType.BELOW => value < Threshold,
                AlarmThresholdType.ABOVE => value > Threshold,
                _ => throw new NotImplementedException()
            };
        }
    }
}