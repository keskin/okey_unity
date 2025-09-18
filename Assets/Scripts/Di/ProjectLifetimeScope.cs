using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Service;
using Networking;
using GameLogic;

namespace Di
{
    public class ProjectLifetimeScope : LifetimeScope 
    {
        [SerializeField] private AssetManager _assetManagerPrefab;
        [SerializeField] private WebSocketNetworking _webSocketNetworkingPrefab;

        protected override void Awake()
        {
            base.Awake(); 
            DontDestroyOnLoad(this.gameObject);
        }
        
        private void Start()
        {
            SceneManager.LoadScene("HomeScene");
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_assetManagerPrefab, Lifetime.Singleton)
                .DontDestroyOnLoad() 
                .As<IAssetManager>();

            builder.RegisterComponentInNewPrefab(_webSocketNetworkingPrefab, Lifetime.Singleton)
                .DontDestroyOnLoad()
                .As<IWebSocketNetworking>();

            builder.Register<SmartBot>(Lifetime.Singleton);
        }
    }
}