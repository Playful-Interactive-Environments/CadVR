using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BundleDownloader
{
    static class ConfigReader
    {
        public static dynamic Get(string directory, string filename)
        {
            if (Directory.Exists(directory))
            {
                string configFile = Path.GetFullPath(Path.Combine(directory, filename));
                if (File.Exists(configFile))
                {
                    dynamic config;
                    using (StreamReader r = new StreamReader(configFile))
                    {
                        string json = r.ReadToEnd();
                        config = JsonConvert.DeserializeObject(json);
                    }
                    return config;
                } else
                {
                    throw new FileNotFoundException("The file \"" + configFile + "\" could not be found.");
                }
            } else
            {
                throw new DirectoryNotFoundException("The directory \"" + directory + "\" could not be found.");
            }
        }
    }
}
