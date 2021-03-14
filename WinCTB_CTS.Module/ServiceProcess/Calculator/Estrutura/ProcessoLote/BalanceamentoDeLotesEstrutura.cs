using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Interfaces;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote {
    public class BalanceamentoDeLotesEstrutura : CalculatorProcessBase {
        public BalanceamentoDeLotesEstrutura(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
            : base(cancellationToken, progress) {
        }

        protected override void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            base.OnCalculator(provider, cancellationToken, progress);

            var uow = new UnitOfWork(provider.GetSimpleDataLayer());
            var contratos = uow.QueryInTransaction<Contrato>();
            var valuesENDS = Enum.GetValues(typeof(ENDS));
    
            foreach (var contrato in contratos) {
                foreach (ENDS end in valuesENDS) {

                    int possibilidadesPrevista = 0;
                    int possibilidadesAtual = 0;

                    progress.Report(new ImportProgressReport {
                        TotalRows = 0,
                        CurrentRow = 0,
                        MessageImport = $"Verificando Lote de {end.ToString()} para balancemanto em {contrato.NomeDoContrato}..."
                    });

                    do {
                        var filterQueryLotesExcesso = new XPQuery<LoteEstrutura>(uow, false).TransformExpression(x => x.Contrato.Oid == contrato.Oid && x.Ensaio == end && x.ExcessoDeInspecao > 0 && x.NecessidadeDeInspecao <= 0 && x.LotejuntaEstruturas.Any(l => l.InspecaoExcesso == true));
                        var filterQueryLotesPendente = new XPQuery<LoteEstrutura>(uow, false).TransformExpression(x => x.Contrato.Oid == contrato.Oid && x.Ensaio == end && x.NecessidadeDeInspecao > 0 && x.LotejuntaEstruturas.Any(l => l.NumeroDoRelatorio == null));

                        var QueryLotesExcesso = new XPCollection<LoteEstrutura>(uow, filterQueryLotesExcesso);
                        var QueryLotesPendente = new XPCollection<LoteEstrutura>(uow, filterQueryLotesPendente);

                        var lotesComPossibilidade =
                            (from ex in QueryLotesExcesso
                             join pd in QueryLotesPendente on new { ex.PercentualNivelDeInspecao, ex.TipoJunta } equals new { pd.PercentualNivelDeInspecao, pd.TipoJunta } into PendenteGroup
                             where PendenteGroup.Count() > 0
                             select new { Excesso = ex, Pendentes = PendenteGroup.OrderByDescending(od => od.JuntasNoLote).OrderBy(o => o.QuantidadeInspecionada) }).ToList();

                        possibilidadesAtual = lotesComPossibilidade.Count();

                        if (possibilidadesPrevista == 0)
                            possibilidadesPrevista = possibilidadesAtual;

                        foreach (var l in lotesComPossibilidade) {
                            var loteExcesso = uow.GetObjectByKey<LoteEstrutura>(l.Excesso.NumeroDoLote);
                            var lotePendente = uow.GetObjectByKey<LoteEstrutura>(l.Pendentes.FirstOrDefault().NumeroDoLote);

                            var filtroExecesso = new XPQuery<LoteJuntaEstrutura>(uow, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == loteExcesso.NumeroDoLote && x.NumeroDoRelatorio != null && x.InspecaoExcesso == true);
                            var filtroPendente = new XPQuery<LoteJuntaEstrutura>(uow, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == lotePendente.NumeroDoLote && x.NumeroDoRelatorio == null);

                            var JuntaExcesso = uow.FindObject<LoteJuntaEstrutura>(filtroExecesso);
                            var JuntaPendente = uow.FindObject<LoteJuntaEstrutura>(filtroPendente);

                            //De-Para
                            if (JuntaPendente != null && JuntaExcesso != null && lotePendente.SituacaoInspecao == SituacoesInspecao.Pendente) {

                                //de                                
                                JuntaExcesso.InspecaoExcesso = false;
                                lotePendente.LotejuntaEstruturas.Add(JuntaExcesso);

                                //para
                                loteExcesso.LotejuntaEstruturas.Add(JuntaPendente);

                                //Marcar quando foi executado balanceamaneto
                                lotePendente.ExecutouBalanceamentoEm = DateTime.UtcNow;
                                loteExcesso.ExecutouBalanceamentoEm = DateTime.UtcNow;
                                uow.CommitChanges();

                                LotesDeEstruturaAlinhamento.AtualizarStatusLote(lotePendente);
                                LotesDeEstruturaAlinhamento.AtualizarStatusLote(loteExcesso);
                                uow.CommitChanges();
                            }
                        }
                        progress.Report(new ImportProgressReport {
                            TotalRows = possibilidadesPrevista,
                            CurrentRow = possibilidadesPrevista - possibilidadesAtual,
                            MessageImport = $"Balanceando Lotes de {end.ToString()} - {contrato.NomeDoContrato} | {(possibilidadesPrevista - possibilidadesAtual).ToString().PadLeft(5, '0')}/{possibilidadesAtual.ToString().PadLeft(5, '0')}"
                        });
                    } while (possibilidadesAtual > 0);

                    progress.Report(new ImportProgressReport {
                        TotalRows = possibilidadesPrevista,
                        CurrentRow = possibilidadesPrevista,
                        MessageImport = $"Balanceando Lotes de {end.ToString()} - {contrato.NomeDoContrato} | {(possibilidadesPrevista - possibilidadesAtual).ToString().PadLeft(5, '0')}/{possibilidadesAtual.ToString().PadLeft(5, '0')}"
                    });
                }
            }
        }
    }
}
