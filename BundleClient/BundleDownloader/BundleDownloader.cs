using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using RestSharp;
using RestSharp.Contrib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace BundleDownloader
{
    class BundleDownloader
    {

        // configuration loaded at initialization time
        private static dynamic config;
        private static string[] validFileTypes;
        private static string currentDir;
        private static string downloadFolder;
        // saves the synchronization state of the files in the current directory
        private static DbObject db;

        private static BundlesApi bundlesApi;
        private static Timer recheckTimer;

        // indicates that the application is currently rechecking the directory
        private static bool recheckActive = false;
        // indicates to the async recheck method that the application should quit
        private static bool shutdown = false;
        static void Main(string[] args)
        {
            // Disable validation of server certificate to not mess with certificate stores an stuff.
            // This is very ugly but can only be resolved by using official payed certificates or by installing 
            // the custom certificate into a certificate store.
            // TODO: In case of actual deployment of this software this should be removed!!
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(Shutdown);
            Init();

            // Start
            recheckTimer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit the program.");
            Console.ReadLine();

        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("I'm out of here");
        }

        private static void Init()
        {
            // load db and configuration
            string currentDir = Directory.GetCurrentDirectory();
            DbReader.DbFile = (Path.Combine(currentDir, "db.json"));
            db = DbReader.Load();
            config = ConfigReader.Get(currentDir, "config.json");

            downloadFolder = Path.GetFullPath(Path.Combine(currentDir, (string)config.downloadFolder));
            Directory.CreateDirectory(downloadFolder);

            // setup the client
            Configuration clientConfig = new Configuration();
            clientConfig.Timeout = (int)config.timeout;
            clientConfig.setApiClientUsingDefault();
            clientConfig.AddApiKey("api_key_both", (string)config.apiPwd);

            clientConfig.ApiClient = new ApiClient((string)config.basePath);

            bundlesApi = new BundlesApi(clientConfig);

            RestClient client = bundlesApi.Configuration.ApiClient.RestClient;

            //X509Certificate cert = CertificateReader.LoadFromPemFile(Path.GetFullPath(Path.Combine(currentDir, (string)config.crt.cert)));
            //client.ClientCertificates = new X509CertificateCollection{ cert };

            Console.WriteLine("Api calls will be sent to: \"" + bundlesApi.GetBasePath() + "\".");

            // The timer for checking new files in the local directory
            recheckTimer = new Timer((double)config.interval);
            recheckTimer.Elapsed += new ElapsedEventHandler(
                async (object source, ElapsedEventArgs evt) =>
                {
                    try
                    {
                        // Queue the recheck on the thread pool because
                        // synchronization back into this context isn't needed
                        // flags are used to prevent the application from exiting early
                        await Task.Run(async () =>
                        {
                            await DoRecheckBundles(source, evt);
                        });
                    }
                    catch (ApiException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine(
                            "An Api Exception occured:\n\tMessage: " + e.Message +
                            "\n\tHTTP Status Code: " + e.ErrorCode +
                            "\n\tBody: " + e.ErrorContent
                        );
                        Console.ResetColor();
                    }
                }
            );
            Console.WriteLine("The available bundles will be checked every " + recheckTimer.Interval + " ms and saved to  \"" + downloadFolder + "\" .");
        }


        private static async Task DoRecheckBundles(object source, ElapsedEventArgs e)
        {
            // throwing exception before setting the recheckActive flag to maintain consistent state
            if (!Directory.Exists(downloadFolder))
            {
                throw new DirectoryNotFoundException("The directory \"" + downloadFolder + "\" could not be found. You may take a look into the config.json and configure the watch folder properly.");
            }

            recheckActive = true;
#if DEBUG
            Console.WriteLine("Rechecking started at {0}", e.SignalTime);
#endif
            // stop rechecking continously during processing the directory
            recheckTimer.Enabled = false;
            try
            {
                BundleList availableBundles = await bundlesApi.GetAvailableBundlesAsync();

                List<Task> runningTasks = new List<Task>();
                // iterate over all available bundles
                availableBundles.ForEach( bundle => runningTasks.Add(HandleBundle(bundle)));

                // wait for recheck and downloads to complete before signaling completion back to the main thread
                await Task.WhenAll(runningTasks);
            }
            finally
            {   // finish the recheck properly even if an ApiException (e.g. timeout) has been thrown 
                // continue rechecking available bundles
                if (!shutdown)
                {
                    recheckTimer.Enabled = true;
                }
                // allow potentially exiting the application
                recheckActive = false;
            }
        }

        private static async Task HandleBundle(BundleListInner bundle)
        {

            string name = Path.GetFileName(bundle.UniqueId);

            // download is needed if the bundle has no entry in the dbObject
            // this will redownload all bundles if the db has been reset even if the bundles are actually there
            if (!db.bundles.ContainsKey(bundle.UniqueId))
            {
                DbObject.BundleEntry bundleEntry = new DbObject.BundleEntry(bundle.DisplayName, bundle.UniqueId);
                db.bundles.Add(name, bundleEntry);
                await DownloadFile(bundle.UniqueId, bundleEntry);
            }  
             
        }

        private static async Task DownloadFile(string uniqueId, DbObject.BundleEntry bundleEntry)
        {
            // download file
            Console.WriteLine("downloading bundle:\"" + uniqueId + "\".");

            // the actual download api call
            byte[] fileContents = await bundlesApi.GetBundleAsync(HttpUtility.UrlEncode(uniqueId));

            // remove the ugly hard coded ".zip" by the file extension from the file details
            string destinationPath = Path.Combine(downloadFolder, uniqueId + ".zip");
            using (FileStream writeStream = System.IO.File.Open(destinationPath, FileMode.Create))
            {
#if DEBUG
                Console.WriteLine("Saving file to:\"" + destinationPath + "\".");
#endif
                await writeStream.WriteAsync(fileContents, 0, fileContents.Length);
            }
            // TODO: move the unzip functionality to another place and control it by adding retrieving the bunlde details with the file extension
            // TODO: before unzipping
            ZipFile.ExtractToDirectory(destinationPath, Path.Combine(downloadFolder, uniqueId));

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Bundle download succeeded! (" + bundleEntry.uniqueId + ")");
            Console.ResetColor();

            DbReader.Save(db);
        }

        private static void Shutdown(object sender, EventArgs e)
        {
            recheckTimer.Enabled = false;
            if (recheckActive)
            {
                // indicate to not reenable the timer after recheck is complete
                shutdown = true;
            }
            // check if recheck has completed to savely story the db object
            while (recheckActive)
            {
                Task.Delay(10).Wait(); // check very often because we have limited time on shutdown
            }
        }
    }

}


