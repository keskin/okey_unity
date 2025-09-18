using Networking;
using System;
using System.Net.WebSockets;
using Service;

namespace Scene.Game
{
    public class GamePresenter : IDisposable
    {
        private const string GameConnectionId = "game";
        public event Action<string> OnNewMessage;
        
        private readonly IWebSocketService _webSocketService;
        private IWebSocketConnection _gameConnection;
        private readonly ILogger _logger;

        public GamePresenter(IWebSocketService webSocketService, ILogger logger)
        {
            _webSocketService = webSocketService;
            _logger = logger;
            InitializeConnectionListener();
        }

        private void InitializeConnectionListener()
        {
            _gameConnection = _webSocketService.GetOrCreateConnection(GameConnectionId);

            if (_gameConnection.Status != ConnectionStatus.Connected)
            {
                _logger.LogError("GamePresenter: Bağlantı hazır değil! Durum: " + _gameConnection.Status);
                return;
            }
            
            _logger.Log("GamePresenter: Bağlantı hazır. Event'lere abone olunuyor.");
            _gameConnection.OnConnected += OnConnected;
            _gameConnection.OnDisconnected += OnDisconnected;
            _gameConnection.OnMessageReceived += OnGameMessageReceived;

            _gameConnection.ProcessPendingMessages();
        }

        private void OnGameMessageReceived(string message)
        {
            OnNewMessage?.Invoke($"Sunucudan mesaj: {message}");
        }
        
        private void OnConnected()
        {
            _logger.Log("[GamePresenter] Bağlantı kuruldu/yenilendi.");
        }
        
        private void OnDisconnected(WebSocketCloseStatus closeStatus)
        {
            _logger.LogWarning($"[GamePresenter] Bağlantı koptu. Durum: {closeStatus}");
        }

        public void Dispose()
        {
            if (_gameConnection != null)
            {
                _gameConnection.OnConnected -= OnConnected;
                _gameConnection.OnDisconnected -= OnDisconnected;
                _gameConnection.OnMessageReceived -= OnGameMessageReceived;
            }
        }
    }
}