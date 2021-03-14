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
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Interfaces;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote
{
    public class LotesDeEstruturaAlinhamento : CalculatorProcessBase
    {
        public LotesDeEstruturaAlinhamento(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress) { 
        
        }

        protected override void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            base.OnCalculator(provider, cancellationToken, progress);

            UnitOfWork uow = new UnitOfWork(provider.GetSimpleDataLayer());
            var lotes = new XPCollection<LoteEstrutura>(uow);
            lotes.Sorting.Add(new SortProperty("NumeroDoLote", SortingDirection.Ascending));

            int totalDatastore = lotes.EvaluateDatastoreCount();
            int currentProcess = 0;

            uow.BeginTransaction();

            foreach (var lote in lotes) {
                AtualizarStatusLote(lote);
                currentProcess++;

                if (currentProcess % 100 == 0) {
                    uow.CommitTransaction();
                    progress.Report(new ImportProgressReport {
                        TotalRows = totalDatastore,
                        CurrentRow = currentProcess,
                        MessageImport = $"Alinhamento lotes {currentProcess}/{totalDatastore}"
                    });
                }
            }

            progress.Report(new ImportProgressReport
            {
                TotalRows = totalDatastore,
                CurrentRow = totalDatastore,
                MessageImport = $"Alinhamento finalizado {totalDatastore}/{totalDatastore}"
            });

            uow.CommitTransaction();
            uow.CommitChanges();
            uow.Dispose();
        }

        public static void AtualizarStatusLote(LoteEstrutura lote)
        {
            int Necessidade = (int)Math.Ceiling(lote.JuntasNoLote * lote.PercentualNivelDeInspecao);
            if (Necessidade == 0)
                Necessidade = 1;
            int Reprovacao = 0;
            int NecessidadeDeInspecaoFinal = 0;
            DateTime DataDaJuntaQueAprovouLote = DateTime.MinValue;
            lote.ComJuntaReprovada = lote.LotejuntaEstruturas.Any(x => x.Laudo == InspecaoLaudo.R);
            lote.JuntasNoLote = lote.LotejuntaEstruturas.EvaluateDatastoreCount();

            if (lote.LotejuntaEstruturas.Count < lote.QuantidadeNecessaria)
                lote.SituacaoQuantidade = SituacoesQuantidade.Incompleto;
            else if (lote.LotejuntaEstruturas.Count == lote.QuantidadeNecessaria)
                lote.SituacaoQuantidade = SituacoesQuantidade.Completo;
            lote.JuntasNoLote = lote.LotejuntaEstruturas.Count;

            using (var LoteJuntas = new XPCollection<LoteJuntaEstrutura>(PersistentCriteriaEvaluationBehavior.BeforeTransaction, lote.Session, new BinaryOperator(nameof(LoteEstrutura), lote.NumeroDoLote)))
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
                var NaoInspecionado = lote.LotejuntaEstruturas.Where(x => string.IsNullOrEmpty(x.NumeroDoRelatorio)).Count();
                NecessidadeDeInspecaoFinal = Reprovacao > 3 ? NaoInspecionado : Necessidade;
                lote.NecessidadeDeInspecao = NecessidadeDeInspecaoFinal > 0 ? NecessidadeDeInspecaoFinal : 0;
                lote.QuantidadeInspecionada = lote.LotejuntaEstruturas.Count(x => !string.IsNullOrEmpty(x.NumeroDoRelatorio));
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
