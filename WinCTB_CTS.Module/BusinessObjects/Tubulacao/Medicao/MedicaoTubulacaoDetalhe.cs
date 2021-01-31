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


        private double avancoSpoolENDFab;
        private double avancoSpoolSoldFab;
        private double avancoSpoolVAFab;
        private double avancoSpoolCorteFab;
        private double avancoJuntaENDMont;
        private double avancoJuntaSoldMont;
        private double avancoJuntaVAMont;
        private double wdiJuntaENDMont;
        private double wdiJuntaSoldMont;
        private double wdiJuntaVAMont;
        private double wdiJuntaTotalMont;
        private MedicaoTubulacao medicaoTubulacao;
        private Spool spool;

        [Association("Spool-MedicaoTubulacaoDetalhes")]
        public Spool Spool
        {
            get => spool;
            set => SetPropertyValue(nameof(Spool), ref spool, value);
        }


        [VisibleInLookupListView(false), VisibleInListView(false), VisibleInDetailView(false)]
        public double WdiJuntaTotalMont
        {
            get => wdiJuntaTotalMont;
            set => SetPropertyValue(nameof(WdiJuntaTotalMont), ref wdiJuntaTotalMont, value);
        }


        [VisibleInLookupListView(false), VisibleInListView(false), VisibleInDetailView(false)]
        public double WdiJuntaVAMont
        {
            get => wdiJuntaVAMont;
            set => SetPropertyValue(nameof(WdiJuntaVAMont), ref wdiJuntaVAMont, value);
        }


        [VisibleInLookupListView(false), VisibleInListView(false), VisibleInDetailView(false)]
        public double WdiJuntaSoldMont
        {
            get => wdiJuntaSoldMont;
            set => SetPropertyValue(nameof(WdiJuntaSoldMont), ref wdiJuntaSoldMont, value);
        }


        [VisibleInLookupListView(false), VisibleInListView(false), VisibleInDetailView(false)]
        public double WdiJuntaENDMont
        {
            get => wdiJuntaENDMont;
            set => SetPropertyValue(nameof(WdiJuntaENDMont), ref wdiJuntaENDMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolCorteFab
        {
            get => avancoSpoolCorteFab;
            set => SetPropertyValue(nameof(AvancoSpoolCorteFab), ref avancoSpoolCorteFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolVAFab
        {
            get => avancoSpoolVAFab;
            set => SetPropertyValue(nameof(AvancoSpoolVAFab), ref avancoSpoolVAFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolSoldFab
        {
            get => avancoSpoolSoldFab;
            set => SetPropertyValue(nameof(AvancoSpoolSoldFab), ref avancoSpoolSoldFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolENDFab
        {
            get => avancoSpoolENDFab;
            set => SetPropertyValue(nameof(AvancoSpoolENDFab), ref avancoSpoolENDFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaVAMont
        {
            get => avancoJuntaVAMont;
            set => SetPropertyValue(nameof(AvancoJuntaVAMont), ref avancoJuntaVAMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaSoldMont
        {
            get => avancoJuntaSoldMont;
            set => SetPropertyValue(nameof(AvancoJuntaSoldMont), ref avancoJuntaSoldMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaENDMont
        {
            get => avancoJuntaENDMont;
            set => SetPropertyValue(nameof(AvancoJuntaENDMont), ref avancoJuntaENDMont, value);
        }




        [Association("MedicaoTubulacao-MedicaoTubulacaoDetalhes")]
        public MedicaoTubulacao MedicaoTubulacao
        {
            get => medicaoTubulacao;
            set => SetPropertyValue(nameof(MedicaoTubulacao), ref medicaoTubulacao, value);
        }
    }
}