using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using USca_AlarmDisplay.Alarm;
using USca_AlarmDisplay.Util;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;

namespace USca_AlarmDisplay
{

    public partial class AlarmAlerts : Window, INotifyPropertyChanged
    {
        public ObservableCollection<AlarmLogDTO> AlarmLogs { get; set; } = new();
        public ObservableCollection<ActiveAlarm> ActiveAlarms { get; set; } = new();
        public ActiveAlarm? SelectedActiveAlarm { get; set; }
        private readonly ClientWebSocket ws = new();

        public AlarmAlerts()
        {
            InitializeComponent();
            OpenWebSocketAndInitializeActiveAlarms();
        }

        ~AlarmAlerts()
        {
            ws.Dispose();
        }

        private async void OpenWebSocketAndInitializeActiveAlarms()
        {
            await ws.ConnectAsync(new Uri("ws://localhost:5274/api/alarm/ws"), CancellationToken.None);
            WebSocketLoop();
            var activeAlarms = await AlarmService.GetActiveAlarms();
            ActiveAlarms.Clear();
            activeAlarms.ForEach(ActiveAlarms.Add);
        }


        private async void WebSocketLoop()
        {
            var buffer = new byte[1024 * 4];

            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                }
                else
                {
                    // Get data...
                    var dtoJson = Encoding.ASCII.GetString(buffer, 0, result.Count);
                    SocketMessageDTO? socketMessage;
                    try
                    {
                        socketMessage = JsonSerializer.Deserialize<SocketMessageDTO>(dtoJson);
                    } catch (JsonException)
                    {
                        // Probably failed to deserialize SocketMessageType, which is fine
                        continue;
                    }
                    if (socketMessage == null || socketMessage?.Type == null || socketMessage?.Message == null)
                    {
                        Console.WriteLine($"Strange socket message: {dtoJson}");
                        continue;
                    }
                    switch (socketMessage.Type)
                    {
                        case SocketMessageType.ALARM_TRIGGERED:
                            LoadAlarmLog(JsonSerializer.Deserialize<AlarmLogDTO>(socketMessage.Message));
                            break;
                        default:
                            Console.WriteLine($"Unsupported message type: {socketMessage.Type}");
                            break;
                    }
                }
            }
        }

        private void LoadAlarmLog(AlarmLogDTO? log)
        {
            if (log == null)
            {
                return;
            }
            switch (log.Priority)
            {
                case AlarmPriority.LOW:
                    AlarmLogs.Add(log);
                    break;
                case AlarmPriority.MEDIUM:
                    AlarmLogs.Add(log);
                    AlarmLogs.Add(log);
                    break;
                case AlarmPriority.HIGH:
                    AlarmLogs.Add(log);
                    AlarmLogs.Add(log);
                    AlarmLogs.Add(log);
                    break;
                default:
                    throw new NotImplementedException();
            }
            LbLogs.ScrollIntoView(LbLogs.Items[^1]);
            var item = ActiveAlarms.FirstOrDefault(t => t.AlarmId == log.AlarmId);
            int idx = (item != null) ? ActiveAlarms.IndexOf(item) : -1;
            if (log.IsActive && idx == -1)
            {
                ActiveAlarms.Add(new ActiveAlarm(log));
            }
            else
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
                return $"Alarm {alarm.AlarmId} for tag {alarm.TagName}{muted(alarm)}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
