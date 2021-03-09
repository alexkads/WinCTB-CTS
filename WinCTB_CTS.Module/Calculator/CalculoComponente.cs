//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.ExpressApp.Xpo;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using DevExpress.Xpo;
//using DevExpress.XtraEditors;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar;
//using WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
//using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;
//using WinCTB_CTS.Module.Comum;
//using WinCTB_CTS.Module.ServiceProcess.Base;

//namespace WinCTB_CTS.Module.Calculator {
//    public class CalculoComponente {
//        private IObjectSpace _objectSpace = null;

//        public CalculoComponente(IObjectSpace objectSpace) {
//            this._objectSpace = objectSpace;
//        }

//        public delegate void MedicaoDetalheHandler(Session session, MedicaoEstrutura medicao, Componente componente);

//        private MedicaoDetalheHandler medicaoDetalhe = new MedicaoDetalheHandler(OnMedicaoDetalhe);

//        public void ExecutarCalculo(IProgress<ImportProgressReport> progress) {
//            var session = ((XPObjectSpace)_objectSpace).Session;
//            UnitOfWork uow = new UnitOfWork(session.ObjectLayer);

//            var componentes = new XPCollection<Componente>(PersistentCriteriaEvaluationBehavior.InTransaction, uow, null);
//            var QuantidadeDeComponentes = componentes.Count;

//            progress.Report(new ImportProgressReport {
//                TotalRows = QuantidadeDeComponentes,
//                CurrentRow = 0,
//                MessageImport = "Inicializando Fechamento"
//            });

//            var SaveTemp = new List<dynamic>();

//            uow.BeginTransaction();

//            var medicaoAnterior = uow.FindObject<MedicaoEstrutura>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoEstrutura>].Max(DataFechamentoMedicao)"));
//            var medicao = new MedicaoEstrutura(uow);
//            medicao.DataFechamentoMedicao = DateTime.Now;
//            medicao.Save();

//            Observable.Range(0, QuantidadeDeComponentes).Subscribe(i => {
//                var componente = componentes[i];
//                var detalheMedicaoAnterior = medicaoAnterior is null ? null : uow.FindObject<MedicaoEstruturaDetalhe>(CriteriaOperator.Parse("Componente.Oid = ? And MedicaoEstrutura.Oid = ?", componente.Oid, medicaoAnterior.Oid));
//                //var eap = session.FindObject<TabEAPPipe>(new BinaryOperator("Contrato.Oid", componente.Contrato.Oid));

//                medicaoDetalhe?.Invoke(uow, medicao, componente);

//                if (i % 1000 == 0) {
//                    try {
//                        uow.CommitTransaction();
//                    } catch {
//                        uow.RollbackTransaction();
//                        throw new Exception("Process aborted by system");
//                    }
//                }

//                progress.Report(new ImportProgressReport {
//                    TotalRows = QuantidadeDeComponentes,
//                    CurrentRow = i,
//                    MessageImport = $"Fechando Componentes: {i}/{QuantidadeDeComponentes}"
//                });

//            });

//            progress.Report(new ImportProgressReport {
//                TotalRows = QuantidadeDeComponentes,
//                CurrentRow = QuantidadeDeComponentes,
//                MessageImport = $"Gravando Alterações no Banco"
//            });

//            uow.CommitTransaction();
//            uow.PurgeDeletedObjects();
//            uow.CommitChanges();
//            uow.Dispose();
//        }

//        private static void OnMedicaoDetalhe(Session session, MedicaoEstrutura medicao, Componente componente) {
//            var detalhe = new MedicaoEstruturaDetalhe(session);            

//            //Carregar somente as juntas com medJoints igual ao componente
//            var medJoints = componente.JuntaComponentes.Where(x => x.MedJoint.Oid == x.Componente.Oid).ToList();

//            //Cagarda da EAP
//            var eap = session.QueryInTransaction<TabEAPEst>().Single(x => x.Contrato.Oid == componente.Contrato.Oid && x.Modulo == componente.Modulo);

//            //Comprimento total com medjoints
//            var MedJointMM = medJoints.Sum(x => x.Comprimento);            
//            var FitUpExecutadoMM = medJoints.Where(x => x.StatusFitup == "AP").Sum(s => s.Comprimento);
//            var SoldaExecutadoMM = medJoints.Where(x => x.StatusSolda == "AP").Sum(s => s.Comprimento);

//            //End
//            var VisualPrevistoMM = MedJointMM;
//            var VisualExecutadoMM = medJoints.Where(x => x.StatusVisualSolda == "AP").Sum(s => s.Comprimento);
//            var LPPMPrevistoMM = medJoints.Where(x => x.StatusFitup != "NA").Sum(s => s.Comprimento);
//            var LPPMExecutadoMM = medJoints.Where(x => x.StatusLp == "AP" || x.StatusPm == "AP" || x.StatusLp == "AL" || x.StatusPm == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.LPPM && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
//            var USPrevistoMM = medJoints.Where(x => x.StatusUs != "NA").Sum(s => s.Comprimento);
//            var USExecutadoMM = medJoints.Where(x => x.StatusUs == "AP" || x.StatusUs == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.US && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
//            var RXPrevistoMM = medJoints.Where(x => x.StatusRx != "NA").Sum(s => s.Comprimento);
//            var RXExecutadoMM = medJoints.Where(x => x.StatusRx == "AP" || x.StatusRx == "AL" || x.LoteJuntaEstruturas.Any(a => a.LoteEstrutura.Ensaio == Interfaces.ENDS.RX && a.LoteEstrutura.SituacaoInspecao == Interfaces.SituacoesInspecao.Aprovado)).Sum(s => s.Comprimento);
//            var ENDPrevistoMM = (VisualPrevistoMM + LPPMPrevistoMM + USPrevistoMM + RXPrevistoMM);
//            var ENDExecutaMM = (VisualExecutadoMM + LPPMExecutadoMM + USExecutadoMM + RXExecutadoMM);

