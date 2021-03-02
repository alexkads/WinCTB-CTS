using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
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

namespace WinCTB_CTS.Module.Importer
{
    [DomainComponent]
    [FileAttachment("PadraoDeArquivo"), ImageName("Action_SingleChoiceAction"), ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable"), NonPersistent]
    public abstract class ParametrosImportBase : IXafEntityObject, IObjectSpaceLink, INotifyPropertyChanged
    {
        private FileData padraoDeArquivo;
        private double progresso { get; set; }
        private IObjectSpace objectSpace;

        [NonPersistent, Browsable(false)]
        public virtual string NomeDoRecurso { get; }

        public ParametrosImportBase(Session session){  }

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
                    var fdata = objectSpace.FindObject<FileData>(new BinaryOperator("FileName", NomeDoRecurso));
                    if (fdata == null)
                        fdata = objectSpace.CreateObject<FileData>();

                    fdata.LoadFromStream(NomeDoRecurso, GetManifestResource(NomeDoRecurso));
                    fdata.Save();
                    padraoDeArquivo = fdata;
                }
                return padraoDeArquivo;
            }
        }


        [EditorAlias(EditorsProviders.ProgressPropertyAlias)]
        public double Progresso
        {
            get => progresso;
            set
            {
                if (progresso != value)
                {
                    progresso = value;
                    OnPropertyChanged(nameof(Progresso));
                }
            }
        }

        #region EventRegister
        // IObjectSpaceLink
        [Browsable(false)]
        public IObjectSpace ObjectSpace
        {
            get { return objectSpace; }
            set { objectSpace = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        void IXafEntityObject.OnCreated()
        {
            // Place the entity initialization code here.
            // You can initialize reference properties using Object Space methods; e.g.:
            // this.Address = objectSpace.CreateObject<Address>();
        }
        void IXafEntityObject.OnLoaded()
        {
            // Place the code that is executed each time the entity is loaded here.
        }
        void IXafEntityObject.OnSaving()
        {
            // Place the code that is executed each time the entity is saved here.
        }
        #endregion
    }
}