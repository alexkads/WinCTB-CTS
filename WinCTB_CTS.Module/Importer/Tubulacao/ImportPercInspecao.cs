using DevExpress.Data.Filtering;
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
    public class ImportPercInspecao : DataImporter
    {
        public ImportPercInspecao(CancellationTokenSource cancellationTokenSource, string TabName, ParametrosImportBase parametros)
            : base(cancellationTokenSource, TabName, parametros)
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
