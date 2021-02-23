using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;

namespace WinCTB_CTS.Module.Calculator.ProcessoLote
{
    public class LotesDeEstruturaInspecao
    {
        private IObjectSpaceProvider ObjectSpaceProvider;

        public LotesDeEstruturaInspecao(IObjectSpaceProvider objectSpaceProvider) => this.ObjectSpaceProvider = objectSpaceProvider;
                
        public async Task InserirInspecaoLPPMEstrutura(IProgress<string> progress )
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                using (var JuntaComponentes = new XPCollection<JuntaComponente>(((XPObjectSpace)ObjectSpace).Session))
                {
                    CriteriaOperator criteria = CriteriaOperator.Parse("(Not IsNullOrEmpty(DataLP) Or Not IsNullOrEmpty(DataPm)) And LoteLPPMJuntaEstruturas.Exists and LoteLPPMJuntaEstruturas[ IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao) ]");                        

                    JuntaComponentes.Criteria = criteria;
                    JuntaComponentes.Sorting.Add(new SortProperty("Componente", SortingDirection.Ascending));
                    JuntaComponentes.Sorting.Add(new SortProperty("Junta", SortingDirection.Ascending));
                    var registros = JuntaComponentes.Count();
                    var progresso = 0;
                    var observable = JuntaComponentes.ToObservable();
                    observable.Subscribe(current => {

                        foreach (var juntaDoLote in current.LoteJuntaEstruturas)
                        {
                            if (current.DataLP != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioLp;
                                juntaDoLote.DataInspecao = current.DataLP.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            } else if(current.DataPm != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioPm;
                                juntaDoLote.DataInspecao = current.DataPm.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            }                                                                                   
                        }
                        
                        progresso++;
                        try
                        {
                            ObjectSpace.CommitChanges();
                            progress.Report($"Inserindo inspeções de (LP ou PM) nos lotes LP/PM Estrutura");
                        }
                        catch (Exception ex)
                        {
                            ObjectSpace.Rollback();
                            progress.Report($"Erro ao inserir inspeções de (LP) nos lote de LP/PM Estrutura {ex.Message}");
                        }
                    });

                    await observable
                        .Any()
                        .LastAsync();
                }
            }
        }

        public async Task InserirInspecaoRXEstrutura(IProgress<string> progress)
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                using (var JuntaComponentes = new XPCollection<JuntaComponente>(((XPObjectSpace)ObjectSpace).Session))
                {
                    CriteriaOperator criteria = CriteriaOperator.Parse("(Not IsNullOrEmpty(DataLP) Or Not IsNullOrEmpty(DataPm)) And LoteLPPMJuntaEstruturas.Exists and LoteLPPMJuntaEstruturas[ IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao) ]");

                    JuntaComponentes.Criteria = criteria;
                    JuntaComponentes.Sorting.Add(new SortProperty("Componente", SortingDirection.Ascending));
                    JuntaComponentes.Sorting.Add(new SortProperty("Junta", SortingDirection.Ascending));
                    var registros = JuntaComponentes.Count();
                    var progresso = 0;
                    var observable = JuntaComponentes.ToObservable();
                    observable.Subscribe(current => {

                        foreach (var juntaDoLote in current.LoteJuntaEstruturas)
                        {
                            if (current.DataLP != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioLp;
                                juntaDoLote.DataInspecao = current.DataLP.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            }
                            else if (current.DataPm != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioPm;
                                juntaDoLote.DataInspecao = current.DataPm.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            }
                        }

                        progresso++;
                        try
                        {
                            ObjectSpace.CommitChanges();
                            progress.Report($"Inserindo inspeções de (LP ou PM) nos lotes LP/PM Estrutura");
                        }
                        catch (Exception ex)
                        {
                            ObjectSpace.Rollback();
                            progress.Report($"Erro ao inserir inspeções de (LP) nos lote de LP/PM Estrutura {ex.Message}");
                        }
                    });

                    await observable
                        .Any()
                        .LastAsync();
                }
            }
        }

        public async Task InserirInspecaoUSEstrutura(IProgress<string> progress)
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                using (var JuntaComponentes = new XPCollection<JuntaComponente>(((XPObjectSpace)ObjectSpace).Session))
                {
                    CriteriaOperator criteria = CriteriaOperator.Parse("(Not IsNullOrEmpty(DataLP) Or Not IsNullOrEmpty(DataPm)) And LoteLPPMJuntaEstruturas.Exists and LoteLPPMJuntaEstruturas[ IsNullOrEmpty(NumeroDoRelatorio) Or IsNullOrEmpty(DataInspecao) ]");

                    JuntaComponentes.Criteria = criteria;
                    JuntaComponentes.Sorting.Add(new SortProperty("Componente", SortingDirection.Ascending));
                    JuntaComponentes.Sorting.Add(new SortProperty("Junta", SortingDirection.Ascending));
                    var registros = JuntaComponentes.Count();
                    var progresso = 0;
                    var observable = JuntaComponentes.ToObservable();
                    observable.Subscribe(current => {

                        foreach (var juntaDoLote in current.LoteJuntaEstruturas)
                        {
                            if (current.DataLP != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioLp;
                                juntaDoLote.DataInspecao = current.DataLP.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            }
                            else if (current.DataPm != null)
                            {
                                juntaDoLote.NumeroDoRelatorio = current.RelatorioPm;
                                juntaDoLote.DataInspecao = current.DataPm.Value;
                                juntaDoLote.Laudo = BusinessObjects.Comum.InspecaoLaudo.A;
                            }
                        }

                        progresso++;
                        try
                        {
                            ObjectSpace.CommitChanges();
                            progress.Report($"Inserindo inspeções de (LP ou PM) nos lotes LP/PM Estrutura");
                        }
                        catch (Exception ex)
                        {
                            ObjectSpace.Rollback();
                            progress.Report($"Erro ao inserir inspeções de (LP) nos lote de LP/PM Estrutura {ex.Message}");
                        }
                    });

                    await observable
                        .Any()
                        .LastAsync();
                }
            }
        }
    }
}
