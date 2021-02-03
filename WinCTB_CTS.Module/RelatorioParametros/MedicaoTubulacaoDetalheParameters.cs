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
    public class MedicaoTubulacaoDetalheParameters : CustomReportParametersObjectBase
    {
        public MedicaoTubulacaoDetalheParameters(IObjectSpaceCreator provider) : base(provider)
        {
            TableCriteria = typeof(MedicaoTubulacaoDetalhe);
            Contrato = ObjectSpace.FindObject<Contrato>(CriteriaOperator.Parse(""));
            Medicao = ObjectSpace.FindObject<MedicaoTubulacao>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoTubulacao>].Max(DataFechamentoMedicao)"));
        }

        [ImmediatePostData, XafDisplayName("Contrato")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceProperty("ContratosDisponiveis")]
        //[DataSourceCriteria("Oid = CurrentProjectOid()")]
        public Contrato Contrato { get; set; }

        [ImmediatePostData, XafDisplayName("Medição")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceProperty("MedicaoDisponiveis")]
        public MedicaoTubulacao Medicao { get; set; }

        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("");
            var CustomCriteriaOperator = CriteriaOperator.Parse(CriterioAdicional);
            //ObjectSpace.GetObjectsQuery<MedicaoTubulacaoDetalhe>()
            //    .Where(x=> x.MedicaoTubulacao.Oid)

            if (Contrato != null && Medicao?.Oid != null)
                criteriaOperator = CriteriaOperator.Parse("Spool.Contrato.Oid = ? And MedicaoTubulacao.Oid = ?", Contrato.Oid, Medicao.Oid);
            else if (Medicao?.Oid != null)
                criteriaOperator = new BinaryOperator("MedicaoTubulacao.Oid", Medicao.Oid);
            else if (Contrato?.Oid != null)
                criteriaOperator = new BinaryOperator("Spool.Contrato.Oid", Contrato.Oid);

            if (!(criteriaOperator is null) && !(CustomCriteriaOperator is null))
                return CriteriaOperator.And(criteriaOperator, CustomCriteriaOperator);
            else if (!(CustomCriteriaOperator is null))
                return CustomCriteriaOperator;
            else
                return criteriaOperator;
        }

        public override SortProperty[] GetSorting()
        {
            //ObjectSpace.GetObjectsQuery<MedicaoTubulacaoDetalhe>()
            //    .Select(x=> x.Spool.Documento)

            List<SortProperty> sorting = new List<SortProperty> {
                new SortProperty("Spool.Documento", SortingDirection.Ascending),
                new SortProperty("Spool.TagSpool", SortingDirection.Ascending)
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
