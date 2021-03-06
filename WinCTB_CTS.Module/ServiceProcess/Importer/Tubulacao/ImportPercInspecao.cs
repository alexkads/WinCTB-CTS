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
    public class ImportPercInspecao : CalculatorProcessBase
    {
        public ImportPercInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                var row = rowForMap;
                var spec = row[0].ToString();
                var insp = Utils.ConvertDouble(row[1]) * 0.01D;

                var criteriaOperator = new BinaryOperator("Spec", spec);
                var tabperc = uow.FindObject<TabPercInspecao>(criteriaOperator);

                if (tabperc == null)
                    tabperc = new TabPercInspecao(uow);

                tabperc.Spec = spec;
                tabperc.PercentualDeInspecao = insp;
            }
        }
    }
}
