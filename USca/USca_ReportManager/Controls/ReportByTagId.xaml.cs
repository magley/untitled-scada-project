using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using USca_ReportManager.Util;

namespace USca_ReportManager.Controls
{
    public partial class ReportByTagId : UserControl
    {
        public ObservableCollection<TagLogDTO> TagLogs { get; set; } = new();
        private TagLogService _tagLogService = new();

        public ReportByTagId()
        {
            InitializeComponent();
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = TxtTagID.Text;
            if (!int.TryParse(searchQuery,  out int id))
            {
                MessageBox.Show("Please enter an integer!", "Failure", MessageBoxButton.OK);
                return;
            }

            var li = await _tagLogService.GetByTag(id);

            TagLogs.Clear();
            foreach (var o in li)
            {
                TagLogs.Add(o);
            }
        }
    }
}
