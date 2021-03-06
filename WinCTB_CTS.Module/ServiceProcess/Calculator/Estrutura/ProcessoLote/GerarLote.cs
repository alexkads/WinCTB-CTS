using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.Importer;
using WinCTB_CTS.Module.Interfaces;
using WinCTB_CTS.Module.ServiceProcess.Base;

namespace WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.ProcessoLote
{
    public class GerarLote : CalculatorProcessBase
    {
        public GerarLote(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        : base(cancellationToken, progress) { }

        protected override void OnCalculator(ProviderDataLayer provider, CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            base.OnCalculator(provider, cancellationToken, progress);            
            var valuesENDS = Enum.GetValues(typeof(ENDS));
            foreach (ENDS ensaio in valuesENDS)
            {
                var uow = new UnitOfWork(provider.GetSimpleDataLayer());
                var juntasSemLote = GetJuntasSemLotes(uow, ensaio);
                int totalDataStore = juntasSemLote.EvaluateDatastoreCount();

                if (totalDataStore == 0)
                {
                    progress.Report(new ImportProgressReport
                    {
                        TotalRows = totalDataStore,
                        CurrentRow = 0,
                        MessageImport = $"Gerando lotes de {ensaio.ToString()} 0/{totalDataStore}"
                    });
                    return;
                }

                double currentProgress = 0D;

                foreach (var juntaComponente in juntasSemLote)
                {
                    Guid GuidComponente = Guid.NewGuid();
                    GuidComponente = juntaComponente.Componente.Oid;

                    CriteriaOperator CriterioFormacao = string.Empty;

                    if (ensaio == ENDS.LPPM)
                        CriterioFormacao = CriteriaOperator.Parse($"Ensaio = ? And PercentualNivelDeInspecao = ? And JuntasNoLote < QuantidadeNecessaria", ensaio, juntaComponente.PercLpPm);
                    if (ensaio == ENDS.RX)
                        CriterioFormacao = CriteriaOperator.Parse($"Ensaio = ? And PercentualNivelDeInspecao = ? And JuntasNoLote < QuantidadeNecessaria", ensaio, juntaComponente.PercRt);
                    if (ensaio == ENDS.US)
                        CriterioFormacao = CriteriaOperator.Parse($"Ensaio = ? And PercentualNivelDeInspecao = ? And JuntasNoLote < QuantidadeNecessaria", ensaio, juntaComponente.PercUt);

                    var loteEstrutura = uow.FindObject<LoteEstrutura>(CriterioFormacao);

                    if (loteEstrutura == null)
                        loteEstrutura = NovoLote(uow, ensaio, juntaComponente);

                    IncluirJuntaNoLote(uow, ensaio, loteEstrutura, juntaComponente, 1);
                    loteEstrutura.JuntasNoLote = loteEstrutura.LotejuntaEstruturas.Count();
                    uow.CommitChanges();

                    currentProgress++;

                    if (currentProgress % 100 == 0)
                    {
                        progress.Report(new ImportProgressReport
                        {
                            TotalRows = totalDataStore,
                            CurrentRow = currentProgress + 1,
                            MessageImport = $"Gerando lotes de {ensaio.ToString()} {currentProgress}/{totalDataStore}"
                        });
                    }
                }

                progress.Report(new ImportProgressReport
                {
                    TotalRows = totalDataStore,
                    CurrentRow = totalDataStore,
                    MessageImport = $"Finalizados lotes de {ensaio.ToString()}..."
                });

                juntasSemLote.Dispose();
            }
        }

        #region Outros Processos
        private void IncluirJuntaNoLote(UnitOfWork uow, ENDS ensaio, LoteEstrutura lote, JuntaComponente juntaComponente, int cicloTermico)
        {
            var juntaLote = new LoteJuntaEstrutura(uow);
            juntaLote.LoteEstrutura = lote;
            juntaLote.JuntaComponente = juntaComponente;
            juntaLote.DataInclusao = DateTime.Now;
            juntaLote.CicloTermico = cicloTermico;

            if (ensaio == ENDS.LPPM)
                juntaLote.PercentualNivelDeInspecao = juntaComponente.PercLpPm;

            if (ensaio == ENDS.RX)
                juntaLote.PercentualNivelDeInspecao = juntaComponente.PercRt;

            if (ensaio == ENDS.US)
                juntaLote.PercentualNivelDeInspecao = juntaComponente.PercUt;
        }

        private LoteEstrutura NovoLote(UnitOfWork uow, ENDS ensaio, JuntaComponente juntaComponente)
        {
            var lote = new LoteEstrutura(uow);

            if (ensaio == ENDS.LPPM)
                lote.PercentualNivelDeInspecao = juntaComponente.PercLpPm;

            if (ensaio == ENDS.RX)
                lote.PercentualNivelDeInspecao = juntaComponente.PercRt;

            if (ensaio == ENDS.US)
                lote.PercentualNivelDeInspecao = juntaComponente.PercUt;

            lote.Ensaio = ensaio;
            lote.QuantidadeNecessaria = QuantidadeDeJunta(lote.PercentualNivelDeInspecao);
            return lote;
        }

        public string FieldPercInspecaoEndJuntaComponente(ENDS ensaio)
        {
            var FieldPercInspecaoEndJuntaComponente = string.Empty;

            switch (ensaio)
            {
                case ENDS.LPPM:
                    FieldPercInspecaoEndJuntaComponente = "PercLpPm";
                    break;
                case ENDS.US:
                    FieldPercInspecaoEndJuntaComponente = "PercUt";
                    break;
                case ENDS.RX:
                    FieldPercInspecaoEndJuntaComponente = "PercRt";
                    break;
            }

            return FieldPercInspecaoEndJuntaComponente;
        }

        private XPCollection<JuntaComponente> GetJuntasSemLotes(Session session, ENDS ensaio)
        {
            var field = FieldPercInspecaoEndJuntaComponente(ensaio);

            //uow.Query<JuntaComponente>().Where(x=> x.LoteJuntaEstruturas.Where(j=> j.LoteEstrutura.Ensaio == ENDS.US))

            var FiltroSemLote00 = CriteriaOperator.Parse("Not IsNullOrEmpty(DataVisual)");
            var FiltroSemLote01 = CriteriaOperator.Parse("Not LoteJuntaEstruturas[ LoteEstrutura.Ensaio = ? ].Exists", ensaio);
            var FiltroSemLote02 = new BetweenOperator(field, 0.01, 0.99);

            var criteria = new GroupOperator(GroupOperatorType.And, FiltroSemLote00, FiltroSemLote01, FiltroSemLote02);
            var juntasSemLote = new XPCollection<JuntaComponente>(session);

            juntasSemLote.Criteria = criteria;
            juntasSemLote.Sorting.Add(new SortProperty(field, SortingDirection.Ascending));
            juntasSemLote.Sorting.Add(new SortProperty("DataVisual", SortingDirection.Ascending));

            return juntasSemLote;
        }

        public int QuantidadeDeJunta(double percent)
        {

            if (percent == 0.05)
                return 20;
            else if (percent == 0.10)
                return 10;
            else if (percent == 0.20)
                return 5;
            else if (percent == 0.25)
                return 4;
            else if (percent == 0.50)
                return 2;
            else
                return 100;
        }
        #endregion
    }
}
