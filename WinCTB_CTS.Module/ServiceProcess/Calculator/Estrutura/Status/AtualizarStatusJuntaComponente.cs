using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.Medicao {
    public class AtualizarStatusJuntaComponente : CalculatorProcessBase {

        public event EventHandler<(Session session, MedicaoEstrutura medicao, Componente componente)> MedicaoDetalheHandler;
        private string _contrato;

        public AtualizarStatusJuntaComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress, string contrato = null)
            : base(cancellationToken, progress) {
            this._contrato = contrato;
        }

        protected override void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            base.OnCalculator(provider, cancellationToken, progress);

            var uow = new UnitOfWork(provider.GetSimpleDataLayer());
            var juntas = new XPCollection<JuntaComponente>(PersistentCriteriaEvaluationBehavior.InTransaction, uow, new BinaryOperator("Componente.Contrato.NomeDoContrato", _contrato));
            var QuantidadeDeJunta = juntas.Count;

            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeJunta,
                CurrentRow = 0,
                MessageImport = $"Inicializando Atualização de Status {_contrato}"
            });

            var SaveTemp = new List<dynamic>();
            uow.BeginTransaction();
            Observable.Range(0, QuantidadeDeJunta).Subscribe(i => {
                var junta = juntas[i];

                //x.StatusLp == "AP" || x.StatusPm == "AP" || x.StatusLp == "AL" || x.StatusPm == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.LPPM && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);

                if (junta.Componente.ProgFitup == 0)
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoProgramacao;
                else if (junta.Componente.DataPosicionamento == null)
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoPosicionamento;
                else if (junta.DataFitup == null)
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoAcoplamento;
                else if (junta.DataSolda == null)
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoSolda;
                else if (junta.DataVisual == null)
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoVisualDeSolda;
                else if (junta.StatusLp != "NA" && !(junta.StatusLp == "AP" || junta.StatusPm == "AP" || junta.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.LPPM && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)))
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoLPPM;
                else if (junta.StatusUs != "NA" && !(junta.StatusUs == "AP" || junta.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.US && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)))
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoUS;
                else if (junta.StatusRx != "NA" && !(junta.StatusRx == "AP" || junta.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.RX && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)))
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.AguardandoRX;
                else
                    junta.StatusCustomizadoDaJunta = JuntaComponente.StatusJuntaComponente.JuntaLiberada;

                if (i % 100 == 0) {
                    try {
                        uow.CommitTransaction();
                    } catch {
                        uow.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }

                    progress.Report(new ImportProgressReport {
                        TotalRows = QuantidadeDeJunta,
                        CurrentRow = i,
                        MessageImport = $"Atualização de Status {_contrato} : {i}/{QuantidadeDeJunta}"
                    });
                }
            });

            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeJunta,
                CurrentRow = QuantidadeDeJunta,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();

            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeJunta,
                CurrentRow = QuantidadeDeJunta,
                MessageImport = $"Atualização do status {_contrato} da Junta componente foi Finalizado!"
            });
        }
    }
}
