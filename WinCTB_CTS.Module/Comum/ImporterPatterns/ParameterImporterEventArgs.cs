using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Comum.ImporterPatterns
{
    public class ParameterImporterEventArgs : EventArgs
    {
        public ParametrosImportBase parametrosImportBase;
        public string tabName;
        public ParameterImporterEventArgs(ParametrosImportBase parametrosImportBase, string tabName)
        {
            this.parametrosImportBase = parametrosImportBase;
            this.tabName = tabName;
        }
    }
}
