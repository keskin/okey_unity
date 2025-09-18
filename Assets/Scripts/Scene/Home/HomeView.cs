using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Scene.Home
{
    public class HomeView : MonoBehaviour, IHomeView
    {
        [SerializeField] private Button _playButton;
        private HomePresenter _presenter;

        [Inject]
        public void Construct(HomePresenter presenter)
        {
            _presenter = presenter;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(_presenter.OnPlayButtonClicked);
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(_presenter.OnPlayButtonClicked);
        }
    }
}