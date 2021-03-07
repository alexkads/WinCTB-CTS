using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.ServiceProcess.Base
{
    public class ImportProgressReport
    {
        public int TotalRows { get; set; }
        public double CurrentRow { get; set; }
        public string MessageImport { get; set; }
    }
}
