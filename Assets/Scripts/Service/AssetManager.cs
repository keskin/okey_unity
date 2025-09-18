using UnityEngine;

namespace Service
{
    public class AssetManager : MonoBehaviour, IAssetManager
    {
        public void LoadAssets()
        {
            Debug.Log("AssetManager varlıkları yüklüyor...");
        }
    }
}