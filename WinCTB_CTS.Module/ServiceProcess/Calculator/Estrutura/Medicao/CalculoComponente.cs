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
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.Medicao {
    public class CalculoComponente : CalculatorProcessBase {

        public event EventHandler<(Session session, MedicaoEstrutura medicao, Componente componente)> MedicaoDetalheHandler;

        public CalculoComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
            : base(cancellationToken, progress) {
        }

        protected override void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            base.OnCalculator(provider, cancellationToken, progress);

            var uow = new UnitOfWork(provider.GetSimpleDataLayer());

            var componentes = new XPCollection<Componente>(PersistentCriteriaEvaluationBehavior.InTransaction, uow, null);
            var QuantidadeDeComponentes = componentes.Count;


            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeComponentes,
                CurrentRow = 0,
                MessageImport = "Inicializando Fechamento"
            });

            var SaveTemp = new List<dynamic>();

            uow.BeginTransaction();
            var medicaoAnterior = uow.FindObject<MedicaoEstrutura>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoEstrutura>].Max(DataFechamentoMedicao)"));
            var medicao = new MedicaoEstrutura(uow);
            medicao.DataFechamentoMedicao = DateTime.Now;
            medicao.Save();

            for (int i = 0; i < QuantidadeDeComponentes; i++) {
                var componente = componentes[i];
                var detalheMedicaoAnterior = medicaoAnterior is null ? null : uow.FindObject<MedicaoEstruturaDetalhe>(CriteriaOperator.Parse("Componente.Oid = ? And MedicaoEstrutura.Oid = ?", componente.Oid, medicaoAnterior.Oid));

                //MedicaoDetalheHandler += CalculoComponente_MedicaoDetalheHandler;
                //MedicaoDetalheHandler.Invoke(this, ExecMedicaoDetalhe(uow, medicao, componente));
                ExecMedicaoDetalhe(uow, medicao, componente, detalheMedicaoAnterior);

                if (i % 1000 == 0) {
                    try {
                        uow.CommitTransaction();
                    } catch {
                        uow.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }

                    progress.Report(new ImportProgressReport {
                        TotalRows = QuantidadeDeComponentes,
                        CurrentRow = i,
                        MessageImport = $"Fechando Componentes: {i}/{QuantidadeDeComponentes}"
                    });
                }
            }

            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeComponentes,
                CurrentRow = QuantidadeDeComponentes,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();

            progress.Report(new ImportProgressReport {
                TotalRows = QuantidadeDeComponentes,
                CurrentRow = QuantidadeDeComponentes,
                MessageImport = $"Medição de Estrutura Finalizada!"
            });
        }

        private void ExecMedicaoDetalhe(Session session, MedicaoEstrutura medicao, Componente componente, MedicaoEstruturaDetalhe detalheMedicaoAnterior) {
            var detalhe = new MedicaoEstruturaDetalhe(session);

            //Carregar somente as juntas com medJoints igual ao componente
            var medJoints = new XPCollection<JuntaComponente>(PersistentCriteriaEvaluationBehavior.InTransaction, session, new BinaryOperator("MedJoint.Oid", componente.Oid));
            
            //Cagarda da EAP
            var eap = session.QueryInTransaction<TabEAPEst>().Single(x => x.Contrato.Oid == componente.Contrato.Oid && x.Modulo == componente.Modulo);

            //Comprimento total com medjoints
            var MedJointMM = medJoints.Sum(x => x.Comprimento);
            var FitUpExecutadoMM = medJoints.Where(x => x.StatusFitup == "AP").Sum(s => s.Comprimento);
            var SoldaExecutadoMM = medJoints.Where(x => x.StatusSolda == "AP").Sum(s => s.Comprimento);

            //End
            var VisualPrevistoMM = MedJointMM;
            var VisualExecutadoMM = medJoints.Where(x => x.StatusVisualSolda == "AP").Sum(s => s.Comprimento);
            var LPPMPrevistoMM = medJoints.Where(x => x.StatusFitup != "NA").Sum(s => s.Comprimento);
            var LPPMExecutadoMM = medJoints.Where(x => x.StatusLp == "AP" || x.StatusPm == "AP" || x.StatusLp == "AL" || x.StatusPm == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.LPPM && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
            var USPrevistoMM = medJoints.Where(x => x.StatusUs != "NA").Sum(s => s.Comprimento);
            var USExecutadoMM = medJoints.Where(x => x.StatusUs == "AP" || x.StatusUs == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.US && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
            var RXPrevistoMM = medJoints.Where(x => x.StatusRx != "NA").Sum(s => s.Comprimento);
            var RXExecutadoMM = medJoints.Where(x => x.StatusRx == "AP" || x.StatusRx == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.RX && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
            var ENDPrevistoMM = (VisualPrevistoMM + LPPMPrevistoMM + USPrevistoMM + RXPrevistoMM);
            var ENDExecutadoMM = (VisualExecutadoMM + LPPMExecutadoMM + USExecutadoMM + RXExecutadoMM);

            ////Condicional Lógico
            //if (ENDExecutadoMM > SoldaExecutadoMM)
            //    SoldaExecutadoMM = ENDExecutadoMM;

            //if (SoldaExecutadoMM > FitUpExecutadoMM)
            //    FitUpExecutadoMM = SoldaExecutadoMM;

            //Avanço
            var PercPosicionamento = componente.DataPosicionamento != null ? 1 : 0;
            var PercAvancoFitUp = FitUpExecutadoMM / MedJointMM;
            var PercAvancoSolda = SoldaExecutadoMM / MedJointMM;
            var PercAvancoVisual = VisualExecutadoMM / MedJointMM;
            var PercAvancoLPPM = LPPMExecutadoMM / LPPMPrevistoMM;
            var PercAvancoUS = USExecutadoMM / USPrevistoMM;
            var PercAvancoRX = RXExecutadoMM / RXPrevistoMM;
            var PercAvancoEND = ENDExecutadoMM / ENDPrevistoMM;

            //Peso
            var PesoPosicionamento = componente.PesoTotal * PercPosicionamento;
            var PesoFitUp = componente.PesoTotal * PercAvancoFitUp;
            var PesoSolda = componente.PesoTotal * PercAvancoSolda;
            var PesoVisual = componente.PesoTotal * PercAvancoVisual;
            var PesoLPPM = componente.PesoTotal * PercAvancoLPPM;
            var PesoUS = componente.PesoTotal * PercAvancoUS;
            var PesoRX = componente.PesoTotal * PercAvancoRX;
            var PesoEND = componente.PesoTotal * PercAvancoEND;

            //Avanço Peso EAP
            var EAPPesoPosicionamento = PesoPosicionamento * eap.Posicionamento;
            var EAPPesoFitUP = PesoFitUp * eap.Acoplamento;
            var EAPPesoSolda = PesoSolda * eap.Solda;
            var EAPPesoEND = PesoEND * eap.End;
            var PesoAvancoTotalPoderado = EAPPesoPosicionamento + EAPPesoFitUP + EAPPesoSolda + EAPPesoEND;

            //Percentual de Avanco Total
            var PercAvancoTotalPoderado = PesoAvancoTotalPoderado / componente.PesoTotal;

            //Gravar Medição
            detalhe.Componente = componente;
            detalhe.PesoTotal = componente.PesoTotal;
            detalhe.MedJointMM = MedJointMM;
            detalhe.FitUpExecutadoMM = FitUpExecutadoMM;
            detalhe.SoldaExecutadoMM = SoldaExecutadoMM;
            detalhe.VisualPrevistoMM = VisualPrevistoMM;
            detalhe.VisualExecutadoMM = VisualExecutadoMM;
            detalhe.LPPMPrevistoMM = LPPMPrevistoMM;
            detalhe.LPPMExecutadoMM = LPPMExecutadoMM;
            detalhe.USPrevistoMM = USPrevistoMM;
            detalhe.USExecutadoMM = USExecutadoMM;
            detalhe.RXPrevistoMM = RXPrevistoMM;
            detalhe.RXExecutadoMM = RXExecutadoMM;
            detalhe.ENDPrevistoMM = ENDPrevistoMM;
            detalhe.ENDExecutaMM = ENDExecutadoMM;
            detalhe.PercPosicionamento = PercPosicionamento;
            detalhe.PercAvancoFitUp = PercAvancoFitUp;
            detalhe.PercAvancoSolda = PercAvancoSolda;
            detalhe.PercAvancoVisual = PercAvancoVisual;
            detalhe.PercAvancoLPPM = PercAvancoLPPM;
            detalhe.PercAvancoUS = PercAvancoUS;
            detalhe.PercAvancoRX = PercAvancoRX;
            detalhe.PercAvancoEND = PercAvancoEND;
            detalhe.PesoPosicionamento = PesoPosicionamento;
            detalhe.PesoFitUp = PesoFitUp;
            detalhe.PesoSolda = PesoSolda;
            detalhe.PesoVisual = PesoVisual;
            detalhe.PesoLPPM = PesoLPPM;
            detalhe.PesoUS = PesoUS;
            detalhe.PesoRX = PesoRX;
            detalhe.PesoEND = PesoEND;
            detalhe.EAPPesoPosicionamento = EAPPesoPosicionamento;
            detalhe.EAPPesoFitUP = EAPPesoFitUP;
            detalhe.EAPPesoSolda = EAPPesoSolda;
            detalhe.EAPPesoEND = EAPPesoEND;
            detalhe.PesoAvancoTotalPoderado = PesoAvancoTotalPoderado;
            detalhe.PercAvancoTotalPoderado = PercAvancoTotalPoderado;
            detalhe.MedicaoAnterior = detalheMedicaoAnterior;
            medicao.MedicaoEstruturaDetalhes.Add(detalhe);

            if (detalhe?.Componente.Oid != detalhe?.MedicaoAnterior?.Componente.Oid 
                && detalhe?.MedicaoAnterior != null)
                throw new Exception("Erro ao definir medição anterior");

            medJoints.Dispose();
        }
    }
}
