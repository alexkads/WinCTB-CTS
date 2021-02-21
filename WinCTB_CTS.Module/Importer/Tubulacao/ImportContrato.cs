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
    public class ImportContrato : DataImporter
    {
        public ImportContrato(CancellationTokenSource cancellationTokenSource, string TabName, ParametrosImportBase parametros) 
            : base(cancellationTokenSource, TabName, parametros)
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
