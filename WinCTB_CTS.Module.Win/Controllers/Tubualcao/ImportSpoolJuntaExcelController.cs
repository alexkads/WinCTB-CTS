using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Tubulacao;
using WinCTB_CTS.Module.Win.Actions;
using WinCTB_CTS.Module.Win.Editors;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportSpoolJuntaExcelController : WindowController
    {
        IObjectSpace objectSpace = null;
        ParametrosImportSpoolJuntaExcel parametrosImportSpoolJuntaExcel;
        public ImportSpoolJuntaExcelController()
        {
            TargetWindowType = WindowType.Main;

            SimpleAction simpleActionImport = new SimpleAction(this, "PopupWindowShowActionImportSpoolJuntaExcelController", PredefinedCategory.RecordEdit)
            {
                Caption = "Importar Tubulação",
                ImageName = "Action_Debug_Step"
            };

            simpleActionImport.Execute += SimpleActionImport_Execute;
        }

        private void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpace = Application.CreateObjectSpace(typeof(ParametrosImportSpoolJuntaExcel));
            parametrosImportSpoolJuntaExcel = objectSpace.CreateObject<ParametrosImportSpoolJuntaExcel>();
            DetailView view = Application.CreateDetailView(objectSpace, parametrosImportSpoolJuntaExcel);

            view.ViewEditMode = ViewEditMode.Edit;

            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
        }

        private DialogController dialogControllerAcceptingImportarPlanilha()
        {
            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
            dialogControllerImportarPlanilha.AcceptAction.Caption = "Importar";
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting; ;
            dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }

        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {

            DataTableCollection dtcollectionImport = null;

            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = false;
            //Necessário para não fechar a janela após a conclusão do processamento
            e.Cancel = true;
            e.AcceptActionArgs.Action.Caption = "Procesando";

            var parametros = (ParametrosImportSpoolJuntaExcel)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }

            var import = new ImportSpoolEJunta(objectSpace, parametrosImportSpoolJuntaExcel);
            var progress = new Progress<ImportProgressReport>(import.LogTrace);

            await Observable.Start(() => import.ImportarSpools(dtcollectionImport["SGS"], progress));
            await Observable.Start(() => import.ImportarJuntas(dtcollectionImport["SGJ"], progress));

            objectSpace.CommitChanges();
            e.AcceptActionArgs.Action.Caption = "Finalizado";
            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = true;
        }
    }
}
