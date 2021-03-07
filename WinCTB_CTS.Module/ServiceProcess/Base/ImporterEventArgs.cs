using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.ServiceProcess.Base
{
    public class ImporterEventArgs : EventArgs
    {
        public ProviderDataLayer provider;
        public CancellationToken cancellationToken;
        public IProgress<ImportProgressReport> progress;
        public ImporterEventArgs(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            this.provider = provider;
            this.cancellationToken = cancellationToken;
            this.progress = progress;
        }
    }
}
