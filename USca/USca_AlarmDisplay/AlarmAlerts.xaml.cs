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

namespace USca_AlarmDisplay
{
    public partial class AlarmAlerts : Window
    {
        public ObservableCollection<AlarmLogDTO> AlarmLogs { get; set; } = new();

        public AlarmAlerts()
        {
            InitializeComponent();
            OpenWebSocket();
        }


        private async void OpenWebSocket()
        {
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://localhost:5274/api/alarm/ws"), CancellationToken.None);
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
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            AlarmLogs.Clear();
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
}
