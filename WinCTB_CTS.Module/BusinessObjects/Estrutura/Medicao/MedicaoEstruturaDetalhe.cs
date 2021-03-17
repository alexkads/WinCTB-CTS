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

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao {
    public class MedicaoEstruturaDetalhe : BaseObject {
        public MedicaoEstruturaDetalhe(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
        }

        MedicaoEstruturaDetalhe medicaoAnterior;
        double percAvancoTotalPoderado;
        double pesoAvancoTotalPoderado;
        double eAPPesoEND;
        double eAPPesoSolda;
        double eAPPesoFitUP;
        double eAPPesoPosicionamento;
        double pesoEND;
        double pesoRX;
        double pesoUS;
        double pesoLPPM;
        double pesoVisual;
        double pesoSolda;
        double pesoFitUp;
        double pesoPosicionamento;
        double percAvancoEND;
        double percAvancoRX;
        double percAvancoUS;
        double percAvancoLPPM;
        double percAvancoVisual;
        double percAvancoSolda;
        double percAvancoFitUp;
        double percPosicionamento;
        double eNDExecutaMM;
        double eNDPrevistoMM;
        double rXExecutadoMM;
        double rXPrevistoMM;
        double uSExecutadoMM;
        double uSPrevistoMM;
        double lPPMExecutadoMM;
        double lPPMPrevistoMM;
        double visualExecutadoMM;
        double visualPrevistoMM;
        double soldaExecutadoMM;
        double fitUpExecutadoMM;
        double medJointMM;
        double pesoTotal;
        private Componente componente;
        private MedicaoEstrutura medicaoEstrutura;

        [Association("MedicaoEstrutura-MedicaoEstruturaDetalhes")]
        public MedicaoEstrutura MedicaoEstrutura {
            get => medicaoEstrutura;
            set => SetPropertyValue(nameof(MedicaoEstrutura), ref medicaoEstrutura, value);
        }

        [Association("Componente-MedicaoEstruturaDetalhes")]
        public Componente Componente {
            get => componente;
            set => SetPropertyValue(nameof(Componente), ref componente, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoTotal {
            get => pesoTotal;
            set => SetPropertyValue(nameof(PesoTotal), ref pesoTotal, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double MedJointMM {
            get => medJointMM;
            set => SetPropertyValue(nameof(MedJointMM), ref medJointMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double FitUpExecutadoMM {
            get => fitUpExecutadoMM;
            set => SetPropertyValue(nameof(FitUpExecutadoMM), ref fitUpExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double SoldaExecutadoMM {
            get => soldaExecutadoMM;
            set => SetPropertyValue(nameof(SoldaExecutadoMM), ref soldaExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double VisualPrevistoMM {
            get => visualPrevistoMM;
            set => SetPropertyValue(nameof(VisualPrevistoMM), ref visualPrevistoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double VisualExecutadoMM {
            get => visualExecutadoMM;
            set => SetPropertyValue(nameof(VisualExecutadoMM), ref visualExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double LPPMPrevistoMM {
            get => lPPMPrevistoMM;
            set => SetPropertyValue(nameof(LPPMPrevistoMM), ref lPPMPrevistoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double LPPMExecutadoMM {
            get => lPPMExecutadoMM;
            set => SetPropertyValue(nameof(LPPMExecutadoMM), ref lPPMExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double USPrevistoMM {
            get => uSPrevistoMM;
            set => SetPropertyValue(nameof(USPrevistoMM), ref uSPrevistoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double USExecutadoMM {
            get => uSExecutadoMM;
            set => SetPropertyValue(nameof(USExecutadoMM), ref uSExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double RXPrevistoMM {
            get => rXPrevistoMM;
            set => SetPropertyValue(nameof(RXPrevistoMM), ref rXPrevistoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double RXExecutadoMM {
            get => rXExecutadoMM;
            set => SetPropertyValue(nameof(RXExecutadoMM), ref rXExecutadoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double ENDPrevistoMM {
            get => eNDPrevistoMM;
            set => SetPropertyValue(nameof(ENDPrevistoMM), ref eNDPrevistoMM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double ENDExecutaMM {
            get => eNDExecutaMM;
            set => SetPropertyValue(nameof(ENDExecutaMM), ref eNDExecutaMM, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercPosicionamento {
            get => percPosicionamento;
            set => SetPropertyValue(nameof(PercPosicionamento), ref percPosicionamento, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoFitUp {
            get => percAvancoFitUp;
            set => SetPropertyValue(nameof(PercAvancoFitUp), ref percAvancoFitUp, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoSolda {
            get => percAvancoSolda;
            set => SetPropertyValue(nameof(PercAvancoSolda), ref percAvancoSolda, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoVisual {
            get => percAvancoVisual;
            set => SetPropertyValue(nameof(PercAvancoVisual), ref percAvancoVisual, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoLPPM {
            get => percAvancoLPPM;
            set => SetPropertyValue(nameof(PercAvancoLPPM), ref percAvancoLPPM, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoUS {
            get => percAvancoUS;
            set => SetPropertyValue(nameof(PercAvancoUS), ref percAvancoUS, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoRX {
            get => percAvancoRX;
            set => SetPropertyValue(nameof(PercAvancoRX), ref percAvancoRX, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoEND {
            get => percAvancoEND;
            set => SetPropertyValue(nameof(PercAvancoEND), ref percAvancoEND, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoPosicionamento {
            get => pesoPosicionamento;
            set => SetPropertyValue(nameof(PesoPosicionamento), ref pesoPosicionamento, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoFitUp {
            get => pesoFitUp;
            set => SetPropertyValue(nameof(PesoFitUp), ref pesoFitUp, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoSolda {
            get => pesoSolda;
            set => SetPropertyValue(nameof(PesoSolda), ref pesoSolda, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoVisual {
            get => pesoVisual;
            set => SetPropertyValue(nameof(PesoVisual), ref pesoVisual, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoLPPM {
            get => pesoLPPM;
            set => SetPropertyValue(nameof(PesoLPPM), ref pesoLPPM, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoUS {
            get => pesoUS;
            set => SetPropertyValue(nameof(PesoUS), ref pesoUS, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoRX {
            get => pesoRX;
            set => SetPropertyValue(nameof(PesoRX), ref pesoRX, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoEND {
            get => pesoEND;
            set => SetPropertyValue(nameof(PesoEND), ref pesoEND, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double EAPPesoPosicionamento {
            get => eAPPesoPosicionamento;
            set => SetPropertyValue(nameof(EAPPesoPosicionamento), ref eAPPesoPosicionamento, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double EAPPesoFitUP {
            get => eAPPesoFitUP;
            set => SetPropertyValue(nameof(EAPPesoFitUP), ref eAPPesoFitUP, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double EAPPesoSolda {
            get => eAPPesoSolda;
            set => SetPropertyValue(nameof(EAPPesoSolda), ref eAPPesoSolda, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double EAPPesoEND {
            get => eAPPesoEND;
            set => SetPropertyValue(nameof(EAPPesoEND), ref eAPPesoEND, value);
        }

        [ModelDefault("DisplayFormat", "n2"), ModelDefault("EditMask", "n2")]
        public double PesoAvancoTotalPoderado {
            get => pesoAvancoTotalPoderado;
            set => SetPropertyValue(nameof(PesoAvancoTotalPoderado), ref pesoAvancoTotalPoderado, value);
        }

        [ModelDefault("DisplayFormat", "P4"), ModelDefault("EditMask", "P4")]
        public double PercAvancoTotalPoderado {
            get => percAvancoTotalPoderado;
            set => SetPropertyValue(nameof(PercAvancoTotalPoderado), ref percAvancoTotalPoderado, value);
        }

        public MedicaoEstruturaDetalhe MedicaoAnterior {
            get => medicaoAnterior;
            set => SetPropertyValue(nameof(MedicaoAnterior), ref medicaoAnterior, value);
        }

    }
}