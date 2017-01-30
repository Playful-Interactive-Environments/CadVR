using UnityEngine;

[AddComponentMenu("Cad Vr/Bundle Client Logger")]
public class BundleClientLogHandler : MonoBehaviour {

    private static BundleClient.OnLogDelegate defautlbundleClientLogger = (string msg, LogType logType) =>
    {
        Debug.logger.Log(logType,"<b>BundleClient:</b>" + msg);
    };

    private static BundleClient.OnDownloaderStartedDelegate defaultOnDownloaderStartedHandler = () =>
    {
        Debug.Log("The downloader is now running.");
    };

    private static BundleClient.OnAssetBundleAvailableDelegate defaultOnAssetBundleAvailableHandler = (bundleName) =>
    {
        Debug.Log("A new asset bundle is availabel: \"" + bundleName + "\".");
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitBundleClient()
    {
        BundleClient.OnLog += defautlbundleClientLogger;
        BundleClient.LogDownloaderOutputStream = true;
        BundleClient.LogDownloaderErrorStream = true;
        BundleClient.OnDownloaderStarted += defaultOnDownloaderStartedHandler;
    }

}
