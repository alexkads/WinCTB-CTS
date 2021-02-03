﻿using DevExpress.Data.Filtering;
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
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;
using WinCTB_CTS.Module.Comum;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class FechamentoMedicaoTubulacaoController : ViewController
    {
        public FechamentoMedicaoTubulacaoController()
        {
            TargetObjectType = typeof(MedicaoTubulacao);

            SimpleAction ExecutarMedicao = new SimpleAction(this, "FechamentoMedicaoTubulacaoSimpleActionController", PredefinedCategory.RecordEdit)
            {
                Caption = "Executar Medição",
                Id = nameof(MedicaoTubulacao),
                TargetObjectType = typeof(MedicaoTubulacao),
                TargetViewType = ViewType.ListView,
                TargetViewNesting = Nesting.Any,
                ToolTip = nameof(MedicaoTubulacao),
                SelectionDependencyType = SelectionDependencyType.Independent,
                ImageName = "Action_Debug_Step"
            };

            ExecutarMedicao.Execute += ExecutarMedicao_Execute;
        }

        private XtraForm FormProgressImport;
        private ProgressBarControl progressBarControl;
        private SimpleButton cancelProgress;
        private LabelControl statusProgess;

        private void InitializeInteface()
        {
            FormProgressImport = new XtraProgressImport();

            progressBarControl = FormProgressImport.Controls.OfType<ProgressBarControl>().FirstOrDefault();
            statusProgess = FormProgressImport.Controls.OfType<LabelControl>().FirstOrDefault();
            cancelProgress = FormProgressImport.Controls.OfType<SimpleButton>().FirstOrDefault();

            progressBarControl.Properties.ShowTitle = true;
            progressBarControl.Properties.Step = 1;
            progressBarControl.Properties.PercentView = true;
            progressBarControl.Properties.Minimum = 0;

            FormProgressImport.Show();
        }

        private async void ExecutarMedicao_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            InitializeInteface();

            var progress = new Progress<ImportProgressReport>(value =>
            {
                progressBarControl.Properties.Maximum = value.TotalRows;
                statusProgess.Text = value.MessageImport;

                if (value.CurrentRow > 0)
                    progressBarControl.PerformStep();

                progressBarControl.Update();
                statusProgess.Update();
            });

            await Task.Run(() =>
                ExecutarCalculo((XPObjectSpace)View.ObjectSpace, progress));

            View.ObjectSpace.Refresh();
            FormProgressImport.Close();
        }

        private void ExecutarCalculo(XPObjectSpace objectSpace, IProgress<ImportProgressReport> progress)
        {
            var session = objectSpace.Session;
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)ObjectSpace).Session.ObjectLayer);

            var spools = new XPCollection<Spool>(PersistentCriteriaEvaluationBehavior.InTransaction, uow, null);
            var QuantidadeDeSpool = spools.Count;

            progress.Report(new ImportProgressReport
            {
                TotalRows = QuantidadeDeSpool,
                CurrentRow = 0,
                MessageImport = "Inicializando Fechamento"
            });

            uow.BeginTransaction();
            var medicaoAnterior = uow.FindObject<MedicaoTubulacao>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoTubulacao>].Max(DataFechamentoMedicao)"));
            var medicao = new MedicaoTubulacao(uow);
            medicao.DataFechamentoMedicao = DateTime.Now;
            medicao.Save();

            for (int i = 0; i < QuantidadeDeSpool; i++)
            {
                var spool = spools[i];
                var detalheMedicaoAnterior = medicaoAnterior is null ? null : uow.FindObject<MedicaoTubulacaoDetalhe>(CriteriaOperator.Parse("Spool.Oid = ? And MedicaoTubulacao.Oid = ?", spool.Oid, medicaoAnterior.Oid)); 
                var eap = session.FindObject<TabEAPPipe>(new BinaryOperator("Contrato.Oid", spool.Contrato.Oid));
                var detalhe = new MedicaoTubulacaoDetalhe(uow);

                //var testeLogica = spool.DataCorte;

                var QtdJuntaPipe = Utils.ConvertINT(spool.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'PIPE'].Count()")));
                var QtdJuntaMont = Utils.ConvertINT(spool.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'CAMPO'].Count()")));


                //Cálculo de Montagem (Memória de Cálculo)
                var WdiJuntaTotalMont = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));
                var WdiJuntaVAMont = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[Not IsNullorEmpty(DataVa) And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));
                var WdiJuntaSoldMont = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[Not IsNullorEmpty(DataSoldagem) And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));
                var WdiJuntaENDMont = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[Not IsNullorEmpty(DataLiberacaoJunta) And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));

                //Avanço de Fabricação (Memória de Cálculo)
                var ExecutadoSpoolDFFab = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataDfFab)"));
                var AvancoSpoolCorteFab = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataCorte)"));
                var AvancoSpoolVAFab = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataVaFab)"));
                var AvancoSpoolSoldFab = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataSoldaFab)"));
                var AvancoSpoolENDFab = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataEndFab)"));

                //Avanço de Montagem (Memória de Cálculo)
                var ExecutadoSpoolPosiMont = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataPreMontagem)"));
                var ExecutadoSpoolDIMont = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataDiMontagem)"));
                var ExecutadoSpoolLineCheckMont = (Boolean)spool.Evaluate(CriteriaOperator.Parse("Not IsNullorEmpty(DataLineCheck)"));
                var AvancoJuntaVAMont = Utils.CalculoPercentual(WdiJuntaVAMont, WdiJuntaTotalMont);
                var AvancoJuntaSoldMont = Utils.CalculoPercentual(WdiJuntaSoldMont, WdiJuntaTotalMont);
                var AvancoJuntaENDMont = Utils.CalculoPercentual(WdiJuntaENDMont, WdiJuntaTotalMont);

                detalhe.MedicaoTubulacao = medicao;
                detalhe.Spool = spool;
                detalhe.WdiJuntaTotalMont = WdiJuntaTotalMont;

                //Gravar Cálculo de Montagem
                detalhe.WdiJuntaVAMont = WdiJuntaVAMont;
                detalhe.WdiJuntaSoldMont = WdiJuntaSoldMont;
                detalhe.WdiJuntaENDMont = WdiJuntaENDMont;

                //Cálculo Fabricação
                var AvancarTrechoRetoFab = QtdJuntaPipe == 0 && ExecutadoSpoolDFFab;
                var LogicAvancoSpoolENDFab = AvancoSpoolENDFab || AvancarTrechoRetoFab;
                var LogicAvancoSpoolSoldFab = AvancoSpoolSoldFab || LogicAvancoSpoolENDFab;
                var LogicAvancoSpoolVAFab = AvancoSpoolVAFab || LogicAvancoSpoolSoldFab;
                var LogicAvancoSpoolCorteFab = AvancoSpoolCorteFab || LogicAvancoSpoolVAFab;

                //Gravar Avanço de Fabricação
                detalhe.AvancoSpoolCorteFab = LogicAvancoSpoolCorteFab ? 1 : 0;
                detalhe.AvancoSpoolVAFab = LogicAvancoSpoolVAFab ? 1 : 0;
                detalhe.AvancoSpoolSoldFab = LogicAvancoSpoolSoldFab ? 1 : 0;
                detalhe.AvancoSpoolENDFab = LogicAvancoSpoolENDFab ? 1 : 0;

 
                //Aplicar EAP no Avanço de Fabricação
                detalhe.PesoSpoolCorteFab = (spool.PesoFabricacao * detalhe.AvancoSpoolCorteFab) * eap.AvancoSpoolCorteFab;
                detalhe.PesoSpoolVAFab = (spool.PesoFabricacao * detalhe.AvancoSpoolVAFab) * eap.AvancoSpoolVAFab;
                detalhe.PesoSpoolSoldFab = (spool.PesoFabricacao * detalhe.AvancoSpoolSoldFab) * eap.AvancoSpoolSoldaFab;
                detalhe.PesoSpoolENDFab = (spool.PesoFabricacao * detalhe.AvancoSpoolENDFab) * eap.AvancoSpoolENDFab;

                //Cálculo Montagem
                var AvancarTrechoRetoPosiMont = QtdJuntaMont == 0 && ExecutadoSpoolPosiMont;
                var AvancarTrechoRetoDIMont = QtdJuntaMont == 0 && ExecutadoSpoolDIMont;
                
                var LogicAvancoJuntaENDMont = 0D;
                var LogicAvancoJuntaSoldMont = 0D;
                var LogicAvancoJuntaVAMont = 0D;
                var LogicAvancoSpoolPosiMont = 0D;

                if (AvancarTrechoRetoPosiMont)
                {
                    LogicAvancoSpoolPosiMont = 1;
                    LogicAvancoJuntaVAMont = 1;
                }
                else if (AvancarTrechoRetoDIMont)
                {
                    LogicAvancoSpoolPosiMont = 1;
                    LogicAvancoJuntaVAMont = 1;
                    LogicAvancoJuntaSoldMont = 1;
                    LogicAvancoJuntaENDMont = 1;
                }
                else
                {
                    LogicAvancoSpoolPosiMont = ExecutadoSpoolPosiMont ? 1 : 0;
                    LogicAvancoJuntaVAMont = AvancoJuntaVAMont;
                    LogicAvancoJuntaSoldMont = AvancoJuntaSoldMont;
                    LogicAvancoJuntaENDMont = AvancoJuntaENDMont;
                }

                //Gravar Avanço de Montagem
                detalhe.AvancoSpoolPosiMont = LogicAvancoSpoolPosiMont;
                detalhe.AvancoJuntaVAMont = LogicAvancoJuntaVAMont;
                detalhe.AvancoJuntaSoldMont = LogicAvancoJuntaSoldMont;
                detalhe.AvancoJuntaENDMont = LogicAvancoJuntaENDMont;
                detalhe.AvancoSpoolLineCheckMont = ExecutadoSpoolLineCheckMont ? 1 : 0;

                //Aplicar EAP no Avanço de Montagem
                detalhe.PesoSpoolPosiMont = (spool.PesoMontagem * detalhe.AvancoSpoolPosiMont) * eap.AvancoSpoolPosicionamento;
                detalhe.PesoJuntaVAMont = (spool.PesoMontagem * detalhe.AvancoJuntaVAMont) * eap.AvancoJuntaVAMont;
                detalhe.PesoJuntaSoldMont = (spool.PesoMontagem * detalhe.AvancoJuntaSoldMont) * eap.AvancoJuntaSoldMont;
                detalhe.PesoJuntaENDMont = (spool.PesoMontagem * detalhe.AvancoJuntaENDMont) * eap.AvancoJuntaENDMont;
                detalhe.PesoSpoolLineCheckMont = (spool.PesoMontagem * detalhe.AvancoSpoolLineCheckMont) * eap.AvancoSpoolLineCheck;
                detalhe.MedicaoAnterior = detalheMedicaoAnterior;

                detalhe.Save();

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
                    TotalRows = QuantidadeDeSpool,
                    CurrentRow = i,
                    MessageImport = $"Fechando Spools: {i}/{QuantidadeDeSpool}"
                });
            }

            progress.Report(new ImportProgressReport
            {
                TotalRows = QuantidadeDeSpool,
                CurrentRow = QuantidadeDeSpool,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
