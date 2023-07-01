using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System;
using System.Windows.Controls;

namespace USca_Trending.Tags
{
    public partial class TagValues : UserControl
    {
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
                        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
                    }
                }
            }
        }
    }
}
