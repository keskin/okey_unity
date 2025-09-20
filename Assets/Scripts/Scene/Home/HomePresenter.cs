using Networking;
using System;
using System.Net.WebSockets;
using Service;

namespace Scene.Home
{
    public class HomePresenter : IDisposable
    {
        private const string GameConnectionId = "game";
        private const string ServerAddress = "ws://127.0.0.1:8000/ws/";
        private const string PlayerId = "P1";

        private readonly IWebSocketService _webSocketService;
        private IWebSocketConnection _gameConnection;
        private ILogger _logger;
        private INavigationService _navigationService;
        private bool _isConnecting = false;

        public HomePresenter(IWebSocketService webSocketService, ILogger logger, INavigationService navigationService)
        {
            _webSocketService = webSocketService;
            _logger = logger;
            _navigationService = navigationService;
        }

        public async void OnPlayButtonClicked()
        {
            if (_isConnecting) return;

            try
            {
                _isConnecting = true;
                _logger.Log("Bağlantı işlemi başlatılıyor...");

                _gameConnection = _webSocketService.GetOrCreateConnection(GameConnectionId);
                _gameConnection.OnConnected += OnConnected;
                _gameConnection.OnDisconnected += OnDisconnected;

                bool success = await _gameConnection.ConnectAsync(ServerAddress, PlayerId);

                if (success)
                {
                    _logger.Log("Bağlantı başarılı. GameScene yükleniyor.");
                    await _navigationService.LoadSceneAsync("GameScene");
                }
                else
                {
                    _logger.LogError("Bağlantı kurulamadı. Lütfen sunucunun çalıştığından emin olun.");
                    Dispose();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Bağlantı sırasında beklenmedik bir hata oluştu: {e.Message}");
                Dispose();
            }
            finally
            {
                _isConnecting = false;
            }
        }

        private void OnConnected()
        {
            _logger.Log("[HomePresenter] Bağlantı kuruldu.");
        }
        
        private void OnDisconnected(WebSocketCloseStatus closeStatus)
        {
            _logger.LogWarning($"[HomePresenter] Bağlantı koptu. Durum: {closeStatus}");
        }

        public void Dispose()
        {
            if (_gameConnection != null)
            {
                _gameConnection.OnConnected -= OnConnected;
                _gameConnection.OnDisconnected -= OnDisconnected;
            }
        }
    }
}

