using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using UnityEngine;

public static class BundleClient {

    public const string BUNDLES_DIRECTORY = "./bundles";
    public const string BUNDLE_CLIENT_PATH = "./bin/BundleDownloader.exe";

    // this MonoBehaviour is automatically instantiated to manage lifecycle
    private class BundleClientManager : MonoBehaviour
    {
        void OnApplicationQuit()
        {
            BundleClient.OnApplicationQuit();
        }
    }

    private static BundleClientManager bundleManager;


    public delegate void OnDownloaderStartedDelegate();
    /// <summary>
    /// This delegate is invoked from another thread. Make sure you do not directly interacte with the unity api from this delegate. 
    /// </summary>
    public static OnDownloaderStartedDelegate OnDownloaderStarted;

    public delegate void OnAssetBundleAvailableDelegate(string bundleName);
    /// <summary>
    /// This delegate is invoked from another thread. Make sure you do not directly interacte with the unity api from this delegate. 
    /// </summary>
    public static OnAssetBundleAvailableDelegate OnAssetBundleAvailable;

    public delegate void OnLogDelegate(string msg, LogType type = LogType.Log);
    public static OnLogDelegate OnLog;

    /// <summary>
    /// The interval to pool for new asset bundles in seconds
    /// </summary>
    public static float PollFrequency { get { return (float)(recheckTimer.Interval * 0.001); } set {recheckTimer.Interval = value * 1000;} }
    /// <summary>
    /// Should the output of the downloader be logged to the OnLog delegate?
    /// </summary>
    public static bool LogDownloaderOutputStream { get; set; }
    /// <summary>
    /// Should errors of the downloader be logged to the OnLog delegate?
    /// </summary>
    public static bool LogDownloaderErrorStream { get; set; }

    private static Process downloaderProcess;
    private static Timer recheckTimer = new Timer();
    private static bool bundleFolderFound = false;
    private static Dictionary<string, string> knownAssetBundles;
    private static Dictionary<string, AssetBundle> loadedAssetBundles;

    static BundleClient()
    {
        // default poll frequency to 2s
        PollFrequency = 2;
    }

    /// <summary>
    /// This is called after the awake methods have run so that subscribing to the delegates is possible from within instantiated scripts.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (OnLog != null)
        {
            OnLog("Initializing the bundle client.", LogType.Log);
        }

        knownAssetBundles = new Dictionary<string, string>();

        if (!File.Exists(BUNDLE_CLIENT_PATH))
        {
            if (OnLog != null)
            {
                OnLog("Cannot find BundleDownloader.exe next to app executable at \"" + Path.GetFullPath(BUNDLE_CLIENT_PATH) + "\".", LogType.Error);
            }
            return;
        }

        // check if downloader is already running
        if (downloaderProcess != null && !downloaderProcess.HasExited)
        {
            return;
        }

        bundleFolderFound = false;

        recheckTimer.AutoReset = true;
        recheckTimer.Elapsed += Recheck;
        // configure how the bundle downloader will run
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.CreateNoWindow = false;
        startInfo.UseShellExecute = false; // must be false if redirect standard output is true
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.FileName = BUNDLE_CLIENT_PATH;
        startInfo.CreateNoWindow = true;
        startInfo.WorkingDirectory = Path.GetDirectoryName(BUNDLE_CLIENT_PATH);
        //startInfo.Arguments = "";

        try
        {
            // start the downloader
            downloaderProcess = Process.Start(startInfo);
            // register exit handler
            downloaderProcess.Exited += OnDownLoaderProcessExited;
            downloaderProcess.Disposed += OnDownLoaderProcessDisposed;

            // register log handlers
            downloaderProcess.OutputDataReceived += OnBundleDownloaderOutputStream;
            downloaderProcess.ErrorDataReceived += OnBundleDownloaderErrorStream;
            downloaderProcess.BeginOutputReadLine();
            downloaderProcess.BeginErrorReadLine();
        }
        catch (Exception e)
        {
            if (e is InvalidOperationException || e is ArgumentNullException || e is ObjectDisposedException || e is FileNotFoundException || e is Win32Exception)
            {
                if (OnLog != null)
                {
                    OnLog("An exception occurred when starting the bundle downloader. The process could not be started:\n" + 
                        e.ToString(), LogType.Exception);
                }
                return;
            }

            throw e;
        }

        if (!bundleManager)
        {
            // create an instance in the scene to manage lifecycle
            GameObject go = new GameObject("[BundleClientManager]", typeof(BundleClientManager));
            GameObject.DontDestroyOnLoad(go);
            bundleManager = go.GetComponent<BundleClientManager>();
        }

        // start polling from directory
        recheckTimer.Enabled = true;

