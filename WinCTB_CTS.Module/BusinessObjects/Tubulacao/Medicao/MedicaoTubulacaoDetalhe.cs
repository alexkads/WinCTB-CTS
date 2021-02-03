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


        private MedicaoTubulacaoDetalhe medicaoAnterior;
        double pesoSpoolLineCheckMont;
        double avancoSpoolLineCheckMont;
        private double pesoSpoolPosiMont;
        private double avancoSpoolPosiMont;
        private double pesoJuntaENDMont;
        private double pesoJuntaSoldMont;
        private double pesoJuntaVAMont;
        private double pesoSpoolENDFab;
        private double pesoSpoolSoldFab;
        private double pesoSpoolVAFab;
        private double pesoSpoolCorteFab;
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

        [Association("MedicaoTubulacao-MedicaoTubulacaoDetalhes")]
        public MedicaoTubulacao MedicaoTubulacao
        {
            get => medicaoTubulacao;
            set => SetPropertyValue(nameof(MedicaoTubulacao), ref medicaoTubulacao, value);
        }

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

        public double PesoSpoolCorteFab
        {
            get => pesoSpoolCorteFab;
            set => SetPropertyValue(nameof(PesoSpoolCorteFab), ref pesoSpoolCorteFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolVAFab
        {
            get => avancoSpoolVAFab;
            set => SetPropertyValue(nameof(AvancoSpoolVAFab), ref avancoSpoolVAFab, value);
        }


        public double PesoSpoolVAFab
        {
            get => pesoSpoolVAFab;
            set => SetPropertyValue(nameof(PesoSpoolVAFab), ref pesoSpoolVAFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolSoldFab
        {
            get => avancoSpoolSoldFab;
            set => SetPropertyValue(nameof(AvancoSpoolSoldFab), ref avancoSpoolSoldFab, value);
        }


        public double PesoSpoolSoldFab
        {
            get => pesoSpoolSoldFab;
            set => SetPropertyValue(nameof(PesoSpoolSoldFab), ref pesoSpoolSoldFab, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolENDFab
        {
            get => avancoSpoolENDFab;
            set => SetPropertyValue(nameof(AvancoSpoolENDFab), ref avancoSpoolENDFab, value);
        }


        public double PesoSpoolENDFab
        {
            get => pesoSpoolENDFab;
            set => SetPropertyValue(nameof(PesoSpoolENDFab), ref pesoSpoolENDFab, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolPosiMont
        {
            get => avancoSpoolPosiMont;
            set => SetPropertyValue(nameof(AvancoSpoolPosiMont), ref avancoSpoolPosiMont, value);
        }

        public double PesoSpoolPosiMont
        {
            get => pesoSpoolPosiMont;
            set => SetPropertyValue(nameof(PesoSpoolPosiMont), ref pesoSpoolPosiMont, value);
        }

        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaVAMont
        {
            get => avancoJuntaVAMont;
            set => SetPropertyValue(nameof(AvancoJuntaVAMont), ref avancoJuntaVAMont, value);
        }


        public double PesoJuntaVAMont
        {
            get => pesoJuntaVAMont;
            set => SetPropertyValue(nameof(PesoJuntaVAMont), ref pesoJuntaVAMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaSoldMont
        {
            get => avancoJuntaSoldMont;
            set => SetPropertyValue(nameof(AvancoJuntaSoldMont), ref avancoJuntaSoldMont, value);
        }


        public double PesoJuntaSoldMont
        {
            get => pesoJuntaSoldMont;
            set => SetPropertyValue(nameof(PesoJuntaSoldMont), ref pesoJuntaSoldMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoJuntaENDMont
        {
            get => avancoJuntaENDMont;
            set => SetPropertyValue(nameof(AvancoJuntaENDMont), ref avancoJuntaENDMont, value);
        }


        public double PesoJuntaENDMont
        {
            get => pesoJuntaENDMont;
            set => SetPropertyValue(nameof(PesoJuntaENDMont), ref pesoJuntaENDMont, value);
        }


        [ModelDefault("DisplayFormat", "P4")]
        [ModelDefault("EditMask", "P4")]
        public double AvancoSpoolLineCheckMont
        {
            get => avancoSpoolLineCheckMont;
            set => SetPropertyValue(nameof(AvancoSpoolLineCheckMont), ref avancoSpoolLineCheckMont, value);
        }


        public double PesoSpoolLineCheckMont
        {
            get => pesoSpoolLineCheckMont;
            set => SetPropertyValue(nameof(PesoSpoolLineCheckMont), ref pesoSpoolLineCheckMont, value);
        }
        
        public MedicaoTubulacaoDetalhe MedicaoAnterior
        {
            get => medicaoAnterior;
            set => SetPropertyValue(nameof(MedicaoAnterior), ref medicaoAnterior, value);
        }
    }
}