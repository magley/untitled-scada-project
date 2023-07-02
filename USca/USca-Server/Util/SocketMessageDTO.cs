namespace USca_Server.Util
{
    public enum SocketMessageType
    {
        DELETE_TAG_READING,
        UPDATE_TAG_READING,
    }

    public class SocketMessageDTO
    {
        public SocketMessageType Type { get; set; }
        public string? Message { get; set; }
    }
}
