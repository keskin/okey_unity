using UnityEngine.SceneManagement;
using Service;
using Networking;

namespace Scene.Home
{
    public class HomePresenter
    {
        private readonly IAssetManager _assetManager;
        private readonly IWebSocketNetworking _webSocketNetworking;

        public HomePresenter(IAssetManager assetManager, IWebSocketNetworking webSocketNetworking)
        {
            _assetManager = assetManager;
            _webSocketNetworking = webSocketNetworking;
        }

        public void OnPlayButtonClicked()
        {
            _assetManager.LoadAssets();
            _webSocketNetworking.Connect();
            SceneManager.LoadScene("GameScene");
        }
    }
}