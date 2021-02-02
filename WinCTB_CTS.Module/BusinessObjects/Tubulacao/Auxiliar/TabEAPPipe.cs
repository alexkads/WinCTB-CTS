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
using WinCTB_CTS.Module.BusinessObjects.Comum;

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar
{
    [DefaultClassOptions, ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
    public class TabEAPPipe : BaseObject
    {
        public TabEAPPipe(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        double avancoSpoolLineCheck;
        double avancoJuntaENDMont;
        double avancoJuntaSoldMont;
        double avancoJuntaVAMont;
        double avancoSpoolPosicionamento;
        double avancoSpoolENDFab;
        double avancoSpoolSoldaFab;
        double avancoSpoolVAFab;
        private double avancoSpoolCorteFab;
        private Contrato contrato;

        [Association("Contrato-TabEAPPipes")]
        public Contrato Contrato
        {
            get => contrato;
            set => SetPropertyValue(nameof(Contrato), ref contrato, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolCorteFab
        {
            get => avancoSpoolCorteFab;
            set => SetPropertyValue(nameof(AvancoSpoolCorteFab), ref avancoSpoolCorteFab, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolVAFab
        {
            get => avancoSpoolVAFab;
            set => SetPropertyValue(nameof(AvancoSpoolVAFab), ref avancoSpoolVAFab, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolSoldaFab
        {
            get => avancoSpoolSoldaFab;
            set => SetPropertyValue(nameof(AvancoSpoolSoldaFab), ref avancoSpoolSoldaFab, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolENDFab
        {
            get => avancoSpoolENDFab;
            set => SetPropertyValue(nameof(AvancoSpoolENDFab), ref avancoSpoolENDFab, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolPosicionamento
        {
            get => avancoSpoolPosicionamento;
            set => SetPropertyValue(nameof(AvancoSpoolPosicionamento), ref avancoSpoolPosicionamento, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoJuntaVAMont
        {
            get => avancoJuntaVAMont;
            set => SetPropertyValue(nameof(AvancoJuntaVAMont), ref avancoJuntaVAMont, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoJuntaSoldMont
        {
            get => avancoJuntaSoldMont;
            set => SetPropertyValue(nameof(AvancoJuntaSoldMont), ref avancoJuntaSoldMont, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoJuntaENDMont
        {
            get => avancoJuntaENDMont;
            set => SetPropertyValue(nameof(AvancoJuntaENDMont), ref avancoJuntaENDMont, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double AvancoSpoolLineCheck
        {
            get => avancoSpoolLineCheck;
            set => SetPropertyValue(nameof(AvancoSpoolLineCheck), ref avancoSpoolLineCheck, value);
        }
    }
}