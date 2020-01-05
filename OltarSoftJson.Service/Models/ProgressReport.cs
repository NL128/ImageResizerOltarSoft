using System;
using System.Collections.Generic;
using System.Text;

namespace OltarSoftJson.Service.Models
{
  public  class ProgressReport: IProgressReport
    {
        public int ProgressComplete { get; set; }

        public  float Progressing(int total, ref int currentProgress , ref float progressComplete)
        {
            float incremental = ++currentProgress * 100 / total;
           
            progressComplete = incremental;
            
            return incremental;
        }
    }
}
