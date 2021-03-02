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
    public class ParametrosImportComponentEJunta : ParametrosImportBase, INotifyPropertyChanged, ISerializable
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
            set => SetPropertyValue(nameof(PathFileForImport), ref pathFileForImport, value);
        }


        public override string NomeDoRecurso { get => "MapaMontagemEBR.xlsx"; }

        [ModelDefault("AllowEdit", "False")]
        public bool ConcluidoComponente
        {
            get => concluidoComponente;
            set => SetPropertyValue(nameof(ConcluidoComponente), ref concluidoComponente, value);
        }

        [ModelDefault("AllowEdit", "False")]
        public bool ConcluidoJuntas
        {
            get => concluidoJuntas;
            set => SetPropertyValue(nameof(ConcluidoJuntas), ref concluidoJuntas, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (LP/PM)")]
        public bool ConcluidoLoteLPPM
        {
            get => concluidoLoteLPPM;
            set => SetPropertyValue(nameof(ConcluidoLoteLPPM), ref concluidoLoteLPPM, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (RX)")]
        public bool ConcluidoLoteRX
        {
            get => concluidoLoteRX;
            set => SetPropertyValue(nameof(ConcluidoLoteRX), ref concluidoLoteRX, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Concluído Lotes (US)")]
        public bool ConcluidoLoteUS
        {
            get => concluidoLoteUS;
            set => SetPropertyValue(nameof(ConcluidoLoteUS), ref concluidoLoteUS, value);
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [System.Security.SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("PathFileForImport", PathFileForImport);
        }
    }
}
