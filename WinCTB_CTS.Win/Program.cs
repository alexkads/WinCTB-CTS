using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Internal;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraEditors;

namespace WinCTB_CTS.Win
{
    static class Program
    {
        private static string connectionStringSettings()
        {
            String result = null;
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if (connectionStringSettings != null)
            {
                result = connectionStringSettings.ConnectionString;
            }
            else
            {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                if (connectionStringSettings != null)
                {
                    result = DbEngineDetector.PatchConnectionString(connectionStringSettings.ConnectionString);
                }
            }

            return result;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if EASYTEST
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            WindowsFormsSettings.LoadApplicationSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;

            if (Tracing.GetFileLocationFromSettings() == DevExpress.Persistent.Base.FileLocation.CurrentUserApplicationDataFolder)
            {
                Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
            }

            Tracing.Initialize();
            XpoDefault.TrackPropertiesModifications = false;
            XpoDefault.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;

            WinCTB_CTSWindowsFormsApplication winApplication = new WinCTB_CTSWindowsFormsApplication();
            DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
            winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            winApplication.EnableModelCache = true;
            winApplication.CustomizeFormattingCulture += WinApplication_CustomizeFormattingCulture;
            winApplication.CreateCustomObjectSpaceProvider += WinApplication_CreateCustomObjectSpaceProvider;
            winApplication.DatabaseVersionMismatch += WinApplication_DatabaseVersionMismatch;
            winApplication.ConnectionString = connectionStringSettings();

            try
            {
                winApplication.Setup();
                winApplication.CheckCompatibility();
                winApplication.Start();
            }
            catch (Exception e)
            {
                winApplication.HandleException(e);
            }
        }

        private static void WinApplication_CreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs e)
        {
            e.ObjectSpaceProvider = new XPObjectSpaceProvider(e.ConnectionString, e.Connection, true);
            e.ObjectSpaceProvider = new XPObjectSpaceProvider(new Module.Comum.CachedDataStoreProvider(e.ConnectionString), true);
        }

        private static void WinApplication_CustomizeFormattingCulture(object sender, CustomizeFormattingCultureEventArgs e)
        {
            e.FormattingCulture.DateTimeFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            e.FormattingCulture.NumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
        }

        private static void WinApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }
    }
}
