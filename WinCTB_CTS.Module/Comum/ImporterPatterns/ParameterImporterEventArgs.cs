using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Comum.ImporterPatterns
{
    public class ParameterImporterEventArgs : EventArgs
    {
        public string tabName;
        public ParameterImporterEventArgs(string tabName)
        {
            this.tabName = tabName;
        }
    }
}
