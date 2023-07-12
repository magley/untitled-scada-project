using System.ComponentModel;

namespace USca_AlarmDisplay.Alarm
{
    public partial class ActiveAlarm : INotifyPropertyChanged
    {
        public int AlarmId { get; set; }
        public int TagId { get; set; }
        public string? TagName { get; set; }
        public AlarmPriority Priority { get; set; }
        public int MutedFor { get; set; } = 0;
        public bool IsMuted { get { return MutedFor > 0; } }

        public ActiveAlarm()
        {

        }

        public ActiveAlarm(ActiveAlarm other)
        {
            AlarmId = other.AlarmId;
            TagId = other.TagId;
            TagName = other.TagName;
            MutedFor = other.MutedFor;
            Priority = other.Priority;
        }
        public ActiveAlarm(AlarmLogDTO log)
        {
            AlarmId = log.AlarmId;
            TagId = log.TagId;
            TagName = log.TagName;
            Priority = log.Priority;
        }
    }
}
