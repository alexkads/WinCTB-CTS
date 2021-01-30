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

            var progress = new Progress<ImportProgressReport>(LogTrace);
            await Observable.Start(() => ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
            await Observable.Start(() => ImportarSchedule(dtcollectionImport["Schedule"], progress));
            await Observable.Start(() => ImportarPercInspecao(dtcollectionImport["PercInspecao"], progress));
            await Observable.Start(() => ImportarProcessoSoldagem(dtcollectionImport["ProcessoSoldagem"], progress));

            e.AcceptActionArgs.Action.Caption = "Finalizado";
        }

        private void LogTrace(ImportProgressReport value)
        {
            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
                ? (value.CurrentRow / value.TotalRows)
                : 0D;

            parametrosAtualizacaoTabelasAuxiliares.Progresso = progresso;
            //statusProgess.Text = value.MessageImport;
        }

        private void ImportarDiametro(DataTable dt, IProgress<ImportProgressReport> progress)
        {

            var TotalRows = dt.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dt.Rows[i];
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

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalRows,
                CurrentRow = TotalRows
            });
        }

        private void ImportarSchedule(DataTable dt, IProgress<ImportProgressReport> progress)
        {
            var schedules = ConvertListFromPivot(dt);
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

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalRows,
                CurrentRow = TotalRows
            });
        }

        private void ImportarPercInspecao(DataTable dt, IProgress<ImportProgressReport> progress)
        {

            var TotalRows = dt.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dt.Rows[i];
                    var spec = row[0].ToString();
                    var insp = Utils.ConvertDouble(row[1]) * 0.01D;

                    var criteriaOperator = new BinaryOperator("Spec", spec);
                    var tabperc = uow.FindObject<TabPercInspecao>(criteriaOperator);

                    if (tabperc == null)
                        tabperc = new TabPercInspecao(uow);

                    tabperc.Spec = spec;
                    tabperc.PercentualDeInspecao = insp;

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

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalRows,
                CurrentRow = TotalRows
            });
        }

        private void ImportarProcessoSoldagem(DataTable dt, IProgress<ImportProgressReport> progress)
        {

            var TotalRows = dt.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dt.Rows[i];
                    var eps = row[0].ToString();
                    var raiz = row[1].ToString();
                    var ench = row[2].ToString();

                    var criteriaOperator = new BinaryOperator("Eps", eps);
                    var tabprocesso = uow.FindObject<TabProcessoSoldagem>(criteriaOperator);

                    if (tabprocesso == null)
                        tabprocesso = new TabProcessoSoldagem(uow);

                    tabprocesso.Eps = eps;
                    tabprocesso.Raiz = raiz;
                    tabprocesso.Ench = ench;

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

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalRows,
                CurrentRow = TotalRows
            });
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
