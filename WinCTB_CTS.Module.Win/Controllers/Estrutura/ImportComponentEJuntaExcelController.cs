﻿using DevExpress.Data.Filtering;
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
using WinCTB_CTS.Module.Importer.Estrutura;
using WinCTB_CTS.Module.Importer.Tubulacao;
using WinCTB_CTS.Module.Win.Actions;
using WinCTB_CTS.Module.Win.Editors;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportComponentEJuntaExcelController : WindowController
    {
        IObjectSpace objectSpace = null;
        IObjectSpaceProvider objectSpaceProvider;
        ParametrosImportComponentEJunta parametrosImportComponentEJunta;
        public ImportComponentEJuntaExcelController()
        {
            TargetWindowType = WindowType.Main;

            SimpleAction simpleActionImport = new SimpleAction(this, "PopupWindowShowActionImportComponentEJuntaExcelController", PredefinedCategory.RecordEdit)
            {
                Caption = "Importar Estrutura",
                ImageName = "Action_Debug_Step"
            };

            simpleActionImport.Execute += SimpleActionImport_Execute;
        }

        private void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpaceProvider = Application.ObjectSpaceProvider;
            objectSpace = Application.CreateObjectSpace(typeof(ParametrosImportSpoolJuntaExcel));
            parametrosImportComponentEJunta = objectSpace.CreateObject<ParametrosImportComponentEJunta>();
            DetailView view = Application.CreateDetailView(objectSpace, parametrosImportComponentEJunta);

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

            var parametros = (ParametrosImportComponentEJunta)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }


            var import = new ImportComponentEJunta(objectSpaceProvider, parametrosImportComponentEJunta);
            var progress = new Progress<ImportProgressReport>(import.LogTrace);
            var simpleProgress = new Progress<string>();

            await Observable.Start(() => import.ImportarComponente(dtcollectionImport["Piece"], progress));
            parametros.ConcluidoComponente = true;

            await Observable.Start(() => import.ImportarJuntas(dtcollectionImport["Joints"], progress));
            parametros.ConcluidoJuntas = true;
                                   
            //var gerador = new Calculator.ProcessoLoteLPPM.GerarLoteLPPM(objectSpaceProvider);
            //await gerador.GerarLoteLPPMAsync(simpleProgress);
            //parametros.ConcluidoLoteLPPM = true;                      


            objectSpace.CommitChanges();
            e.AcceptActionArgs.Action.Caption = "Finalizado";
            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = true;
        }
    }
}
