//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit.Framework;
//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.ServiceProcess.Base;
//using WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote;

//namespace WinCTB_CTS.UnitTests {
//    [TestClass]
//    public class NewProcess {
//        [TestMethod]
//        [TestCase()]
//        public async Task TesteBalanceamentoDeLotes() {
//            var cts = new CancellationTokenSource();
//            var progress = new Progress<ImportProgressReport>();

//            var balaceamento = new BalanceamentoDeLotesEstrutura(cts.Token, progress);
//            await balaceamento.ProcessarTarefa();
//            balaceamento.Dispose();
//        }
//    }
//}
