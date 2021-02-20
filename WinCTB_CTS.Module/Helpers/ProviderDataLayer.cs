﻿using DevExpress.ExpressApp.Utils;
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
    class ProviderDataLayer : IDisposable
    {
        private IDataLayer cacheDataLayer;
        private DataCacheRoot cacheRoot;
        private DataCacheNode cacheNode;

        public ProviderDataLayer() { }

        public void Dispose()
        {
            cacheRoot.Reset();
            cacheNode.Reset();
            cacheRoot = null;
            cacheNode = null;
            cacheDataLayer.Dispose();
        }

        //public static IDataLayer GetThreadSafeDataLayer()
        //{
        //    XpoDefault.Session = null;
        //    string conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //    Guard.ArgumentNotNull(conn, "connection");

        //    conn = XpoDefault.GetConnectionPoolString(conn);
        //    XPDictionary dict = new ReflectionDictionary();
        //    IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
        //    dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly());

        //    IDataLayer dataLayer = new ThreadSafeDataLayer(dict, store);
        //    return dataLayer;
        //}

        public IDataLayer GetCacheDataLayer()
        {
            XpoDefault.Session = null;
            string conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            Guard.ArgumentNotNull(conn, "connection");

            conn = XpoDefault.GetConnectionPoolString(conn);
            XPDictionary dict = new ReflectionDictionary();
            IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
            dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly());

            cacheRoot = new DataCacheRoot(store);
            cacheNode = new DataCacheNode(cacheRoot)
            {
                MaxCacheLatency = TimeSpan.FromMinutes(60),
                TotalMemoryPurgeThreshold = 32 * 1024 * 1024
            };

            cacheDataLayer = new SimpleDataLayer(dict, cacheNode);
            return cacheDataLayer;
        }

        //public static IDataLayer GetSimpleDataLayer()
        //{
        //    XpoDefault.Session = null;
        //    string conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //    Guard.ArgumentNotNull(conn, "connection");

        //    conn = XpoDefault.GetConnectionPoolString(conn);
        //    XPDictionary dict = new ReflectionDictionary();
        //    IDataStore store = XpoDefault.GetConnectionProvider(conn, AutoCreateOption.None);
        //    dict.GetDataStoreSchema(System.Reflection.Assembly.GetExecutingAssembly());

        //    IDataLayer dataLayer = new SimpleDataLayer(dict, store);
        //    return dataLayer;
        //}
    }
}
