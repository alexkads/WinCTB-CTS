using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportSpool : CalculatorProcessBase
    {
        public ImportSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex >= 7)
            {
                var linha = rowForMap;
                var contrato = uow.FindObject<Contrato>(new BinaryOperator("NomeDoContrato", linha[0].ToString()));
                var documento = linha[2].ToString();
                var isometrico = linha[9].ToString();
                var tagSpool = $"{Convert.ToString(linha[9])}-{Convert.ToString(linha[10])}";

                var criteriaOperator = CriteriaOperator.Parse("Contrato.Oid = ? And Documento = ? And Isometrico = ? And TagSpool = ?",
                    contrato.Oid, documento, isometrico, tagSpool);

                var spool = uow.FindObject<Spool>(criteriaOperator);

                if (spool == null)
                    spool = new Spool(uow);

                //var spool = objectSpace.CreateObject<Spool>();
                spool.Contrato = contrato;
                spool.SiteFabricante = linha[8].ToString();
                spool.ArranjoFisico = linha[1].ToString();
                spool.Documento = documento;
                spool.CampoAuxiliar = linha[3].ToString();
                spool.SubSop = linha[4].ToString();
                spool.AreaFisica = Convert.ToString(linha[5]);
                spool.Sth = Convert.ToString(linha[6]);
                spool.Linha = Convert.ToString(linha[7]);
                spool.Isometrico = isometrico;
                spool.TagSpool = tagSpool;
                spool.RevSpool = Convert.ToString(linha[11]);
                spool.RevIso = Convert.ToString(linha[12]);
                spool.Material = Convert.ToString(linha[13]);
                spool.Norma = Convert.ToString(linha[14]);
                spool.Diametro = Utils.ConvertINT(linha[15]);
                spool.DiametroPolegada = Convert.ToString(linha[16]);
                spool.Espessura = Utils.ConvertINT(linha[17]);
                spool.Espec = Convert.ToString(linha[18]);
                spool.PNumber = Convert.ToString(linha[19]);
                spool.Fluido = Convert.ToString(linha[20]);
                spool.TipoIsolamento = Convert.ToString(linha[21]);
                spool.CondicaoPintura = Convert.ToString(linha[22]);
                spool.Comprimento = Convert.ToDouble(linha[23]);
                spool.PesoFabricacao = Convert.ToDouble(linha[24]);
                spool.Area = Convert.ToString(linha[25]);
                spool.EspIsolamento = Convert.ToString(linha[26]);
                spool.QuantidadeIsolamento = Utils.ConvertINT(linha[27]);
                spool.TotaldeJuntas = Utils.ConvertINT(linha[28]);
                spool.TotaldeJuntasPipe = Utils.ConvertINT(linha[29]);
                spool.DataCadastro = Utils.ConvertDateTime(linha[30]);
                spool.NrProgFab = linha[31].ToString();
                spool.DataProgFab = Utils.ConvertDateTime(linha[32]);
                spool.DataCorte = Utils.ConvertDateTime(linha[33]);
                spool.DataVaFab = Utils.ConvertDateTime(linha[34]);
                spool.DataSoldaFab = Utils.ConvertDateTime(linha[35]);
                spool.DataVsFab = Utils.ConvertDateTime(linha[36]);
                spool.DataDfFab = Utils.ConvertDateTime(linha[37]);
                spool.RelatorioDf = Convert.ToString(linha[38]);
                spool.InspetorDf = Convert.ToString(linha[39]);
                spool.DataEndFab = Utils.ConvertDateTime(linha[40]);
                spool.DataPiFundo = Utils.ConvertDateTime(linha[41]);
                spool.InspPinturaFundo = Convert.ToString(linha[42]);
                spool.RelatorioPinFundo = Convert.ToString(linha[43]);
                spool.RelIndFundo = Convert.ToString(linha[44]);
                spool.DataPiIntermediaria = Utils.ConvertDateTime(linha[45]);
                spool.InspPiIntermediaria = Convert.ToString(linha[46]);
                spool.RelPiIntermediaria = Convert.ToString(linha[47]);
                spool.RelIndIntermediaria = Convert.ToString(linha[48]);
                spool.DataPiAcabamento = Utils.ConvertDateTime(linha[49]);
                spool.InspPintAcabamento = Convert.ToString(linha[50]);
                spool.RelPintAcabamento = Convert.ToString(linha[51]);
                spool.RelIndPintAcabamento = Convert.ToString(linha[52]);
                spool.DataPiRevUnico = Utils.ConvertDateTime(linha[53]);
                spool.InspPiRevUnico = Convert.ToString(linha[54]);
                spool.RelPiRevUnico = Convert.ToString(linha[55]);
                spool.DataPintFab = Utils.ConvertDateTime(linha[56]);
                spool.ProgMontagem = Convert.ToString(linha[57]);
                spool.Elevacao = Convert.ToString(linha[58]);
                spool.ProgPintura = Convert.ToString(linha[59]);
                spool.EscopoMontagem = Convert.ToString(linha[60]);
                spool.DataPreMontagem = Utils.ConvertDateTime(linha[61]);
                spool.DataVaMontagem = Utils.ConvertDateTime(linha[62]);
                spool.DataSoldaMontagem = Utils.ConvertDateTime(linha[63]);
                spool.DataVsMontagem = Utils.ConvertDateTime(linha[64]);
                spool.InspDiMontagem = Convert.ToString(linha[65]);
                spool.DataDiMontagem = Utils.ConvertDateTime(linha[66]);
                spool.DataEndMontagem = Utils.ConvertDateTime(linha[67]);
                spool.DataPintMontagem = Utils.ConvertDateTime(linha[68]);
                spool.TagComponente = Convert.ToString(linha[69]);
                spool.IdComponente = Convert.ToString(linha[70]);
                spool.Romaneio = Convert.ToString(linha[71]);
                spool.DataRomaneio = Utils.ConvertDateTime(linha[72]);
                spool.DataLiberacao = Utils.ConvertDateTime(linha[73]);
                spool.PesoMontagem = Utils.ConvertDouble(linha[74]);
                spool.SituacaoFabricacao = Convert.ToString(linha[75]);
                spool.SituacaoMontagem = Convert.ToString(linha[76]);
                //spool.DataLineCheck = Utils.ConvertDateTime(linha[75]);
            }
        }
    }
}
