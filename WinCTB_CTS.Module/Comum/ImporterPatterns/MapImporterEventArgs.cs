using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Comum.ImporterPatterns
{
    public class MapImporterEventArgs : EventArgs
    {
        UnitOfWork uow;
        private DataRow rowForMap;
        public MapImporterEventArgs(UnitOfWork uow, DataRow rowForMap)
        {
            this.uow = uow;
            this.rowForMap = rowForMap;
        }
    }
}
