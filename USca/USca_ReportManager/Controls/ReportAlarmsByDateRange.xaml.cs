using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    /// <summary>
    /// Interaction logic for ReportAlarmsByDateRange.xaml
    /// </summary>
    public partial class ReportAlarmsByDateRange : UserControl
    {
        public ObservableCollection<AlarmLogDTO> AlarmLogs { get; set; } = new();
        private AlarmLogService _alarmLogService = new();  // TODO: do a singleton?
        private DateTime? _startTime = null;
        private DateTime? _endTime = null;

        public ReportAlarmsByDateRange()
        {
            InitializeComponent();
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
            Calendar calendar = (Calendar)sender;
            try
            {
                _startTime = calendar.SelectedDates[0];
                _endTime = calendar.SelectedDates.Last();
                if (_endTime.HasValue)
                {
                    _endTime.Value.AddDays(1);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            BtnSearch.IsEnabled = false;
            if (_startTime == null || _endTime == null)
            {
                MessageBox.Show("Must select a date range!", "Failure", MessageBoxButton.OK);
                return;
            }
            if (_startTime >= _endTime)
            {
                MessageBox.Show("Start must come before end!", "Failure", MessageBoxButton.OK);
                return;
            }
            try
            {
                var res = await _alarmLogService.GetByDateRange((DateTime) _startTime, (DateTime) _endTime);
                AlarmLogs.Clear();
                foreach (var o in res.Logs.OrderByDescending(log => log.Timestamp).OrderByDescending(log => log.Priority))
                {
                    AlarmLogs.Add(o);
                }
            }
            catch (NotFoundException)
            {
                AlarmLogs.Clear();
                MessageBox.Show("Alarms not found!", "Failure", MessageBoxButton.OK);
            }
            BtnSearch.IsEnabled = true;
        }
    }
}
