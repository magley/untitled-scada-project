using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Windows;
using USca_AlarmDisplay.Alarm;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;
using USca_WebSocketUtil;
using RestSharp.Serializers.Xml;

namespace USca_AlarmDisplay
{

    public partial class AlarmAlerts : Window, INotifyPropertyChanged
    {
        public ObservableCollection<AlarmLogDTO> AlarmLogs { get; set; } = new();
        public ObservableCollection<ActiveAlarm> ActiveAlarms { get; set; } = new();
        public ActiveAlarm? SelectedActiveAlarm { get; set; }
        private const string serverSocketEndpoint = "ws://localhost:5274/api/alarm/ws";

        public AlarmAlerts()
        {
            InitializeComponent();
            ClientWebSocketUtil util = new(
                serverSocketEndpoint,
                HandleSocketMessage,
                handleAfterSocketEstablished: async () => await LoadActiveAlarms());
            util.WebSocketLoop();
        }

        private void HandleSocketMessage(SocketMessageType type, string? message)
        {
            switch (type)
            {
                case SocketMessageType.DELETE_ALARM:
                    if (message == null)
                    {
                        throw new InvalidSocketMessageException(type, message);
                    }
                    DeleteActiveAlarm(JsonSerializer.Deserialize<int>(message));
                    break;
                case SocketMessageType.ALARM_TRIGGERED:
                    if (message == null)
                    {
                        throw new InvalidSocketMessageException(type, message);
                    }
                    LoadAlarmLog(JsonSerializer.Deserialize<AlarmLogDTO>(message));
                    break;
                default:
                    throw new UnsupportedSocketMessageTypeException(type);
            }
        }

        private void DeleteActiveAlarm(int alarmId)
        {
            var item = ActiveAlarms.FirstOrDefault(a => a.AlarmId == alarmId);
            int idx = (item != null) ? ActiveAlarms.IndexOf(item) : -1;
            if (idx != -1)
            {
                ActiveAlarms.RemoveAt(idx);
            }
        }

        private async Task LoadActiveAlarms()
        {
            var activeAlarms = await AlarmService.GetActiveAlarms();
            ActiveAlarms.Clear();
            activeAlarms.ForEach(ActiveAlarms.Add);
        }

        private void LoadAlarmLog(AlarmLogDTO? log)
        {
            if (log == null)
            {
                return;
            }
            AlarmLogs.Add(log);
            LbLogs.ScrollIntoView(LbLogs.Items[^1]);
            var item = ActiveAlarms.FirstOrDefault(t => t.AlarmId == log.AlarmId);
            int idx = (item != null) ? ActiveAlarms.IndexOf(item) : -1;
            if (log.IsActive && idx == -1)
            {
                ActiveAlarms.Add(new ActiveAlarm(log));
            }
            else if (idx != -1)
            {
                ActiveAlarms.RemoveAt(idx);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            AlarmLogs.Clear();
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateMuteDisplay();
        }

        private void UpdateMuteDisplay()
        {
            if (SelectedActiveAlarm == null)
            {
                PanelMute.Visibility = Visibility.Collapsed;
            }
            else
            {
                PanelMute.Visibility = Visibility.Visible;
                if (SelectedActiveAlarm.IsMuted)
                {
                    LblMute.IsEnabled = false;
                    TbSeconds.IsEnabled = false;
                    BtnMute.IsEnabled = false;
                    BtnUnmute.IsEnabled = true;
                }
                else
                {
                    LblMute.IsEnabled = true;
                    TbSeconds.IsEnabled = true;
                    BtnMute.IsEnabled = TbSeconds.Text != "";
                    BtnUnmute.IsEnabled = false;
                }
            }
        }

        private void TbSeconds_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void Snooze(int alarmId)
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    var alarm = ActiveAlarms.FirstOrDefault(a => a.AlarmId == alarmId);
                    if (alarm == null || !alarm.IsMuted)
                    {
                        break;
                    }
                    int idx = ActiveAlarms.IndexOf(alarm);
                    alarm.MutedFor -= 1;

                    // https://stackoverflow.com/a/18336392
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        // ActiveAlarms doesn't seem to display alarm changes, so have to do this hack
                        // FIXME: Find something better? Why doesn't ActiveAlarms[idx] = alarm work?
                        var curSelectedIdx = LbActiveAlarms.SelectedIndex;
                        UpdateActiveAlarmAt(idx, alarm, curSelectedIdx);

                    });
                }
            } catch
            {
                // When we close the GUI while the thread is still running we get an exception
                // FIXME: This is ugly.
                return;
            }
        }

        private void BtnMute_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedActiveAlarm == null)
            {
                return;
            }
            int seconds = int.Parse(TbSeconds.Text);
            SelectedActiveAlarm.MutedFor = seconds;
            int alarmId = SelectedActiveAlarm.AlarmId;
            int idx = ActiveAlarms.IndexOf(SelectedActiveAlarm);
            // We make a copy because in UpdateActiveAlarmAt SelectedActiveAlarm will be set to null
            var newAlarm = new ActiveAlarm(SelectedActiveAlarm);
            UpdateActiveAlarmAt(idx, newAlarm, idx);
            Thread thread = new(() => Snooze(alarmId));
            thread.Start();
            TbSeconds.Text = "";
            UpdateMuteDisplay();
        }

        private void BtnUnmute_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedActiveAlarm == null)
            {
                return;
            }
            SelectedActiveAlarm.MutedFor = 0;
            int idx = ActiveAlarms.IndexOf(SelectedActiveAlarm);
            var newAlarm = new ActiveAlarm(SelectedActiveAlarm);
            UpdateActiveAlarmAt(idx, newAlarm, idx);
        }

        private void UpdateActiveAlarmAt(int idx, ActiveAlarm alarm, int idxToSelect)
        {
            // FIXME: Find something better? Why doesn't ActiveAlarms[idx] = alarm work?
            ActiveAlarms.RemoveAt(idx);
            ActiveAlarms.Insert(idx, alarm);
            LbActiveAlarms.SelectedIndex = idxToSelect;
        }

        private void TbSeconds_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (SelectedActiveAlarm != null && !SelectedActiveAlarm.IsMuted)
            {
                BtnMute.IsEnabled = TbSeconds.Text != "";
            }
        }
    }
}
