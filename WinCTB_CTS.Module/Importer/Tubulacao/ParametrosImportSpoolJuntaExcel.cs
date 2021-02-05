using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using System;
using System.IO;
using DevExpress.Xpo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using WinCTB_CTS.Module.BusinessObjects.Padrao;
using WinCTB_CTS.Module.Action;

namespace WinCTB_CTS.Module.Importer.Tubulacao
{
    //[ModelDefault("Caption", "Parâmetros de importação")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    [FileAttachment(nameof(Padrao))]
    //[RuleCriteria("", DefaultContexts.Save, @"DataInicial > DataFinal", SkipNullOrEmptyValues = false, CustomMessageTemplate = "Data Inicial > Data Final")]
    public class ParametrosImportSpoolJuntaExcel : BaseObject
    {

        private FileData padrao;

        public ParametrosImportSpoolJuntaExcel(Session session)
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
                    //var NomeDoRecurso = "CSGeCSJ.xlsx";
                    var NomeDoRecurso = "SGSeSGJOriginal.xlsx";
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
    }
}
