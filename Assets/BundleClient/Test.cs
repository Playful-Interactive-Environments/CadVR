using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Cad Vr/Test")]
public class Test : MonoBehaviour {

    private static BundleClient.OnLogDelegate defautlbundleClientLogger = (string msg, LogType logType) =>
    {
        Debug.logger.Log(logType,"<b>BundleClient:</b>" + msg);
    };

    private static BundleClient.OnDownloaderStartedDelegate defaultOnDownloaderStartedHandler = () =>
    {
        Debug.Log("The downloader is now running.");
    };

    private static BundleClient.OnAssetBundleAvailableDelegate defaultOnAssetBundleAvailableHandler = (bundleName, assetNames) =>
    {

    };

    private static void InitBundleClient()
    {
        BundleClient.OnLog += defautlbundleClientLogger;
        BundleClient.LogDownloaderOutputStream = true;
        BundleClient.LogDownloaderErrorStream = true;
        BundleClient.OnDownloaderStarted += defaultOnDownloaderStartedHandler;
    }

    private void Awake()
    {
        InitBundleClient();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
