using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportDiametro : CalculatorProcessBase
    {
        public ImportDiametro(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
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
                var polegada = row[0].ToString();
                var wdi = Utilidades.ConvertDouble(row[1]);
                var mm = Utilidades.ConvertINT(row[2]);

                var criteriaOperator = new BinaryOperator("DiametroPolegada", polegada);
                var tabDiametro = uow.FindObject<TabDiametro>(criteriaOperator);

                if (tabDiametro == null)
                    tabDiametro = new TabDiametro(uow);

                tabDiametro.DiametroPolegada = polegada;
                tabDiametro.DiametroMilimetro = mm;
                tabDiametro.Wdi = wdi;
            }
        }
    }
}
