﻿using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Comum;

namespace WinCTB_CTS.Module.ServiceProcess.Base {
    public abstract class CalculatorProcessBase : IDisposable {
        private readonly ProviderDataLayer _providerDataLayer;
        public CancellationToken cancellationToken;
        private IProgress<ImportProgressReport> _progress { get; set; }

        [Description("Ocorre na importação de dados"), Category("Events")]
        public event EventHandler<ImporterEventArgs> ImporterHandler;
        public event EventHandler<MapImporterEventArgs> ImporterWithStreamHandler;

        public CalculatorProcessBase(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            this._providerDataLayer = new ProviderDataLayer();
            this.cancellationToken = cancellationToken;
            this._progress = progress;
        }

        public async Task ProcessarTarefaSimples() {
            await Task.Run(() => OnCalculator(_providerDataLayer, cancellationToken, _progress));
        }

        public async Task ProcessarTarefaWithStream(string TabName, string ResourceNameExemplo, string PathFileForImport) {
            await Task.Run(async () => {
                Stream streamResourceNameExemplo = GetManifestResource(ResourceNameExemplo);
                MemoryStream stream = new MemoryStream();
                StreamReader streamReader;
                stream.Seek(0, SeekOrigin.Begin);

                if (!String.IsNullOrWhiteSpace(PathFileForImport)) {
                    streamReader = GetFileStream(PathFileForImport);
                    streamReader.BaseStream.CopyTo(stream);
                } else {
                    streamResourceNameExemplo.CopyTo(stream);
                }

                _progress.Report(new ImportProgressReport {
                    TotalRows = 100,
                    CurrentRow = 0,
                    MessageImport = $"Inicializando importação da tabela {TabName}"
                });

                stream.Seek(0, SeekOrigin.Begin);
                using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream)) {
                    var dtcollectionImport = excelReader.CreateDataTableCollection(false);
                    await InitializeImportWithStream(TabName, dtcollectionImport[TabName], _progress);
                };
            });
        }

        public async Task InitializeImportWithStream(string TabName, DataTable DataTableImport, IProgress<ImportProgressReport> progress) {
            await Task.Run(() => {
                var TotalRowsForImporter = DataTableImport.Rows.Count;
                UnitOfWork uow = new UnitOfWork(_providerDataLayer.GetDataLayerThreadSafe());

                //var queryParts = DataTableImport.Rows.AsEnumerable().Select((x, y) => new { x.ItemArray, y }).SplitQuery(10);

                //var tasks = new List<Task>();

                //foreach (var parts in queryParts) {
                //    tasks.Add(Task.Run(() => {
                //        foreach (var item in parts) {
                //            var teste = item.ItemArray;
                //        }
                //    }));
                //}

                //Task.WhenAll(tasks);

                uow.BeginTransaction();

                for (int i = 0; i < TotalRowsForImporter; i++) {
                    cancellationToken.ThrowIfCancellationRequested();
                    var linha = DataTableImport.Rows[i];

                    OnMapImporter(uow, DataTableImport, linha, TotalRowsForImporter, i);

                    if (i % 1000 == 0) {
                        try {
                            uow.CommitTransaction();
                        } catch {
                            uow.RollbackTransaction();
                            throw new Exception("Process aborted by system");
                        }
                        progress.Report(new ImportProgressReport {
                            TotalRows = TotalRowsForImporter,
                            CurrentRow = i + 1,
                            MessageImport = $"Importando tabela {TabName} {i}/{TotalRowsForImporter}"
                        });
                    }
                }

                progress.Report(new ImportProgressReport {
                    TotalRows = TotalRowsForImporter,
                    CurrentRow = TotalRowsForImporter,
                    MessageImport = $"Gravando alterações da {TabName}..."
                });

                uow.CommitTransaction();
                uow.CommitChanges();
                uow.Dispose();

                progress.Report(new ImportProgressReport {
                    TotalRows = TotalRowsForImporter,
                    CurrentRow = TotalRowsForImporter,
                    MessageImport = $"Finalizado {TabName}!"
                });
            });
        }

        protected virtual void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            if (ImporterHandler != null)
                ImporterHandler(this, new ImporterEventArgs(provider, cancellationToken, progress));
        }

        protected virtual void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex) {
            if (ImporterWithStreamHandler != null) {
                //Debug.WriteLine($"Importando registro: {index.ToString().PadLeft(8, '0')}/{expectedTotal.ToString().PadLeft(8, '0')} da Tabela {SetTabName}");
                ImporterWithStreamHandler(this, new MapImporterEventArgs(uow, dataTable, rowForMap, expectedTotal, currentIndex));
            }
        }

        private StreamReader GetFileStream(string PathFileForImport) {
            string filePath = Path.GetFullPath(PathFileForImport);
            StreamReader sr = new StreamReader(filePath);
            return sr;
        }

        private static Stream GetManifestResource(string ResourceName) {
            Type moduleType = typeof(WinCTB_CTSModule);
            string name = $"{moduleType.Namespace}.Resources.{ResourceName}";
            return moduleType.Assembly.GetManifestResourceStream(name);
        }

        public void Dispose() {
            _providerDataLayer?.Dispose();
        }
    }
}
