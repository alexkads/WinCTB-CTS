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
    public class ImportDiametro : DataImporter
    {
        public ImportDiametro(CancellationTokenSource cancellationTokenSource, string TabName, ParametrosImportBase parametros)
            : base(cancellationTokenSource, TabName, parametros)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex > 0)
            {
                var row = rowForMap;
                var polegada = row[0].ToString();
                var wdi = Utils.ConvertDouble(row[1]);
                var mm = Utils.ConvertINT(row[2]);

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
