using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportProcessoSoldagem : CalculatorProcessBase
    {
        public ImportProcessoSoldagem(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                var row = rowForMap;
                var eps = row[0].ToString();
                var raiz = row[1].ToString();
                var ench = row[2].ToString();

                var criteriaOperator = new BinaryOperator("Eps", eps);
                var tabprocesso = uow.FindObject<TabProcessoSoldagem>(criteriaOperator);

                if (tabprocesso == null)
                    tabprocesso = new TabProcessoSoldagem(uow);

                tabprocesso.Eps = eps;
                tabprocesso.Raiz = raiz;
                tabprocesso.Ench = ench;
            }
        }
    }
}
