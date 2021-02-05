using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.Comum;
using static WinCTB_CTS.Module.BusinessObjects.Tubulacao.JuntaSpool;

namespace WinCTB_CTS.UnitTests
{
    public class Application : IDisposable
    {
        public ServerApplication serverApplication;

        private bool cacheDatabase;

        public Application(bool CacheDatabase)
        {
            this.cacheDatabase = CacheDatabase;
            Provider();
        }

        private void Provider()
        {
            if (serverApplication == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                ValueManager.ValueManagerType = typeof(MultiThreadValueManager<>).GetGenericTypeDefinition();
                var serverApplication = new ServerApplication();

                serverApplication.ApplicationName = "WinCTB_CTS";
                serverApplication.Modules.Add(new WinCTB_CTSModule());
                serverApplication.DatabaseVersionMismatch += ServerApplication_DatabaseVersionMismatch;
                serverApplication.CreateCustomObjectSpaceProvider += ServerApplication_CreateCustomObjectSpaceProvider;
                serverApplication.ConnectionString = connectionString;

                EnumProcessingHelper.RegisterEnum(typeof(CampoPipe), "CampoPipe");


                XpoTypesInfoHelper.GetXpoTypeInfoSource();
                XafTypesInfo.Instance.RegisterEntity(typeof(Contrato));
                XafTypesInfo.Instance.RegisterEntity(typeof(Contrato));
                XafTypesInfo.Instance.RegisterEntity(typeof(ReportDataV2));
                XafTypesInfo.Instance.RegisterEntity(typeof(FileData));
                XafTypesInfo.Instance.RegisterEntity(typeof(BaseObject));
                XafTypesInfo.Instance.RegisterEntity(typeof(ReportsModuleV2));
                XafTypesInfo.Instance.RegisterEntity(typeof(DashboardData));
                XafTypesInfo.Instance.RegisterEntity(typeof(OidGenerator));
                XafTypesInfo.Instance.RegisterEntity(typeof(FileAttachmentBase));

                //foreach (string parameterName in ParametersFactory.GetRegisteredParameterNames())
                //{
                //    IParameter parameter = ParametersFactory.FindParameter(parameterName);
                //    if (parameter.IsReadOnly && (CriteriaOperator.GetCustomFunction(parameter.Name) == null))
                //        CriteriaOperator.RegisterCustomFunction(new ReadOnlyParameterCustomFunctionOperator(parameter));
                //}

                serverApplication.Setup();
                serverApplication.CheckCompatibility();
                this.serverApplication = serverApplication;
            }
        }

        private void ServerApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }

        private void ServerApplication_CreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs e)
        {
            if (cacheDatabase)
                e.ObjectSpaceProvider = new XPObjectSpaceProvider(new CachedDataStoreProvider(e.ConnectionString), true);
            else
                e.ObjectSpaceProvider = new XPObjectSpaceProvider(new CustomIXpoDataStoreProvider(e.ConnectionString, e.Connection, true), true);
        }

        public void Dispose()
        {
            serverApplication.Dispose();
        }
    }
}
