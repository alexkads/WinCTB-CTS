using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao
{
    public class ImportJuntaSpool : CalculatorProcessBase
    {
        public ImportJuntaSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress)
        {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex)
        {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex >= 9)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var linha = rowForMap;
                var PesquisarSpool = linha[8].ToString();
                var FiltroPesquisa = CriteriaOperator.Parse("TagSpool = ?", PesquisarSpool);

                var spool = uow.FindObject<Spool>(FiltroPesquisa);
                //var spool = spools.FirstOrDefault(x => x.TagSpool == PesquisarSpool);

                if (spool != null)
                {
                    var junta = linha[9].ToString();

                    var criteriaOperator = CriteriaOperator.Parse("Spool.Oid = ? And Junta = ?",
                        spool.Oid, junta);

                    var juntaSpool = uow.FindObject<JuntaSpool>(criteriaOperator);
                    //var juntaSpool = juntas.FirstOrDefault(x => x.Spool.Oid == spool.Oid && x.Junta == junta);

                    if (juntaSpool == null)
                        juntaSpool = new JuntaSpool(uow);

                    juntaSpool.Site = linha[0].ToString();
                    juntaSpool.ArranjoFisico = linha[1].ToString();
                    juntaSpool.CampoAuxiliar = linha[2].ToString();
                    juntaSpool.ProgFab = linha[3].ToString();
                    juntaSpool.Sop = linha[4].ToString();
                    juntaSpool.Sth = linha[5].ToString();
                    juntaSpool.Documento = linha[6].ToString();
                    juntaSpool.Linha = linha[7].ToString();
                    juntaSpool.Junta = junta;
                    juntaSpool.TabDiametro = uow.FindObject<TabDiametro>(new BinaryOperator("DiametroPolegada", linha[11].ToString()));
                    juntaSpool.Espessura = Utils.ConvertDouble(linha[12]);
                    juntaSpool.TipoJunta = linha[14].ToString();
                    juntaSpool.TabPercInspecao = uow.FindObject<TabPercInspecao>(new BinaryOperator("Spec", linha[15].ToString()));
                    juntaSpool.MaterialPt = linha[16].ToString();
                    juntaSpool.ClasseInspecao = linha[18].ToString();
                    juntaSpool.Norma = linha[19].ToString();
                    juntaSpool.CampoOuPipe = Utils.ConvertStringEnumCampoPipe(linha[20].ToString());
                    juntaSpool.StatusVa = linha[21].ToString();
                    juntaSpool.RelatorioVa = linha[22].ToString();
                    juntaSpool.ExecutanteVa = linha[23].ToString();
                    juntaSpool.DataVa = Utils.ConvertDateTime(linha[24]);
                    juntaSpool.StatusResold = linha[25].ToString();
                    juntaSpool.SoldadorRaiz = linha[26].ToString();
                    juntaSpool.SoldadorEnch = linha[27].ToString();
                    juntaSpool.SoldadorAcab = linha[28].ToString();
                    juntaSpool.RelatorioSoldagem = linha[29].ToString();
                    juntaSpool.ExecutanteResold = linha[30].ToString();
                    juntaSpool.Eps = linha[31].ToString();
                    juntaSpool.DataSoldagem = Utils.ConvertDateTime(linha[32]);
                    juntaSpool.ConsumivelRaiz = linha[33].ToString();
                    juntaSpool.ConsumivelEnch = linha[34].ToString();
                    juntaSpool.ConsumivelAcab = linha[35].ToString();
                    juntaSpool.VisualStatus = linha[36].ToString();
                    juntaSpool.RelatorioVs = linha[37].ToString();
                    juntaSpool.InspetorVS = linha[38].ToString();
                    juntaSpool.DataVisualSolda = Utils.ConvertDateTime(linha[39]);
                    juntaSpool.StatusLpPm = linha[40].ToString();
                    juntaSpool.RelatorioLp = linha[41].ToString();
                    juntaSpool.InspetorLp = linha[42].ToString();
                    juntaSpool.DataLp = Utils.ConvertDateTime(linha[43]);
                    juntaSpool.RelatorioPm = linha[44].ToString();
                    juntaSpool.InspetorPm = linha[45].ToString();
                    juntaSpool.DataPm = Utils.ConvertDateTime(linha[46]);
                    juntaSpool.StatusTt = linha[47].ToString();
                    juntaSpool.RelatorioTt = linha[48].ToString();
                    juntaSpool.DataTt = Utils.ConvertDateTime(linha[49]);
                    juntaSpool.StatusDureza = linha[50].ToString();
                    juntaSpool.RelatorioDureza = linha[51].ToString();
                    juntaSpool.InspetorDureza = linha[52].ToString();
                    juntaSpool.DataDureza = Utils.ConvertDateTime(linha[53]);
                    juntaSpool.StatusRastMaterial = linha[54].ToString();
                    juntaSpool.RelRastMaterial = linha[55].ToString();
                    juntaSpool.ExecutanteRastMaterial = linha[56].ToString();
                    juntaSpool.DataRastMaterial = Utils.ConvertDateTime(linha[57]);
                    juntaSpool.StatusIdLiga = linha[58].ToString();
                    juntaSpool.RelIdLiga = linha[59].ToString();
                    juntaSpool.InspetorIdLiga = linha[60].ToString();
                    juntaSpool.DataIdLiga = Utils.ConvertDateTime(linha[61]);
                    juntaSpool.StatusRxUs = linha[62].ToString();
                    juntaSpool.ProgRx = linha[63].ToString();
                    juntaSpool.RelatorioRx = linha[64].ToString();
                    juntaSpool.InspetorRx = linha[65].ToString();
                    juntaSpool.DataRx = Utils.ConvertDateTime(linha[66]);
                    juntaSpool.RelatorioUs = linha[67].ToString();
                    juntaSpool.InspetorUs = linha[68].ToString();
                    juntaSpool.DataUs = Utils.ConvertDateTime(linha[69]);
                    juntaSpool.StatusFerrita = linha[70].ToString();
                    juntaSpool.RelatorioFerrita = linha[71].ToString();
                    juntaSpool.InspetorFerrita = linha[72].ToString();
                    juntaSpool.DataFerrita = Utils.ConvertDateTime(linha[73]);
                    juntaSpool.StatusEstanqueidade = linha[74].ToString();
                    juntaSpool.RelatorioEstanqueidade = linha[75].ToString();
                    juntaSpool.InspetorEstanqueidade = linha[76].ToString();
                    juntaSpool.DataEstanqueidade = Utils.ConvertDateTime(linha[77]);
                    juntaSpool.RelDimFab = linha[78].ToString();
                    juntaSpool.ProgFabJunta = linha[79].ToString();
                    juntaSpool.LoteRx = linha[80].ToString();
                    juntaSpool.LoteLp = linha[81].ToString();
                    juntaSpool.DataLiberacaoJunta = Utils.ConvertDateTime(linha[82]);
                    juntaSpool.SituacaoJunta = linha[83].ToString();
                    juntaSpool.Spool = spool;

                    //Complemento                        
                    juntaSpool.ProcessoRaiz =
                        string.IsNullOrWhiteSpace(juntaSpool.Eps)
                        ? null
                        : uow.QueryInTransaction<TabProcessoSoldagem>()
                            .FirstOrDefault(proc => proc.Eps == juntaSpool.Eps)?.Raiz;

                    juntaSpool.ProcessoEnch =
                        string.IsNullOrWhiteSpace(juntaSpool.Eps)
                        ? null
                        : uow.QueryInTransaction<TabProcessoSoldagem>()
                            .FirstOrDefault(proc => proc.Eps == juntaSpool.Eps)?.Ench;

                    juntaSpool.TabSchedule =
                        juntaSpool?.TabPercInspecao == null
                        ? null
                        : uow.QueryInTransaction<TabSchedule>()
                            .FirstOrDefault(sch => sch.PipingClass == juntaSpool.TabPercInspecao.Spec && sch.Wdi == juntaSpool.Wdi);
                }
            }
        }
    }
}
