using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Calculator
{
    public class CalculoComponente
    {
        private IObjectSpace _objectSpace = null;

        public CalculoComponente(IObjectSpace objectSpace)
        {
            this._objectSpace = objectSpace;
        }

        public delegate void MedicaoDetalheHandler(Session session, MedicaoEstrutura medicao, Componente componente);

        private MedicaoDetalheHandler medicaoDetalhe = new MedicaoDetalheHandler(OnMedicaoDetalhe);

        public void ExecutarCalculo(IProgress<ImportProgressReport> progress)
        {
            var session = ((XPObjectSpace)_objectSpace).Session;
            UnitOfWork uow = new UnitOfWork(session.ObjectLayer);

            var componentes = new XPCollection<Componente>(PersistentCriteriaEvaluationBehavior.InTransaction, uow, null);
            var QuantidadeDeComponentes = componentes.Count;

            progress.Report(new ImportProgressReport
            {
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

            Observable.Range(0, QuantidadeDeComponentes).Subscribe(i =>
            {
                var componente = componentes[i];
                var detalheMedicaoAnterior = medicaoAnterior is null ? null : uow.FindObject<MedicaoEstruturaDetalhe>(CriteriaOperator.Parse("Componente.Oid = ? And MedicaoEstrutura.Oid = ?", componente.Oid, medicaoAnterior.Oid));
                //var eap = session.FindObject<TabEAPPipe>(new BinaryOperator("Contrato.Oid", componente.Contrato.Oid));

                medicaoDetalhe?.Invoke(uow, medicao, componente);

                if (i % 1000 == 0)
                {
                    try
                    {
                        uow.CommitTransaction();
                    }
                    catch
                    {
                        uow.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }
                }

                progress.Report(new ImportProgressReport
                {
                    TotalRows = QuantidadeDeComponentes,
                    CurrentRow = i,
                    MessageImport = $"Fechando Componentes: {i}/{QuantidadeDeComponentes}"
                });

            });

            progress.Report(new ImportProgressReport
            {
                TotalRows = QuantidadeDeComponentes,
                CurrentRow = QuantidadeDeComponentes,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();
        }

        private static void OnMedicaoDetalhe(Session session, MedicaoEstrutura medicao, Componente componente)
        {
            var detalhe = new MedicaoEstruturaDetalhe(session);
            medicao.MedicaoEstruturaDetalhes.Add(detalhe);
            detalhe.Componente = componente;
            detalhe.PesoTotal = componente.PesoTotal;

            //Carregar somente as juntas com medJoints igual ao componente
            var medJoints = componente.JuntaComponentes.Where(x => x.MedJoint.Oid == x.Componente.Oid).ToList();
            var comprimentoTotal = CalculaComprimento(medJoints);

            //var QtdJuntaPipe = Utils.ConvertINT(componente.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'PIPE'].Count()")));
            //var QtdJuntaMont = Utils.ConvertINT(componente.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'CAMPO'].Count()")));
        }

        private static double CalculaComprimento(IList<JuntaComponente> medJoints)
        {
            double comprimentoTotal = 0D;

            foreach (var item in medJoints)
            {
                // DF1 == DF2
                if (item.Df1 == item.Df2)
                    comprimentoTotal += 0;

                //Tipo de estrutura e progfitup - Falta comparação entre ProgfitupDf1 e ProgfitupDf2
                if (item.PosicaoDf1 == "column" || item.PosicaoDf1 == "coluna" || item.PosicaoDf1 == "contaventamento")
                    comprimentoTotal += item.Comprimento;


                //Categoria da estrutura e tipo de junta
                if (item.TipoDf1 == "Primary" && (item.TipoJunta == "JASA" || item.TipoJunta == "JTPP" || item.TipoJunta == "JAPP"))
                    comprimentoTotal += 0;
                
               
            }

            return comprimentoTotal;
        }
    }
}
