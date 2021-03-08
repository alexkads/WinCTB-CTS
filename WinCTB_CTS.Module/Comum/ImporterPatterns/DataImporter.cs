//using DevExpress.Xpo;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.Helpers;
//using WinCTB_CTS.Module.ServiceProcess.Base;

//namespace WinCTB_CTS.Module.Comum.ImporterPatterns
//{
//    public abstract class DataImporter : IDataImporter
//    {
//        public CancellationTokenSource SetCancellationTokenSource { get; set; }
//        public ProviderDataLayer providerDataLayer { get; set; }
//        public IList ObjectsForImporter { get; set; }
//        public object CurrentObject { get; set; }
//        public object ObjectMap { get; set; }
//        public string SetTabName { get; set; }
//        public IProgress<ImportProgressReport> SetProgress { get; set; }

//        public event EventHandler<MapImporterEventArgs> MapImporter;

//        //public event EventHandler<ImportProgressReport> ProgressHandler;

//        public DataImporter(CancellationTokenSource cancellationTokenSource, string TabName)
//        {
//            this.SetCancellationTokenSource = cancellationTokenSource;
//            this.SetTabName = TabName;
//            this.SetProgress = new Progress<ImportProgressReport>(LogTrace);
//            this.providerDataLayer = new ProviderDataLayer();
//        }

//        public void LogTrace(ImportProgressReport value)
//        {
//            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
//                ? (value.CurrentRow / value.TotalRows)
//                : 0D;

//            //SetParametros.Progresso = progresso;

//            ////Notificar Progresso

//            //if (ProgressHandler != null && value != null)
//            //    ProgressHandler(this, value);
//        }

//        private StreamReader GetFileStream(string PathFileForImport) 
//        {
//            string filePath = Path.GetFullPath(PathFileForImport);
//            StreamReader sr = new StreamReader(filePath);
//            return sr;
//        }

//        public async Task Start()
//        {
//            MemoryStream stream = new MemoryStream();
//            StreamReader streamReader;
//            stream.Seek(0, SeekOrigin.Begin);                                
//            var parametros = (ParametrosImportBase)SetParametros;
//            var arquivo = ((ParametrosImportBase)SetParametros).PadraoDeArquivo;

//            if (!String.IsNullOrWhiteSpace(parametros.PathFileForImport))
//            {
//                streamReader = GetFileStream(parametros.PathFileForImport);
//                streamReader.BaseStream.CopyTo(stream);
//            }
//            else
//            {
//                arquivo.SaveToStream(stream);
//            }

//            stream.Seek(0, SeekOrigin.Begin);

//            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
//            {
//                var dtcollectionImport = excelReader.CreateDataTableCollection(false);
//                var progress = new Progress<ImportProgressReport>(LogTrace);
//                await InitializeImport(dtcollectionImport[SetTabName], SetProgress);
//            }
//        }

//        public async Task InitializeImport(DataTable DataTableImport, IProgress<ImportProgressReport> progress)
//        {
//            await Task.Factory.StartNew(() =>
//            {
//                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetCacheDataLayer());
//                var TotalRowsForImporter = DataTableImport.Rows.Count;

//                progress.Report(new ImportProgressReport
//                {
//                    TotalRows = TotalRowsForImporter,
//                    CurrentRow = 0,
//                    MessageImport = "Inicializando importação"
//                });

//                Debug.WriteLine($"Inicializando importação da Tabela {SetTabName}");

//                uow.BeginTransaction();

//                Observable.Range(0, TotalRowsForImporter)
//                .Subscribe(i =>
//                {
//                    var linha = DataTableImport.Rows[i];

//                    //Mapear importação
//                    OnMapImporter(uow, DataTableImport, linha, TotalRowsForImporter, i);

//                    if (i % 1000 == 0)
//                    {
//                        try
//                        {
//                            uow.CommitTransaction();
//                        }
//                        catch
//                        {
//                            uow.RollbackTransaction();
//                            throw new Exception("Process aborted by system");
//                        }

//                        progress.Report(new ImportProgressReport
//                        {
//                            TotalRows = TotalRowsForImporter,
//                            CurrentRow = i + 1,
//                            MessageImport = $"Importando linha {i}/{TotalRowsForImporter}"
//                        });
//                    }
//                });


//                progress.Report(new ImportProgressReport
//                {
//                    TotalRows = TotalRowsForImporter,
//                    CurrentRow = TotalRowsForImporter,
//                    MessageImport = $"Gravando Alterações no Banco"
//                });

//                uow.CommitTransaction();                
//                uow.CommitChanges();
//                uow.Dispose();

//                progress.Report(new ImportProgressReport
//                {
//                    TotalRows = TotalRowsForImporter,
//                    CurrentRow = TotalRowsForImporter,
//                    MessageImport = $"Processo Finalizado"
//                });
//            });
//        }

//        protected virtual void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
//        {
//            if (MapImporter != null)
//            {
//                //Debug.WriteLine($"Importando registro: {index.ToString().PadLeft(8, '0')}/{expectedTotal.ToString().PadLeft(8, '0')} da Tabela {SetTabName}");
//                MapImporter(this, new MapImporterEventArgs(uow, dataTable, rowForMap, expectedTotal, currentIndex));
//            }
//        }
//    }
//}
