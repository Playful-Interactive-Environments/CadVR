using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("Cad Vr/AutoLoadAllAvailableBundles")]
public class AutoLoadAllAvailableBundles : MonoBehaviour {

    public delegate void OnGameObjectLoadedDelegate(string assetBundleName, string assetName, GameObject go);
    /// <summary>
    /// Is called if an asset which is a game object has finished loading
    /// </summary>
    public OnGameObjectLoadedDelegate OnGameObjectLoaded;

    public delegate void OnPreAssetLoadDelegate(string assetBundleName, string assetName);
    /// <summary>
    /// Is called before a new asset will be loaded
    /// </summary>
    public OnPreAssetLoadDelegate OnPreAssetLoad;

    public delegate void OnAssetLoadProgressDelegate(string assetBundleName, string assetName, float progress);
    /// <summary>
    /// Is called when an asset has progressed loading
    /// </summary>
    public OnAssetLoadProgressDelegate OnAssetLoadProgress;


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
            StartCoroutine(BundleClient.GetAssetBundle(bundle, ReceiveAssetBundle, (progress) => {
                Debug.Log(Mathf.RoundToInt(progress * 100) + " % Loading Progress (Bundle: \"" + bundle + "\")");
            }));
        }
    }

    // The asset bundle loaded callback
    private void ReceiveAssetBundle(AssetBundle assetBundle)
    {
        // pre asset load
        if (OnPreAssetLoad != null)
        {
            foreach (string assetName in assetBundle.GetAllAssetNames())
            {
                OnPreAssetLoad(assetBundle.name, Path.GetFileNameWithoutExtension(assetName).ToLower());
            }
        }

        // asset load
        StartCoroutine(LoadAllAssets(assetBundle));
    }

    private IEnumerator LoadAllAssets(AssetBundle assetBundle)
    {
        AssetBundleRequest request = assetBundle.LoadAllAssetsAsync();

        if (OnAssetLoadProgress != null)
        {
            // call progress delegate every frame
            while (!request.isDone) {
                foreach(string assetName in assetBundle.GetAllAssetNames())
                {
                    OnAssetLoadProgress(assetBundle.name, Path.GetFileNameWithoutExtension(assetName).ToLower(), request.progress);
                }

                yield return null;
            }
        } else
        {
            // yield the request directly
            yield return request;
        }
        
        // post asset load
        foreach(Object asset in request.allAssets)
        {
            if (asset is GameObject && OnGameObjectLoaded != null)
            {
                GameObject go = asset as GameObject;
                foreach (BaseGameObjectProcessor processor in preprocessors)
                {
                    processor.Process(go);
                }
                OnGameObjectLoaded(assetBundle.name, asset.name.ToLower(), go);
            }
        }
    }



}
