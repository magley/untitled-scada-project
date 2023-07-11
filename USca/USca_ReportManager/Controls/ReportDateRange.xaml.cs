using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    /// <summary>
    /// Interaction logic for ReportDateRange.xaml
    /// </summary>
    public partial class ReportDateRange : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<TagLogDTO> TagLogs { get; set; } = new();
        private TagLogService _tagLogService = new();  // TODO: do a singleton?
        private DateTime? _startTime = null;
        private DateTime? _endTime = null;

        public ReportDateRange()
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
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
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
                var res = await _tagLogService.GetAllByDateRange((DateTime) _startTime, (DateTime) _endTime);
                TagLogs.Clear();
                foreach (var o in res.Logs.OrderByDescending(log => log.Timestamp))
                {
                    TagLogs.Add(o);
                }
            }
            catch (NotFoundException)
            {
                TagLogs.Clear();
                MessageBox.Show("Tag not found!", "Failure", MessageBoxButton.OK);
            }
        }
    }
}
