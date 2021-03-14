using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote {
    public class LotesDeEstruturaInspecao : CalculatorProcessBase {
        private XPCollection<JuntaComponente> juntaComponentes { get; set; }

        public LotesDeEstruturaInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress) { }

        protected override async void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            base.OnCalculator(provider, cancellationToken, progress);

            await InserirInspecaoLPPMEstrutura(provider, cancellationToken, progress);
            await InserirInspecaoRXEstrutura(provider, cancellationToken, progress);
            await InserirInspecaoUSEstrutura(provider, cancellationToken, progress);
        }

        public async Task InserirInspecaoLPPMEstrutura(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            await Task.Run(() => {
                UnitOfWork uow = new UnitOfWork(provider.GetSimpleDataLayer());
                CriteriaOperator criteria = CriteriaOperator.Parse("(Not IsNullOrEmpty(DataLP) Or Not IsNullOrEmpty(DataPm)) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'LPPM' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao))].Exists");
                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
                var registros = JuntaComponentes.Count();
                var progresso = 0;

                uow.BeginTransaction();

                foreach (var current in JuntaComponentes) {
                    foreach (var juntaDoLote in current.LoteJuntaEstruturas) {
                        if (current.DataLP != null) {
                            juntaDoLote.NumeroDoRelatorio = current.RelatorioLp;
                            juntaDoLote.DataInspecao = current.DataLP.Value;
                            juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                        } else if (current.DataPm != null) {
                            juntaDoLote.NumeroDoRelatorio = current.RelatorioPm;
                            juntaDoLote.DataInspecao = current.DataPm.Value;
                            juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                        }
                    }

                    progresso++;
                    try {
                        uow.CommitTransaction();
                        progress.Report(new ImportProgressReport {
                            TotalRows = registros,
                            CurrentRow = progresso,
                            MessageImport = $"Inserindo inspeção de LP/PM {progresso}/{registros}"
                        });
                    } catch (Exception) {
                        uow.RollbackTransaction();
                    }
                }

                uow.CommitTransaction();
                uow.CommitChanges();
                uow.Dispose();
            });
        }

        public async Task InserirInspecaoRXEstrutura(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            await Task.Run(() => {
                UnitOfWork uow = new UnitOfWork(provider.GetSimpleDataLayer());
                CriteriaOperator criteria = CriteriaOperator.Parse("Not IsNullOrEmpty(DataRx) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'RX' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao)) ].Exists");
                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
                var registros = JuntaComponentes.Count();
                var progresso = 0;

                uow.BeginTransaction();

                foreach (var current in JuntaComponentes) {
                    foreach (var juntaDoLote in current.LoteJuntaEstruturas) {
                        juntaDoLote.NumeroDoRelatorio = current.RelatorioRx;
                        juntaDoLote.DataInspecao = current.DataRx.Value;
                        juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                    }

                    progresso++;
                    try {
                        uow.CommitTransaction();
                        progress.Report(new ImportProgressReport {
                            TotalRows = registros,
                            CurrentRow = progresso,
                            MessageImport = $"Inserindo inspeção de RX {progresso}/{registros}"
                        });
                    } catch (Exception) {
                        uow.RollbackTransaction();
                    }
                }

                uow.CommitTransaction();
                uow.CommitChanges();
                uow.Dispose();
            });
        }

        public async Task InserirInspecaoUSEstrutura(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            await Task.Run(() => {
                UnitOfWork uow = new UnitOfWork(provider.GetSimpleDataLayer());
                CriteriaOperator criteria = CriteriaOperator.Parse("Not IsNullOrEmpty(DataUs) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'US' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao)) ].Exists");
                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
                var registros = JuntaComponentes.Count();
                var progresso = 0;

                uow.BeginTransaction();

                foreach (var current in JuntaComponentes) {
                    foreach (var juntaDoLote in current.LoteJuntaEstruturas) {
                        juntaDoLote.NumeroDoRelatorio = current.RelatorioUs;
                        juntaDoLote.DataInspecao = current.DataUs.Value;
                        juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                    }

                    progresso++;
                    try {
                        uow.CommitTransaction();
                        progress.Report(new ImportProgressReport {
                            TotalRows = registros,
                            CurrentRow = progresso,
                            MessageImport = $"Inserindo inspeção de US {progresso}/{registros}"
                        });
                    } catch (Exception) {
                        uow.RollbackTransaction();
                    }
                }

                uow.CommitTransaction();
                uow.CommitChanges();
                uow.Dispose();
            });
        }

        public XPCollection<JuntaComponente> GetJuntaComponentes(UnitOfWork uow, CriteriaOperator criteria) {
            juntaComponentes = new XPCollection<JuntaComponente>(uow);
            juntaComponentes.Criteria = criteria;
            juntaComponentes.Sorting.Add(new SortProperty("Componente", SortingDirection.Ascending));
            juntaComponentes.Sorting.Add(new SortProperty("Junta", SortingDirection.Ascending));
            return juntaComponentes;
        }
    }
}
