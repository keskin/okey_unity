using UnityEngine;

namespace Networking
{
    public class WebSocketNetworking : MonoBehaviour, IWebSocketNetworking
    {
        public void Connect()
        {
            Debug.Log("WebSocketNetworking sunucuya bağlanıyor...");
        }
    }
}