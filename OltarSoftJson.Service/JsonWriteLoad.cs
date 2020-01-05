using ImageResizerOltarSoft.Models;
using Newtonsoft.Json;
using OltarSoftJson.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace OltarSoftJson.Service
{
    public class JsonWriteLoad
    {
        public static Settings LoadJsonSettings(string fileName)
        {
            string path = Path.GetFullPath(fileName);
            using (StreamReader r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<Settings>(json);

                return items;
            }
        }
        public static void WriteJsonSettings(string fileName,Settings r)
        {
            string path = Path.GetFullPath(fileName);

            string json = JsonConvert.SerializeObject(r);

            //write string to file
            System.IO.File.WriteAllText(path, json);
        }
        public static void WriteJson(string fileName, List<Resolution> r)
        {
            string path = Path.GetFullPath(fileName);

            string json = JsonConvert.SerializeObject(r);

            //write string to file
            System.IO.File.WriteAllText(path, json);
        }
        public static List<Resolution> LoadJson(string fileName)
        {
            if (!File.Exists(Path.GetFullPath(fileName)))
            {
                File.Create(Path.GetFullPath(fileName));
            }
            string path = Path.GetFullPath(fileName);
            using (StreamReader r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<List<Resolution>>(json);

                return items;
            }
        }
    }
}
