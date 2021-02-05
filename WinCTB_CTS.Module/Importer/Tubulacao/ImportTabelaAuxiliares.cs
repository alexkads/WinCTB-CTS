using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;

namespace WinCTB_CTS.Module.Importer.Tubulacao
{
    public class ImportTabelaAuxiliares
    {
        IObjectSpace objectSpace = null;
        ParametrosAtualizacaoTabelasAuxiliares parametrosAtualizacaoTabelasAuxiliares;

        public ImportTabelaAuxiliares(IObjectSpace _objectSpace, ParametrosAtualizacaoTabelasAuxiliares _parametrosAtualizacaoTabelasAuxiliares)
        {
            this.objectSpace = _objectSpace;
            this.parametrosAtualizacaoTabelasAuxiliares = _parametrosAtualizacaoTabelasAuxiliares;
        }

        public void LogTrace(ImportProgressReport value)
        {
            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
                ? (value.CurrentRow / value.TotalRows)
                : 0D;

            parametrosAtualizacaoTabelasAuxiliares.Progresso = progresso;
            //statusProgess.Text = value.MessageImport;
        }

        public void ImportarDiametro(DataTable dt, IProgress<ImportProgressReport> progress)
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
                    var wdi = Utils.ConvertDouble(row[1]);
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

        public void ImportarSchedule(DataTable dt, IProgress<ImportProgressReport> progress)
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

        public void ImportarPercInspecao(DataTable dt, IProgress<ImportProgressReport> progress)
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

        public void ImportarProcessoSoldagem(DataTable dt, IProgress<ImportProgressReport> progress)
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

        public void ImportarContrato(DataTable dt, IProgress<ImportProgressReport> progress)
        {
            var TotalRows = dt.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dt.Rows[i];
                    var siteNome = row[0].ToString();

                    var criteriaOperator = new BinaryOperator("NomeDoContrato", siteNome);
                    var contrato = uow.FindObject<Contrato>(criteriaOperator);

                    if (contrato == null)
                        contrato = new Contrato(uow);

                    contrato.NomeDoContrato = siteNome;

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

        public void ImportarEAP(DataTable dt, IProgress<ImportProgressReport> progress)
        {
            var TotalRows = dt.Rows.Count;

            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            uow.BeginTransaction();

            for (int i = 0; i < TotalRows; i++)
            {
                if (i > 0)
                {
                    var row = dt.Rows[i];

                    Func<string, int, object> lheader = (header, indexRow) =>
                    {
                        var idxcol = dt.Rows[0].ItemArray.ToList().IndexOf(header);
                        return (dt.Rows[indexRow])[idxcol];
                    };

                    var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", lheader("Contrato", i).ToString()));

                    var criteriaOperator = new BinaryOperator("Contrato.Oid", contrato);
                    var TabContrato = uow.FindObject<TabEAPPipe>(criteriaOperator);

                    if (TabContrato == null)
                        TabContrato = new TabEAPPipe(uow);

                    TabContrato.Contrato = contrato;
                    TabContrato.AvancoSpoolCorteFab = Utils.ConvertDouble(lheader("AvancoSpoolCorteFab", i));
                    TabContrato.AvancoSpoolVAFab = Utils.ConvertDouble(lheader("AvancoSpoolVAFab", i));
                    TabContrato.AvancoSpoolSoldaFab = Utils.ConvertDouble(lheader("AvancoSpoolSoldaFab", i));
                    TabContrato.AvancoSpoolENDFab = Utils.ConvertDouble(lheader("AvancoSpoolENDFab", i));
                    TabContrato.AvancoSpoolPosicionamento = Utils.ConvertDouble(lheader("AvancoSpoolPosicionamento", i));

                    TabContrato.AvancoJuntaVAMont = Utils.ConvertDouble(lheader("AvancoJuntaVAMont", i));
                    TabContrato.AvancoJuntaSoldMont = Utils.ConvertDouble(lheader("AvancoJuntaSoldMont", i));
                    TabContrato.AvancoJuntaENDMont = Utils.ConvertDouble(lheader("AvancoJuntaENDMont", i));
                    TabContrato.AvancoSpoolLineCheck = Utils.ConvertDouble(lheader("AvancoSpoolLineCheck", i));

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
                            wdi = Utils.ConvertDouble(((dt.Rows[0])[idxcol]).ToString()),
                            scheduleTag = row[idxcol].ToString()
                        });
                    }
                }
            }

            return result;
        };
    }

    public class ScheduleMapping
    {
        public int numeroLinha { get; set; }
        public string pipingClass { get; set; }
        public string material { get; set; }
        public double wdi { get; set; }
        public string scheduleTag { get; set; }

    }
}
