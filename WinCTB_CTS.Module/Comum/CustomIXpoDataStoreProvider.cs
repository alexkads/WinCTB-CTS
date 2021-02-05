using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Comum
{
    public class CustomIXpoDataStoreProvider : IXpoDataStoreProvider
    {
        private IXpoDataStoreProvider innerDataStoreProvider = null;
        public CustomIXpoDataStoreProvider(string connectionString, IDbConnection connection, bool enablePoolingInConnectionString)
        {
            innerDataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, enablePoolingInConnectionString);
        }
        public string ConnectionString
        {
            get { return this.innerDataStoreProvider.ConnectionString; }
        }
        public IDataStore CreateSchemaCheckingStore(out IDisposable[] disposableObjects)
        {
            return innerDataStoreProvider.CreateSchemaCheckingStore(out disposableObjects);
        }
        public IDataStore CreateUpdatingStore(bool allowUpdateSchema, out IDisposable[] disposableObjects)
        {
            return innerDataStoreProvider.CreateUpdatingStore(allowUpdateSchema, out disposableObjects);
        }
        public IDataStore CreateWorkingStore(out IDisposable[] disposableObjects)
        {
            IDataStore ds = innerDataStoreProvider.CreateWorkingStore(out disposableObjects);
            if (ds is ConnectionProviderSql connectionProvider)
            {
                connectionProvider.DefaultCommandTimeout = new TimeSpan(0, 5, 0).Milliseconds;
            }
            return ds;
        }
    }
}
