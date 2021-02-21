using DevExpress.ExpressApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Importer.Estrutura;
using WinCTB_CTS.Module.Importer.Tubulacao;

namespace WinCTB_CTS.UnitTests
{
    [TestClass]
    public class UnitTestWinCTBCTS
    {
        [TestMethod]
        [TestCase()]
        public async Task TesteImportacaoTabelaAuxiliar()
        {
            var cts = new CancellationTokenSource();
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var objectSpace = objectSpaceProvider.CreateObjectSpace();
            var parametros = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();

            var tabDia = new ImportDiametro(cts, "TabDiametro", parametros);
            var tabSch = new ImportSchedule(cts, "Schedule", parametros);
            var tabPIn = new ImportPercInspecao(cts, "PercInspecao", parametros);
            var tabPSo = new ImportProcessoSoldagem(cts, "ProcessoSoldagem", parametros);
            var tabCon = new ImportContrato(cts, "Contrato", parametros);
            var tabEAP = new ImportEAP(cts, "EAPPipe", parametros);
            
            await tabDia.Start();
            await tabSch.Start();
            await tabPIn.Start();
            await tabPSo.Start();
            await tabCon.Start();
            await tabEAP.Start();
        }

        [TestMethod]
        [TestCase()]
        public async Task TesteImportacaoSpoolEJunta()
        {
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var objectSpace = objectSpaceProvider.CreateObjectSpace();
            var parametros = objectSpace.CreateObject<ParametrosImportSpoolJuntaExcel>();
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.PadraoDeArquivo;
            arquivo.SaveToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new Module.ExcelDataReaderHelper.Excel.Reader(stream))
            {
                var dtcollectionImport = excelReader.CreateDataTableCollection(false);

                var cts = new CancellationTokenSource();
                var itba = new ImportSpoolEJunta(parametros, cts);
                var progress = new Progress<ImportProgressReport>(itba.LogTrace);

                await itba.ImportarSpools(dtcollectionImport["SGS"], progress);
                await itba.ImportarJuntas(dtcollectionImport["SGJ"], progress);

                objectSpace.CommitChanges();
                itba.Dispose();
            }
        }

        [TestMethod]
        [TestCase()]
        public async Task TesteImportacaoPieceAndJoints()
        {
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var objectSpace = objectSpaceProvider.CreateObjectSpace();
            var parametros = objectSpace.CreateObject<ParametrosImportComponentEJunta>();
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.PadraoDeArquivo;
            arquivo.SaveToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new Module.ExcelDataReaderHelper.Excel.Reader(stream))
            {
                var dtcollectionImport = excelReader.CreateDataTableCollection(false);

                var cts = new CancellationTokenSource();
                var piecejoints = new ImportComponentEJunta(parametros, cts);
                var progress = new Progress<ImportProgressReport>(piecejoints.LogTrace);

                await piecejoints.ImportarComponente(dtcollectionImport["Piece"], progress);
                await piecejoints.ImportarJuntas(dtcollectionImport["Joints"], progress);
                objectSpace.CommitChanges();
            }
        }

        [TestMethod]
        [TestCase()]
        public void TesteImportacaoPieceAndJointsThread()
        {
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var objectSpace = objectSpaceProvider.CreateObjectSpace();
            var parametros = objectSpace.CreateObject<ParametrosImportComponentEJunta>();
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.PadraoDeArquivo;
            arquivo.SaveToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new Module.ExcelDataReaderHelper.Excel.Reader(stream))
            {
                var dtcollectionImport = excelReader.CreateDataTableCollection(false);
                var cts = new CancellationTokenSource();
                var piecejoints = new ImportComponentEJunta(parametros, cts);
                var progress = new Progress<ImportProgressReport>(piecejoints.LogTrace);

                //await Observable.Start(() => piecejoints.ImportarJuntas(dtcollectionImport["Joints"], progress));

                objectSpace.CommitChanges();
                piecejoints.Dispose();
            }
        }

        [TestMethod]
        [TestCase()]
        public async Task TesteGeradoresDeLotes()
        {
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var progress = new Progress<string>();
            var gerador = new Module.Calculator.ProcessoLoteLPPM.GerarLoteLPPM(objectSpaceProvider);
            await gerador.GerarLoteLPPMAsync(progress);
        }
    }
}
