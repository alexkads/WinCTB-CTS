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
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.Importer.Estrutura
{

    //[AutoCreatableObject]
    [ModelDefault("Caption", "Importação DE Componentes e Juntas")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    public class ParametrosImportComponentEJunta : ParametrosImportBase, IEtapasImportEstrutura, IEtapasFormacaoLotes
    {
        private bool concluidoJuntas;
        private bool concluidoComponente;

        public ParametrosImportComponentEJunta(Session session) : base(session) {
            
        }              

        public override string NomeDoRecurso { get => "MapaMontagemEBR_MV32.xlsx"; }

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
        
        public bool ConcluidoLPPM { get; set; }
        public bool ConcluidoRX { get; set; }
        public bool ConcluidoUS { get; set; }
        public bool ConcluidoInspecaoLPPM { get; set; }
        public bool ConcluidoInspecaoRX { get; set; }
        public bool ConcluidoInspecaoUS { get; set; }
        public bool ConcluidoAlinhamentoDeLotes { get; set; }
        public bool ConcluidoBalanceamentoDeLotes { get; set; }
    }
}
