using DevExpress.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Estrutura;

namespace WinCTB_CTS.Module.Comum.ImporterPatterns
{
    interface IDataImporter
    {
        CancellationTokenSource SetCancellationTokenSource { get; set; }
        IProgress<ImportProgressReport> SetProgress { get; set; }
        ParametrosImportBase SetParametros { get; set; }
        string SetTabName { get; set; }
        ProviderDataLayer providerDataLayer { get; set; }

        void LogTrace(ImportProgressReport value);

        event EventHandler<MapImporterEventArgs> MapImporter;
        
        //event EventHandler<ImportProgressReport> ProgressHandler;

        Task Start();

        Task InitializeImport(DataTable DataTableImport, IProgress<ImportProgressReport> progress);
    }
}
