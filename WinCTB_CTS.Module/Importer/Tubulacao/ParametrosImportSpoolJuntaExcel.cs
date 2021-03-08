//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp.Model;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.BaseImpl;
//using DevExpress.Persistent.Validation;
//using System;
//using System.IO;
//using DevExpress.Xpo;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Xpo;
//using WinCTB_CTS.Module.BusinessObjects.Padrao;
////using WinCTB_CTS.Module.Action;
//using WinCTB_CTS.Module.Importer.Estrutura;
//using WinCTB_CTS.Module.Interfaces;

//namespace WinCTB_CTS.Module.Importer.Tubulacao
//{
//    [ModelDefault("Caption", "Importação de Spool e Juntas")]
//    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
//    [NonPersistent, ImageName("Action_SingleChoiceAction")]
//    public class ParametrosImportSpoolJuntaExcel : ParametrosImportBase, IEtapasImportTubulacao
//    {
//        public ParametrosImportSpoolJuntaExcel(Session session) : base(session) { }

//        private bool concluidoSpool { get; set; }
//        private bool concluidoJunta { get; set; }
//        public override string NomeDoRecurso { get => "SGSeSGJ.xlsx"; }

//        [ModelDefault("AllowEdit", "False")]
//        public bool ConcluidoSpool
//        {
//            get => concluidoSpool;
//            set
//            {
//                if (concluidoSpool != value)
//                {
//                    concluidoSpool = value;
//                    OnPropertyChanged(nameof(ConcluidoSpool));
//                }
//            }
//        }

//        [ModelDefault("AllowEdit", "False")]
//        public bool ConcluidoJunta
//        {
//            get => concluidoJunta;
//            set
//            {
//                if (concluidoJunta != value)
//                {
//                    concluidoJunta = value;
//                    OnPropertyChanged(nameof(ConcluidoJunta));
//                }
//            }
//        }
//    }
//}
