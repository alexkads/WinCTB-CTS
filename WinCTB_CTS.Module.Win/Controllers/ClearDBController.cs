using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ClearDBController : WindowController
    {
        SimpleAction ActionClearDB;
        public ClearDBController()
        {
            TargetWindowType = WindowType.Main;
            ActionClearDB = new SimpleAction(this, "ActionClearDB", PredefinedCategory.RecordEdit)
            {
                Caption = "Excluir Banco de Dados",
                ImageName = "UpdateTableOfContents"
            };

            ActionClearDB.CustomizeControl += ActionClearDB_CustomizeControl;
        }

        private void ActionClearDB_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            BarButtonItem barItem = e.Control as BarButtonItem;
            if (barItem != null)
            {
                barItem.ItemClick += (s, args) => {
                    XtraMessageBox.Show("Item Clicked");
                };
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
