using System;
using System.Globalization;
using System.Windows.Data;
using USca_AlarmDisplay.Alarm;

namespace USca_AlarmDisplay.Util
{
    public class AlarmLogDTOToCustomStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AlarmLogDTO log)
            {
                return AlarmLogDTO.LogEntry(log);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ActiveAlarmToCustomStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            static string muted(ActiveAlarm alarm)
            {
                string secondsPlurality = alarm.MutedFor > 1 ? "seconds" : "second";
                return alarm.IsMuted ? $" (Muted for {alarm.MutedFor} {secondsPlurality})" : "";
            }
            if (value is ActiveAlarm alarm)
            {
                return $"[{alarm.Priority}] Alarm {alarm.AlarmId} for {alarm.TagName}{muted(alarm)}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class AlarmSeverityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AlarmPriority priority = (AlarmPriority)parameter;
            if (value is ActiveAlarm alarm)
            {
                if (alarm.IsMuted)
                {
                    return false;
                }
                return priority == alarm.Priority;
            }
            else if (value is AlarmLogDTO log)
            {
                if (!log.IsActive)
                {
                    return false;
                }
                return priority == log.Priority;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class AlarmActiveToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AlarmLogDTO log)
            {
                return log.IsActive;
            }
            else if (value is ActiveAlarm alarm)
            {
                return alarm.IsMuted;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
