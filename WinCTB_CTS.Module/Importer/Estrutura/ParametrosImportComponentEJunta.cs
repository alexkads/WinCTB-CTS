﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Padrao;

namespace WinCTB_CTS.Module.Importer.Estrutura
{
    //[ModelDefault("Caption", "Parâmetros de importação")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    [FileAttachment(nameof(Padrao))]
    //[RuleCriteria("", DefaultContexts.Save, @"DataInicial > DataFinal", SkipNullOrEmptyValues = false, CustomMessageTemplate = "Data Inicial > Data Final")]
    public class ParametrosImportComponentEJunta : BaseObject
    {

        private bool concluidoLoteUS;
        private bool concluidoLoteRX;
        private bool concluidoLoteLPPM;
        private bool concluidoJuntas;
        private bool concluidoComponente;
        private FileData padrao;

        public ParametrosImportComponentEJunta(Session session)
            : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private static Stream GetManifestResource(string ResourceName)
        {
            Type moduleType = typeof(WinCTB_CTSModule);
            string name = $"{moduleType.Namespace}.Resources.{ResourceName}";
            return moduleType.Assembly.GetManifestResourceStream(name);
        }

        [RuleRequiredField, ImmediatePostData]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Padrão")]
        [NonPersistent]
        public FileData Padrao
        {
            get
            {
                if (padrao == null)
                {
                    var NomeDoRecurso = "MapaMontagemEBR.xlsx";
                    var os = XPObjectSpace.FindObjectSpaceByObject(this);
                    var fdata = os.FindObject<FileData>(new BinaryOperator("FileName", NomeDoRecurso));
                    if (fdata == null)
                        fdata = os.CreateObject<FileData>();

                    fdata.LoadFromStream(NomeDoRecurso, GetManifestResource(NomeDoRecurso));
                    fdata.Save();
                    padrao = fdata;
                }
                return padrao;
            }
        }

        [EditorAlias(EditorsProviders.ProgressPropertyAlias)]
        [Delayed, VisibleInListView(false)]
        public double Progresso
        {
            get { return GetDelayedPropertyValue<double>("Progresso"); }
            set { SetDelayedPropertyValue<double>("Progresso", value); }
        }

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
    }
}
