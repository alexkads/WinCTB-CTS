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
//using DevExpress.XtraEditors;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Data;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reactive.Concurrency;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using WinCTB_CTS.Module.BusinessObjects.Comum;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
//using WinCTB_CTS.Module.Calculator.ProcessoLote;
//using WinCTB_CTS.Module.Comum;
//using WinCTB_CTS.Module.Helpers;
//using WinCTB_CTS.Module.Importer;
//using WinCTB_CTS.Module.Importer.Estrutura;
//using WinCTB_CTS.Module.Importer.Tubulacao;
//using WinCTB_CTS.Module.Interfaces;
//using WinCTB_CTS.Module.Win.Actions;
//using WinCTB_CTS.Module.Win.Editors;

//namespace WinCTB_CTS.Module.Win.Controllers
//{
//    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
//    public partial class GerarLotesController : WindowController
//    {
//        IObjectSpace objectSpace = null;
//        IObjectSpaceProvider objectSpaceProvider;
//        ProgressoGerarLotes TelaProgressoGerarLotes;
//        public GerarLotesController()
//        {
//            TargetWindowType = WindowType.Main;

//            SimpleAction SimpleActionGerarLotes = new SimpleAction(this, "SimpleActionGerarLotesController", PredefinedCategory.RecordEdit)
//            {
//                Caption = "Gerar Lotes",
//                ImageName = "Action_Debug_Step"
//            };

//            SimpleActionGerarLotes.Execute += SimpleActionImport_Execute;
//        }

//        private void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
//        {
//            objectSpaceProvider = Application.ObjectSpaceProvider;
//            objectSpace = Application.CreateObjectSpace(typeof(ProgressoGerarLotes));
//            TelaProgressoGerarLotes = objectSpace.CreateObject<ProgressoGerarLotes>();
//            DetailView view = Application.CreateDetailView(objectSpace, TelaProgressoGerarLotes);

//            view.ViewEditMode = ViewEditMode.Edit;

//            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
//            e.ShowViewParameters.CreatedView = view;
//            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
//            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
//        }

//        private DialogController dialogControllerAcceptingImportarPlanilha()
//        {
//            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
//            dialogControllerImportarPlanilha.AcceptAction.Caption = "Importar";
//            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting; ;
//            dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
//            return dialogControllerImportarPlanilha;
//        }

//        public void LogTrace(ImportProgressReport value)
//        {
//            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
//                ? (value.CurrentRow / value.TotalRows)
//                : 0D;

//            TelaProgressoGerarLotes.Progresso = progresso;
//        }

//        //private void ClearLotes() {
//        //    var providerDataLayer = new ProviderDataLayer();
//        //    UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());

//        //    //progress.Report($"Limpando lotes");
//        //    Utils.DeleteAllRecords<LoteJuntaEstrutura>(uow);
//        //    Utils.DeleteAllRecords<LoteEstrutura>(uow);
//        //    uow.Dispose();
//        //    providerDataLayer.Dispose();
//        //}

//        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
//        {

//            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = false;
//            //Necessário para não fechar a janela após a conclusão do processamento
//            e.Cancel = true;
//            e.AcceptActionArgs.Action.Caption = "Processando";

//            var telaDeProgresso = (ProgressoGerarLotes)e.AcceptActionArgs.SelectedObjects[0];

//            //Executar em testes;
//            //ClearLotes();

//            var progress = new Progress<ImportProgressReport>(LogTrace);
//            var gerador = new GerarLote();
//            var inspecao = new LotesDeEstruturaInspecao();
//            var alinhamento = new LotesDeEstruturaAlinhamento();
//            var balanceamento = new BalanceamentoDeLotesEstrutura();

//            await gerador.GerarLoteAsync(ENDS.LPPM, progress);
//            telaDeProgresso.ConcluidoLPPM = true;

//            await gerador.GerarLoteAsync(ENDS.RX, progress);
//            telaDeProgresso.ConcluidoRX = true;

//            await gerador.GerarLoteAsync(ENDS.US, progress);
//            telaDeProgresso.ConcluidoUS = true;

//            await inspecao.InserirInspecaoLPPMEstrutura(progress);
//            telaDeProgresso.ConcluidoInspecaoLPPM = true;

//            await inspecao.InserirInspecaoRXEstrutura(progress);
//            telaDeProgresso.ConcluidoInspecaoRX = true;

//            await inspecao.InserirInspecaoUSEstrutura(progress);
//            telaDeProgresso.ConcluidoInspecaoUS = true;

//            await alinhamento.AlinhaLotes(progress);
//            telaDeProgresso.ConcluidoAlinhamentoDeLotes = true;

//            await balanceamento.BalancearLotesEstruturaPorPercentualAsync();
//            telaDeProgresso.ConcluidoBalanceamentoDeLotes = true;

//            objectSpace.CommitChanges();

//            e.AcceptActionArgs.Action.Caption = "Finalizado";
//            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = false;
//        }
//    }
//}
