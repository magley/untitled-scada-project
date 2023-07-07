using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using USca_Server.Tags;
using USca_Server.Util;

namespace USca_Server.Shared
{

    public class SocketWorker
    {
        public WebSocket Ws { get; set; }
        public List<SocketMessageType> SupportedTypes { get; set; }
        public SocketWorker(WebSocket ws, List<SocketMessageType> supportedTypes)
        {
            Ws = ws;
            SupportedTypes = supportedTypes;
        }

        public async void HandleTagWorkerEvent(object? sender, SocketMessageDTO e)
        {
            if (!SupportedTypes.Contains(e.Type))
            {
                return;
            }
            var json = JsonSerializer.Serialize(e);
            await Ws.SendAsync(
                new(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        public async Task Start()
        {
            TagWorker.RaiseWorkerEvent += HandleTagWorkerEvent;
            Console.WriteLine("Started socket loop");
            await WebSocketLoop();
            TagWorker.RaiseWorkerEvent -= HandleTagWorkerEvent;
            Console.WriteLine("Stopped socket loop (connection closed)");
        }

        /// <summary>
        /// This method runs the main WebSocket code for this Worker.
        /// <br/>
        /// It waits for a message from the other side, and if it receives a <c>WebSocketMessageType.Close</c>, it closes the WebSocket.
        /// <br/>
        /// This method returning implies that the WebSocket connection is closed.
        /// </summary>
        private async Task WebSocketLoop()
        {
            while (Ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];

                try
                {
                    var result = await Ws.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await Ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                    else
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
                    }
                }
                catch (WebSocketException)
                {
                    break;
                }
            }
        }
    }
}