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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class AllProcessController : WindowController
    {
        SimpleAction ActionStartAllProcessInterface;
        WinCustomProcess.FormAllProcess FormAllProcess;
        public AllProcessController()
        {
            TargetWindowType = WindowType.Main;
            ActionStartAllProcessInterface = new SimpleAction(this, "ActionStartAllProcessInterfaceController", PredefinedCategory.RecordEdit);
        }
        protected override void OnActivated()
        {
            ActionStartAllProcessInterface.Caption = "Executar Todos os Processos";
            ActionStartAllProcessInterface.ImageName = "BO_Audit_ChangeHistory";
            ActionStartAllProcessInterface.Execute += ActionStartAllProcessInterface_Execute;
        }

        private void ActionStartAllProcessInterface_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            FormAllProcess = new WinCustomProcess.FormAllProcess();
            FormAllProcess.Show();
        }

        protected override void OnDeactivated()
        {
            FormAllProcess.Dispose();
            ActionStartAllProcessInterface.Dispose();
        }
    }
}
