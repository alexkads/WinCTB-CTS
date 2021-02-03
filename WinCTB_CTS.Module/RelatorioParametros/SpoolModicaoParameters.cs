using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;

namespace WinCTB_CTS.Module.RelatorioParametros
{
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113594.aspx.
    public class SpoolMedicaoParameters : CustomReportParametersObjectBase
    {
        public SpoolMedicaoParameters(IObjectSpaceCreator provider) : base(provider)
        {

        }

        [ImmediatePostData, XafDisplayName("Contrato")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [DataSourceProperty("ContratosDisponiveis")]
        public Contrato Contrato { get; set; }

        [ImmediatePostData, XafDisplayName("Contrato")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [DataSourceProperty("MedicaoDisponiveis")]
        public MedicaoTubulacao Medicao { get; set; }

        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteriaOperator = string.Empty;

            if (Contrato != null && Medicao.Oid != null)
                    criteriaOperator = CriteriaOperator.Parse("Contrato.Oid = ? And MedicaoTubulacaoDetalhes[ MedicaoTubulacao.Oid = ? ]", Contrato.Oid, Medicao.Oid);

            return criteriaOperator;
        }

        public override SortProperty[] GetSorting()
        {
            List<SortProperty> sorting = new List<SortProperty> {
                new SortProperty("Documento", SortingDirection.Ascending),
                new SortProperty("TagSpool", SortingDirection.Ascending)
            };

            return sorting.ToArray();
        }

        [Browsable(false)]
        [CollectionOperationSet(AllowAdd = false)]
        public IList<Contrato> ContratosDisponiveis
        {
            get
            {
                return ObjectSpace.GetObjects<Contrato>();
            }
        }


        [Browsable(false)]
        [CollectionOperationSet(AllowAdd = false)]
        public IList<MedicaoTubulacao> MedicaoDisponiveis
        {
            get
            {
                return ObjectSpace.GetObjects<MedicaoTubulacao>();
            }
        }
    }
}
