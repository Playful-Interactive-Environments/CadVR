using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cad Vr/AutoLoadAllAvailableBundles")]
public class AutoLoadAllAvailableBundles : MonoBehaviour {

    public delegate void OnGameObjectLoadedDelegate(GameObject go);
    public OnGameObjectLoadedDelegate OnGameObjectLoaded;

    [SerializeField]
    private List<BaseGameObjectProcessor> preprocessors = new List<BaseGameObjectProcessor>();

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
            string bundle = assetBundlesToLoad.Dequeue();
            StartCoroutine(BundleClient.GetAssetBundle(bundle, receiveAssetBundle, (progress) => {
                Debug.Log(Mathf.RoundToInt(progress * 100) + "% Loading Progress (Bundle: \"" + bundle + "\")");
            }));
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
                GameObject go = asset as GameObject;
                foreach (BaseGameObjectProcessor processor in preprocessors)
                {
                    processor.Process(go);
                }
                OnGameObjectLoaded(go);
            }
        }
    }
}
