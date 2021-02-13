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
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Importer.Estrutura
{
    public class ImportComponentEJunta
    {
        IObjectSpace objectSpace = null;
        ParametrosImportComponentEJunta parametrosImportComponentEJunta;
        public ImportComponentEJunta(IObjectSpace _objectSpace, ParametrosImportComponentEJunta _parametrosImportComponentEJunta)
        {
            this.objectSpace = _objectSpace;
            this.parametrosImportComponentEJunta = _parametrosImportComponentEJunta;
        }

        public void LogTrace(ImportProgressReport value)
        {
            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
                ? (value.CurrentRow / value.TotalRows)
                : 0D;

            parametrosImportComponentEJunta.Progresso = progresso;
            //statusProgess.Text = value.MessageImport;
        }

        public void ImportarComponente(DataTable dtSpoolsImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtSpoolsImport.Rows.Count;

            var oldComponets = Utils.GetOldDatasForCheck<Componente>(uow);

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = 0,
                MessageImport = "Inicializando importação"
            });

            uow.BeginTransaction();

            for (int i = 0; i < TotalDeJuntas; i++)
            {
                if (i >= 3)
                {
                    var linha = dtSpoolsImport.Rows[i];
                    var documentoReferencia = linha[1].ToString();
                    var desenhoMontagem = linha[2].ToString();
                    var transmital = linha[3].ToString();
                    var peca = linha[4].ToString();

                    var criteriaOperator = CriteriaOperator.Parse("DesenhoMontagem = ? And Peca = ?",
                        desenhoMontagem, peca);

                    var componente = uow.FindObject<Componente>(criteriaOperator);

                    if (componente == null)
                        componente = new Componente(uow);
                    else
                        oldComponets.FirstOrDefault(x => x.Oid == componente.Oid).DataExist = true;

                    //Mapear campos aqui
                    //componente.Contrato = contrato;

                    componente.Modulo = linha[0].ToString();
                    componente.DocumentoReferencia = documentoReferencia;
                    componente.DesenhoMontagem = desenhoMontagem;
                    componente.Transmital = transmital;
                    componente.Peca = peca;
                    componente.Revisao = linha[5].ToString();
                    componente.TipoEstrutura = linha[6].ToString();
                    componente.Posicao = linha[7].ToString();
                    componente.Dwg = linha[8].ToString();
                    componente.Elevacao = linha[9].ToString();
                    componente.PesoTotal = Convert.ToDouble(linha[10]);
                    componente.AreaPintura = Convert.ToDouble(linha[11]);
                    componente.RelatorioRecebimento = linha[14].ToString();
                    componente.DataRecebimento = Utils.ConvertDateTime(linha[15]);
                    componente.ProgFitup = linha[16].ToString();
                    componente.ProgWeld = linha[17].ToString();
                    componente.ProgNdt = linha[18].ToString();
                    componente.DataPosicionamento = Utils.ConvertDateTime(linha[19]);
                    componente.RelatorioDimensional = linha[26].ToString();
                    componente.DataDimensional = Utils.ConvertDateTime(linha[27]);
                    componente.ProgPintura = linha[28].ToString();
                    componente.RelPintura = linha[29].ToString();
                    componente.InspPintura = linha[30].ToString();
                    componente.DataPintura = Utils.ConvertDateTime(linha[31]);
                    componente.StatusPeca = linha[32].ToString();

                }




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
                    TotalRows = TotalDeJuntas,
                    CurrentRow = i + 1,
                    MessageImport = $"Importando linha {i}/{TotalDeJuntas}"
                });
            }

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = TotalDeJuntas,
                MessageImport = $"Gravando Alterações no Banco"
            });


            // Implatar funcionalidade
            //var excluirSpoolsNaoImportado = oldSpools.Where(x => x.DataExist = false);
        }

        public void ImportarJuntas(DataTable dtJuntasImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtJuntasImport.Rows.Count;

            var oldJuntas = Utils.GetOldDatasForCheck<JuntaComponente>(uow);

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = 0,
                MessageImport = "Inicializando importação de juntas"
            });

            uow.BeginTransaction();

            ////Limpar registros
            //Utils.DeleteAllRecords<JuntaSpool>(uow);
            //uow.CommitTransaction();

            for (int i = 0; i < TotalDeJuntas; i++)
            {
                if (i >= 2)
                {
                    var linha = dtJuntasImport.Rows[i];
                    var desenhoMontagem = linha[2].ToString();
                    var peca = linha[3].ToString();
                    var FiltroPesquisa = CriteriaOperator.Parse("DesenhoMontagem = ? And Peca = ?", desenhoMontagem, peca);
                    var componente = uow.FindObject<Componente>(FiltroPesquisa);
                    if (componente != null)
                    {
                        var junta = linha[9].ToString();

                        var criteriaOperator = CriteriaOperator.Parse("Componente.Oid = ? And Junta = ?",
                            componente.Oid, junta);

                        var juntaComponente = uow.FindObject<JuntaComponente>(criteriaOperator);

                        if (juntaComponente == null)
                            juntaComponente = new JuntaComponente(uow);
                        else
                            oldJuntas.FirstOrDefault(x => x.Oid == juntaComponente.Oid).DataExist = true;

                        //Mapear campos aqui
                        //juntaComponente.Site = linha[0].ToString();

                        juntaComponente.Junta = linha[5].ToString();
                        juntaComponente.TipoJunta = linha[7].ToString();
                        juntaComponente.Site = linha[8].ToString();
                        juntaComponente.Comprimento = Utils.ConvertDouble(linha[9]);
                        juntaComponente.ClasseInspecao = linha[10].ToString();
                        juntaComponente.Df1 = linha[11].ToString();
                        juntaComponente.Mat1 = linha[12].ToString();
                        juntaComponente.Esp1 = Utils.ConvertDouble(linha[13]);
                        juntaComponente.TipoDf1 = linha[14].ToString();
                        juntaComponente.PosicaoDf1 = linha[15].ToString();
                        juntaComponente.Df2 = linha[17].ToString();
                        juntaComponente.Mat2 = linha[18].ToString();
                        juntaComponente.Esp2 = Utils.ConvertDouble(linha[19]);
                        juntaComponente.TipoDf2 = linha[20].ToString();
                        juntaComponente.PosicaoDf2 = linha[21].ToString();
                        juntaComponente.Posiocionamento = Utils.ConvertDateTime(linha[23]);
                        juntaComponente.DataFitup = Utils.ConvertDateTime(linha[24]);
                        juntaComponente.RelatorioFitup = linha[25].ToString();
                        juntaComponente.InspFitup = linha[26].ToString();
                        juntaComponente.StatusFitup = linha[27].ToString();
                        juntaComponente.DataSolda = Utils.ConvertDateTime(linha[28]);
                        juntaComponente.Soldadores = linha[29].ToString();
                        juntaComponente.Consumiveis = linha[30].ToString();
                        juntaComponente.Wps = linha[31].ToString();
                        juntaComponente.RelatorioSolda = linha[32].ToString();
                        juntaComponente.InspetorSoldagem = linha[33].ToString();
                        juntaComponente.StatusSolda = linha[34].ToString();
                        juntaComponente.DataVisual = Utils.ConvertDateTime(linha[35]);
                        juntaComponente.RelatorioVisualSolda = linha[36].ToString();
                        juntaComponente.InspetorVisualSolda = linha[37].ToString();
                        juntaComponente.StatusVisualSolda = linha[38].ToString();
                        juntaComponente.SampleMp = linha[39].ToString();
                        juntaComponente.DataLP = Utils.ConvertDateTime(linha[40]);
                        juntaComponente.RelatorioLp = linha[41].ToString();
                        juntaComponente.InspetorLp = linha[42].ToString();
                        juntaComponente.StatusLp = linha[43].ToString();
                        juntaComponente.DataPm = Utils.ConvertDateTime(linha[44]);
                        juntaComponente.RelatorioPm = linha[45].ToString();
                        juntaComponente.InspetorPm = linha[46].ToString();
                        juntaComponente.StatusPm = linha[47].ToString();
                        juntaComponente.SampleRx = linha[48].ToString();
                        juntaComponente.DataRx = Utils.ConvertDateTime(linha[49]);
                        juntaComponente.RelatorioRx = linha[50].ToString();
                        juntaComponente.InspetorRx = linha[51].ToString();
                        juntaComponente.ComprimentoReparoRx = Utils.ConvertDouble(linha[52]);
                        juntaComponente.StatusRx = linha[53].ToString();
                        juntaComponente.SampleUs = linha[54].ToString();
                        juntaComponente.DataUs = Utils.ConvertDateTime(linha[55]);
                        juntaComponente.RelatorioUs = linha[56].ToString();
                        juntaComponente.InspetorUs = linha[57].ToString();
                        juntaComponente.ComprimentoReparoUs = Utils.ConvertDouble(linha[58]);
                        juntaComponente.StatusUs = linha[59].ToString();
                        juntaComponente.StatusJunta = linha[60].ToString();
                        juntaComponente.PercLpPm = Utils.ConvertDouble(linha[63]) / 100;
                        juntaComponente.PercUt = Utils.ConvertDouble(linha[64]) / 100;
                        juntaComponente.PercRt = Utils.ConvertDouble(linha[65]) / 100;
                        juntaComponente.Componente = componente;

                        //Complemento
                        //juntaComponente.PosDf1 = uow.FindObject<Componente>(new BinaryOperator("df1", Componente.dataPosicionamento);

                        juntaComponente.PosDf1 =
                                string.IsNullOrEmpty(juntaComponente.Df1)
                                ? null
                                : uow.QueryInTransaction<Componente>()
                                    .FirstOrDefault(Posdf1 => componente.Peca == juntaComponente.Df1)?.DataPosicionamento;

                        juntaComponente.PosDf2 =
                                string.IsNullOrEmpty(juntaComponente.Df2)
                                ? null
                                : uow.QueryInTransaction<Componente>()
                                    .FirstOrDefault(Posdf2 => componente.Peca == juntaComponente.Df2)?.DataPosicionamento;

                    }
                }


                if (i % 500 == 0)
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
                    TotalRows = TotalDeJuntas,
                    CurrentRow = i + 1,
                    MessageImport = $"Importando linha {i}/{TotalDeJuntas}"
                });
            }

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = TotalDeJuntas,
                MessageImport = $"Gravando Alterações no Banco"
            });

            // Implatar funcionalidade
            //var excluirJuntasNaoImportado = oldJuntas.Where(x => x.DataExist = false);
        }
    }
}
