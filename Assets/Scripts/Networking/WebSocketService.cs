using System.Collections.Generic;
using VContainer;

namespace Networking
{
    public class WebSocketService : IWebSocketService
    {
        private readonly IObjectResolver _resolver;
        private readonly Dictionary<string, IWebSocketConnection> _connections = new Dictionary<string, IWebSocketConnection>();

        public WebSocketService(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public IWebSocketConnection GetOrCreateConnection(string connectionId)
        {
            if (_connections.TryGetValue(connectionId, out IWebSocketConnection connection))
            {
                return connection;
            }

            var newConnection = _resolver.Resolve<IWebSocketConnection>();
            _connections[connectionId] = newConnection;
            return newConnection;
        }

        public void CloseConnection(string connectionId)
        {
            if (_connections.TryGetValue(connectionId, out IWebSocketConnection connection))
            {
                _connections.Remove(connectionId);
                connection.Dispose();
            }
        }
    }
}