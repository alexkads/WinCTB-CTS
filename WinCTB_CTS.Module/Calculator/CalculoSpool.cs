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
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Calculator
{
    public class CalculoSpool
    {
        private IObjectSpace _objectSpace = null;
        
        public CalculoSpool(IObjectSpace objectSpace)
        {
            this._objectSpace = objectSpace;
        }

        public void ExecutarCalculo(IProgress<ImportProgressReport> progress)
        {
            var session = ((XPObjectSpace)_objectSpace).Session;
            UnitOfWork uow = new UnitOfWork(session.ObjectLayer);

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

                var WdiJuntaVANaMontPrev = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[statusVa == 'N' And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));
                var WdiJuntaVAApMontPrev = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[statusVa <> 'N' And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));
                var WdiJuntaVAApMontExec = Utils.ConvertDouble(spool.Evaluate(CriteriaOperator.Parse("Juntas[Not IsNullorEmpty(DataVa) And statusVa <> 'N' And CampoOuPipe == 'CAMPO'].Sum(TabDiametro.Wdi)")));


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

                //Verificar trecho reto
                if (QtdJuntaMont > 0)
                {

                    // Avanço das juntas de VA "NA" quando todas as juntas forem NA com posicionamento
                    if (WdiJuntaVANaMontPrev == WdiJuntaTotalMont && ExecutadoSpoolPosiMont)
                    {
                        AvancoJuntaVAMont = 1;
                    }

                    //Avanço das juntas de VA "NA" quando tiverem juntas NA e AP com posicionamento
                    if (WdiJuntaVANaMontPrev > 0 && WdiJuntaVAApMontPrev > 0 && ExecutadoSpoolPosiMont)
                    {
                        AvancoJuntaVAMont = Utils.CalculoPercentual(WdiJuntaVANaMontPrev + WdiJuntaVAApMontExec, WdiJuntaTotalMont);
                        //AvancoJuntaVAMont = WdiJuntaVANaMontPrev + WdiJuntaVAApMontExec;
                    }



                    LogicAvancoJuntaENDMont = AvancoJuntaENDMont;

                    LogicAvancoJuntaSoldMont = LogicAvancoJuntaENDMont > AvancoJuntaSoldMont
                        ? LogicAvancoJuntaENDMont
                        : AvancoJuntaSoldMont;

                    LogicAvancoJuntaVAMont = LogicAvancoJuntaSoldMont > AvancoJuntaVAMont
                        ? LogicAvancoJuntaSoldMont
                        : AvancoJuntaVAMont;

                    LogicAvancoSpoolPosiMont = LogicAvancoJuntaVAMont > (ExecutadoSpoolPosiMont ? 1 : 0)
                        ? LogicAvancoJuntaVAMont
                        : (ExecutadoSpoolPosiMont ? 1 : 0);
                }
                else
                {
                    if (AvancarTrechoRetoPosiMont)
                    {
                        LogicAvancoSpoolPosiMont = 1;
                        LogicAvancoJuntaVAMont = 1;
                    }

                    if (AvancarTrechoRetoDIMont)
                    {
                        LogicAvancoSpoolPosiMont = 1;
                        LogicAvancoJuntaVAMont = 1;
                        LogicAvancoJuntaSoldMont = 1;
                        LogicAvancoJuntaENDMont = 1;
                    }
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
    }
}
