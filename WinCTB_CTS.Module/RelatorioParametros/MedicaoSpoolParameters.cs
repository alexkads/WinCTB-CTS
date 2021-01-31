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
using System.Runtime.CompilerServices;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;

namespace WinCTB_CTS.Module.RelatorioParametros
{
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113594.aspx.
    public class MedicaoSpoolParameters : CustomReportParametersObjectBase
    {
        public MedicaoSpoolParameters(IObjectSpaceCreator provider) : base(provider)
        {

        }

        [ImmediatePostData, XafDisplayName("Spool")]
        [LookupEditorMode(LookupEditorMode.AllItemsWithSearch)]
        [DataSourceProperty("SpoolsDisponiveis")]
        public Spool Spool { get; set; }

        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteriaOperator = null;

                if (Spool != null)
                    criteriaOperator = CriteriaOperator.Parse("Spool.Oid == ?", Spool.Oid);

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
        public IList<Spool> SpoolsDisponiveis
        {
            get
            {
                //CriteriaOperator criterio = null;

                //if (Spool != null)
                //    criterio = CriteriaOperator.Parse("Projeto.Oid = CurrentProjectOid() And SubUnidade.Unidade.Oid = ? And StatusCadastro = 'Ativo'", Area.Oid);
                //else
                //    criterio = CriteriaOperator.Parse("Projeto.Oid = CurrentProjectOid() And StatusCadastro = 'Ativo'");

                return ObjectSpace.GetObjects<Spool>();
            }
        }
    }
}
