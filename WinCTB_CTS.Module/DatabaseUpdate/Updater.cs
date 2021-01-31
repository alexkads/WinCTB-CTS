using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Dashboards;
using System.Reflection;

namespace WinCTB_CTS.Module.DatabaseUpdate {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppUpdatingModuleUpdatertopic.aspx
    public class Updater : ModuleUpdater {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            UpdateStatus("FeatureCenter.Module.Updater.UpdateDatabaseAfterUpdateSchema", "", "Creating initial demo data...");
            if (ObjectSpace is XPObjectSpace)
            {
                InitializeDashboards();
            }

            ObjectSpace.CommitChanges();
        }
        public override void UpdateDatabaseBeforeUpdateSchema() {
            base.UpdateDatabaseBeforeUpdateSchema();
        }

        private void InitializeDashboards()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            DashboardsModule.AddDashboardDataFromResources<DevExpress.Persistent.BaseImpl.DashboardData>(ObjectSpace, "Medição de Spool", assembly, "WinCTB_CTS.Module.Dashboards.MedicaoSpool.xml");
        }
    }
}
