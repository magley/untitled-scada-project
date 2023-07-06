using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Windows;
using USca_DbManager.Tags;

namespace USca_DbManager.Alarms
{
    /// <summary>
    /// Interaction logic for AddAlarm.xaml
    /// </summary>
    public partial class AddAlarm : Window, INotifyPropertyChanged
    {
        public AlarmDTO Alarm { get; set; } = new();
        public ObservableCollection<TagDTO> Tags { get; set; } = new();
        public TagDTO? SelectedTag { get; set; }
        public bool CanSave { get { return SelectedTag != null; } }

        public AddAlarm(AlarmDTO? alarm = null)
        {
            InitializeComponent();
            if (alarm != null)
            {
                Alarm = JsonSerializer.Deserialize<AlarmDTO>(JsonSerializer.Serialize(alarm)) ?? new();
            }
            LoadTags(alarm != null);
        }

        private async void LoadTags(bool alarmLoaded)
        {
            var tags = await TagService.GetAnalogTags();
            Tags.Clear();
            tags.ForEach(Tags.Add);
            if (alarmLoaded)
            {
                SelectedTag = tags.Find(a => a.Id == Alarm.TagId) ?? null;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void SelectedTag_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Alarm != null && SelectedTag != null)
            {
                Alarm.TagId = SelectedTag.Id;
            }
        }
    }
}
