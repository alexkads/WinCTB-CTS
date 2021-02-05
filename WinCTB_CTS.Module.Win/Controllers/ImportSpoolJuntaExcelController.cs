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
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Tubulacao;
using WinCTB_CTS.Module.Win.Actions;
using WinCTB_CTS.Module.Win.Editors;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportSpoolJuntaExcelController : WindowController
    {
        IObjectSpace objectSpace = null;
        ParametrosImportSpoolJuntaExcel parametrosImportSpoolJuntaExcel;
        public ImportSpoolJuntaExcelController()
        {
            TargetWindowType = WindowType.Main;

            SimpleAction simpleActionImport = new SimpleAction(this, "PopupWindowShowActionImportSpoolJuntaExcelController", PredefinedCategory.RecordEdit)
            {
                Caption = "Importar SGS e SGJ",
                ImageName = "Action_Debug_Step"
            };

            simpleActionImport.Execute += SimpleActionImport_Execute;
        }

        private void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpace = Application.CreateObjectSpace(typeof(ParametrosImportSpoolJuntaExcel));
            parametrosImportSpoolJuntaExcel = objectSpace.CreateObject<ParametrosImportSpoolJuntaExcel>();
            DetailView view = Application.CreateDetailView(objectSpace, parametrosImportSpoolJuntaExcel);

            view.ViewEditMode = ViewEditMode.Edit;

            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
        }

        private DialogController dialogControllerAcceptingImportarPlanilha()
        {
            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
            dialogControllerImportarPlanilha.AcceptAction.Caption = "Importar";
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting; ;
            dialogControllerImportarPlanilha.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }

        private async void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {

            DataTableCollection dtcollectionImport = null;

            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = false;
            //Necessário para não fechar a janela após a conclusão do processamento
            e.Cancel = true;
            e.AcceptActionArgs.Action.Caption = "Procesando";

            var parametros = (ParametrosImportSpoolJuntaExcel)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);

            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }

            var import = new ImportSpoolEJunta(objectSpace, parametrosImportSpoolJuntaExcel);
            var progress = new Progress<ImportProgressReport>(import.LogTrace);

            await Observable.Start(() => import.ImportarSpools(dtcollectionImport["SGS"], progress));
            await Observable.Start(() => import.ImportarJuntas(dtcollectionImport["SGJ"], progress));

            objectSpace.CommitChanges();
            e.AcceptActionArgs.Action.Caption = "Finalizado";
            ((DialogController)sender).AcceptAction.Enabled["NoEnabled"] = true;
        }

        private void LogTrace(ImportProgressReport value)
        {
            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
                ? (value.CurrentRow / value.TotalRows)
                : 0D;

            parametrosImportSpoolJuntaExcel.Progresso = progresso;
            //statusProgess.Text = value.MessageImport;
        }

        private void ImportarSpools(DataTable dtSpoolsImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtSpoolsImport.Rows.Count;

            var oldSpools = Utils.GetOldDatasForCheck<Spool>(uow);

            progress.Report(new ImportProgressReport
            {
                TotalRows = TotalDeJuntas,
                CurrentRow = 0,
                MessageImport = "Inicializando importação"
            });

            uow.BeginTransaction();

            for (int i = 0; i < TotalDeJuntas; i++)
            {
                if(i >= 7)
                {
                    var linha = dtSpoolsImport.Rows[i];
                    var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", linha[0].ToString()));
                    var documento = linha[2].ToString();
                    var isometrico = linha[9].ToString();
                    var tagSpool = $"{Convert.ToString(linha[9])}-{Convert.ToString(linha[10])}";

                    var criteriaOperator = CriteriaOperator.Parse("Contrato.Oid = ? And Documento = ? And Isometrico = ? And TagSpool = ?",
                        contrato.Oid, documento, isometrico, tagSpool);

                    var spool = uow.FindObject<Spool>(criteriaOperator);

                    if (spool == null)
                        spool = new Spool(uow);
                    else
                        oldSpools.FirstOrDefault(x => x.Oid == spool.Oid).DataExist = true;

                    //var spool = objectSpace.CreateObject<Spool>();
                    spool.Contrato = contrato;
                    spool.SiteFabricante = linha[8].ToString();
                    spool.ArranjoFisico = linha[1].ToString();
                    spool.Documento = documento;
                    spool.CampoAuxiliar = linha[3].ToString();
                    spool.SubSop = linha[4].ToString();
                    spool.AreaFisica = Convert.ToString(linha[5]);
                    spool.Sth = Convert.ToString(linha[6]);
                    spool.Linha = Convert.ToString(linha[7]);
                    spool.Isometrico = isometrico;
                    spool.TagSpool = tagSpool;
                    spool.RevSpool = Convert.ToString(linha[11]);
                    spool.RevIso = Convert.ToString(linha[12]);
                    spool.Material = Convert.ToString(linha[13]);
                    spool.Norma = Convert.ToString(linha[14]);
                    spool.Diametro = Utils.ConvertINT(linha[15]);
                    spool.DiametroPolegada = Convert.ToString(linha[16]);
                    spool.Espessura = Utils.ConvertINT(linha[17]);
                    spool.Espec = Convert.ToString(linha[18]);
                    spool.PNumber = Convert.ToString(linha[19]);
                    spool.Fluido = Convert.ToString(linha[20]);
                    spool.TipoIsolamento = Convert.ToString(linha[21]);
                    spool.CondicaoPintura = Convert.ToString(linha[22]);
                    spool.Comprimento = Convert.ToDouble(linha[23]);
                    spool.PesoFabricacao = Convert.ToDouble(linha[24]);
                    spool.Area = Convert.ToString(linha[25]);
                    spool.EspIsolamento = Convert.ToString(linha[26]);
                    spool.QuantidadeIsolamento = Utils.ConvertINT(linha[27]);
                    spool.TotaldeJuntas = Utils.ConvertINT(linha[28]);
                    spool.TotaldeJuntasPipe = Utils.ConvertINT(linha[29]);
                    spool.DataCadastro = Utils.ConvertDateTime(linha[30]);
                    spool.NrProgFab = linha[31].ToString();
                    spool.DataProgFab = Utils.ConvertDateTime(linha[32]);
                    spool.DataCorte = Utils.ConvertDateTime(linha[33]);
                    spool.DataVaFab = Utils.ConvertDateTime(linha[34]);
                    spool.DataSoldaFab = Utils.ConvertDateTime(linha[35]);
                    spool.DataVsFab = Utils.ConvertDateTime(linha[36]);
                    spool.DataDfFab = Utils.ConvertDateTime(linha[37]);
                    spool.RelatorioDf = Convert.ToString(linha[38]);
                    spool.InspetorDf = Convert.ToString(linha[39]);
                    spool.DataEndFab = Utils.ConvertDateTime(linha[40]);
                    spool.DataPiFundo = Utils.ConvertDateTime(linha[41]);
                    spool.InspPinturaFundo = Convert.ToString(linha[42]);
                    spool.RelatorioPinFundo = Convert.ToString(linha[43]);
                    spool.RelIndFundo = Convert.ToString(linha[44]);
                    spool.DataPiIntermediaria = Utils.ConvertDateTime(linha[45]);
                    spool.InspPiIntermediaria = Convert.ToString(linha[46]);
                    spool.RelPiIntermediaria = Convert.ToString(linha[47]);
                    spool.RelIndIntermediaria = Convert.ToString(linha[48]);
                    spool.DataPiAcabamento = Utils.ConvertDateTime(linha[49]);
                    spool.InspPintAcabamento = Convert.ToString(linha[50]);
                    spool.RelPintAcabamento = Convert.ToString(linha[51]);
                    spool.RelIndPintAcabamento = Convert.ToString(linha[52]);
                    spool.DataPiRevUnico = Utils.ConvertDateTime(linha[53]);
                    spool.InspPiRevUnico = Convert.ToString(linha[54]);
                    spool.RelPiRevUnico = Convert.ToString(linha[55]);
                    spool.DataPintFab = Utils.ConvertDateTime(linha[56]);
                    spool.ProgMontagem = Convert.ToString(linha[57]);
                    spool.Elevacao = Convert.ToString(linha[58]);
                    spool.ProgPintura = Convert.ToString(linha[59]);
                    spool.EscopoMontagem = Convert.ToString(linha[60]);
                    spool.DataPreMontagem = Utils.ConvertDateTime(linha[61]);
                    spool.DataVaMontagem = Utils.ConvertDateTime(linha[62]);
                    spool.DataSoldaMontagem = Utils.ConvertDateTime(linha[63]);
                    spool.DataVsMontagem = Utils.ConvertDateTime(linha[64]);
                    spool.InspDiMontagem = Convert.ToString(linha[65]);
                    spool.DataDiMontagem = Utils.ConvertDateTime(linha[66]);
                    spool.DataEndMontagem = Utils.ConvertDateTime(linha[67]);
                    spool.DataPintMontagem = Utils.ConvertDateTime(linha[68]);
                    spool.TagComponente = Convert.ToString(linha[69]);
                    spool.IdComponente = Convert.ToString(linha[70]);
                    spool.Romaneio = Convert.ToString(linha[71]);
                    spool.DataRomaneio = Utils.ConvertDateTime(linha[72]);
                    spool.DataLiberacao = Utils.ConvertDateTime(linha[73]);
                    spool.PesoMontagem = Utils.ConvertDouble(linha[74]);
                    spool.SituacaoFabricacao = Convert.ToString(linha[75]);
                    spool.SituacaoMontagem = Convert.ToString(linha[76]);
                    //spool.DataLineCheck = Utils.ConvertDateTime(linha[75]);

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

        private void ImportarJuntas(DataTable dtJuntasImport, IProgress<ImportProgressReport> progress)
        {
            UnitOfWork uow = new UnitOfWork(((XPObjectSpace)objectSpace).Session.ObjectLayer);
            var TotalDeJuntas = dtJuntasImport.Rows.Count;

            var oldJuntas = Utils.GetOldDatasForCheck<JuntaSpool>(uow);

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
                if(i >= 9)
                {
                    var linha = dtJuntasImport.Rows[i];
                    var PesquisarSpool = linha[8].ToString();
                    var FiltroPesquisa = new BinaryOperator("TagSpool", PesquisarSpool);
                    var spool = uow.FindObject<Spool>(FiltroPesquisa);
                    if (spool != null)
                    {
                        var junta = linha[9].ToString();

                        var criteriaOperator = CriteriaOperator.Parse("Spool.Oid = ? And Junta = ?",
                            spool.Oid, junta);

                        var juntaSpool = uow.FindObject<JuntaSpool>(criteriaOperator);

                        if (juntaSpool == null)
                            juntaSpool = new JuntaSpool(uow);
                        else
                            oldJuntas.FirstOrDefault(x => x.Oid == juntaSpool.Oid).DataExist = true;

                        juntaSpool.Site = linha[0].ToString();
                        juntaSpool.ArranjoFisico = linha[1].ToString();
                        juntaSpool.CampoAuxiliar = linha[2].ToString();
                        juntaSpool.ProgFab = linha[3].ToString();
                        juntaSpool.Sop = linha[4].ToString();
                        juntaSpool.Sth = linha[5].ToString();
                        juntaSpool.Documento = linha[6].ToString();
                        juntaSpool.Linha = linha[7].ToString();
                        juntaSpool.Junta = junta;
                        juntaSpool.TabDiametro = uow.FindObject<TabDiametro>(new BinaryOperator("DiametroPolegada", linha[11].ToString()));
                        juntaSpool.Espessura = Utils.ConvertDouble(linha[12]);
                        juntaSpool.TipoJunta = linha[14].ToString();
                        juntaSpool.TabPercInspecao = uow.FindObject<TabPercInspecao>(new BinaryOperator("Spec", linha[15].ToString()));
                        juntaSpool.MaterialPt = linha[16].ToString();
                        //juntaSpool.MaterialEn = linha[17].ToString();
                        juntaSpool.ClasseInspecao = linha[18].ToString();
                        juntaSpool.Norma = linha[19].ToString();
                        juntaSpool.CampoOuPipe = Utils.ConvertStringEnumCampoPipe(linha[20].ToString());
                        juntaSpool.StatusVa = linha[21].ToString();
                        juntaSpool.RelatorioVa = linha[22].ToString();
                        juntaSpool.ExecutanteVa = linha[23].ToString();
                        juntaSpool.DataVa = Utils.ConvertDateTime(linha[24]);
                        juntaSpool.StatusResold = linha[25].ToString();
                        juntaSpool.SoldadorRaiz = linha[26].ToString();
                        juntaSpool.SoldadorEnch = linha[27].ToString();
                        juntaSpool.SoldadorAcab = linha[28].ToString();
                        juntaSpool.RelatorioSoldagem = linha[29].ToString();
                        juntaSpool.ExecutanteResold = linha[30].ToString();
                        juntaSpool.Eps = linha[31].ToString();
                        juntaSpool.DataSoldagem = Utils.ConvertDateTime(linha[32]);
                        juntaSpool.ConsumivelRaiz = linha[33].ToString();
                        juntaSpool.ConsumivelEnch = linha[34].ToString();
                        juntaSpool.ConsumivelAcab = linha[35].ToString();
                        juntaSpool.VisualStatus = linha[36].ToString();
                        juntaSpool.RelatorioVs = linha[37].ToString();
                        juntaSpool.InspetorVS = linha[38].ToString();
                        juntaSpool.DataVisualSolda = Utils.ConvertDateTime(linha[39]);
                        juntaSpool.StatusLpPm = linha[40].ToString();
                        juntaSpool.RelatorioLp = linha[41].ToString();
                        juntaSpool.InspetorLp = linha[42].ToString();
                        juntaSpool.DataLp = Utils.ConvertDateTime(linha[43]);
                        juntaSpool.RelatorioPm = linha[44].ToString();
                        juntaSpool.InspetorPm = linha[45].ToString();
                        juntaSpool.DataPm = Utils.ConvertDateTime(linha[46]);
                        juntaSpool.StatusTt = linha[47].ToString();
                        juntaSpool.RelatorioTt = linha[48].ToString();
                        juntaSpool.DataTt = Utils.ConvertDateTime(linha[49]);
                        juntaSpool.StatusDureza = linha[50].ToString();
                        juntaSpool.RelatorioDureza = linha[51].ToString();
                        juntaSpool.InspetorDureza = linha[52].ToString();
                        juntaSpool.DataDureza = Utils.ConvertDateTime(linha[53]);
                        juntaSpool.StatusRastMaterial = linha[54].ToString();
                        juntaSpool.RelRastMaterial = linha[55].ToString();
                        juntaSpool.ExecutanteRastMaterial = linha[56].ToString();
                        juntaSpool.DataRastMaterial = Utils.ConvertDateTime(linha[57]);
                        juntaSpool.StatusIdLiga = linha[58].ToString();
                        juntaSpool.RelIdLiga = linha[59].ToString();
                        juntaSpool.InspetorIdLiga = linha[60].ToString();
                        juntaSpool.DataIdLiga = Utils.ConvertDateTime(linha[61]);
                        juntaSpool.StatusRxUs = linha[62].ToString();
                        juntaSpool.ProgRx = linha[63].ToString();
                        juntaSpool.RelatorioRx = linha[64].ToString();
                        juntaSpool.InspetorRx = linha[65].ToString();
                        juntaSpool.DataRx = Utils.ConvertDateTime(linha[66]);
                        juntaSpool.RelatorioUs = linha[67].ToString();
                        juntaSpool.InspetorUs = linha[68].ToString();
                        juntaSpool.DataUs = Utils.ConvertDateTime(linha[69]);
                        juntaSpool.StatusFerrita = linha[70].ToString();
                        juntaSpool.RelatorioFerrita = linha[71].ToString();
                        juntaSpool.InspetorFerrita = linha[72].ToString();
                        juntaSpool.DataFerrita = Utils.ConvertDateTime(linha[73]);
                        juntaSpool.StatusEstanqueidade = linha[74].ToString();
                        juntaSpool.RelatorioEstanqueidade = linha[75].ToString();
                        juntaSpool.InspetorEstanqueidade = linha[76].ToString();
                        juntaSpool.DataEstanqueidade = Utils.ConvertDateTime(linha[77]);
                        juntaSpool.RelDimFab = linha[78].ToString();
                        juntaSpool.ProgFabJunta = linha[79].ToString();
                        juntaSpool.LoteRx = linha[80].ToString();
                        juntaSpool.LoteLp = linha[81].ToString();
                        juntaSpool.DataLiberacaoJunta = Utils.ConvertDateTime(linha[82]);
                        juntaSpool.SituacaoJunta = linha[83].ToString();
                        juntaSpool.Spool = spool;
                    }

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
            //var excluirJuntasNaoImportado = oldJuntas.Where(x => x.DataExist = false);
        }
    }


}
