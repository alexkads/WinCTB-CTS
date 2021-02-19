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
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.Comum;

namespace WinCTB_CTS.Module.Calculator.ProcessoLoteLPPM
{
    public class GerarLoteLPPM
    {
        private IObjectSpace ObjectSpace;

        private IObjectSpaceProvider ObjectSpaceProvider;

        public GerarLoteLPPM(IObjectSpaceProvider objectSpaceProvider) => this.ObjectSpaceProvider = objectSpaceProvider;

        private void IncluirJuntaNoLote(LoteLPPMEstrutura lote, JuntaComponente juntaComponente, int cicloTermico)
        {
            var nestedObjectSpace = ObjectSpace.CreateNestedObjectSpace();
            var juntaLote = nestedObjectSpace.CreateObject<LoteLPPMJuntaEstrutura>();
            juntaLote.LoteLPPMEstrutura = nestedObjectSpace.GetObject(lote);
            juntaLote.JuntaComponente = nestedObjectSpace.GetObject(juntaComponente);
            juntaLote.DataInclusao = DateTime.Now;                      
            juntaLote.CicloTermico = cicloTermico;
            juntaLote.PercentualNivelDeInspecao = juntaComponente.PercLpPm;
            nestedObjectSpace.CommitChanges();
        }

        private LoteLPPMEstrutura NovoLote(JuntaComponente juntaComponente)
        {
            var nestedObjectSpace = ObjectSpace.CreateNestedObjectSpace();
            var lote = nestedObjectSpace.CreateObject<LoteLPPMEstrutura>();
            lote.PercentualNivelDeInspecao = juntaComponente.PercLpPm;
            lote.QuantidadeNecessaria = QuantidadeDeJunta(lote.PercentualNivelDeInspecao);
            nestedObjectSpace.CommitChanges();
            return lote;
        }

        public async Task GerarLoteLPPMAsync(IProgress<string> progress)
        {
            ObjectSpace = ObjectSpaceProvider.CreateObjectSpace();

            progress.Report($"Limpando lotes de LPPM");
            Utils.DeleteAllRecords<LoteLPPMJuntaEstrutura>(((XPObjectSpace)ObjectSpace).Session);
            Utils.DeleteAllRecords<LoteLPPMEstrutura>(((XPObjectSpace)ObjectSpace).Session);

            var FiltroSemLote00 = CriteriaOperator.Parse("Not IsNullOrEmpty(DataVisual)");
            var FiltroSemLote01 = new UnaryOperator(UnaryOperatorType.Not, new AggregateOperand("LoteLPPMJuntaEstruturas", Aggregate.Exists));
            var FiltroSemLote02 = new BetweenOperator("PercLpPm", 0.01, 0.99);

            var criteria = new GroupOperator(GroupOperatorType.And, FiltroSemLote00, FiltroSemLote01, FiltroSemLote02);

            var juntasSemLote = new XPCollection<JuntaComponente>(((XPObjectSpace)ObjectSpace).Session);

            juntasSemLote.Criteria = criteria;
            juntasSemLote.Sorting.Add(new SortProperty("PercLpPm", SortingDirection.Ascending));
            juntasSemLote.Sorting.Add(new SortProperty("DataVisual", SortingDirection.Ascending));

            double totalDataStore = juntasSemLote.EvaluateDatastoreCount();
            if (totalDataStore == 0)
            {
                progress.Report($"Não existem lotes de LP/PM [Estrutura]");
                return;
            }

            var observable = juntasSemLote.ToObservable(Scheduler.CurrentThread);

            double currentProgress = 0D;

            observable.Subscribe(juntaComponente =>
            {
                  Guid GuidComponente = Guid.NewGuid();
                GuidComponente = juntaComponente.Componente.Oid;

                Func<CriteriaOperator> CriterioFormacao = () =>
                    CriteriaOperator.Parse($"PercentualNivelDeInspecao = ? And JuntasNoLote < QuantidadeNecessaria", juntaComponente.PercLpPm); ;

                var loteLPPMEstrutura = ObjectSpace.FindObject<LoteLPPMEstrutura>(CriterioFormacao());

                if (loteLPPMEstrutura == null)
                    loteLPPMEstrutura = NovoLote(juntaComponente);

                IncluirJuntaNoLote(loteLPPMEstrutura, juntaComponente, 1);

                loteLPPMEstrutura.JuntasNoLote = loteLPPMEstrutura.LoteLPPMjuntaEstruturas.Count;
                ObjectSpace.CommitChanges();

                currentProgress++;
                progress.Report($"Montando lotes {currentProgress.ToString().PadLeft(6, '0')}/{totalDataStore.ToString().PadLeft(6, '0')} [LP/PM Estrutura]");
            });

            await observable.LastAsync();

            juntasSemLote.Dispose();
            ObjectSpace.Dispose();
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
    }
}
