using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
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
using WinCTB_CTS.Module.BusinessObjects.Padrao;

namespace WinCTB_CTS.Module.Importer.Estrutura
{
    [ModelDefault("Caption", "Importação DE Componentes e Juntas")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    [Serializable]
    public class ParametrosImportComponentEJunta : ParametrosImportBase, ISerializable
    {
        private string pathFileForImport;
        private bool concluidoLoteUS;
        private bool concluidoLoteRX;
        private bool concluidoLoteLPPM;
        private bool concluidoJuntas;
        private bool concluidoComponente;

        public ParametrosImportComponentEJunta(Session session) : base(session) { }

        // ISerializable 
        public ParametrosImportComponentEJunta(Session session, SerializationInfo info, StreamingContext context) : base(session)
        {
            if (info.MemberCount > 0)
            {
                PathFileForImport = info.GetString("PathFileForImport");
            }
        }

        [XafDisplayName("Caminho do Arquivo para Importação")]
        public string PathFileForImport
        {
            get => pathFileForImport;
            set
            {
                if (pathFileForImport != value)
                {
                    pathFileForImport = value;
                    OnPropertyChanged(nameof(PathFileForImport));
                }
            }
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

        [System.Security.SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PathFileForImport", PathFileForImport);
        }
    }
}
