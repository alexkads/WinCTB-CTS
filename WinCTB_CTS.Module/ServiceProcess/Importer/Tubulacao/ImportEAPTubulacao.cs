using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportEAPTubulacao : CalculatorProcessBase
    {
        public ImportEAPTubulacao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
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
                TabContrato.AvancoSpoolCorteFab = Utilidades.ConvertDouble(lheader("AvancoSpoolCorteFab", currentIndex));
                TabContrato.AvancoSpoolVAFab = Utilidades.ConvertDouble(lheader("AvancoSpoolVAFab", currentIndex));
                TabContrato.AvancoSpoolSoldaFab = Utilidades.ConvertDouble(lheader("AvancoSpoolSoldaFab", currentIndex));
                TabContrato.AvancoSpoolENDFab = Utilidades.ConvertDouble(lheader("AvancoSpoolENDFab", currentIndex));
                TabContrato.AvancoSpoolPosicionamento = Utilidades.ConvertDouble(lheader("AvancoSpoolPosicionamento", currentIndex));

                TabContrato.AvancoJuntaVAMont = Utilidades.ConvertDouble(lheader("AvancoJuntaVAMont", currentIndex));
                TabContrato.AvancoJuntaSoldMont = Utilidades.ConvertDouble(lheader("AvancoJuntaSoldMont", currentIndex));
                TabContrato.AvancoJuntaENDMont = Utilidades.ConvertDouble(lheader("AvancoJuntaENDMont", currentIndex));
                TabContrato.AvancoSpoolLineCheck = Utilidades.ConvertDouble(lheader("AvancoSpoolLineCheck", currentIndex));
            }
        }
    }
}
