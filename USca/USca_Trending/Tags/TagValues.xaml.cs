using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;

namespace USca_Trending.Tags
{
    public partial class TagValues : UserControl
    {
        public ObservableCollection<InputTagReadingDTO> TagReadings { get; set; } = new();

        public TagValues()
        {
            InitializeComponent();
            OpenWebSocket();
        }

        private async void OpenWebSocket()
        {
            using (var ws = new ClientWebSocket())
            {
                await ws.ConnectAsync(new Uri("ws://localhost:5274/api/tag/ws"), CancellationToken.None);
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
                        LoadTagReading(JsonSerializer.Deserialize<InputTagReadingDTO>(dtoJson));
                    }
                }
            }
        }

        private void LoadTagReading(InputTagReadingDTO? dto)
        {
            if (dto == null)
            {
                return;
            }
            var item = TagReadings.FirstOrDefault(t => t.Id == dto.Id);
            int idx =  (item != null) ? TagReadings.IndexOf(item) : -1;
            if (idx == -1)
            {
                TagReadings.Add(dto);
            } else
            {
                TagReadings[idx] = dto;
            }
        }
    }
}
