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
    //[DefaultClassOptions]
    public class MedicaoTubulacaoDetalhe : BaseObject
    { 
        public MedicaoTubulacaoDetalhe(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        private int qtdJuntaSoldFab;
        private int qtdJuntaVAFab;
        private MedicaoTubulacao medicaoTubulacao;
        private Spool spool;

        [Association("Spool-MedicaoTubulacaoDetalhes")]
        public Spool Spool
        {
            get => spool;
            set => SetPropertyValue(nameof(Spool), ref spool, value);
        }

        [XafDisplayName("Quantidade de Junta VA Fabricação")]
        public int QtdJuntaVAFab
        {
            get => qtdJuntaVAFab;
            set => SetPropertyValue(nameof(QtdJuntaVAFab), ref qtdJuntaVAFab, value);
        }

        [XafDisplayName("Quantidade de Junta Resold Fabricação")]
        public int QtdJuntaSoldFab
        {
            get => qtdJuntaSoldFab;
            set => SetPropertyValue(nameof(QtdJuntaSoldFab), ref qtdJuntaSoldFab, value);
        }

        [Association("MedicaoTubulacao-MedicaoTubulacaoDetalhes")]
        public MedicaoTubulacao MedicaoTubulacao
        {
            get => medicaoTubulacao;
            set => SetPropertyValue(nameof(MedicaoTubulacao), ref medicaoTubulacao, value);
        }
    }
}