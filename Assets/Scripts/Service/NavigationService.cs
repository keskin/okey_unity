using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Service
{
    public class NavigationService : INavigationService
    {
        public async Task LoadSceneAsync(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}