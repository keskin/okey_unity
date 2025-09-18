using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Networking
{
    public interface IWebSocketConnection : IDisposable
    {
        event Action OnConnected;
        event Action<string> OnMessageReceived;
        event Action<WebSocketCloseStatus> OnDisconnected;

        ConnectionStatus Status { get; }

        Task<bool> ConnectAsync(string serverAddress, string connectionParams);
        Task DisconnectAsync();
        Task SendMessageAsync(string message);
        
        void ProcessPendingMessages();
    }
}