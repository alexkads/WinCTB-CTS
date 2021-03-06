using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.ServiceProcess.Base
{
    public abstract class CalculatorProcessBase : IDisposable
    {
        private readonly ProviderDataLayer _providerDataLayer;
        private CancellationToken _cancellationToken;
        private IProgress<ImportProgressReport> _progress { get; set; }

        [Description("Ocorre na importação de dados"), Category("Events")]
        public event EventHandler<ImporterEventArgs> ImporterHandler;

        public CalculatorProcessBase(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            this._providerDataLayer = new ProviderDataLayer();
            this._cancellationToken = cancellationToken;
            this._progress = progress;
        }
        public async Task ProcessarTarefa()
        {
            await Task.Factory.StartNew(() =>
            {
                OnCalculator(_providerDataLayer, _cancellationToken, _progress);
            });
        }

        protected virtual void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            if (ImporterHandler != null)
                ImporterHandler(this, new ImporterEventArgs(provider, cancellationToken, progress));
        }

        public void Dispose()
        {
            _providerDataLayer?.Dispose();
        }
    }
}
