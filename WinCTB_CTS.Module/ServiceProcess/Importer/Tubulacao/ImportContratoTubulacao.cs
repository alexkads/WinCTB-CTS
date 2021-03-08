using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportContratoTubulacao : CalculatorProcessBase
    {
        public ImportContratoTubulacao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                var row = rowForMap;
                var siteNome = row[0].ToString();

                var criteriaOperator = new BinaryOperator("NomeDoContrato", siteNome);
                var contrato = uow.FindObject<Contrato>(criteriaOperator);

                if (contrato == null)
                    contrato = new Contrato(uow);

                contrato.NomeDoContrato = siteNome;
            }
        }
    }
}
