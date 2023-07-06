using System.Text.Json.Serialization;

namespace USca_AlarmDisplay.Util
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SocketMessageType
    {
        ALARM_TRIGGERED,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }
}
