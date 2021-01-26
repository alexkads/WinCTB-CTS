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

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao
{
    [DefaultClassOptions, DefaultProperty("DataFechamentoMedicao"), ImageName("BO_Contract"), NavigationItem("Medição")]
    public class MedicaoTubulacao : BaseObject
    { 
        public MedicaoTubulacao(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        private string numeroDoFechamento;
        private DateTime dataFechamentoMedicao;
        
        [Size(50), XafDisplayName("Número do Fechamento")]
        [ModelDefault("AllowEdit", "False")]
        public string NumeroDoFechamento
        {
            get => numeroDoFechamento;
            set => SetPropertyValue(nameof(NumeroDoFechamento), ref numeroDoFechamento, value);
        }


        [XafDisplayName("Data Fechamento")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        public DateTime DataFechamentoMedicao
        {
            get => dataFechamentoMedicao;
            set => SetPropertyValue(nameof(DataFechamentoMedicao), ref dataFechamentoMedicao, value);
        }


        [Association("MedicaoTubulacao-MedicaoTubulacaoDetalhes")]
        public XPCollection<MedicaoTubulacaoDetalhe> MedicaoTubulacaoDetalhes
        {
            get
            {
                return GetCollection<MedicaoTubulacaoDetalhe>(nameof(MedicaoTubulacaoDetalhes));
            }
        }
    }
}