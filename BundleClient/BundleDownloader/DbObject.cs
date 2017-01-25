using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BundleDownloader
{
    public class DbObject
    {
        public class BundleEntry
        {
            public string originalFilename = "";
            public string uniqueId = "";

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
