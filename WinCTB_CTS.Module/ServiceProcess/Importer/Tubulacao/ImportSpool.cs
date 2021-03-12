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
                cancellationToken.ThrowIfCancellationRequested();
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
                spool.Diametro = Utilidades.ConvertINT(linha[15]);
                spool.DiametroPolegada = Convert.ToString(linha[16]);
                spool.Espessura = Utilidades.ConvertINT(linha[17]);
                spool.Espec = Convert.ToString(linha[18]);
                spool.PNumber = Convert.ToString(linha[19]);
                spool.Fluido = Convert.ToString(linha[20]);
                spool.TipoIsolamento = Convert.ToString(linha[21]);
                spool.CondicaoPintura = Convert.ToString(linha[22]);
                spool.Comprimento = Convert.ToDouble(linha[23]);
                spool.PesoFabricacao = Convert.ToDouble(linha[24]);
                spool.Area = Convert.ToString(linha[25]);
                spool.EspIsolamento = Convert.ToString(linha[26]);
                spool.QuantidadeIsolamento = Utilidades.ConvertINT(linha[27]);
                spool.TotaldeJuntas = Utilidades.ConvertINT(linha[28]);
                spool.TotaldeJuntasPipe = Utilidades.ConvertINT(linha[29]);
                spool.DataCadastro = Utilidades.ConvertDateTime(linha[30]);
                spool.NrProgFab = linha[31].ToString();
                spool.DataProgFab = Utilidades.ConvertDateTime(linha[32]);
                spool.DataCorte = Utilidades.ConvertDateTime(linha[33]);
                spool.DataVaFab = Utilidades.ConvertDateTime(linha[34]);
                spool.DataSoldaFab = Utilidades.ConvertDateTime(linha[35]);
                spool.DataVsFab = Utilidades.ConvertDateTime(linha[36]);
                spool.DataDfFab = Utilidades.ConvertDateTime(linha[37]);
                spool.RelatorioDf = Convert.ToString(linha[38]);
                spool.InspetorDf = Convert.ToString(linha[39]);
                spool.DataEndFab = Utilidades.ConvertDateTime(linha[40]);
                spool.DataPiFundo = Utilidades.ConvertDateTime(linha[41]);
                spool.InspPinturaFundo = Convert.ToString(linha[42]);
                spool.RelatorioPinFundo = Convert.ToString(linha[43]);
                spool.RelIndFundo = Convert.ToString(linha[44]);
                spool.DataPiIntermediaria = Utilidades.ConvertDateTime(linha[45]);
                spool.InspPiIntermediaria = Convert.ToString(linha[46]);
                spool.RelPiIntermediaria = Convert.ToString(linha[47]);
                spool.RelIndIntermediaria = Convert.ToString(linha[48]);
                spool.DataPiAcabamento = Utilidades.ConvertDateTime(linha[49]);
                spool.InspPintAcabamento = Convert.ToString(linha[50]);
                spool.RelPintAcabamento = Convert.ToString(linha[51]);
                spool.RelIndPintAcabamento = Convert.ToString(linha[52]);
                spool.DataPiRevUnico = Utilidades.ConvertDateTime(linha[53]);
                spool.InspPiRevUnico = Convert.ToString(linha[54]);
                spool.RelPiRevUnico = Convert.ToString(linha[55]);
                spool.DataPintFab = Utilidades.ConvertDateTime(linha[56]);
                spool.ProgMontagem = Convert.ToString(linha[57]);
                spool.Elevacao = Convert.ToString(linha[58]);
                spool.ProgPintura = Convert.ToString(linha[59]);
                spool.EscopoMontagem = Convert.ToString(linha[60]);
                spool.DataPreMontagem = Utilidades.ConvertDateTime(linha[61]);
                spool.DataVaMontagem = Utilidades.ConvertDateTime(linha[62]);
                spool.DataSoldaMontagem = Utilidades.ConvertDateTime(linha[63]);
                spool.DataVsMontagem = Utilidades.ConvertDateTime(linha[64]);
                spool.InspDiMontagem = Convert.ToString(linha[65]);
                spool.DataDiMontagem = Utilidades.ConvertDateTime(linha[66]);
                spool.DataEndMontagem = Utilidades.ConvertDateTime(linha[67]);
                spool.DataPintMontagem = Utilidades.ConvertDateTime(linha[68]);
                spool.TagComponente = Convert.ToString(linha[69]);
                spool.IdComponente = Convert.ToString(linha[70]);
                spool.Romaneio = Convert.ToString(linha[71]);
                spool.DataRomaneio = Utilidades.ConvertDateTime(linha[72]);
                spool.DataLiberacao = Utilidades.ConvertDateTime(linha[73]);
                spool.PesoMontagem = Utilidades.ConvertDouble(linha[74]);
                spool.SituacaoFabricacao = Convert.ToString(linha[75]);
                spool.SituacaoMontagem = Convert.ToString(linha[76]);
                //spool.DataLineCheck = Utils.ConvertDateTime(linha[75]);
            }
        }
    }
}
