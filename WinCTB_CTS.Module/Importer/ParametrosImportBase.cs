using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
using System.IO;
using WinCTB_CTS.Module.BusinessObjects.Padrao;

namespace WinCTB_CTS.Module.Importer.Estrutura
{
    [FileAttachment("PadraoDeArquivo"), ImageName("Action_SingleChoiceAction"), ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable"), NonPersistent]
    public abstract class ParametrosImportBase : BaseObject
    {
        private FileData padraoDeArquivo;

        [NonPersistent, Browsable(false)]
        public virtual string NomeDoRecurso { get; }

        public ParametrosImportBase(Session session)
            : base(session) { }

        private static Stream GetManifestResource(string ResourceName)
        {
            Type moduleType = typeof(WinCTB_CTSModule);
            string name = $"{moduleType.Namespace}.Resources.{ResourceName}";
            return moduleType.Assembly.GetManifestResourceStream(name);
        }

        [RuleRequiredField, ImmediatePostData]
        [ExpandObjectMembers(ExpandObjectMembers.Never)]
        [XafDisplayName("Arquivo Padrão para importação")]
        [NonPersistent]
        public FileData PadraoDeArquivo
        {
            get
            {
                if (padraoDeArquivo == null)
                {
                    var os = XPObjectSpace.FindObjectSpaceByObject(this);
                    var fdata = os.FindObject<FileData>(new BinaryOperator("FileName", NomeDoRecurso));
                    if (fdata == null)
                        fdata = os.CreateObject<FileData>();

                    fdata.LoadFromStream(NomeDoRecurso, GetManifestResource(NomeDoRecurso));
                    fdata.Save();
                    padraoDeArquivo = fdata;
                }
                return padraoDeArquivo;
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