        OnDownloaderStarted();
    }

    private static void OnBundleDownloaderOutputStream(object sender, DataReceivedEventArgs args)
    {
        if (LogDownloaderOutputStream && OnLog != null)
        {
            OnLog("Bundle downloader logged: \"" + args.Data + "\"");
        }
    }

    private static void OnBundleDownloaderErrorStream(object sender, DataReceivedEventArgs args)
    {
        if (LogDownloaderErrorStream && OnLog != null)
        {
            OnLog("Bundle downloader logged: \"" + args.Data + "\"", LogType.Error);
        }
    }

    private static void OnDownLoaderProcessExited(object sender, EventArgs e)
    {
        if (OnLog != null)
        {
            OnLog("Bundle downloader exited at: " + downloaderProcess.ExitTime +
                "\r\n With Exit code: " + downloaderProcess.ExitCode);
        }
    }

    private static void OnDownLoaderProcessDisposed(object sender, EventArgs e)
    {
        if (OnLog != null)
        {
            OnLog("Bundle downloader has been disposed");
        }
    }

    private static void OnApplicationQuit()
    {
        recheckTimer.Enabled = false;

        if (!downloaderProcess.HasExited)
        {
            downloaderProcess.Kill();
        }

        foreach (KeyValuePair<string, AssetBundle> loadedAssetBundle in loadedAssetBundles)
        {
            loadedAssetBundle.Value.Unload(true);
            loadedAssetBundles.Remove(loadedAssetBundle.Key);
        }
    }

    private static void Recheck(object sender, EventArgs e)
    {
        //AssetBundle.LoadFromFile("");
        // check for the download folder until it is created by the bundle downloader
        if (!bundleFolderFound && !Directory.Exists(BUNDLES_DIRECTORY))
        {
            if (OnLog != null)
            {
                OnLog("Cannot find the bundle directory at \"" + Path.GetFullPath(BUNDLES_DIRECTORY) + "\"", LogType.Error);
            }
            return;
        }
        else
        {
            bundleFolderFound = true;
        }

        if (OnLog != null)
        {
            OnLog("Rechecking!");
        }

        string[] directories = Directory.GetDirectories(BUNDLES_DIRECTORY);
        foreach (string dir in directories)
        {
            HandleDirectory(dir);
        }
    }

    private static void HandleDirectory(string dir)
    {
        string[] files = Directory.GetFiles(dir);
        foreach (string file in files)
        {
            try
            {
                if (file.EndsWith(".manifest"))
                {
                    string bundleName = file.Replace(".manifest", "");
                    if (!knownAssetBundles.ContainsKey(bundleName)) {
                        string bundlePath = Path.Combine(dir, bundleName);
                        if (File.Exists(bundlePath)) {
                            if (OnLog != null)
                            {
                                OnLog("Found new asset bundle \"" + bundleName + "\".");
                            }
                            knownAssetBundles.Add(bundleName, bundlePath);
                            OnAssetBundleAvailable(bundleName);
                        }
                    }

                }
            } catch
            {
                if (OnLog != null)
                {
                    OnLog("An exception occured scanning the bundles folder.", LogType.Error);
                }
            }
        }
    }
    /// <summary>
    /// Start this coroutine to asynchronously get an assetbundle.
    /// Note that the callback will be called synchronously if the assetbundle is already loaded
    /// </summary>
    /// <param name="name">The name of the asset bundle</param>
    /// <param name="assetBundleCb">A callback receiving the loaded asset bundle</param>
    /// <param name="progressCb">A callback receiving the progress of the load operation every update</param>
    /// <returns></returns>
    public static IEnumerator GetAssetBundle(string name, Action<AssetBundle> assetBundleCb, Action<float> progressCb = null)
    {
        AssetBundle bundle;
        if (loadedAssetBundles.TryGetValue(name, out bundle))
        {
            assetBundleCb(bundle);
            yield break;
        }

        string bundlePath;
        if (knownAssetBundles.TryGetValue(name, out bundlePath))
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
            if (progressCb == null)
            {   // we don't need to update the progress so wait directly for the request to complete
                yield return request;
            }
            else
            {   // update progress
                while (!request.isDone)
                {
                    yield return null;
                    progressCb(request.progress);
                }
            }

            // asset successfully loaded
            assetBundleCb(request.assetBundle);
        }

        if (OnLog != null)
        {
            OnLog("The requested assed bundle could not be found! " +
                "Are you sure you received the bundle name through the OnAssetBundleAvailable delegate or the GetAvailableAssetBunldes method",
                LogType.Error);
        }
    }

    /// <summary>
    /// Either use the OnAssetBundleAvailable delegate or this method to retrieve information about what asset bundles are available
    /// </summary>
    /// <returns>An array of all available asset bundle names that can be passed the GetAssetBundle coroutine</returns>
    public static string[] GetAvailableAssetBundles()
    {
        string[] names = new string[knownAssetBundles.Count];
        knownAssetBundles.Keys.CopyTo(names, 0);
        return names;
    }
}
