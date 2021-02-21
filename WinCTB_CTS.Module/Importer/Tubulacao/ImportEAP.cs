﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Comum.ImporterPatterns;
using WinCTB_CTS.Module.Importer.Estrutura;

namespace WinCTB_CTS.Module.Importer.Tubulacao
{
    public class ImportEAP : DataImporter
    {
        public ImportEAP(CancellationTokenSource cancellationTokenSource, string TabName, ParametrosImportBase parametros)
            : base(cancellationTokenSource, TabName, parametros)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                var row = rowForMap;

                Func<string, int, object> lheader = (header, indexRow) =>
                {
                    var idxcol = dataTable.Rows[0].ItemArray.ToList().IndexOf(header);
                    return (dataTable.Rows[indexRow])[idxcol];
                };

                var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", lheader("Contrato", currentIndex).ToString()));

                var criteriaOperator = new BinaryOperator("Contrato.Oid", contrato);
                var TabContrato = uow.FindObject<TabEAPPipe>(criteriaOperator);

                if (TabContrato == null)
                    TabContrato = new TabEAPPipe(uow);

                TabContrato.Contrato = contrato;
                TabContrato.AvancoSpoolCorteFab = Utils.ConvertDouble(lheader("AvancoSpoolCorteFab", currentIndex));
                TabContrato.AvancoSpoolVAFab = Utils.ConvertDouble(lheader("AvancoSpoolVAFab", currentIndex));
                TabContrato.AvancoSpoolSoldaFab = Utils.ConvertDouble(lheader("AvancoSpoolSoldaFab", currentIndex));
                TabContrato.AvancoSpoolENDFab = Utils.ConvertDouble(lheader("AvancoSpoolENDFab", currentIndex));
                TabContrato.AvancoSpoolPosicionamento = Utils.ConvertDouble(lheader("AvancoSpoolPosicionamento", currentIndex));

                TabContrato.AvancoJuntaVAMont = Utils.ConvertDouble(lheader("AvancoJuntaVAMont", currentIndex));
                TabContrato.AvancoJuntaSoldMont = Utils.ConvertDouble(lheader("AvancoJuntaSoldMont", currentIndex));
                TabContrato.AvancoJuntaENDMont = Utils.ConvertDouble(lheader("AvancoJuntaENDMont", currentIndex));
                TabContrato.AvancoSpoolLineCheck = Utils.ConvertDouble(lheader("AvancoSpoolLineCheck", currentIndex));
            }
        }
    }
}