namespace USca_Server.Util.Socket
{
    public interface INotifySocket
    {
        event EventHandler<SocketMessageDTO>? RaiseSocketEvent;
    }
}
