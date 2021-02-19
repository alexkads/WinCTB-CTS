using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.Calculator.ProcessoLoteLPPM
{
    class LotesDeEstruturaAlinhamento
    {
        private IObjectSpaceProvider ObjectSpaceProvider;

        public LotesDeEstruturaAlinhamento(IObjectSpaceProvider objectSpaceProvider) => this.ObjectSpaceProvider = objectSpaceProvider;

        public async Task AlinhaLotesLPPM(Guid OidEstabelecimento, IProgress<string> progress)
        {
            using (var ObjectSpace = ObjectSpaceProvider.CreateObjectSpace())
            {
                var lotes = new XPCollection<LoteLPPMEstrutura>(((XPObjectSpace)ObjectSpace).Session, new BinaryOperator("Projeto.Estabelecimento.Oid", OidEstabelecimento));
                lotes.Sorting.Add(new SortProperty("NumeroDoLote", SortingDirection.Ascending));

                double totalDatastore = lotes.EvaluateDatastoreCount();
                double currentProcess = 0D;

                await Observable.ForEachAsync<LoteLPPMEstrutura>(lotes.ToObservable(), lote => {
                    AtualizarStatusLote(lote);
                    ObjectSpace.CommitChanges();
                    currentProcess++;
                    progress.Report($"Inserindo inspeções de (LP ou PM) nos lotes LP/PM Estrutura");
                });
            }
        }

        public static void AtualizarStatusLote(LoteLPPMEstrutura lote)
        {
            int Necessidade = (int)Math.Ceiling(lote.JuntasNoLote * lote.PercentualNivelDeInspecao);
            if (Necessidade == 0)
                Necessidade = 1;
            int Reprovacao = 0;
            int NecessidadeDeInspecaoFinal = 0;
            DateTime DataDaJuntaQueAprovouLote = DateTime.MinValue;
            lote.ComJuntaReprovada = lote.LoteLPPMjuntaEstruturas.Any(x => x.Laudo == InspecaoLaudo.R);
            lote.JuntasNoLote = lote.LoteLPPMjuntaEstruturas.EvaluateDatastoreCount();

            if (lote.LoteLPPMjuntaEstruturas.Count < lote.QuantidadeNecessaria)
                lote.SituacaoQuantidade = SituacoesQuantidade.Incompleto;
            else if (lote.LoteLPPMjuntaEstruturas.Count == lote.QuantidadeNecessaria)
                lote.SituacaoQuantidade = SituacoesQuantidade.Completo;
            lote.JuntasNoLote = lote.LoteLPPMjuntaEstruturas.Count;

            using (var LoteJuntas = new XPCollection<LoteLPPMJuntaEstrutura>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, lote.Session, new BinaryOperator(nameof(LoteLPPMEstrutura), lote.NumeroDoLote)))
            {
                foreach (var LoteJunta in LoteJuntas.OrderBy(o => o.DataInspecao).ToArray())
                {
                    if (LoteJunta.Laudo == InspecaoLaudo.A)
                        Necessidade -= 1;

                    if (Necessidade == 0 && LoteJunta.Laudo == InspecaoLaudo.A)
                        LoteJunta.AprovouLote = true;
                    else
                        LoteJunta.AprovouLote = false;

                    if (LoteJunta.AprovouLote)
                        DataDaJuntaQueAprovouLote = LoteJunta.DataInspecao;

                    if (Necessidade < 0 && LoteJunta.Laudo == InspecaoLaudo.A)
                        LoteJunta.InspecaoExcesso = true;
                    else
                        LoteJunta.InspecaoExcesso = false;
                }

                lote.ComJuntaReprovada = LoteJuntas.Any(x => x.Laudo == InspecaoLaudo.R);
                var NaoInspecionado = lote.LoteLPPMjuntaEstruturas.Where(x => string.IsNullOrEmpty(x.NumeroDoRelatorio)).Count();
                NecessidadeDeInspecaoFinal = Reprovacao > 3 ? NaoInspecionado : Necessidade;
                lote.NecessidadeDeInspecao = NecessidadeDeInspecaoFinal > 0 ? NecessidadeDeInspecaoFinal : 0;
                lote.QuantidadeInspecionada = lote.LoteLPPMjuntaEstruturas.Count(x => !string.IsNullOrEmpty(x.NumeroDoRelatorio));
                lote.ExcessoDeInspecao = LoteJuntas.Count(x => x.InspecaoExcesso);

                if (Necessidade <= 0)
                {
                    lote.SituacaoInspecao = SituacoesInspecao.Aprovado;
                }
                else if (Necessidade > 0)
                {
                    lote.SituacaoInspecao = SituacoesInspecao.Pendente;
                }

#if Test
                if(LoteJuntas.Count(x => x.AprovouLote) > 1)
                    throw new InvalidOperationException("Não é permitido existirem mais de uma junta aprovando um lote");
#endif
            }
        }
    }
}
