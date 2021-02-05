using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Comum
{
    public class CachedDataStoreProvider : ConnectionStringDataStoreProvider, IXpoDataStoreProvider
    {
        public static void ResetDataCacheRoot()
        {
            root = null;
            if (rootDisposableObjects != null)
            {
                foreach (IDisposable disposableObject in rootDisposableObjects)
                    disposableObject.Dispose();

                rootDisposableObjects = null;
            }
        }
        private static IDisposable[] rootDisposableObjects;
        private static DataCacheRoot root;
        public CachedDataStoreProvider(String connectionString) : base(connectionString) { }
        IDataStore IXpoDataStoreProvider.CreateWorkingStore(out IDisposable[] disposableObjects)
        {
            if (base.CreateWorkingStore(out rootDisposableObjects) is ConnectionProviderSql connectionProvider)
                connectionProvider.DefaultCommandTimeout = new TimeSpan(0, 5, 0).Milliseconds;

            if (root == null)
                root = new DataCacheRoot(base.CreateWorkingStore(out rootDisposableObjects));

            disposableObjects = new IDisposable[0];
            return new DataCacheNode(root);
        }
    }
}
