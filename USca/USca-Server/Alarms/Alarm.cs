using System.ComponentModel.DataAnnotations;
using USca_Server.Tags;

namespace USca_Server.Alarms
{
    public enum AlarmThresholdType
    {
        BELOW, // Trigger alarm when value drops below threshold
        ABOVE, // Trigger alarm when value rises above threshold
    }

    public enum AlarmPriority
    {
        LOW = 1,
        MEDIUM = 2,
        HIGH = 3,
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
        public virtual Tag Tag { get; set; } = new();

        public override string ToString()
        {
            return $"Alarm[Id={Id}, ThresholdType={ThresholdType}, Priority={Priority}, Threshold={Threshold}, TagId={TagId}]";
        }
    }
}