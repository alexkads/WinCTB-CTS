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
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.Calculator.ProcessoLote
{
    class BalanceamentoDeLotesEstrutura
    {
        private IObjectSpaceProvider ObjectSpaceProvider;

        public BalanceamentoDeLotesEstrutura(IObjectSpaceProvider objectSpaceProvider) => this.ObjectSpaceProvider = objectSpaceProvider;

        public void BalancearLotesEstruturaPorPercentualAsync()
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                var session = ((XPObjectSpace)ObjectSpace).Session;
                int possibilidades = 0;
                do
                {
                    var QueryLotesExcesso = new XPCollection<LoteEstrutura>(session, CriteriaOperator.Parse($"ExcessoDeInspecao > 0 And NecessidadeDeInspecao <= 0 And LotejuntaEstruturas[ InspecaoExcesso = 'True' ].Exists"));
                    var QueryLotesPendente = new XPCollection<LoteEstrutura>(session, CriteriaOperator.Parse($"NecessidadeDeInspecao > 0 And LotejuntaEstruturas[ IsNull(NumeroDoRelatorio) ].Exists"));

                    var lotesComPossibilidade =
                        from ex in QueryLotesExcesso
                        join pd in QueryLotesPendente on ex.PercentualNivelDeInspecao equals pd.PercentualNivelDeInspecao into PendenteGroup
                        where PendenteGroup.Count() > 0
                        select new { Excesso = ex, Pendentes = PendenteGroup.OrderByDescending(od => od.JuntasNoLote).OrderBy(o => o.QuantidadeInspecionada) };

                    possibilidades = lotesComPossibilidade.Count();
                    foreach (var l in lotesComPossibilidade)
                    {
                        var loteExcesso = session.GetObjectByKey<LoteEstrutura>(l.Excesso.NumeroDoLote);
                        var lotePendente = session.GetObjectByKey<LoteEstrutura>(l.Pendentes.FirstOrDefault().NumeroDoLote);

                        var filtroExecesso = new XPQuery<LoteJuntaEstrutura>(session, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == loteExcesso.NumeroDoLote && x.NumeroDoRelatorio != null && x.InspecaoExcesso == true);
                        var filtroPendente = new XPQuery<LoteJuntaEstrutura>(session, false).TransformExpression(x => x.LoteEstrutura.NumeroDoLote == lotePendente.NumeroDoLote && x.NumeroDoRelatorio == null);

                        var JuntaExcesso = session.FindObject<LoteJuntaEstrutura>(filtroExecesso);
                        var JuntaPendente = session.FindObject<LoteJuntaEstrutura>(filtroPendente);

                        if (JuntaPendente != null && JuntaExcesso != null && lotePendente.SituacaoInspecao == SituacoesInspecao.Pendente)
                        {
                            lotePendente.LotejuntaEstruturas.Add(JuntaExcesso);
                            loteExcesso.LotejuntaEstruturas.Add(JuntaPendente);
                            JuntaExcesso.InspecaoExcesso = false;
                            LotesDeEstruturaAlinhamento.AtualizarStatusLote(lotePendente);
                            LotesDeEstruturaAlinhamento.AtualizarStatusLote(loteExcesso);
                            ObjectSpace.CommitChanges();
                        }
                    }
                } while (possibilidades > 0);
            }
        }
    }
}
