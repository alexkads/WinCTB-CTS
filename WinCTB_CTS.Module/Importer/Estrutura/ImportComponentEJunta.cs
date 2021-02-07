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
                if (i >= 7)
                {
                    var linha = dtSpoolsImport.Rows[i];
                    var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", linha[0].ToString()));
                    var documento = linha[2].ToString();
                    var desenho = linha[9].ToString();
                    var tagSpool = $"{Convert.ToString(linha[9])}-{Convert.ToString(linha[10])}";

                    var criteriaOperator = CriteriaOperator.Parse("Contrato.Oid = ? And Documento = ? And Isometrico = ? And TagSpool = ?",
                        contrato.Oid, documento, desenho, tagSpool);

                    var componente = uow.FindObject<Componente>(criteriaOperator);

                    if (componente == null)
                        componente = new Componente(uow);
                    else
                        oldComponets.FirstOrDefault(x => x.Oid == componente.Oid).DataExist = true;

                    //Mapear campos aqui
                    //componente.Contrato = contrato;
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
                if (i >= 9)
                {
                    var linha = dtJuntasImport.Rows[i];
                    var PesquisarSpool = linha[8].ToString();
                    var FiltroPesquisa = new BinaryOperator("TagSpool", PesquisarSpool);
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
