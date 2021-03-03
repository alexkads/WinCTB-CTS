using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
//using WinCTB_CTS.Module.Action;
using WinCTB_CTS.Module.BusinessObjects.Padrao;

namespace WinCTB_CTS.Module.Importer.Estrutura
{

    //[AutoCreatableObject]
    [ModelDefault("Caption", "Importação DE Componentes e Juntas")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    [Serializable]
    public class ParametrosImportComponentEJunta : ParametrosImportBase 
    {
        
        private bool concluidoLoteUS;
        private bool concluidoLoteRX;
        private bool concluidoLoteLPPM;
        private bool concluidoJuntas;
        private bool concluidoComponente;

        public ParametrosImportComponentEJunta(Session session) : base(session) {
            
        }              



        public override string NomeDoRecurso { get => "MapaMontagemEBR.xlsx"; }

        [ModelDefault("AllowEdit", "False")]
        public bool ConcluidoComponente
        {
            get => concluidoComponente;
            set
            {
                if (concluidoComponente != value)
                {
                    concluidoComponente = value;
                    OnPropertyChanged(nameof(ConcluidoComponente));
                }
            }
        }

        [ModelDefault("AllowEdit", "False")]
        public bool ConcluidoJuntas
        {
            get => concluidoJuntas;
            set
            {
                if (concluidoJuntas != value)
                {
                    concluidoJuntas = value;
                    OnPropertyChanged(nameof(ConcluidoJuntas));
                }
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (LP/PM)")]
        public bool ConcluidoLoteLPPM
        {
            get => concluidoLoteLPPM;
            set
            {
                if (concluidoLoteLPPM != value)
                {
                    concluidoLoteLPPM = value;
                    OnPropertyChanged(nameof(ConcluidoLoteLPPM));
                }
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (RX)")]
        public bool ConcluidoLoteRX
        {
            get => concluidoLoteRX;
            set
            {
                if (concluidoLoteRX != value)
                {
                    concluidoLoteRX = value;
                    OnPropertyChanged(nameof(ConcluidoLoteRX));
                }
            }
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (US)")]
        public bool ConcluidoLoteUS
        {
            get => concluidoLoteUS;
            set
            {
                if (concluidoLoteUS != value)
                {
                    concluidoLoteUS = value;
                    OnPropertyChanged(nameof(ConcluidoLoteUS));
                }
            }
        }

    }
}
