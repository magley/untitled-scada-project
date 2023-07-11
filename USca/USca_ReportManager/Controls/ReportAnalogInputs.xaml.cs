using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    /// <summary>
    /// Interaction logic for ReportAnalogInputs.xaml
    /// </summary>
    public partial class ReportAnalogInputs : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<TagLogDTO> TagLogs { get; set; } = new();
        private TagLogService _tagLogService = new();  // TODO: do a singleton?

        public ReportAnalogInputs()
        {
            InitializeComponent();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private async void Refresh()
        {
            try
            {
                var res = await _tagLogService.GetLatestAnalogInputs();
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
