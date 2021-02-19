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
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.Calculator.ProcessoLoteLPPM
{
    class BalanceamentoDeLotesEstrutura
    {
        private IObjectSpaceProvider ObjectSpaceProvider;

        public BalanceamentoDeLotesEstrutura(IObjectSpaceProvider objectSpaceProvider) => this.ObjectSpaceProvider = objectSpaceProvider;

        public void BalancearLotesLPPMEstruturaPorPercentualAsync()
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                var session = ((XPObjectSpace)ObjectSpace).Session;
                int possibilidades = 0;
                do
                {
                    var QueryLotesExcesso = new XPCollection<LoteLPPMEstrutura>(session, CriteriaOperator.Parse($"ExcessoDeInspecao > 0 And NecessidadeDeInspecao <= 0 And LoteLPPMjuntaEstruturas[ InspecaoExcesso = 'True' ].Exists"));
                    var QueryLotesPendente = new XPCollection<LoteLPPMEstrutura>(session, CriteriaOperator.Parse($"NecessidadeDeInspecao > 0 And LoteLPPMjuntaEstruturas[ IsNull(NumeroDoRelatorio) ].Exists"));

                    var lotesComPossibilidade =
                        from ex in QueryLotesExcesso
                        join pd in QueryLotesPendente on ex.PercentualNivelDeInspecao equals pd.PercentualNivelDeInspecao into PendenteGroup
                        where PendenteGroup.Count() > 0
                        select new { Excesso = ex, Pendentes = PendenteGroup.OrderByDescending(od => od.JuntasNoLote).OrderBy(o => o.QuantidadeInspecionada) };

                    possibilidades = lotesComPossibilidade.Count();
                    foreach (var l in lotesComPossibilidade)
                    {
                        var loteExcesso = session.GetObjectByKey<LoteLPPMEstrutura>(l.Excesso.NumeroDoLote);
                        var lotePendente = session.GetObjectByKey<LoteLPPMEstrutura>(l.Pendentes.FirstOrDefault().NumeroDoLote);

                        var filtroExecesso = new XPQuery<LoteLPPMJuntaEstrutura>(session, false).TransformExpression(x => x.LoteLPPMEstrutura.NumeroDoLote == loteExcesso.NumeroDoLote && x.NumeroDoRelatorio != null && x.InspecaoExcesso == true);
                        var filtroPendente = new XPQuery<LoteLPPMJuntaEstrutura>(session, false).TransformExpression(x => x.LoteLPPMEstrutura.NumeroDoLote == lotePendente.NumeroDoLote && x.NumeroDoRelatorio == null);

                        var JuntaExcesso = session.FindObject<LoteLPPMJuntaEstrutura>(filtroExecesso);
                        var JuntaPendente = session.FindObject<LoteLPPMJuntaEstrutura>(filtroPendente);

                        if (JuntaPendente != null && JuntaExcesso != null && lotePendente.SituacaoInspecao == SituacoesInspecao.Pendente)
                        {
                            lotePendente.LoteLPPMjuntaEstruturas.Add(JuntaExcesso);
                            loteExcesso.LoteLPPMjuntaEstruturas.Add(JuntaPendente);
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
