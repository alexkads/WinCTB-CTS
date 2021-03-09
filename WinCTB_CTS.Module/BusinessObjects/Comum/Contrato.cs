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
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;

namespace WinCTB_CTS.Module.BusinessObjects.Comum
{
    [DefaultClassOptions, DefaultProperty("NomeDoContrato"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
    public class Contrato : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Contrato(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string nomeDoContrato;

        [Size(50)]
        [Indexed(Unique = true)]
        [RuleRequiredField(DefaultContexts.Save, ResultType = ValidationResultType.Error)]
        public string NomeDoContrato
        {
            get => nomeDoContrato;
            set => SetPropertyValue(nameof(NomeDoContrato), ref nomeDoContrato, value);
        }

        [Association("Contrato-Spools")]
        public XPCollection<Spool> Spools
        {
            get
            {
                return GetCollection<Spool>(nameof(Spools));
            }
        }

        [Association("Contrato-Componentes")]
        public XPCollection<Componente> Componentes {
            get {
                return GetCollection<Componente>(nameof(Componentes));
            }
        }

        [Association("Contrato-TabEAPPipes")]
        public XPCollection<TabEAPPipe> TabEAPPipes
        {
            get
            {
                return GetCollection<TabEAPPipe>(nameof(TabEAPPipes));
            }
        }

        [Association("Contrato-TabEAPEsts")]
        public XPCollection<TabEAPEst> TabEAPEsts
        {
            get
            {
                return GetCollection<TabEAPEst>(nameof(TabEAPEsts));
            }
        }

        [Association("Contrato-LoteEstruturas")]
        public XPCollection<LoteEstrutura> LoteEstruturas {
            get {
                return GetCollection<LoteEstrutura>(nameof(LoteEstruturas));
            }
        }
    }
}