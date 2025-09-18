using System.Threading.Tasks;

namespace Service
{
    public interface INavigationService
    {
        Task LoadSceneAsync(string sceneName);
    }
}