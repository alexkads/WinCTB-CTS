//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.ExpressApp.Xpo;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using DevExpress.Xpo;
//using DevExpress.XtraBars;
//using DevExpress.XtraEditors;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;
//using WinCTB_CTS.Module.Comum;

//namespace WinCTB_CTS.Module.Win.Controllers.Estrutura
//{
//    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
//    public partial class ClearDBController : WindowController
//    {
//        SimpleAction ActionClearDB;
//        public ClearDBController()
//        {
//            TargetWindowType = WindowType.Main;
//            ActionClearDB = new SimpleAction(this, "ActionClearDBEstrutura", PredefinedCategory.RecordEdit)
//            {
//                Caption = "Excluir Estrutura",
//                ImageName = "ClearAll"
//            };

//            ActionClearDB.CustomizeControl += ActionClearDB_CustomizeControl;
//        }

//        private void ActionClearDB_CustomizeControl(object sender, CustomizeControlEventArgs e)
//        {
//            BarButtonItem barItem = e.Control as BarButtonItem;
//            if (barItem != null)
//            {
//                barItem.ItemClick += (s, args) => {
                    
//                    var objectSpace = Application.CreateObjectSpace();
//                    //UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
//                    UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.DataLayer);

//                    Utils.DeleteAllRecords<Componente>(uow);
//                    Utils.DeleteAllRecords<JuntaComponente>(uow);

//                    uow.PurgeDeletedObjects();
//                    uow.CommitChanges();

//                    uow.Dispose();
//                    objectSpace.Dispose();
//                    XtraMessageBox.Show("Componentes e Juntas de Estrura foram execluídos!");
//                };
//            }
//        }

//        protected override void OnActivated()
//        {
//            base.OnActivated();
//        }
//        protected override void OnDeactivated()
//        {
//            base.OnDeactivated();
//        }
//    }
//}
