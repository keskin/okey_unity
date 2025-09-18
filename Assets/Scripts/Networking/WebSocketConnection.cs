using UnityEngine;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using System.Collections.Generic;

namespace Networking
{
    public class WebSocketConnection : IWebSocketConnection
    {
        private const WebSocketCloseStatus AbnormalClosure = (WebSocketCloseStatus)1006;
        
        public event Action OnConnected;
        public event Action<string> OnMessageReceived;
        public event Action<WebSocketCloseStatus> OnDisconnected;

        public ConnectionStatus Status => _status;
        private ConnectionStatus _status;

        private readonly IMainThreadDispatcher _dispatcher;
        private ClientWebSocket _ws;
        private CancellationTokenSource _cancellationSource;

        private string _serverAddress;
        private string _connectionParams;
        
        private float _initialReconnectDelay = 1f;
        private float _maxReconnectDelay = 30f;
        private float _reconnectDelayMultiplier = 2f;
        private float _currentReconnectDelay;
        
        private readonly Queue<string> _pendingMessages = new Queue<string>();
        private bool _isPrimaryListenerAttached = false;

        public WebSocketConnection(IMainThreadDispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                Debug.LogError("[WebSocketConnection] CRITICAL ERROR: IMainThreadDispatcher was not injected.");
            }
            _dispatcher = dispatcher;
            _cancellationSource = new CancellationTokenSource();
        }

        public async Task<bool> ConnectAsync(string serverAddress, string connectionParams)
        {
            if (_status == ConnectionStatus.Connected || _status == ConnectionStatus.Connecting)
            {
                return true;
            }

            _serverAddress = serverAddress;
            _connectionParams = connectionParams;
            _status = ConnectionStatus.Connecting;

            if (_cancellationSource.IsCancellationRequested)
            {
                 _cancellationSource.Dispose();
                 _cancellationSource = new CancellationTokenSource();
            }
            
            var statusUpdateTcs = new TaskCompletionSource<bool>();

            try
            {
                _ws = new ClientWebSocket();
                string fullServerAddress = $"{_serverAddress}{_connectionParams}";
                Debug.Log($"[WebSocketConnection] Connecting to: {fullServerAddress}");
                
                await _ws.ConnectAsync(new Uri(fullServerAddress), _cancellationSource.Token);

                _dispatcher.Enqueue(() =>
                {
                    _status = ConnectionStatus.Connected;
                    _currentReconnectDelay = _initialReconnectDelay;
                    Debug.Log($"[WebSocketConnection] Connection successful!");
                    OnConnected?.Invoke();
                    statusUpdateTcs.TrySetResult(true);
                });

                _ = ListenForMessages();
                return await statusUpdateTcs.Task;
            }
            catch (Exception e)
            {
                Debug.LogError($"[WebSocketConnection] Connection attempt failed: {e.Message}");
                // Başarısız denemeden sonra durumu sıfırla ki bir sonraki deneme yapılilabilsin.
                _status = ConnectionStatus.Disconnected; 
                statusUpdateTcs.TrySetResult(false);
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_status != ConnectionStatus.Connected)
            {
                return;
            }
            
            _cancellationSource.Cancel();
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested disconnection.", CancellationToken.None);
            _status = ConnectionStatus.Disconnected;
        }
        
        public async Task SendMessageAsync(string message)
        {
            if (_status != ConnectionStatus.Connected)
            {
                Debug.LogWarning($"[WebSocketConnection] WebSocket is not connected. Message not sent.");
                return;
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, _cancellationSource.Token);
        }

        private async Task ListenForMessages()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_ws.State == WebSocketState.Open && !_cancellationSource.IsCancellationRequested)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            var segment = new ArraySegment<byte>(buffer);
                            result = await _ws.ReceiveAsync(segment, _cancellationSource.Token);
                            memoryStream.Write(segment.Array, segment.Offset, result.Count);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }

                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                        {
                            string message = await reader.ReadToEndAsync();
                            if (_isPrimaryListenerAttached)
                            {
                                _dispatcher.Enqueue(() => OnMessageReceived?.Invoke(message));
                            }
                            else
                            {
                                lock (_pendingMessages)
                                {
                                    _pendingMessages.Enqueue(message);
                                }
                            }
                        }
                    }
                }
            }
            catch (WebSocketException wse)
            {
                Debug.LogError($"[WebSocketConnection] A network error occurred: {wse.WebSocketErrorCode}\n{wse.Message}");
                Debug.LogError(wse); 
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogError("[WebSocketConnection] An unexpected error occurred while listening:");
                Debug.LogError(e); 
            }
            finally
            {
                if (!_cancellationSource.IsCancellationRequested)
                {
                    HandleDisconnection(true); 
                }
            }
        }

        private async void HandleDisconnection(bool tryReconnect)
        {
            if (_cancellationSource.IsCancellationRequested) return;
            
            // OnDisconnected event'i sadece bir kez, ilk kopmada tetiklenmeli.
            if (_status != ConnectionStatus.Reconnecting)
            {
                 _dispatcher.Enqueue(() =>
                {
                    var closeStatus = _ws?.CloseStatus ?? AbnormalClosure;
                    Debug.LogWarning($"[WebSocketConnection] Disconnected. Status: {closeStatus}.");
                    OnDisconnected?.Invoke(closeStatus);
                });
            }
           
            _status = ConnectionStatus.Reconnecting;
            
            if (!tryReconnect)
            {
                _status = ConnectionStatus.Disconnected;
                return;
            }

            // --- YENİ YENİDEN BAĞLANMA DÖNGÜSÜ ---
            while (!_cancellationSource.IsCancellationRequested)
            {
                Debug.Log($"[WebSocketConnection] Attempting to reconnect in {_currentReconnectDelay} seconds.");
                await Task.Delay((int)(_currentReconnectDelay * 1000));
            
                if (_cancellationSource.IsCancellationRequested) break;

                bool success = await ConnectAsync(_serverAddress, _connectionParams);
                if (success)
                {
                    // Bağlantı başarılı olduğunda döngüden çık ve metodu bitir.
                    return; 
                }

                // Başarısız olursa, bir sonraki deneme için gecikmeyi artır.
                _currentReconnectDelay = Mathf.Min(_currentReconnectDelay * _reconnectDelayMultiplier, _maxReconnectDelay);
            }
        }
        
        public void ProcessPendingMessages()
        {
            _isPrimaryListenerAttached = true;
            lock (_pendingMessages)
            {
                if (_pendingMessages.Count > 0)
                {
                    Debug.Log($"[WebSocketConnection] Processing {_pendingMessages.Count} pending messages...");
                    while (_pendingMessages.Count > 0)
                    {
                        string message = _pendingMessages.Dequeue();
                        _dispatcher.Enqueue(() => OnMessageReceived?.Invoke(message));
                    }
                }
            }
        }
        
        public void Dispose()
        {
            if (_cancellationSource != null)
            {
                _cancellationSource.Cancel();
                _cancellationSource.Dispose();
                _cancellationSource = null;
            }
            _ws?.Dispose();
        }
    }
}

