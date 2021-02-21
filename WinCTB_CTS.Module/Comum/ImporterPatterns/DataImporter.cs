using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Estrutura;

namespace WinCTB_CTS.Module.Comum.ImporterPatterns
{
    public abstract class DataImporter : IDataImporter
    {
        public CancellationTokenSource SetCancellationTokenSource { get; set; }
        public ProviderDataLayer providerDataLayer { get; set; }
        public ParametrosImportBase SetParametros { get; set; }
        public IList ObjectsForImporter { get; set; }
        public object CurrentObject { get; set; }
        public object ObjectMap { get; set; }
        public event EventHandler<MapImporterEventArgs> MapImporter;
        public DataImporter(ParametrosImportBase Parametros, CancellationTokenSource _cancellationTokenSource)
        {
            this.SetCancellationTokenSource = _cancellationTokenSource;
            this.providerDataLayer = new ProviderDataLayer();
            this.SetParametros = Parametros;
        }
        public void LogTrace(ImportProgressReport value)
        {
            var progresso = (value.TotalRows > 0 && value.CurrentRow > 0)
                ? (value.CurrentRow / value.TotalRows)
                : 0D;

            SetParametros.Progresso = progresso;
        }
        public async Task InitializeImport(DataTable DataTableImport, IProgress<ImportProgressReport> progress)
        {
            await Task.Factory.StartNew(() =>
            {
                UnitOfWork uow = new UnitOfWork(providerDataLayer.GetCacheDataLayer());
                var TotalRowsForImporter = DataTableImport.Rows.Count;

                progress.Report(new ImportProgressReport
                {
                    TotalRows = TotalRowsForImporter,
                    CurrentRow = 0,
                    MessageImport = "Inicializando importação"
                });

                uow.BeginTransaction();

                Observable.Range(0, TotalRowsForImporter)
                .Subscribe(i =>
                {
                    var linha = DataTableImport.Rows[i];

                    //Mapear importação
                    OnMapImporter(uow, linha);

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

                        progress.Report(new ImportProgressReport
                        {
                            TotalRows = TotalRowsForImporter,
                            CurrentRow = i + 1,
                            MessageImport = $"Importando linha {i}/{TotalRowsForImporter}"
                        });
                    }
                });


                progress.Report(new ImportProgressReport
                {
                    TotalRows = TotalRowsForImporter,
                    CurrentRow = TotalRowsForImporter,
                    MessageImport = $"Gravando Alterações no Banco"
                });

                uow.CommitTransaction();
                uow.PurgeDeletedObjects();
                uow.CommitChanges();
                uow.Dispose();

                progress.Report(new ImportProgressReport
                {
                    TotalRows = TotalRowsForImporter,
                    CurrentRow = TotalRowsForImporter,
                    MessageImport = $"Processo Finalizado"
                });
            });
        }

        protected virtual void OnMapImporter(UnitOfWork uow, DataRow rowForMap)
        {
            if (MapImporter != null)
            {
                MapImporter(this, new MapImporterEventArgs(uow, rowForMap));
            }
        }
    }
}
