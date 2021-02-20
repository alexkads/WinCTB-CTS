using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Helpers
{
    class ProviderDataLayer
    {
        public static IDataLayer GetThreadSafeDataLayer()
        {
            XpoDefault.Session = null;
            string conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            Guard.ArgumentNotNull(conn, "connection");

            conn = XpoDefault.GetConnectionPoolString(conn);
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
            dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly());

            IDataLayer dataLayer = new ThreadSafeDataLayer(dict, store);
            return dataLayer;
        }

        public static IDataLayer GetCacheDataLayer()
        {
            XpoDefault.Session = null;
            string conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            Guard.ArgumentNotNull(conn, "connection");

            conn = XpoDefault.GetConnectionPoolString(conn);
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
            dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly());

            var cacheRoot = new DataCacheRoot(store);
            var cacheNode = new DataCacheNode(cacheRoot)
            {
                MaxCacheLatency = TimeSpan.FromMinutes(60),
                TotalMemoryPurgeThreshold = 32 * 1024 * 1024
            };

            IDataLayer dataLayer = new SimpleDataLayer(dict, cacheNode);
            return dataLayer;
        }
    }
}
