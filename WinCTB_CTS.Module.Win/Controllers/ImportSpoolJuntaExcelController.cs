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
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Win.Actions;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportSpoolJuntaExcelController : WindowController
    {
        public ImportSpoolJuntaExcelController()
        {
            TargetWindowType = WindowType.Main;

            SimpleAction simpleActionImport = new SimpleAction(this, "PopupWindowShowActionImportSpoolJuntaExcelController", PredefinedCategory.RecordEdit)
            {
                Caption = "Importar",
                ImageName = "Action_Debug_Step"
            };

            simpleActionImport.Execute += SimpleActionImport_Execute;
        }

        private async void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            using (IObjectSpace os = Application.CreateObjectSpace())
            {
                await Executar((XPObjectSpace)os);
                os.Dispose();
            }
        }

        static void cancelProgress_Click(object sender, EventArgs e)
        {
            ((Form)(((SimpleButton)sender).Parent)).Close();
            throw new Exception("Process aborted by user.");
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

        public async Task Executar(XPObjectSpace objectSpace)
        {
            var session = objectSpace.Session;
            var dtSpoolsImport = new DataTable();
            var dtJuntasImport = new DataTable();
            DataTableCollection dtcollectionImport = null;

            InitializeInteface();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileStream = openFileDialog.OpenFile();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        fileStream.CopyTo(stream);

                        using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
                        {
                            dtcollectionImport = excelReader.CreateDataTableCollection();
                        }

                        dtSpoolsImport = dtcollectionImport["SGS"];
                        dtJuntasImport = dtcollectionImport["SGJ"];
                    }

                    fileStream.Dispose();

                    var progress = new Progress<ImportProgressReport>(value =>
                    {
                        //XtraProgressImport;

                        progressBarControl.Properties.Maximum = value.TotalRows;
                        statusProgess.Text = value.MessageImport;

                        if (value.CurrentRow > 0)
                            progressBarControl.PerformStep();

                        progressBarControl.Update();
                        statusProgess.Update();
                    });

                    await Task.Run(() =>
                        ImportarSpools(objectSpace, dtSpoolsImport, progress));

                    progressBarControl.EditValue = 0;

                    await Task.Run(() =>
                        ImportarJuntas(objectSpace, dtJuntasImport, progress));

                    FormProgressImport.Close();
                }
            }

            //int QuantidadeDeRegistro = dtSpoolsImport.Rows.Count;
        }

        private void ImportarSpools(XPObjectSpace objectSpace, DataTable dtSpoolsImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork((objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtSpoolsImport.Rows.Count;

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = 0,
                MessageImport = "Inicializando importação"
            });

            uow.BeginTransaction();

            //Limpar registros
            Utils.DeleteAllRecords<Spool>(uow);
            uow.CommitTransaction();

            for (int i = 0; i < TotalDeJuntas; i++)
            {
                var linha = dtSpoolsImport.Rows[i];
                var spool = new Spool(uow);
                //var spool = objectSpace.CreateObject<Spool>();
                spool.Contrato = linha["contrato"].ToString();
                spool.ArranjoFisico = linha["arranjoFisico"].ToString();
                spool.Documento = linha["documento"].ToString();
                spool.CampoAuxiliar = linha["campoAuxiliar"].ToString();
                spool.SubSop = linha["subSop"].ToString();
                spool.AreaFisica = Convert.ToString(linha["areaFisica"]);
                spool.Sth = Convert.ToString(linha["sth"]);
                spool.Linha = Convert.ToString(linha["linha"]);
                spool.SiteFabricante = Convert.ToString(linha["siteFabricante"]);
                spool.Isometrico = Convert.ToString(linha["isometrico"]);
                spool.TagSpool = $"{Convert.ToString(linha["isometrico"])}-{Convert.ToString(linha["tagSpool"])}" ;
                spool.RevSpool = Convert.ToString(linha["revSpool"]);
                spool.RevIso = Convert.ToString(linha["revIso"]);
                spool.Material = Convert.ToString(linha["material"]);                
                spool.Norma = Convert.ToString(linha["norma"]);
                spool.Diametro = Utils.ConvertINT(linha["diametro"]);
                spool.DiametroPolegada = Convert.ToString(linha["diametroPolegada"]);
                spool.Espessura = Utils.ConvertINT(linha["espessura"]);
                spool.Espec = Convert.ToString(linha["espec"]);
                spool.PNumber = Convert.ToString(linha["pNumber"]);
                spool.Fluido = Convert.ToString(linha["fluido"]);
                spool.TipoIsolamento = Convert.ToString(linha["tipoIsolamento"]);
                spool.CondicaoPintura = Convert.ToString(linha["condicaoPintura"]);
                spool.Comprimento = Convert.ToDouble(linha["comprimento"]);
                spool.PesoFabricacao = Convert.ToDouble(linha["pesoFabricacao"]);
                spool.Area = Convert.ToString(linha["area"]);
                spool.EspIsolamento = Convert.ToString(linha["espIsolamento"]);
                spool.QuantidadeIsolamento = Utils.ConvertINT(linha["quantidadeIsolamento"]);
                spool.TotaldeJuntas = Utils.ConvertINT(linha["totaldeJuntas"]);
                spool.TotaldeJuntasPipe = Utils.ConvertINT(linha["totaldeJuntasPipe"]);
                spool.DataCadastro = Utils.ConvertDateTime(linha["dataCadastro"]);
                spool.NrProgFab = linha["nrProgFab"].ToString();
                spool.DataProgFab = Utils.ConvertDateTime(linha["dataProgFab"]);
                spool.DataCorte = Utils.ConvertDateTime(linha["dataCorte"]);
                spool.DataVaFab = Utils.ConvertDateTime(linha["dataVaFab"]);
                spool.DataSoldaFab = Utils.ConvertDateTime(linha["dataSoldaFab"]);
                spool.DataVsFab = Utils.ConvertDateTime(linha["dataVsFab"]);
                spool.DataDfFab = Utils.ConvertDateTime(linha["dataDfFab"]);
                spool.RelatorioDf = Convert.ToString(linha["relatorioDf"]);
                spool.DataEndFab = Utils.ConvertDateTime(linha["dataEndFab"]);
                spool.DataPiFundo = Utils.ConvertDateTime(linha["dataPiFundo"]);
                spool.InspPinturaFundo = Convert.ToString(linha["inspPinturaFundo"]);
                spool.RelatorioPinFundo = Convert.ToString(linha["relatorioPinFundo"]);
                spool.RelIndFundo = Convert.ToString(linha["relIndFundo"]);
                spool.DataPiIntermediaria = Utils.ConvertDateTime(linha["dataPiIntermediaria"]);
                spool.InspPiIntermediaria = Convert.ToString(linha["inspPiIntermediaria"]);
                spool.RelPiIntermediaria = Convert.ToString(linha["relPiIntermediaria"]);
                spool.RelIndIntermediaria = Convert.ToString(linha["relIndIntermediaria"]);
                spool.DataPiAcabamento = Utils.ConvertDateTime(linha["dataPiAcabamento"]);
                spool.InspPintAcabamento = Convert.ToString(linha["inspPintAcabamento"]);
                spool.RelPintAcabamento = Convert.ToString(linha["relPintAcabamento"]);
                spool.RelIndPintAcabamento = Convert.ToString(linha["relIndPintAcabamento"]);
                spool.DataPiRevUnico = Utils.ConvertDateTime(linha["dataPiRevUnico"]);
                spool.InspPiRevUnico = Convert.ToString(linha["inspPiRevUnico"]);
                spool.RelPiRevUnico = Convert.ToString(linha["relPiRevUnico"]);
                spool.DataPintFab = Utils.ConvertDateTime(linha["dataPintFab"]);
                spool.ProgMontagem = Convert.ToString(linha["progMontagem"]);
                spool.Elevacao = Convert.ToString(linha["elevacao"]);
                spool.ProgPintura = Convert.ToString(linha["progPintura"]);
                spool.EscopoMontagem = Convert.ToString(linha["escopoMontagem"]);
                spool.DataPreMontagem = Utils.ConvertDateTime(linha["dataPreMontagem"]);
                spool.DataVaMontagem = Utils.ConvertDateTime(linha["dataVaMontagem"]);
                spool.DataSoldaMontagem = Utils.ConvertDateTime(linha["dataSoldaMontagem"]);
                spool.DataVsMontagem = Utils.ConvertDateTime(linha["dataVsMontagem"]);
                spool.InspDiMontagem = Convert.ToString(linha["inspDiMontagem"]);
                spool.DataDiMontagem = Utils.ConvertDateTime(linha["dataDiMontagem"]);
                spool.DataEndMontagem = Utils.ConvertDateTime(linha["dataEndMontagem"]);
                spool.DataPintMontagem = Utils.ConvertDateTime(linha["dataPintMontagem"]);
                spool.TagComponente = Convert.ToString(linha["tagComponente"]);
                spool.IdComponente = Convert.ToString(linha["idComponente"]);
                spool.Romaneio = Convert.ToString(linha["romaneio"]);
                spool.DataRomaneio = Utils.ConvertDateTime(linha["dataRomaneio"]);
                spool.DataLiberacao = Utils.ConvertDateTime(linha["dataLiberacao"]);
                spool.PesoMontagem = Utils.ConvertDouble(linha["pesoMontagem"]);
                spool.SituacaoFabricacao = Convert.ToString(linha["situacaoFabricacao"]);
                spool.SituacaoMontagem = Convert.ToString(linha["situacaoMontagem"]);

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
                    CurrentRow = i,
                    MessageImport = $"Importando linha {i}/{TotalDeJuntas}"
                });
            }

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = TotalDeJuntas,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();
        }

        private void ImportarJuntas(XPObjectSpace objectSpace, DataTable dtJuntasImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtJuntasImport.Rows.Count;

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = 0,
                MessageImport = "Inicializando importação de juntas"
            });

            uow.BeginTransaction();

            //Limpar registros
            Utils.DeleteAllRecords<JuntaSpool>(uow);
            uow.CommitTransaction();

            for (int i = 0; i < TotalDeJuntas; i++)
            {
                var linha = dtJuntasImport.Rows[i];
                var PesquisarSpool = linha["TagSpool"].ToString();
                var FiltroPesquisa = new BinaryOperator("TagSpool", PesquisarSpool);
                var spool = uow.FindObject<Spool>(FiltroPesquisa);
                if (spool != null)
                {
                    var juntaSpool = new JuntaSpool(uow);

                    juntaSpool.Site = linha["site"].ToString();
                    juntaSpool.ArranjoFisico = linha["arranjoFisico"].ToString();
                    juntaSpool.CampoAuxiliar = linha["campoAuxiliar"].ToString();
                    juntaSpool.ProgFab = linha["progFab"].ToString();
                    juntaSpool.Sop = linha["sop"].ToString();
                    juntaSpool.Sth = linha["sth"].ToString();
                    juntaSpool.Documento = linha["documento"].ToString();
                    juntaSpool.Linha = linha["linha"].ToString();
                    juntaSpool.Junta = linha["junta"].ToString();
                    // Daniel - Adicionar campos do SGJ aqui dentro
                    juntaSpool.TabDiametro = uow.FindObject<TabDiametro>(new BinaryOperator("DiametroPolegada", linha["diametroPolegada"].ToString()));
                    juntaSpool.Schedule = linha["schedule"].ToString();
                    juntaSpool.TipoJunta = linha["tipoJunta"].ToString();
                    juntaSpool.Spec = linha["spec"].ToString();
                    juntaSpool.MaterialPt = linha["materialPt"].ToString();
                    juntaSpool.MaterialEn = linha["materialEn"].ToString();
                    juntaSpool.ClasseInspecao = linha["classeInspecao"].ToString();
                    juntaSpool.Norma = linha["norma"].ToString();
                    juntaSpool.CampoPipe = linha["campoPipe"].ToString();
                    juntaSpool.StatusVa = linha["statusVa"].ToString();
                    juntaSpool.RelatorioVa = linha["relatorioVa"].ToString();
                    juntaSpool.DataVa = Utils.ConvertDateTime(linha["dataVa"]);
                    juntaSpool.StatusResold = linha["statusResold"].ToString();
                    juntaSpool.SoldadorRaiz = linha["soldadorRaiz"].ToString();
                    juntaSpool.SoldadorEnch = linha["soldadorEnch"].ToString();
                    juntaSpool.SoldadorAcab = linha["soldadorAcab"].ToString();
                    juntaSpool.RelatorioSoldagem = linha["relatorioSoldagem"].ToString();
                    juntaSpool.ExecutanteResold = linha["executanteResold"].ToString();
                    juntaSpool.Eps = linha["eps"].ToString();
                    juntaSpool.DataSoldagem = Utils.ConvertDateTime(linha["dataSoldagem"]);
                    juntaSpool.ConsumivelRaiz = linha["consumivelRaiz"].ToString();
                    juntaSpool.ConsumivelEnch = linha["consumivelEnch"].ToString();
                    juntaSpool.ConsumivelAcab = linha["consumivelAcab"].ToString();
                    juntaSpool.Espessura = Utils.ConvertDouble(linha["espessura"]);
                    juntaSpool.VisualStatus = linha["visualStatus"].ToString();
                    juntaSpool.RelatorioVs = linha["relatorioVs"].ToString();
                    juntaSpool.InspetorVS = linha["inspetorVs"].ToString();
                    juntaSpool.DataVisualSolda = Utils.ConvertDateTime(linha["dataVisualSolda"]);
                    juntaSpool.StatusLpPm = linha["statusLpPm"].ToString();
                    juntaSpool.RelatorioLp = linha["relatorioLp"].ToString();
                    juntaSpool.InspetorLp = linha["inspetorLp"].ToString();
                    juntaSpool.DataLp = Utils.ConvertDateTime(linha["dataLp"]);
                    juntaSpool.RelatorioPm = linha["relatorioPm"].ToString();
                    juntaSpool.InspetorPm = linha["inspetorPm"].ToString();
                    juntaSpool.DataPm = Utils.ConvertDateTime(linha["dataPm"]);
                    juntaSpool.StatusTt = linha["statusTt"].ToString();
                    juntaSpool.RelatorioTt = linha["relatorioTt"].ToString();
                    juntaSpool.DataTt = Utils.ConvertDateTime(linha["dataTt"]);
                    juntaSpool.StatusDureza = linha["statusDureza"].ToString();
                    juntaSpool.RelatorioDureza = linha["relatorioDureza"].ToString();
                    juntaSpool.InspetorDureza = linha["inspetorDureza"].ToString();
                    juntaSpool.DataDureza = Utils.ConvertDateTime(linha["dataDureza"]);
                    juntaSpool.StatusRastMaterial = linha["statusRastMaterial"].ToString();
                    juntaSpool.ExecutanteRastMaterial = linha["executanteRastMaterial"].ToString();
                    juntaSpool.DataRastMaterial = Utils.ConvertDateTime(linha["dataRastMaterial"]);
                    juntaSpool.StatusIdLiga = linha["statusIdLiga"].ToString();
                    juntaSpool.InspetorIdLiga = linha["inspetorIdLiga"].ToString();
                    juntaSpool.DataIdLiga = Utils.ConvertDateTime(linha["dataIdLiga"]);
                    juntaSpool.StatusRxUs = linha["statusRxUs"].ToString();
                    juntaSpool.ProgRx = linha["progRx"].ToString();
                    juntaSpool.RelatorioRx = linha["relatorioRx"].ToString();
                    juntaSpool.DataRx = Utils.ConvertDateTime(linha["dataRx"]);
                    juntaSpool.RelatorioUs = linha["relatorioUs"].ToString();
                    juntaSpool.InspetorUs = linha["inspetorUs"].ToString();
                    juntaSpool.InspetorRx = linha["inspetorRx"].ToString();
                    juntaSpool.DataUs = Utils.ConvertDateTime(linha["dataUs"]);
                    juntaSpool.StatusFerrita = linha["statusFerrita"].ToString();
                    juntaSpool.RelatorioFerrita = linha["relatorioFerrita"].ToString();
                    juntaSpool.InspetorFerrita = linha["inspetorFerrita"].ToString();
                    juntaSpool.DataFerrita = Utils.ConvertDateTime(linha["dataFerrita"]);
                    juntaSpool.StatusEstanqueidade = linha["statusEstanqueidade"].ToString();
                    juntaSpool.RelatorioEstanqueidade = linha["relatorioEstanqueidade"].ToString();
                    juntaSpool.InspetorEstanqueidade = linha["inspetorEstanqueidade"].ToString();
                    juntaSpool.DataEstanqueidade = Utils.ConvertDateTime(linha["dataEstanqueidade"]);
                    juntaSpool.ProgFabJunta = linha["progFabJunta"].ToString();
                    juntaSpool.LoteRx = linha["loteRx"].ToString();
                    juntaSpool.LoteLp = linha["loteLp"].ToString();
                    juntaSpool.DataLiberacaoJunta = Utils.ConvertDateTime(linha["dataLiberacaoJunta"]);
                    juntaSpool.SituacaoJunta = linha["situacaoJunta"].ToString();
                    juntaSpool.RelIdLiga = linha["relIdLiga"].ToString();
                    juntaSpool.RelDimFab = linha["relDimFab"].ToString();
                    // Daniel - Adicionar campos do SGJ aqui dentro
                    juntaSpool.Spool = spool;
                }

                if (i % 2000 == 0)
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
                    CurrentRow = i,
                    MessageImport = $"Importando linha {i}/{TotalDeJuntas}"
                });
            }

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = TotalDeJuntas,
                MessageImport = $"Gravando Alterações no Banco"
            });

            uow.CommitTransaction();
            uow.PurgeDeletedObjects();
            uow.CommitChanges();
            uow.Dispose();
        }
    }

    public class ImportProgressReport
    {
        public int TotalRows { get; set; }
        public int CurrentRow { get; set; }
        public string MessageImport { get; set; }
    }
}
