using System.Text.Json.Serialization;

namespace USca_Server.Util.Socket
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SocketMessageType
    {
        DELETE_TAG,
        DELETE_ALARM,
        UPDATE_TAG_READING,
        ALARM_TRIGGERED,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }
}
