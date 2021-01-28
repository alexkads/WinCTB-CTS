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

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportTabelasAxiliaresController : WindowController
    {
        SimpleAction ActionAtualizarTabelasAuxiliares;
        IObjectSpace objectSpace = null;
        ProgressBarControl progressbar;

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
            var param = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            DetailView view = Application.CreateDetailView(objectSpace, param);
            var ProgressPropertyEditor = ((DetailView)view).FindItem("Progresso") as PropertyEditor;
            ProgressPropertyEditor.ControlCreated += ProgressPropertyEditor_ControlCreated;

            //ProgressPropertyEditor = (WinProgressPropertyEditor)view.FindItem("Progresso");
            view.ViewEditMode = ViewEditMode.Edit;

            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
        }

        private void ProgressPropertyEditor_ControlCreated(object sender, EventArgs e)
        {
            //(((PropertyEditor)sender).Control as TaskProgressBarControl).Properties = 100; ;
            progressbar = ((WinProgressPropertyEditor)sender).Control as ProgressBarControl;
            progressbar.EditValue = 20;
            progressbar.Properties.PercentView = true;
            progressbar.Update();
        }

        private DialogController dialogControllerAcceptingImportarPlanilha()
        {
            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting;
            //dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }


        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            var parametros = (ParametrosAtualizacaoTabelasAuxiliares)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            //Executar importação da planilha anexo em stream
            await ImportAndAddToSpreadsheetStream(stream);
            //FormProgressImport.Close();

        }

        DataTableCollection dtcollectionImport;
        //private XtraForm FormProgressImport;
        //private ProgressBarControl progressBarControl;
        //private SimpleButton cancelProgress;
        //private LabelControl statusProgess;

        //private void InitializeInteface()
        //{
        //    FormProgressImport = new XtraProgressImport();

        //    progressBarControl = FormProgressImport.Controls.OfType<ProgressBarControl>().FirstOrDefault();
        //    statusProgess = FormProgressImport.Controls.OfType<LabelControl>().FirstOrDefault();
        //    cancelProgress = FormProgressImport.Controls.OfType<SimpleButton>().FirstOrDefault();

        //    progressBarControl.Properties.ShowTitle = true;
        //    progressBarControl.Properties.Step = 1;
        //    progressBarControl.Properties.PercentView = true;
        //    progressBarControl.Properties.Minimum = 0;

        //    FormProgressImport.Show();
        //}

        private async Task ImportAndAddToSpreadsheetStream(MemoryStream stream)
        {
            //var dtPlanilha = OpenXMLHelper.Excel.Reader.Read(stream);
            //var NestedObjectSpace = View.ObjectSpace.CreateNestedObjectSpace();
            //var sessionimp = ((XPObjectSpace)NestedObjectSpace).Session;

            //InitializeInteface();

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }

            var progress = new Progress<double>(value =>
            {
                var teste = value;

                progressbar.PerformStep();
                progressbar.Update();

                if (progressbar != null)
                    progressbar.EditValue = value;
            });

            await ImportarDiametro(dtcollectionImport["TabDiametro"], progress);
            await ImportarSchedule(dtcollectionImport["Schedule"], progress);
        }

        private async Task ImportarDiametro(DataTable dtSchedule, IProgress<double> progress)
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

                    progress.Report(50);
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
            await uow.CommitChangesAsync();
            uow.Dispose();
        }
        private async Task ImportarSchedule(DataTable dtSchedule, IProgress<double> progress)
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

                progress.Report(50);

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
            await uow.CommitChangesAsync();
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
