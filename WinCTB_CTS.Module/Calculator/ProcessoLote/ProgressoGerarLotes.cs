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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Padrao;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Calculator.ProcessoLote
{
    [ModelDefault("Caption", "Progresso Geração de Lotes")]
    [ModelDefault("VisibleProperties", "Caption, ToolTip, ImageName, AcceptButtonCaption, CancelButtonCaption, IsSizeable")]
    [NonPersistent, ImageName("Action_SingleChoiceAction")]
    public class ProgressoGerarLotes : BaseObject
    {         
        public ProgressoGerarLotes(Session session)
            : base(session) { }

        [EditorAlias(EditorsProviders.ProgressPropertyAlias)]
        [Delayed, VisibleInListView(false)]
        public double Progresso
        {
            get { return GetDelayedPropertyValue<double>("Progresso"); }
            set { SetDelayedPropertyValue<double>("Progresso", value); }
        }

        [Delayed]
        public bool ConcluidoLPPM
        {
            get { return GetDelayedPropertyValue<bool>("ConcluidoLPPM"); }
            set { SetDelayedPropertyValue<bool>("ConcluidoLPPM", value); }
        }

        [Delayed]
        public bool ConcluidoRX
        {
            get { return GetDelayedPropertyValue<bool>("ConcluidoRX"); }
            set { SetDelayedPropertyValue<bool>("ConcluidoRX", value); }
        }

        [Delayed]
        public bool ConcluidoUS
        {
            get { return GetDelayedPropertyValue<bool>("ConcluidoUS"); }
            set { SetDelayedPropertyValue<bool>("ConcluidoUS", value); }
        }
    }
}
