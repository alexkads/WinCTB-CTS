using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using WinCTB_CTS.Module.BusinessObjects.Comum;

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar
{
    [DefaultClassOptions, ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar Estrutura")]
    public class TabEAPEst : BaseObject
    { 
        public TabEAPEst(Session session)
            : base(session)
        {
        }

        private double end;
        private double solda;
        private double acoplamento;
        private double posicionamento;
        private string modulo;
        private Contrato contrato;

        [Association("Contrato-TabEAPEsts")]
        public Contrato Contrato
        {
            get => contrato;
            set => SetPropertyValue(nameof(Contrato), ref contrato, value);
        }

        [Size(100), XafDisplayName("Módulo")]
        public string Modulo
        {
            get => modulo;
            set => SetPropertyValue(nameof(Modulo), ref modulo, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double Posicionamento
        {
            get => posicionamento;
            set => SetPropertyValue(nameof(Posicionamento), ref posicionamento, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double Acoplamento
        {
            get => acoplamento;
            set => SetPropertyValue(nameof(Acoplamento), ref acoplamento, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double Solda
        {
            get => solda;
            set => SetPropertyValue(nameof(Solda), ref solda, value);
        }

        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "P0")]
        public double End
        {
            get => end;
            set => SetPropertyValue(nameof(End), ref end, value);
        }
    }
}