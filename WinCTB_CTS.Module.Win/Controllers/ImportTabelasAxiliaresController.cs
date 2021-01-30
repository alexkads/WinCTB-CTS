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

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportTabelasAxiliaresController : WindowController
    {
        SimpleAction ActionAtualizarTabelasAuxiliares;
        IObjectSpace objectSpace = null;
        ProgressBarControl progressbar;
        WinProgressPropertyEditor winProgressPropertyEditor;

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

        private void PopupWindowShowAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            objectSpace = Application.CreateObjectSpace(typeof(ParametrosAtualizacaoTabelasAuxiliares));
            var param = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            DetailView view = Application.CreateDetailView(objectSpace, param, true);

            view.ViewEditMode = ViewEditMode.Edit;

            DialogController dialogController = Application.CreateController<DialogController>();
            dialogController.Accepting += DialogController_Accepting;
            dialogController.CancelAction.Active["NoAccept"] = false;
            //e.DialogController.SaveOnAccept = false;
            e.DialogController.Controllers.Add(dialogController);
            e.View = view;
        }

        private void DialogController_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ItemClickSimpleAction_CustomizeControl(object sender, CustomizeControlEventArgs e)
        {
            //Inside BarManager. 
            BarButtonItem barItem = e.Control as BarButtonItem;
            if (barItem != null)
            {
                barItem.ItemClick += (s, args) =>
                {
                    XtraMessageBox.Show("Item Clicked");
                };
            }
            else
            {
                //Inside LayoutManager.
                SimpleButton button = e.Control as SimpleButton;
                if (button != null)
                {
                    button.Click += (s, args) =>
                    {
                        XtraMessageBox.Show("Item Clicked");
                    };
                }
            }
        }

        private void ActionAtualizarTabelasAuxiliares_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpace = Application.CreateObjectSpace();
            var param = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            DetailView view = Application.CreateDetailView(objectSpace, param);
            winProgressPropertyEditor = (WinProgressPropertyEditor)view.FindItem("Progresso");
            winProgressPropertyEditor.ControlCreated += WinProgressPropertyEditor_ControlCreated;

            view.ViewEditMode = ViewEditMode.Edit;

            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
        }

        private void WinProgressPropertyEditor_ControlCreated(object sender, EventArgs e)
        {
            progressbar = ((WinProgressPropertyEditor)sender).Control as ProgressBarControl;
            progressbar.Properties.PercentView = false;
            progressbar.Update();
        }

        private DialogController dialogControllerAcceptingImportarPlanilha()
        {
            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
            dialogControllerImportarPlanilha.AcceptAction.Caption = "Importar";
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting;
            dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }

        DataTableCollection dtcollectionImport;

        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
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

            //var progress = new Progress<ImportProgressReport>(LogTrace);
            //await Task.Run(() => ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
            //await Task.Run(() => ImportarSchedule(dtcollectionImport["Schedule"], progress));


            //Observable.StartAsync(async () =>
            //{
                var progress = new Progress<ImportProgressReport>(LogTrace);
                await Task.Run(() => ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
                await Task.Run(() => ImportarSchedule(dtcollectionImport["Schedule"], progress));
            //});

            e.AcceptActionArgs.Action.Caption = "Finalizado";


            //Observable.StartAsync(async () =>
            //{
            //    var progress = new Progress<ImportProgressReport>(LogTrace);
            //    await Task.Run(() => ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
            //    //await Task.Run(() => ImportarSchedule(dtcollectionImport["Schedule"], progress));
            //}, NewThreadScheduler.Default);
        }

        private void LogTrace(ImportProgressReport value)
        {
            //ProgressPropertyEditor.ControlValue = value.CurrentRow;
            //winProgressPropertyEditor.PropertyValue = 100;


            if (progressbar.Properties.Maximum != value.TotalRows)
            {
                //progressbar.Properties.Maximum = value.TotalRows;
                //progressbar.Refresh();                                  
            }

            if (value.CurrentRow > 0)
                progressbar.PerformStep();

            progressbar.Update();
        }

        private void ImportarDiametro(DataTable dtSchedule, IProgress<ImportProgressReport> progress)
        {

            var TotalRows = dtSchedule.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dtSchedule.Rows[i];
                    var polegada = row[0].ToString();
                    var wdi = row[1].ToString();
                    var mm = Utils.ConvertINT(row[2]);

                    var criteriaOperator = new BinaryOperator("DiametroPolegada", polegada);
                    var tabDiametro = uow.FindObject<TabDiametro>(criteriaOperator);

                    if (tabDiametro == null)
                        tabDiametro = new TabDiametro(uow);

                    tabDiametro.DiametroPolegada = polegada;
                    tabDiametro.DiametroMilimetro = mm;
                    tabDiametro.Wdi = wdi;

                    progress.Report(new ImportProgressReport
                    {
                        TotalRows = TotalRows,
                        CurrentRow = i,
                    });
                }

                if (i % 10 == 0)
                {
                    try
                    {
                        uow.CommitTransaction();
                    }
                    catch
                    {
                        uow.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }
                }
            }

            uow.CommitTransaction();
            uow.CommitChanges();
            uow.Dispose();
        }

        private void ImportarSchedule(DataTable dtSchedule, IProgress<ImportProgressReport> progress)
        {
            var schedules = ConvertListFromPivot(dtSchedule);
            var TotalRows = schedules.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                var criteriaOperator = CriteriaOperator.Parse("PipingClass = ? And Material = ? And TabDiametro.Wdi = ? And ScheduleTag = ?",
                     schedules[i].pipingClass, schedules[i].material, schedules[i].wdi, schedules[i].scheduleTag);

                var tabSchedule = uow.FindObject<TabSchedule>(criteriaOperator);

                if (tabSchedule == null)
                    tabSchedule = new TabSchedule(uow);

                tabSchedule.PipingClass = schedules[i].pipingClass;
                tabSchedule.Material = schedules[i].material;
                tabSchedule.TabDiametro = uow.FindObject<TabDiametro>(new BinaryOperator("Wdi", schedules[i].wdi));
                tabSchedule.ScheduleTag = schedules[i].scheduleTag;

                progress.Report(new ImportProgressReport
                {
                    TotalRows = TotalRows,
                    CurrentRow = i,
                });

                if (i % 10 == 0)
                {
                    try
                    {
                        uow.CommitTransaction();
                    }
                    catch
                    {
                        uow.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }
                }
            }

            uow.CommitTransaction();
            uow.CommitChanges();
            uow.Dispose();
        }

        static private Func<DataTable, IList<ScheduleMapping>> ConvertListFromPivot = (dt) =>
         {
             var result = new List<ScheduleMapping>();

             for (int idxrow = 0; idxrow < dt.Rows.Count; idxrow++)
             {
                 var row = dt.Rows[idxrow];

                 if (idxrow > 0)
                 {
                     for (int idxcol = 2; idxcol < row.ItemArray.Length; idxcol++)
                     {
                         result.Add(new ScheduleMapping
                         {
                             numeroLinha = idxrow,
                             pipingClass = row[0].ToString(),
                             material = row[1].ToString(),
                             wdi = ((dt.Rows[0])[idxcol]).ToString(),
                             scheduleTag = row[idxcol].ToString()
                         });
                     }
                 }
             }

             return result;
         };

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

        }
    }

    class ScheduleMapping
    {
        public int numeroLinha { get; set; }
        public string pipingClass { get; set; }
        public string material { get; set; }
        public string wdi { get; set; }
        public string scheduleTag { get; set; }

    }
}
