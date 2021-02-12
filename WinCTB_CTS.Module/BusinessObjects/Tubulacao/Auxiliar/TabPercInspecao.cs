using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar
{
    [DefaultClassOptions, DefaultProperty("PercentualDeInspecao"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
    [Indices("Spec")]
    public class TabPercInspecao : BaseObject
    { 
        public TabPercInspecao(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }


        private double percentualDeInspecao;
        private string spec;

        [Size(20)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Error)]
        public string Spec
        {
            get => spec;
            set => SetPropertyValue(nameof(Spec), ref spec, value);
        }


        [XafDisplayName("Insp.")]
        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "n1")]
        [RuleRange(DefaultContexts.Save, 0, 1)]
        public double PercentualDeInspecao
        {
            get => percentualDeInspecao;
            set => SetPropertyValue(nameof(PercentualDeInspecao), ref percentualDeInspecao, value);
        }


        [Association("TabPercInspecao-JuntaSpools")]
        public XPCollection<JuntaSpool> JuntaSpools
        {
            get
            {
                return GetCollection<JuntaSpool>(nameof(JuntaSpools));
            }
        }
    }
}