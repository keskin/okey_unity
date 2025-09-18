using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Service;
using Networking;
using GameLogic;
using Scene;
using Utils;

namespace Di
{
    public class ProjectLifetimeScope : LifetimeScope 
    {
        [SerializeField] private AssetManager _assetManagerPrefab;
        [SerializeField] private MainThreadDispatcher _mainThreadDispatcher;

        protected override void Awake()
        {
            base.Awake(); 
            DontDestroyOnLoad(this.gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_assetManagerPrefab, Lifetime.Singleton)
                .DontDestroyOnLoad() 
                .As<IAssetManager>();
            builder.Register<IWebSocketConnection, WebSocketConnection>(Lifetime.Transient);
            builder.Register<IWebSocketService, WebSocketService>(Lifetime.Singleton);
            builder.Register<SmartBot>(Lifetime.Singleton);
            builder.RegisterComponent(_mainThreadDispatcher).As<IMainThreadDispatcher>();
            builder.Register<Service.ILogger, Service.Logger>(Lifetime.Singleton);
            builder.Register<INavigationService, NavigationService>(Lifetime.Singleton);
        }
    }
}