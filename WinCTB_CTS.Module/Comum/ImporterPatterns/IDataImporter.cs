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
        ProviderDataLayer providerDataLayer { get; set; }
        ParametrosImportBase SetParametros { get; set; }
        
        event EventHandler<MapImporterEventArgs> MapImporter;
        void LogTrace(ImportProgressReport value);        
        Task InitializeImport(DataTable DataTableImport, IProgress<ImportProgressReport> progress);
    }
}
