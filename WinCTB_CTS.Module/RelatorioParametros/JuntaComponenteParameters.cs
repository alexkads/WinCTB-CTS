using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Collections.Generic;
using System.ComponentModel;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;

namespace WinCTB_CTS.Module.RelatorioParametros {
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113594.aspx.
    public class JuntaComponenteParameters : CustomReportParametersObjectBase {
        public JuntaComponenteParameters(IObjectSpaceCreator provider) : base(provider) {
            TableCriteria = typeof(JuntaComponente);
        }

        [ImmediatePostData, XafDisplayName("Contrato")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [DataSourceProperty("ContratosDisponiveis")]
        public Contrato Contrato { get; set; }

        public override CriteriaOperator GetCriteria() {
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("");
            CriteriaOperator CustomCriteriaOperator = CriteriaOperator.Parse(CriterioAdicional);           

            if (Contrato != null)
                criteriaOperator = CriteriaOperator.Parse("Componente.Contrato.Oid == ?", Contrato.Oid);

            var criteriaFinal = CustomCriteriaOperator is null
                ? criteriaOperator
                : CriteriaOperator.And(criteriaOperator, CustomCriteriaOperator);

            return criteriaFinal;
        }

        public override SortProperty[] GetSorting() {
            //ObjectSpace.GetObjects<JuntaComponente>().Select(x => x.Junta);

            List<SortProperty> sorting = new List<SortProperty> {
                new SortProperty("Componente.DesenhoMontagem", SortingDirection.Ascending),
                new SortProperty("Componente.Peca", SortingDirection.Ascending),
                new SortProperty("Junta", SortingDirection.Ascending),
            };

            return sorting.ToArray();
        }

        [Browsable(false)]
        [CollectionOperationSet(AllowAdd = false)]
        public IList<Contrato> ContratosDisponiveis {
            get {
                return ObjectSpace.GetObjects<Contrato>();
            }
        }
    }
}
