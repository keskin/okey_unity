using System.Threading;
using System.Threading.Tasks;
using Service;
using UnityEngine;
using VContainer.Unity;
using ILogger = Service.ILogger;

namespace Scene.Boot
{
    public class BootPresenter : IAsyncStartable
    {
        private readonly IBootView _view;
        private readonly INavigationService _navigationService;
        private readonly ILogger _logger;

        public BootPresenter(IBootView view, INavigationService navigationService, ILogger logger)
        {
            _view = view;
            _navigationService = navigationService;
            _logger = logger;
        }

        public async Awaitable StartAsync(CancellationToken cancellation)
        {
            _logger.Log("Başlangıç işlemleri başlatılıyor...");
            _view.ShowLoading();

            await InitializeServicesAsync(cancellation);
            
            _logger.Log("Başlangıç işlemleri tamamlandı. HomeScene yükleniyor.");
            _view.HideLoading();
            await _navigationService.LoadSceneAsync("HomeScene");
        }

        private async Task InitializeServicesAsync(CancellationToken cancellation)
        {
            // Gerçek bir işlem simülasyonu için küçük bir gecikme
            await Task.Delay(2000, cancellation);
            
            _logger.Log("Firebase başlatılıyor...");
            // TODO: await Firebase.InitializeAsync();
            
            _logger.Log("Ayarlar yükleniyor...");
            // TODO: await Preferences.LoadAsync();
        }
    }
}