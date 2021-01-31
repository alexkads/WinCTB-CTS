using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;

namespace WinCTB_CTS.Module.RelatorioParametros
{
    [DomainComponent]
    //[Appearance("CustomReportParametersObjectBase.Hide_TipoDeSaida", TargetItems = "*;TipoDeSaida", Criteria = "Processamento == 'Imediato'", Visibility = ViewItemVisibility.Hide)]
    //[Appearance("CustomReportParametersObjectBase.Hide_TableCriteria", TargetItems = "MasterCriteria", Criteria = "IsNull(TableCriteria)", Visibility = ViewItemVisibility.Hide)]
    public abstract class CustomReportParametersObjectBase : ReportParametersObjectBase
    {


        public CustomReportParametersObjectBase(IObjectSpaceCreator provider) : base(provider)
        {
        }

        protected override IObjectSpace CreateObjectSpace()
        {
            return objectSpaceCreator.CreateObjectSpace(typeof(Spool));
        }

        [Browsable(false)]
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        public Type TableCriteria { get; set; }

        [CriteriaOptions("TableCriteria")]

        [EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [XafDisplayName("Criterio")]
        public string MasterCriteria { get; set; }
    }
}
