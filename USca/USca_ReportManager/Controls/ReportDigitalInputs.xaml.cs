using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    /// <summary>
    /// Interaction logic for ReportDigitalInputs.xaml
    /// </summary>
    public partial class ReportDigitalInputs : UserControl
    {
        public ObservableCollection<TagLogDTO> TagLogs { get; set; } = new();
        private TagLogService _tagLogService = new();  // TODO: do a singleton?

        public ReportDigitalInputs()
        {
            InitializeComponent();
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = await _tagLogService.GetLatestDigitalInputs();
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
