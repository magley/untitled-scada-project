using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace USca_Server.Util.Socket
{

    public class SocketWorker
    {
        private WebSocket ws { get; set; }
        public List<SocketMessageType> SupportedTypes { get; set; }
        private readonly List<INotifySocket> publishers;
        private const string clientClosedConnectionWithoutHandshakeMessage = "The remote party closed the WebSocket connection without completing the close handshake.";

        public SocketWorker(WebSocket ws, List<SocketMessageType> supportedTypes, List<INotifySocket> publishers)
        {
            this.ws = ws;
            SupportedTypes = supportedTypes;
            this.publishers = publishers;
        }

        private async void HandleSocketEvent(object? sender, SocketMessageDTO e)
        {
            if (!SupportedTypes.Contains(e.Type))
            {
                return;
            }
            var json = JsonSerializer.Serialize(e);
            await ws.SendAsync(
                new(Encoding.UTF8.GetBytes(json)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );
        }

        public async Task Start()
        {
            publishers.ForEach(pub => pub.RaiseSocketEvent += HandleSocketEvent);
            Console.WriteLine("Started socket loop");
            await WebSocketLoop();
            publishers.ForEach(pub => pub.RaiseSocketEvent -= HandleSocketEvent);
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
            while (ws.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];

                try
                {
                    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                    }
                    else
                    {
                        Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
                    }
                }
                catch (WebSocketException e)
                {
                    if (e.Message == clientClosedConnectionWithoutHandshakeMessage)
                    {
                        break;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
    }
}
