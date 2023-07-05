using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using USca_DbManager.Util;

namespace USca_DbManager.Alarms
{
    /// <summary>
    /// Interaction logic for AlarmsDashboard.xaml
    /// </summary>
    public partial class AlarmsDashboard : MyPage
    {
        public ObservableCollection<AlarmDTO> Alarms { get; set; } = new();
        public AlarmDTO? SelectedAlarm { get; set; }

        public AlarmsDashboard()
        {
            InitializeComponent();
            LoadAlarms();
        }

        private async void LoadAlarms()
        {
            var alarms = await AlarmService.GetAllAlarms();
            Alarms.Clear();
            foreach (var alarm in alarms)
            {
                Alarms.Add(alarm);
            }
        }

        private async void BtnAddAlarm_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddAlarm
            {
                Owner = Owner
            };
            if (dialog.ShowDialog() == true)
            {
                await AlarmService.AddAlarm(dialog.Alarm);
                LoadAlarms();
            }
        }

        private async void BtnDeleteAlarm_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAlarm == null)
            {
                return;
            }

            var dialog = MessageBox.Show($"Are you sure you want to delete the alarm?", "Delete", MessageBoxButton.YesNo);
            if (dialog == MessageBoxResult.Yes)
            {
                await AlarmService.DeleteAlarm(SelectedAlarm.Id);
                LoadAlarms();
            }
        }

        private async void TbAlarms_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedAlarm == null)
            {
                return;
            }

            var dialog = new AddAlarm(SelectedAlarm)
            {
                Owner = Owner
            };
            if (dialog.ShowDialog() == true)
            {
                await AlarmService.UpdateAlarm(dialog.Alarm);
                LoadAlarms();
            }
        }
    }
}
