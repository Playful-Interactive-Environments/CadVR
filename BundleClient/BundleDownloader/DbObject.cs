using Newtonsoft.Json;
using System.Collections.Generic;

namespace BundleDownloader
{
    public class DbObject
    {
        public class BundleEntry
        {
            public string originalFilename = "";
            public string uniqueId = "";

            [JsonConstructor]
            public BundleEntry(string displayName, string uniqueId)
            {
                // the human readable name
                this.originalFilename = displayName;
                // the of the bundle
                this.uniqueId = uniqueId;
            }
        }

        public Dictionary<string, BundleEntry> bundles = new Dictionary<string, BundleEntry>();
    }
}
