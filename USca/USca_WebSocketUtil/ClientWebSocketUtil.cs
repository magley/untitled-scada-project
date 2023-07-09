using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace USca_WebSocketUtil
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SocketMessageType
    {
        DELETE_TAG_READING,
        UPDATE_TAG_READING,
        ALARM_TRIGGERED,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }

    public class UnsupportedSocketMessageTypeException : Exception
    {
        public UnsupportedSocketMessageTypeException(SocketMessageType type)
            : base($"Unsupported message type '{type}'")
        {
        }
    }
    public class InvalidSocketMessageException : Exception
    {
        public InvalidSocketMessageException(SocketMessageType type, string? message)
            : base($"Invalid message with type '{type}' and message '{message}'")
        {
        }
    }

    public class ClientWebSocketUtil
    {
        private ClientWebSocket ws = new();
        private readonly string serverSocketEndpoint;
        private const string serverConnectionFailedMessage = "Unable to connect to the remote server";
        private const string serverClosedConnectionWithoutHandshakeMessage = "The remote party closed the WebSocket connection without completing the close handshake.";
        public Action<SocketMessageType, string?> HandleSocketMessage { get; set; }
        public Action? HandleAfterSocketEstablished { get; set; }

        public ClientWebSocketUtil(string serverSocketEndpoint, Action<SocketMessageType, string?> handleSocketMessage, Action? handleAffterSocketEstablished = null)
        {
            this.serverSocketEndpoint = serverSocketEndpoint;
            HandleSocketMessage = handleSocketMessage;
            HandleAfterSocketEstablished = handleAffterSocketEstablished;
        }

        ~ClientWebSocketUtil()
        {
            ws.Dispose();
        }

        public async void WebSocketLoop()
        {
            async Task ContinuallyTryToConnectToWebSocket()
            {
                while (true)
                {
                    try
                    {
                        await ws.ConnectAsync(new Uri(serverSocketEndpoint), CancellationToken.None);
                        return;
                    }
                    catch (WebSocketException e)
                    {
                        if (e.Message == serverConnectionFailedMessage)
                        {
                            Console.WriteLine($"Could not connect to {serverSocketEndpoint}. Trying again...");
                            // WebSocket gets disposed at this point, so we have to make a new one.
                            ws = new();
                        }
                        else
                        {
                            throw e;
                        }
                    }
                }
            }

            while (true)
            {
                Console.WriteLine("Trying to connect to web socket...");
                ws = new();
                await ContinuallyTryToConnectToWebSocket();
                HandleAfterSocketEstablished?.Invoke();
                Console.WriteLine("Successfully connected to web socket. Running loop...");
                await RunWebSocketLoop();
            }
        }

        private async Task RunWebSocketLoop()
        {
            async Task<WebSocketReceiveResult?> SafeReceieveAsync(byte[] buffer)
            {
                try
                {
                    return await ws.ReceiveAsync(buffer, CancellationToken.None);
                }
                catch (WebSocketException e)
                {
                    if (e.Message == serverClosedConnectionWithoutHandshakeMessage)
                    {
                        return null;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }

            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await SafeReceieveAsync(buffer);
                if (result == null)
                {
                    Console.WriteLine("Server (improperly) closed connection, closing web socket loop");
                    return;
                }
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
                    }
                    catch (JsonException)
                    {
                        // Probably failed to deserialize SocketMessageType, which is fine
                        continue;
                    }
                    if (socketMessage == null || socketMessage?.Type == null)
                    {
                        Console.WriteLine($"Strange socket message: {dtoJson}");
                        continue;
                    }
                    try
                    {
                        HandleSocketMessage.Invoke(socketMessage.Type, socketMessage.Message);
                    }
                    catch (InvalidSocketMessageException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    catch (UnsupportedSocketMessageTypeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}
