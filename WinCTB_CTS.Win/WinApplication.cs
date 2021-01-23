using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Xpo;

namespace WinCTB_CTS.Win
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/DevExpressExpressAppWinWinApplicationMembersTopicAll.aspx
    public partial class WinCTB_CTSWindowsFormsApplication : WinApplication
    {
        #region Default XAF configuration options (https://www.devexpress.com/kb=T501418)
        static WinCTB_CTSWindowsFormsApplication()
        {
            DevExpress.Persistent.Base.PasswordCryptographer.EnableRfc2898 = true;
            DevExpress.Persistent.Base.PasswordCryptographer.SupportLegacySha512 = false;
            DevExpress.ExpressApp.Utils.ImageLoader.Instance.UseSvgImages = true;
        }
        private void InitializeDefaults()
        {
            LinkNewObjectToParentImmediately = false;
            OptimizedControllersCreation = true;
            UseLightStyle = true;
            SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
            ExecuteStartupLogicBeforeClosingLogonWindow = true;
        }
        #endregion
        public WinCTB_CTSWindowsFormsApplication()
        {
            InitializeComponent();
            InitializeDefaults();
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args)
        {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(XPObjectSpaceProvider.GetDataStoreProvider(args.ConnectionString, args.Connection, true), false));
            args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
        }
        private void WinCTB_CTSWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e)
        {
            string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            if (userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1)
            {
                e.Languages.Add(userLanguageName);
            }
        }
        private void WinCTB_CTSWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e)
        {
            e.Updater.Update();
            e.Handled = true;
        }
    }
}
