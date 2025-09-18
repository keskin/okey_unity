namespace Networking
{
    public interface IWebSocketService
    {
        IWebSocketConnection GetOrCreateConnection(string connectionId);
        void CloseConnection(string connectionId);
    }
}
