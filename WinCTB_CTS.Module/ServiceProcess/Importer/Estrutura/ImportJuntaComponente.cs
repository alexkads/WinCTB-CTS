using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Data;
using System.Threading;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Importer.Estrutura {
    public class ImportJuntaComponente : CalculatorProcessBase {
        public ImportJuntaComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress) {
        }

        protected override void OnMapImporter(UnitOfWork uow, DataTable dataTable, DataRow rowForMap, int expectedTotal, int currentIndex) {
            base.OnMapImporter(uow, dataTable, rowForMap, expectedTotal, currentIndex);

            if (currentIndex >= 2) {
                cancellationToken.ThrowIfCancellationRequested();
                var linha = rowForMap;
                var desenhoMontagem = linha[2].ToString();
                var peca = linha[3].ToString();
                var FiltroPesquisa = CriteriaOperator.Parse("DesenhoMontagem = ? And Peca = ?", desenhoMontagem, peca);
                var componente = uow.FindObject<Componente>(FiltroPesquisa);
                if (componente != null) {
                    var junta = linha[5].ToString();

                    var criteriaOperator = CriteriaOperator.Parse("Componente.Oid = ? And Junta = ?",
                        componente.Oid, junta);

                    var juntaComponente = uow.FindObject<JuntaComponente>(criteriaOperator);

                    if (juntaComponente == null) {
                        juntaComponente = new JuntaComponente(uow);
                        juntaComponente.Junta = junta;
                        juntaComponente.Componente = componente;
                    }

                    //else
                    //    oldJuntas.FirstOrDefault(x => x.Oid == juntaComponente.Oid).DataExist = true;

                    juntaComponente.TipoJunta = linha[7].ToString();
                    juntaComponente.Site = linha[8].ToString();
                    juntaComponente.Comprimento = Utilidades.ConvertDouble(linha[9]);
                    juntaComponente.ClasseInspecao = linha[10].ToString();
                    juntaComponente.Df1 = linha[11].ToString();
                    juntaComponente.Mat1 = linha[12].ToString();
                    juntaComponente.Esp1 = Utilidades.ConvertDouble(linha[13]);
                    juntaComponente.TipoDf1 = linha[14].ToString();
                    juntaComponente.PosicaoDf1 = linha[15].ToString();
                    juntaComponente.Df2 = linha[17].ToString();
                    juntaComponente.Mat2 = linha[18].ToString();
                    juntaComponente.Esp2 = Utilidades.ConvertDouble(linha[19]);
                    juntaComponente.TipoDf2 = linha[20].ToString();
                    juntaComponente.PosicaoDf2 = linha[21].ToString();
                    juntaComponente.Posiocionamento = Utilidades.ConvertDateTime(linha[23]);
                    juntaComponente.DataFitup = Utilidades.ConvertDateTime(linha[24]);
                    juntaComponente.RelatorioFitup = linha[25].ToString();
                    juntaComponente.InspFitup = linha[26].ToString();
                    juntaComponente.StatusFitup = linha[27].ToString();
                    juntaComponente.DataSolda = Utilidades.ConvertDateTime(linha[28]);
                    juntaComponente.Soldadores = linha[29].ToString();
                    juntaComponente.Consumiveis = linha[30].ToString();
                    juntaComponente.Wps = linha[31].ToString();
                    juntaComponente.RelatorioSolda = linha[32].ToString();
                    juntaComponente.InspetorSoldagem = linha[33].ToString();
                    juntaComponente.StatusSolda = linha[34].ToString();
                    juntaComponente.DataVisual = Utilidades.ConvertDateTime(linha[35]);
                    juntaComponente.RelatorioVisualSolda = linha[36].ToString();
                    juntaComponente.InspetorVisualSolda = linha[37].ToString();
                    juntaComponente.StatusVisualSolda = linha[38].ToString();
                    juntaComponente.SampleMp = linha[39].ToString();
                    juntaComponente.DataLP = Utilidades.ConvertDateTime(linha[40]);
                    juntaComponente.RelatorioLp = linha[41].ToString();
                    juntaComponente.InspetorLp = linha[42].ToString();
                    juntaComponente.StatusLp = linha[43].ToString();
                    juntaComponente.DataPm = Utilidades.ConvertDateTime(linha[44]);
                    juntaComponente.RelatorioPm = linha[45].ToString();
                    juntaComponente.InspetorPm = linha[46].ToString();
                    juntaComponente.StatusPm = linha[47].ToString();
                    juntaComponente.SampleRx = linha[48].ToString();
                    juntaComponente.DataRx = Utilidades.ConvertDateTime(linha[49]);
                    juntaComponente.RelatorioRx = linha[50].ToString();
                    juntaComponente.InspetorRx = linha[51].ToString();
                    juntaComponente.ComprimentoReparoRx = Utilidades.ConvertDouble(linha[52]);
                    juntaComponente.StatusRx = linha[53].ToString();
                    juntaComponente.SampleUs = linha[54].ToString();
                    juntaComponente.DataUs = Utilidades.ConvertDateTime(linha[55]);
                    juntaComponente.RelatorioUs = linha[56].ToString();
                    juntaComponente.InspetorUs = linha[57].ToString();
                    juntaComponente.ComprimentoReparoUs = Utilidades.ConvertDouble(linha[58]);
                    juntaComponente.StatusUs = linha[59].ToString();
                    juntaComponente.StatusJunta = linha[60].ToString();
                    juntaComponente.PercLpPm = Utilidades.ConvertDouble(linha[63]) / 100;
                    juntaComponente.PercUt = Utilidades.ConvertDouble(linha[64]) / 100;
                    juntaComponente.PercRt = Utilidades.ConvertDouble(linha[65]) / 100;

                    #region Limpeza de Status de Lote
                    if (juntaComponente.StatusLp == "AL")
                        juntaComponente.StatusLp = null;

                    if (juntaComponente.StatusPm == "AL")
                        juntaComponente.StatusPm = null;

                    if (juntaComponente.StatusUs == "AL")
                        juntaComponente.StatusUs = null;

                    if (juntaComponente.StatusRx == "AL")
                        juntaComponente.StatusRx = null;
                    #endregion

                    #region Definição de DF e MedJoints
                    juntaComponente.PosDf1 = uow.FindObject<Componente>(new BinaryOperator("Peca", juntaComponente.Df1))?.DataPosicionamento;
                    juntaComponente.PosDf2 = uow.FindObject<Componente>(new BinaryOperator("Peca", juntaComponente.Df2))?.DataPosicionamento;
                    //juntaComponente.MedJoint = GetMedJoint(juntaComponente);
                    #endregion
                }
            }
        }

        //private Func<JuntaComponente, Componente> GetMedJoint = (junta) => {
        //    var df1 = junta.Evaluate(CriteriaOperator.Parse("[<Componente>][Peca = ?].Single()", junta.Df1)) as Componente;
        //    var df2 = junta.Evaluate(CriteriaOperator.Parse("[<Componente>][Peca = ?].Single()", junta.Df2)) as Componente;

        //    if (df2 is null) {
        //        return df1;
        //    } else if (df1.ProgFitup == 0) {
        //        return df1;
        //    } else if (df2?.ProgFitup == 0) {
        //        return df2;
        //    } else if (df1.ProgFitup >= df2.ProgFitup) {
        //        return df1;
        //    } else {
        //        return df2;
        //    }
        //};
    }
}
