using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.ServiceProcess.Base;
using WinCTB_CTS.Module.Helpers;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Estrutura
{
    public class ImportComponente : CalculatorProcessBase
    {
        public ImportComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex >= 3)
            {
                var linha = rowForMap;
                var documentoReferencia = linha[1].ToString();
                var desenhoMontagem = linha[2].ToString();
                var transmital = linha[3].ToString();
                var peca = linha[4].ToString();

                var criteriaOperator = CriteriaOperator.Parse("DesenhoMontagem = ? And Peca = ?",
                    desenhoMontagem, peca);

                var componente = uow.FindObject<Componente>(criteriaOperator);

                if (componente == null)
                    componente = new Componente(uow);
                //else
                //oldComponets.FirstOrDefault(x => x.Oid == componente.Oid).DataExist = true;

                //Mapear campos aqui
                //componente.Contrato = contrato;

                componente.Modulo = linha[0].ToString();
                componente.DocumentoReferencia = documentoReferencia;
                componente.DesenhoMontagem = desenhoMontagem;
                componente.Transmital = transmital;
                componente.Peca = peca;
                componente.Revisao = linha[5].ToString();
                componente.TipoEstrutura = linha[6].ToString();
                componente.Posicao = linha[7].ToString();
                componente.Dwg = linha[8].ToString();
                componente.Elevacao = linha[9].ToString();
                componente.PesoTotal = Convert.ToDouble(linha[10]);
                componente.AreaPintura = Convert.ToDouble(linha[11]);
                componente.RelatorioRecebimento = linha[14].ToString();
                componente.DataRecebimento = Utils.ConvertDateTime(linha[15]);
                componente.ProgFitup = linha[16].ToString();
                componente.ProgWeld = linha[17].ToString();
                componente.ProgNdt = linha[18].ToString();
                componente.DataPosicionamento = Utils.ConvertDateTime(linha[19]);
                componente.RelatorioDimensional = linha[26].ToString();
                componente.DataDimensional = Utils.ConvertDateTime(linha[27]);
                componente.ProgPintura = linha[28].ToString();
                componente.RelPintura = linha[29].ToString();
                componente.InspPintura = linha[30].ToString();
                componente.DataPintura = Utils.ConvertDateTime(linha[31]);
                componente.StatusPeca = linha[32].ToString();
            }
        }
    }
}