//            //Avanço
//            var PercPosicionamento = componente.DataPosicionamento != null ? 1 : 0;
//            var PercAvancoFitUp = FitUpExecutadoMM / MedJointMM;
//            var PercAvancoSolda = SoldaExecutadoMM / MedJointMM;
//            var PercAvancoVisual = VisualExecutadoMM / MedJointMM;
//            var PercAvancoLPPM = LPPMExecutadoMM / LPPMPrevistoMM;
//            var PercAvancoUS = USExecutadoMM / USPrevistoMM;
//            var PercAvancoRX = RXExecutadoMM / RXPrevistoMM;
//            var PercAvancoEND = ENDExecutaMM / ENDPrevistoMM;

//            //Peso
//            var PesoPosicionamento = componente.PesoTotal * PercPosicionamento;
//            var PesoFitUp = componente.PesoTotal * PercAvancoFitUp;
//            var PesoSolda = componente.PesoTotal * PercAvancoSolda;
//            var PesoVisual = componente.PesoTotal * PercAvancoVisual;
//            var PesoLPPM = componente.PesoTotal * PercAvancoLPPM;
//            var PesoUS = componente.PesoTotal * PercAvancoUS;
//            var PesoRX = componente.PesoTotal * PercAvancoRX;
//            var PesoEND = componente.PesoTotal * PercAvancoEND;

//            //Avanço Peso EAP
//            var EAPPesoPosicionamento = PesoPosicionamento * eap.Posicionamento;
//            var EAPPesoFitUP = PesoFitUp * eap.Acoplamento;
//            var EAPPesoSolda = PesoSolda * eap.Solda;
//            var EAPPesoEND = PesoEND * eap.End;
//            var PesoAvancoTotalPoderado = EAPPesoPosicionamento + EAPPesoFitUP + EAPPesoSolda + EAPPesoEND;

//            //Percentual de Avanco Total
//            var PercAvancoTotalPoderado = PesoAvancoTotalPoderado / componente.PesoTotal;

//            //Gravar Medição
//            detalhe.Componente = componente;
//            detalhe.PesoTotal = componente.PesoTotal;
//            detalhe.MedJointMM = MedJointMM;
//            detalhe.FitUpExecutadoMM = FitUpExecutadoMM;
//            detalhe.SoldaExecutadoMM = SoldaExecutadoMM;
//            detalhe.VisualPrevistoMM = VisualPrevistoMM;
//            detalhe.VisualExecutadoMM = VisualExecutadoMM;
//            detalhe.LPPMPrevistoMM = LPPMPrevistoMM;
//            detalhe.LPPMExecutadoMM = LPPMExecutadoMM;
//            detalhe.USPrevistoMM = USPrevistoMM;
//            detalhe.USExecutadoMM = USExecutadoMM;
//            detalhe.RXPrevistoMM = RXPrevistoMM;
//            detalhe.RXExecutadoMM = RXExecutadoMM;
//            detalhe.ENDPrevistoMM = ENDPrevistoMM;
//            detalhe.ENDExecutaMM = ENDExecutaMM;
//            detalhe.PercPosicionamento = PercPosicionamento;
//            detalhe.PercAvancoFitUp = PercAvancoFitUp;
//            detalhe.PercAvancoSolda = PercAvancoSolda;
//            detalhe.PercAvancoVisual = PercAvancoVisual;
//            detalhe.PercAvancoLPPM = PercAvancoLPPM;
//            detalhe.PercAvancoUS = PercAvancoUS;
//            detalhe.PercAvancoRX = PercAvancoRX;
//            detalhe.PercAvancoEND = PercAvancoEND;
//            detalhe.PesoPosicionamento = PesoPosicionamento;
//            detalhe.PesoFitUp = PesoFitUp;
//            detalhe.PesoSolda = PesoSolda;
//            detalhe.PesoVisual = PesoVisual;
//            detalhe.PesoLPPM = PesoLPPM;
//            detalhe.PesoUS = PesoUS;
//            detalhe.PesoRX = PesoRX;
//            detalhe.PesoEND = PesoEND;
//            detalhe.EAPPesoPosicionamento = EAPPesoPosicionamento;
//            detalhe.EAPPesoFitUP = EAPPesoFitUP;
//            detalhe.EAPPesoSolda = EAPPesoSolda;
//            detalhe.EAPPesoEND = EAPPesoEND;
//            detalhe.PesoAvancoTotalPoderado = PesoAvancoTotalPoderado;
//            detalhe.PercAvancoTotalPoderado = PercAvancoTotalPoderado;
//            medicao.MedicaoEstruturaDetalhes.Add(detalhe);
//        }
//    }
//}
