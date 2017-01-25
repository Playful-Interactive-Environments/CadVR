using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace BundleDownloader
{
    class DbReader
    {
        private static string _File = "";
        public static string DbFile {
            get { return _File; }
            set {
                if (File.Exists(value))
                {
                    _File = value;
                }
                else
                {
                    throw new FileNotFoundException("The file \"" + value + "\" could not be found.");
                }
            }
        }

        public static DbObject Load()
        {
            if (_File == "")
            {
                throw new InvalidOperationException("There has't been a db file specified.");
            }

            using (StreamReader reader = new StreamReader(_File))
            {
                string json = reader.ReadToEnd();
                if (json == "")
                {
                    return new DbObject();
                } else
                {
                    return JsonConvert.DeserializeObject<DbObject>(json);
                }
            }
        }

        public static void Save(DbObject dbObject)
        {
            if (_File == "")
            {
                throw new InvalidOperationException("There has't been a db file specified.");
            }
            string json = JsonConvert.SerializeObject(dbObject, Formatting.Indented);
            File.WriteAllText(_File, json);
        }
    }
}