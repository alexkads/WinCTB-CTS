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

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao
{
    [DefaultClassOptions, DefaultProperty("NumeroDoFechamento"), ImageName("BO_Contract"), NavigationItem("Medição")]
    public class MedicaoEstrutura : BaseObject
    { 
        public MedicaoEstrutura(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NumeroDoFechamento = "F" + DistributedIdGeneratorHelper.Generate(this.Session.DataLayer, this.GetType().FullName, string.Empty).ToString().PadLeft(8, '0');
        }

        private DateTime dataFechamentoMedicao;
        private string numeroDoFechamento;

        [Size(50), XafDisplayName("Número do Fechamento")]
        [ModelDefault("AllowEdit", "False")]
        public string NumeroDoFechamento
        {
            get => numeroDoFechamento;
            set => SetPropertyValue(nameof(NumeroDoFechamento), ref numeroDoFechamento, value);
        }

        [XafDisplayName("Data Fechamento")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime DataFechamentoMedicao
        {
            get => dataFechamentoMedicao;
            set => SetPropertyValue(nameof(DataFechamentoMedicao), ref dataFechamentoMedicao, value);
        }

        [Association("MedicaoEstrutura-MedicaoEstruturaDetalhes"), DevExpress.Xpo.Aggregated]
        public XPCollection<MedicaoEstruturaDetalhe> MedicaoEstruturaDetalhes
        {
            get
            {
                return GetCollection<MedicaoEstruturaDetalhe>(nameof(MedicaoEstruturaDetalhes));
            }
        }

    }
}