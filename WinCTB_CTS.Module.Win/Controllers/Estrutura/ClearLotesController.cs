//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.Office.Crypto;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using DevExpress.Xpo;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
//using WinCTB_CTS.Module.Calculator.ProcessoLote;
//using WinCTB_CTS.Module.Helpers;
//using WinCTB_CTS.Module.Comum;
//using DevExpress.XtraEditors;

//namespace WinCTB_CTS.Module.Win.Controllers.Estrutura
//{
//    public partial class ClearLotesController : ViewController
//    {
//        public ClearLotesController()
//        {
//            TargetViewType = ViewType.DetailView;
//            TargetObjectType = typeof(ProgressoGerarLotes);

//            var ClearLotesSimpleAction = new SimpleAction(this, "ClearLotesSimpleAction", "DetailViewActions");
//            ClearLotesSimpleAction.Caption = "Limpar Lotes";
//            ClearLotesSimpleAction.Execute += ClearLotesSimpleAction_Execute;
//        }

//        private void ClearLotesSimpleAction_Execute(object sender, SimpleActionExecuteEventArgs e)
//        {
//            var providerDataLayer = new ProviderDataLayer();
//            UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());

//            //progress.Report($"Limpando lotes");
//            WinCTB_CTS.Module.Comum.Utils.DeleteAllRecords<LoteJuntaEstrutura>(uow);
//            WinCTB_CTS.Module.Comum.Utils.DeleteAllRecords<LoteEstrutura>(uow);
//            uow.Dispose();
//            providerDataLayer.Dispose();
//            XtraMessageBox.Show("Lotes foram execluídos!");
//        }
//    }
//}
