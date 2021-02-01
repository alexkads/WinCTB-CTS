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
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;

namespace WinCTB_CTS.Module.BusinessObjects.Comum
{
    [DefaultClassOptions, DefaultProperty("SiteNome"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
    public class TabSite : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public TabSite(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string siteNome;

        [Size(50)]
        [Indexed(Unique = true)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Error)]
        public string SiteNome
        {
            get => siteNome;
            set => SetPropertyValue(nameof(SiteNome), ref siteNome, value);
        }

        [Association("TabSite-Spools")]
        public XPCollection<Spool> Spools
        {
            get
            {
                return GetCollection<Spool>(nameof(Spools));
            }
        }
    }
}