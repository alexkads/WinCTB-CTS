using DevExpress.Xpo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.ServiceProcess.Base;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.ProcessoLote;

namespace WinCTB_CTS.UnitTests {
    [TestClass]
    public class NewProcess {
        [TestMethod]
        [TestCase()]
        public async Task TesteFormacaoDeLotes() {
            var provider = new ProviderDataLayer();
            var uow = new UnitOfWork(provider.GetSimpleDataLayer());
            Utilidades.DeleteAllRecords<LoteJuntaEstrutura>(uow);
            Utilidades.DeleteAllRecords<LoteEstrutura>(uow);
            var idg = uow.FindObject<DevExpress.Persistent.BaseImpl.OidGenerator>(new BinaryOperator("Type", "WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes.LoteEstrutura"));
            uow.Delete(idg);
            uow.CommitChanges();
            uow.Dispose();

            var cts = new CancellationTokenSource();
            var progress = new Progress<ImportProgressReport>();

            var formacao = new GerarLote(cts.Token, progress);
            await formacao.ProcessarTarefaSimples();
            formacao.Dispose();

            var inspecao = new LotesDeEstruturaInspecao(cts.Token, progress);
            await inspecao.ProcessarTarefaSimples();
            inspecao.Dispose();

            var alinhamento = new LotesDeEstruturaAlinhamento(cts.Token, progress);
            await alinhamento.ProcessarTarefaSimples();
            alinhamento.Dispose();

            var balaceamento = new BalanceamentoDeLotesEstrutura(cts.Token, progress);
            await balaceamento.ProcessarTarefaSimples();
            balaceamento.Dispose();
        }
    }
}
