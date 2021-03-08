//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Xpo;
//using DevExpress.Xpo;
//using DevExpress.Xpo.DB;
//using System;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura;
//using WinCTB_CTS.Module.Helpers;
//using WinCTB_CTS.Module.Importer;
//using WinCTB_CTS.Module.Interfaces;

//namespace WinCTB_CTS.Module.Calculator.ProcessoLote
//{
//    public class LotesDeEstruturaInspecao
//    {
//        private ProviderDataLayer providerDataLayer { get; set; }
//        private XPCollection<JuntaComponente> juntaComponentes { get; set; }

//        public LotesDeEstruturaInspecao()
//        {
//            this.providerDataLayer = new ProviderDataLayer();
//        }

//        public async Task InserirInspecaoLPPMEstrutura(IProgress<ImportProgressReport> progress)
//        {
//            await Task.Factory.StartNew(() =>
//            {
//                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());
//                CriteriaOperator criteria = CriteriaOperator.Parse("(Not IsNullOrEmpty(DataLP) Or Not IsNullOrEmpty(DataPm)) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'LPPM' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao))].Exists");
//                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
//                var registros = JuntaComponentes.Count();
//                var progresso = 0;

//                uow.BeginTransaction();

//                foreach (var current in JuntaComponentes)
//                {
//                    foreach (var juntaDoLote in current.LoteJuntaEstruturas)
//                    {
//                        if (current.DataLP != null)
//                        {
//                            juntaDoLote.NumeroDoRelatorio = current.RelatorioLp;
//                            juntaDoLote.DataInspecao = current.DataLP.Value;
//                            juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
//                        }
//                        else if (current.DataPm != null)
//                        {
//                            juntaDoLote.NumeroDoRelatorio = current.RelatorioPm;
//                            juntaDoLote.DataInspecao = current.DataPm.Value;
//                            juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
//                        }
//                    }

//                    progresso++;
//                    try
//                    {
//                        uow.CommitTransaction();
//                        progress.Report(new ImportProgressReport
//                        {
//                            TotalRows = registros,
//                            CurrentRow = progresso,
//                            MessageImport = $"Importando linha {progresso}/{registros}"
//                        });
//                    }
//                    catch (Exception)
//                    {
//                        uow.RollbackTransaction();
//                    }
//                }

//                uow.CommitTransaction();
//                uow.CommitChanges();
//                uow.Dispose();
//            });
//        }

//        public async Task InserirInspecaoRXEstrutura(IProgress<ImportProgressReport> progress)
//        {
//            await Task.Factory.StartNew(() =>
//            {
//                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());
//                CriteriaOperator criteria = CriteriaOperator.Parse("Not IsNullOrEmpty(DataRx) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'RX' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao)) ].Exists");
//                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
//                var registros = JuntaComponentes.Count();
//                var progresso = 0;

//                uow.BeginTransaction();

//                foreach (var current in JuntaComponentes)
//                {
//                    foreach (var juntaDoLote in current.LoteJuntaEstruturas)
//                    {
//                        juntaDoLote.NumeroDoRelatorio = current.RelatorioRx;
//                        juntaDoLote.DataInspecao = current.DataRx.Value;
//                        juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
//                    }

//                    progresso++;
//                    try
//                    {
//                        uow.CommitTransaction();
//                        progress.Report(new ImportProgressReport
//                        {
//                            TotalRows = registros,
//                            CurrentRow = progresso,
//                            MessageImport = $"Importando linha {progresso}/{registros}"
//                        });
//                    }
//                    catch (Exception)
//                    {
//                        uow.RollbackTransaction();
//                    }
//                }

//                uow.CommitTransaction();
//                uow.CommitChanges();
//                uow.Dispose();
//            });
//        }

//        public async Task InserirInspecaoUSEstrutura(IProgress<ImportProgressReport> progress)
//        {
//            await Task.Factory.StartNew(() =>
//            {
//                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());
//                 CriteriaOperator criteria = CriteriaOperator.Parse("Not IsNullOrEmpty(DataUs) And LoteJuntaEstruturas[ LoteEstrutura.Ensaio == 'US' And (IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao)) ].Exists");
//                var JuntaComponentes = GetJuntaComponentes(uow, criteria);
//                var registros = JuntaComponentes.Count();
//                var progresso = 0;

//                uow.BeginTransaction();

//                foreach (var current in JuntaComponentes)
//                {
//                    foreach (var juntaDoLote in current.LoteJuntaEstruturas)
//                    {
//                        juntaDoLote.NumeroDoRelatorio = current.RelatorioUs;
//                        juntaDoLote.DataInspecao = current.DataUs.Value;
//                        juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
//                    }

//                    progresso++;
//                    try
//                    {
//                        uow.CommitTransaction();
//                        progress.Report(new ImportProgressReport
//                        {
//                            TotalRows = registros,
//                            CurrentRow = progresso,
//                            MessageImport = $"Importando linha {progresso}/{registros}"
//                        });
//                    }
//                    catch (Exception)
//                    {
//                        uow.RollbackTransaction();
//                    }
//                }

//                uow.CommitTransaction();
//                uow.CommitChanges();
//                uow.Dispose();
//            });
//        }

//        public XPCollection<JuntaComponente> GetJuntaComponentes(UnitOfWork uow, CriteriaOperator criteria)
//        {
//            juntaComponentes = new XPCollection<JuntaComponente>(uow);
//            juntaComponentes.Criteria = criteria;
//            juntaComponentes.Sorting.Add(new SortProperty("Componente", SortingDirection.Ascending));
//            juntaComponentes.Sorting.Add(new SortProperty("Junta", SortingDirection.Ascending));
//            return juntaComponentes;
//        }
//    }
//}
