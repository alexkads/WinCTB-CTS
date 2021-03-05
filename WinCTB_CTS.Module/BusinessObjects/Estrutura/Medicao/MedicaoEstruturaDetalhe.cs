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
    public class MedicaoEstruturaDetalhe : BaseObject
    { 
        public MedicaoEstruturaDetalhe(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        double pesoTotal;
        private Componente componente;
        private MedicaoEstrutura medicaoEstrutura;

        [Association("MedicaoEstrutura-MedicaoEstruturaDetalhes")]
        public MedicaoEstrutura MedicaoEstrutura
        {
            get => medicaoEstrutura;
            set => SetPropertyValue(nameof(MedicaoEstrutura), ref medicaoEstrutura, value);
        }

        [Association("Componente-MedicaoEstruturaDetalhes")]
        public Componente Componente
        {
            get => componente;
            set => SetPropertyValue(nameof(Componente), ref componente, value);
        }
                
        public double PesoTotal
        {
            get => pesoTotal;
            set => SetPropertyValue(nameof(PesoTotal), ref pesoTotal, value);
        }
    }
}