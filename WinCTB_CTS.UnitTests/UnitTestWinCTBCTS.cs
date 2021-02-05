﻿using DevExpress.ExpressApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WinCTB_CTS.Module.Importer;
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
            var application = new Application(false);
            IObjectSpaceProvider objectSpaceProvider = application.serverApplication.ObjectSpaceProvider;
            var objectSpace = objectSpaceProvider.CreateObjectSpace();
            var parametros = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new Module.ExcelDataReaderHelper.Excel.Reader(stream))
            {
                var dtcollectionImport = excelReader.CreateDataTableCollection(false);

                var itba = new ImportTabelaAuxiliares(objectSpace, parametros);
                var progress = new Progress<ImportProgressReport>(itba.LogTrace);

                await Observable.Start(() => itba.ImportarDiametro(dtcollectionImport["TabDiametro"], progress));
                await Observable.Start(() => itba.ImportarSchedule(dtcollectionImport["Schedule"], progress));
                await Observable.Start(() => itba.ImportarPercInspecao(dtcollectionImport["PercInspecao"], progress));
                await Observable.Start(() => itba.ImportarProcessoSoldagem(dtcollectionImport["ProcessoSoldagem"], progress));
                await Observable.Start(() => itba.ImportarContrato(dtcollectionImport["Contrato"], progress));
                await Observable.Start(() => itba.ImportarEAP(dtcollectionImport["EAPPipe"], progress));

                objectSpace.CommitChanges();
            }
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
            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);
            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new Module.ExcelDataReaderHelper.Excel.Reader(stream))
            {
                var dtcollectionImport = excelReader.CreateDataTableCollection(false);

                var itba = new ImportSpoolEJunta(objectSpace, parametros);
                var progress = new Progress<ImportProgressReport>(itba.LogTrace);

                await Observable.Start(() => itba.ImportarSpools(dtcollectionImport["SGS"], progress));
                await Observable.Start(() => itba.ImportarJuntas(dtcollectionImport["SGJ"], progress));

                objectSpace.CommitChanges();
            }
        }
    }
}
