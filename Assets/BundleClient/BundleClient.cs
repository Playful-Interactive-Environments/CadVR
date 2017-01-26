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
    public static OnDownloaderStartedDelegate OnDownloaderStarted;

    public delegate void OnAssetBundleAvailableDelegate(string bundleName, string[] assetNames);
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
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
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
        if (!downloaderProcess.HasExited)
        {
            downloaderProcess.Kill();
            recheckTimer.Enabled = false;
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

        string[] files = Directory.GetFiles(BUNDLES_DIRECTORY);
        if (OnLog != null)
        {
            foreach (string file in files)
            {
                OnLog("Found File \"" + file + "\".");
            }
        }
    }

    public static AssetBundle GetAssetBundle(string name)
    {
        return new AssetBundle();
    }

}
