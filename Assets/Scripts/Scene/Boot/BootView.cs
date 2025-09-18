using UnityEngine;

namespace Scene.Boot
{
    public class BootView : MonoBehaviour, IBootView
    {
        [SerializeField] private GameObject _loadingScreenRoot;

        private void Awake()
        {
            if (_loadingScreenRoot != null)
            {
                _loadingScreenRoot.SetActive(true);
            }
        }

        public void ShowLoading()
        {
            if (_loadingScreenRoot != null)
            {
                _loadingScreenRoot.SetActive(true);
            }
        }

        public void HideLoading()
        {
            if (_loadingScreenRoot != null)
            {
                _loadingScreenRoot.SetActive(false);
            }
        }
    }
}