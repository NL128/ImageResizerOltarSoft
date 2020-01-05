using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OltarSoftJson.Service
{
   public class JsonWriterLoaderGeneric<T>
    {
        public static T LoadJsonSettings(string fileName)
        {

            string path = Path.GetFullPath(fileName);
            using (StreamReader r = new StreamReader(path))
            {

                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<T>(json);

                return items;
            }
        }
        public static void WriteJsonSettings(string fileName, T r)
        {
            string path = Path.GetFullPath(fileName);

            
                string json = JsonConvert.SerializeObject(r);

                //write string to file
                System.IO.File.WriteAllText(path, json);
            
            
        }
    }
}
