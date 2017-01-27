using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cad Vr/AutoLoadAllAvailableBundles")]
public class AutoLoadAllAvailableBundles : MonoBehaviour {

    public delegate void OnGameObjectLoadedDelegate(GameObject go);
    public OnGameObjectLoadedDelegate OnGameObjectLoaded;

    private Queue<string> assetBundlesToLoad = new Queue<string>();

	void Awake () {
        BundleClient.OnAssetBundleAvailable += (string assetBundleName) =>
        {
            assetBundlesToLoad.Enqueue(assetBundleName);
        };
    }

    private void Update()
    {
        while (assetBundlesToLoad.Count != 0)
        {
            Debug.Log("loading");
            StartCoroutine(BundleClient.GetAssetBundle(assetBundlesToLoad.Dequeue(), receiveAssetBundle));
        }
    }

    private void receiveAssetBundle(AssetBundle assetBundle)
    {
        foreach (string asset in assetBundle.GetAllAssetNames())
        {
            Debug.Log("The loaded asset bundle contains the asset \"" + asset + "\"");
        }

        Object[] allAssets = assetBundle.LoadAllAssets();

        foreach(Object asset in allAssets)
        {
            if (asset is GameObject && OnGameObjectLoaded != null)
            {
                OnGameObjectLoaded(asset as GameObject);
            }
        }
    }
}
