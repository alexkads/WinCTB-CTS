using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;

namespace WinCTB_CTS.Module.RelatorioParametros
{
    [DomainComponent]
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113594.aspx.
    public class MedicaoEstruturaDetalheParameters : CustomReportParametersObjectBase
    {
        public MedicaoEstruturaDetalheParameters(IObjectSpaceCreator provider) : base(provider)
        {
            TableCriteria = typeof(MedicaoEstruturaDetalhe);
            Contrato = ObjectSpace.FindObject<Contrato>(CriteriaOperator.Parse(""));
            Medicao = ObjectSpace.FindObject<MedicaoEstrutura>(CriteriaOperator.Parse("DataFechamentoMedicao = [<MedicaoEstrutura>].Max(DataFechamentoMedicao)"));
        }

        [ImmediatePostData, XafDisplayName("Contrato")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceProperty("ContratosDisponiveis")]
        //[DataSourceCriteria("Oid = CurrentProjectOid()")]
        public Contrato Contrato { get; set; }

        [ImmediatePostData, XafDisplayName("Medição")]
        [LookupEditorMode(LookupEditorMode.AllItems)]
        [DataSourceProperty("MedicaoDisponiveis")]
        public MedicaoEstrutura Medicao { get; set; }

        public override CriteriaOperator GetCriteria()
        {
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("");
            var CustomCriteriaOperator = CriteriaOperator.Parse(CriterioAdicional);
            //ObjectSpace.GetObjectsQuery<MedicaoEstruturaDetalhe>()
            //    .Where(x=> x.Componente.Contrato.Oid)

            if (Contrato != null && Medicao?.Oid != null)
                criteriaOperator = CriteriaOperator.Parse("Componente.Contrato.Oid = ? And MedicaoEstrutura.Oid = ?", Contrato.Oid, Medicao.Oid);
            else if (Medicao?.Oid != null)
                criteriaOperator = new BinaryOperator("MedicaoEstrutura.Oid", Medicao.Oid);
            else if (Contrato?.Oid != null)
                criteriaOperator = new BinaryOperator("Componente.Contrato.Oid", Contrato.Oid);

            if (!(criteriaOperator is null) && !(CustomCriteriaOperator is null))
                return CriteriaOperator.And(criteriaOperator, CustomCriteriaOperator);
            else if (!(CustomCriteriaOperator is null))
                return CustomCriteriaOperator;
            else
                return criteriaOperator;
        }

        public override SortProperty[] GetSorting()
        {
            //ObjectSpace.GetObjectsQuery<MedicaoEstruturaDetalhe>()
            //    .Select(x=> x.Componente.Contrato.NomeDoContrato)

            
            List<SortProperty> sorting = new List<SortProperty> {
                new SortProperty("Componente.Contrato.NomeDoContrato", SortingDirection.Ascending),
                new SortProperty("Componente.Modulo", SortingDirection.Ascending),
                new SortProperty("Componente.Peca", SortingDirection.Ascending)
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
        public IList<MedicaoEstrutura> MedicaoDisponiveis
        {
            get
            {
                return ObjectSpace.GetObjects<MedicaoEstrutura>();
            }
        }
    }
}
