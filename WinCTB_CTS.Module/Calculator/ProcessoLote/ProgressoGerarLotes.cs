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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Padrao;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Calculator.ProcessoLote
{
    [DomainComponent]
    [ModelDefault("Caption", "Progresso Geração de Lotes")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    public class ProgressoGerarLotes : EtapasLotes, IXafEntityObject, IObjectSpaceLink, INotifyPropertyChanged
    {
        private IObjectSpace objectSpace;
        private double progresso;
        //private bool concluidoLPPM;
        //private bool concluidoRX;
        //private bool concluidoUS;
        //private bool concluidoInspecaoLPPM;
        //private bool concluidoInspecaoRX;
        //private bool concluidoInspecaoUS;
        //private bool concluidoAlinhamentoDeLotes;
        //private bool concluidoBalanceamentoDeLotes;

        public ProgressoGerarLotes(Session session) { }

        [EditorAlias(EditorsProviders.ProgressPropertyAlias)]
        [Delayed, VisibleInListView(false)]
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

        //[Delayed]
        //public bool ConcluidoLPPM
        //{
        //    get => concluidoLPPM;
        //    set
        //    {
        //        if (concluidoLPPM != value)
        //        {
        //            concluidoLPPM = value;
        //            OnPropertyChanged(nameof(ConcluidoLPPM));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoRX
        //{
        //    get => concluidoRX;
        //    set
        //    {
        //        if (concluidoRX != value)
        //        {
        //            concluidoRX = value;
        //            OnPropertyChanged(nameof(ConcluidoRX));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoUS
        //{
        //    get => concluidoUS;
        //    set
        //    {
        //        if (concluidoUS != value)
        //        {
        //            concluidoUS = value;
        //            OnPropertyChanged(nameof(ConcluidoUS));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoInspecaoLPPM
        //{
        //    get => concluidoInspecaoLPPM;
        //    set
        //    {
        //        if (concluidoInspecaoLPPM != value)
        //        {
        //            concluidoInspecaoLPPM = value;
        //            OnPropertyChanged(nameof(ConcluidoInspecaoLPPM));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoInspecaoRX
        //{
        //    get => concluidoInspecaoRX;
        //    set
        //    {
        //        if (concluidoInspecaoRX != value)
        //        {
        //            concluidoInspecaoRX = value;
        //            OnPropertyChanged(nameof(ConcluidoInspecaoRX));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoInspecaoUS
        //{
        //    get => concluidoInspecaoUS;
        //    set
        //    {
        //        if (concluidoInspecaoUS != value)
        //        {
        //            concluidoInspecaoUS = value;
        //            OnPropertyChanged(nameof(ConcluidoInspecaoUS));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoAlinhamentoDeLotes
        //{
        //    get => concluidoAlinhamentoDeLotes;
        //    set
        //    {
        //        if (concluidoAlinhamentoDeLotes != value)
        //        {
        //            concluidoAlinhamentoDeLotes = value;
        //            OnPropertyChanged(nameof(ConcluidoAlinhamentoDeLotes));
        //        }
        //    }
        //}

        //[Delayed]
        //public bool ConcluidoBalanceamentoDeLotes
        //{
        //    get => concluidoBalanceamentoDeLotes;
        //    set
        //    {
        //        if (concluidoBalanceamentoDeLotes != value)
        //        {
        //            concluidoBalanceamentoDeLotes = value;
        //            OnPropertyChanged(nameof(ConcluidoBalanceamentoDeLotes));
        //        }
        //    }
        //}

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
