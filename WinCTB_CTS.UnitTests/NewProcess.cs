using DevExpress.ExpressApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Comum.ImporterPatterns;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Estrutura;
using WinCTB_CTS.Module.Importer.Tubulacao;
using WinCTB_CTS.Module.Interfaces;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.ProcessoLote;

namespace WinCTB_CTS.UnitTests
{
    [TestClass]
    public class NewProcess
    {
        [TestMethod]
        [TestCase()]
        public async Task TesteBalanceamentoDeLotes()
        {
            var cts = new CancellationTokenSource();
            var progress = new Progress<ImportProgressReport>();

            var balaceamento = new BalanceamentoDeLotesEstrutura(cts.Token, progress);
            await balaceamento.ProcessarTarefa();
            balaceamento.Dispose();
        }
    }
}
