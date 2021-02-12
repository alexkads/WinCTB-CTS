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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
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
            #region Temp
            //var medicaoAnterior = uow.FindObject<MedicaoTubulacao>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoTubulacao>].Max(DataFechamentoMedicao)"));
            //var medicao = new MedicaoTubulacao(uow);
            //medicao.DataFechamentoMedicao = DateTime.Now;
            //medicao.Save();
            #endregion

            for (int i = 0; i < QuantidadeDeComponentes; i++)
            {
                var componente = componentes[i];                                               
                
                #region Temp
                //var detalheMedicaoAnterior = medicaoAnterior is null ? null : uow.FindObject<MedicaoTubulacaoDetalhe>(CriteriaOperator.Parse("Spool.Oid = ? And MedicaoTubulacao.Oid = ?", spool.Oid, medicaoAnterior.Oid));
                //var eap = session.FindObject<TabEAPPipe>(new BinaryOperator("Contrato.Oid", spool.Contrato.Oid));
                //var detalhe = new MedicaoTubulacaoDetalhe(uow);

                //var testeLogica = spool.DataCorte;
                #endregion

                #region Teste de Cálculo
                var SomaComprimento = componente.JuntaComponentes.Sum(x => x.Comprimento);
                var QuantidadeJunta = componente.JuntaComponentes.Count();
                #endregion

                //var PosicionamentoDF1 = 

                var QtdJuntaPipe = Utils.ConvertINT(componente.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'PIPE'].Count()")));
                var QtdJuntaMont = Utils.ConvertINT(componente.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'CAMPO'].Count()")));


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
                    MessageImport = $"Fechando Spools: {i}/{QuantidadeDeComponentes}"
                });
            }

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
    }
}
