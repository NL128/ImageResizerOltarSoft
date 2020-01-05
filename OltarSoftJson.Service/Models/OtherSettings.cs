using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OltarSoftJson.Service;
namespace OltarSoftJson.Service.Models
{
   public class OtherSettings
    {
       
        private string DefaultSaveName = @"OtherSettings.json";
        public string DefaultImageName { get; set; }
       

        public void SetImageName(string ImageName)
        {
            DefaultImageName = ImageName;
        }
        public string GetImageName()
        {
            return DefaultImageName;
        }
        public OtherSettings SaveChanges()
        {
            if (!File.Exists(Path.GetFullPath(DefaultSaveName)))
            {
                File.Create(Path.GetFullPath(DefaultSaveName));
            }
            OtherSettings other = new OtherSettings();
            other.SetImageName(DefaultImageName);
            JsonWriterLoaderGeneric<OtherSettings>.WriteJsonSettings(DefaultSaveName,  other);
            var result=   JsonWriterLoaderGeneric<OtherSettings>.LoadJsonSettings(DefaultSaveName);
            return result;  
        }
        public string GetUserDefinedName()
        {
            if (!File.Exists(Path.GetFullPath(DefaultSaveName)))
            {
                File.Create(Path.GetFullPath(DefaultSaveName));
            }
            var result = JsonWriterLoaderGeneric<OtherSettings>.LoadJsonSettings(DefaultSaveName);
            return result.DefaultImageName;
        }

    }
}
