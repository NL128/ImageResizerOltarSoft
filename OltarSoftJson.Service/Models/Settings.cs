using System;
using System.Collections.Generic;
using System.Text;

namespace OltarSoftJson.Service.Models
{
     public  class Settings
    {
        public float PPI { get; set; }
        public float Quality { get; set; }
        public bool PreserveAspectRatio { get; set; }
        public string ImageType { get; set; }
        

        public void Initialize(float PPI, float Quality,bool preserveRatio,string ImageType)
        {
            this.PPI = PPI;
            this.Quality = Quality;
            PreserveAspectRatio = preserveRatio;
            this.ImageType = ImageType;
        }
      

    }
}
