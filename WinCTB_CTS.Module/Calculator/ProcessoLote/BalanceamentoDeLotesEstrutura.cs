using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Calculator.ProcessoLote;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.Calculator.ProcessoLote
{
    public class BalanceamentoDeLotesEstrutura
    {
        public ProviderDataLayer providerDataLayer { get; set; }

        public BalanceamentoDeLotesEstrutura()
        {
            this.providerDataLayer = new ProviderDataLayer();
        }

        public async Task BalancearLotesEstruturaPorPercentualAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetSimpleDataLayer());

                var valuesENDS = Enum.GetValues(typeof(ENDS));

                foreach (ENDS end in valuesENDS)
                {
                    int possibilidades = 0;
                    do
                    {
                        var filterQueryLotesExcesso = new XPQuery<LoteEstrutura>(uow, false).TransformExpression(x => x.Ensaio == end && x.ExcessoDeInspecao > 0 && x.NecessidadeDeInspecao <= 0 && x.LotejuntaEstruturas.Any(l => l.InspecaoExcesso == true));
                        var filterQueryLotesPendente = new XPQuery<LoteEstrutura>(uow, false).TransformExpression(x => x.Ensaio == end && x.NecessidadeDeInspecao > 0 && x.LotejuntaEstruturas.Any(l => l.NumeroDoRelatorio == null));

                        //var QueryLotesExcesso = new XPCollection<LoteEstrutura>(uow, CriteriaOperator.Parse("Ensaio = ? And ExcessoDeInspecao > 0 And NecessidadeDeInspecao <= 0 And LotejuntaEstruturas[ InspecaoExcesso ].Exists", end));
                        //var QueryLotesPendente = new XPCollection<LoteEstrutura>(uow, CriteriaOperator.Parse("Ensaio = ? And NecessidadeDeInspecao > 0 And LotejuntaEstruturas[ IsNull(NumeroDoRelatorio) ].Exists", end));

                        var QueryLotesExcesso = new XPCollection<LoteEstrutura>(uow, filterQueryLotesExcesso);
                        var QueryLotesPendente = new XPCollection<LoteEstrutura>(uow, filterQueryLotesPendente);

                        var lotesComPossibilidade =
                            (from ex in QueryLotesExcesso
                             join pd in QueryLotesPendente on new { ex.PercentualNivelDeInspecao, ex.TipoJunta } equals new { pd.PercentualNivelDeInspecao, pd.TipoJunta } into PendenteGroup
                             where PendenteGroup.Count() > 0
                             select new { Excesso = ex, Pendentes = PendenteGroup.OrderByDescending(od => od.JuntasNoLote).OrderBy(o => o.QuantidadeInspecionada) }).ToList();

                        possibilidades = lotesComPossibilidade.Count();
                        foreach (var l in lotesComPossibilidade)
                        {
                            var loteExcesso = uow.GetObjectByKey<LoteEstrutura>(l.Excesso.NumeroDoLote);
                            var lotePendente = uow.GetObjectByKey<LoteEstrutura>(l.Pendentes.FirstOrDefault().NumeroDoLote);

                            var filtroExecesso = new XPQuery<LoteJuntaEstrutura>(uow, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == loteExcesso.NumeroDoLote && x.NumeroDoRelatorio != null && x.InspecaoExcesso == true);
                            var filtroPendente = new XPQuery<LoteJuntaEstrutura>(uow, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == lotePendente.NumeroDoLote && x.NumeroDoRelatorio == null);

                            var JuntaExcesso = uow.FindObject<LoteJuntaEstrutura>(filtroExecesso);
                            var JuntaPendente = uow.FindObject<LoteJuntaEstrutura>(filtroPendente);

                            if (JuntaPendente != null && JuntaExcesso != null && lotePendente.SituacaoInspecao == SituacoesInspecao.Pendente)
                            {
                                lotePendente.LotejuntaEstruturas.Add(JuntaExcesso);
                                loteExcesso.LotejuntaEstruturas.Add(JuntaPendente);
                                JuntaExcesso.InspecaoExcesso = false;
                                LotesDeEstruturaAlinhamento.AtualizarStatusLote(lotePendente);
                                LotesDeEstruturaAlinhamento.AtualizarStatusLote(loteExcesso);
                                uow.CommitChanges();
                            }
                        }
                    } while (possibilidades > 0);
                }
            });
        }
    }
}
