using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Estrutura
{
    public class ImportEAPEstrutura : CalculatorProcessBase
    {
        public ImportEAPEstrutura(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
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

                var modulo = lheader("Módulo", currentIndex).ToString();
                var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", lheader("Contrato", currentIndex).ToString()));                
                var criteriaOperator = CriteriaOperator.Parse("Contrato.Oid = ? And Modulo = ?", contrato, modulo);
                var eap = uow.FindObject<TabEAPEst>(criteriaOperator);

                if (eap == null)
                    eap = new TabEAPEst(uow);

                eap.Contrato = contrato;
                eap.Modulo = modulo;
                eap.Posicionamento = Utils.ConvertDouble(lheader("Posicionamento", currentIndex));
                eap.Acoplamento = Utils.ConvertDouble(lheader("Acoplamento", currentIndex));
                eap.Solda = Utils.ConvertDouble(lheader("Solda", currentIndex));
                eap.End = Utils.ConvertDouble(lheader("End", currentIndex));
            }
        }
    }
}
