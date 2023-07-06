using System.Text.Json.Serialization;

namespace USca_Server.Util
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
}
