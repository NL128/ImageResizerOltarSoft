using System;
using System.Collections.Generic;
using System.Text;

namespace OltarSoftJson.Service.Models
{
   public  interface IProgressReport
    {
         float Progressing(int total, ref int currentProgress, ref float progressComplete);
    }
}
