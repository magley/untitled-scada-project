using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    /// <summary>
    /// Interaction logic for ReportAlarmsByPriority.xaml
    /// </summary>
    public partial class ReportAlarmsByPriority : UserControl
    {
        public ObservableCollection<AlarmLogDTO> AlarmLogs { get; set; } = new();
        private AlarmLogService _alarmLogService = new();  // TODO: do a singleton?

        public ReportAlarmsByPriority()
        {
            InitializeComponent();
            PriorityCombobox.SelectedValue = AlarmPriority.HIGH;
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            BtnSearch.IsEnabled = false;
            var val = PriorityCombobox.SelectedValue;
            if (val == null)
            {
                MessageBox.Show("Please select a priority!", "Failure", MessageBoxButton.OK);
                return;
            }
            try
            {
                var res = await _alarmLogService.GetByPriority((AlarmPriority) val);
                AlarmLogs.Clear();
                foreach (var o in res.Logs.OrderByDescending(log => log.Timestamp))
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
