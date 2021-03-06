using DevExpress.Xpo;
using System;
using System.Data;

namespace WinCTB_CTS.Module.ServiceProcess.Base
{
    public class MapImporterEventArgs : EventArgs
    {
        private UnitOfWork uow;
        private int expectedTotal;
        private int index;
        private DataTable dataTable;
        private DataRow rowForMap;
        public MapImporterEventArgs(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int index)
        {
            this.uow = uow;
            this.expectedTotal = expectedTotal;
            this.index = index;
            this.dataTable = dataTable;
            this.rowForMap = rowForMap;
        }
    }
}
