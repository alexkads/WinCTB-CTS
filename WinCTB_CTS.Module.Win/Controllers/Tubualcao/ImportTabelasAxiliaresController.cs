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
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Win.Editors;
using WinCTB_CTS.Module.Comum;
using DevExpress.XtraBars;
using WinCTB_CTS.Module.Win.Actions;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Tubulacao;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportTabelasAxiliaresController : WindowController
    {
        SimpleAction ActionAtualizarTabelasAuxiliares;
        IObjectSpace objectSpace = null;
        ParametrosAtualizacaoTabelasAuxiliares parametrosAtualizacaoTabelasAuxiliares;

        public ImportTabelasAxiliaresController()
        {
            TargetWindowType = WindowType.Main;
            ActionAtualizarTabelasAuxiliares = new SimpleAction(this, "SimpleActionImportTabelasAxiliaresController", PredefinedCategory.RecordEdit)
            {
                Caption = "Atualizar Tabelas Auxiliares",
                ImageName = "UpdateTableOfContents"
            };

            ActionAtualizarTabelasAuxiliares.Execute += ActionAtualizarTabelasAuxiliares_Execute;
        }

        private void ActionAtualizarTabelasAuxiliares_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpace = Application.CreateObjectSpace();
            parametrosAtualizacaoTabelasAuxiliares = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            DetailView view = Application.CreateDetailView(objectSpace, parametrosAtualizacaoTabelasAuxiliares);

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
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting;
            dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }

        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = false;

            //Necessário para não fechar a janela após a conclusão do processamento
            e.Cancel = true;
            e.AcceptActionArgs.Action.Caption = "Procesando";

            var parametros = (ParametrosAtualizacaoTabelasAuxiliares)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }

            var itba = new ImportTabelaAuxiliares(objectSpace, parametrosAtualizacaoTabelasAuxiliares);
            var progress = new Progress<ImportProgressReport>(itba.LogTrace);

            await Observable.Start(() => itba.ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
            await Observable.Start(() => itba.ImportarSchedule(dtcollectionImport["Schedule"], progress));
            await Observable.Start(() => itba.ImportarPercInspecao(dtcollectionImport["PercInspecao"], progress));
            await Observable.Start(() => itba.ImportarProcessoSoldagem(dtcollectionImport["ProcessoSoldagem"], progress));
            await Observable.Start(() => itba.ImportarContrato(dtcollectionImport["Contrato"], progress));
            await Observable.Start(() => itba.ImportarEAP(dtcollectionImport["EAPPipe"], progress));

            objectSpace.CommitChanges();
            e.AcceptActionArgs.Action.Caption = "Finalizado";
            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = true;
        }

        DataTableCollection dtcollectionImport;

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
