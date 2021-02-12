using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.BusinessObjects.Comum
{
    public enum InspecaoLaudo
    {
        [ImageName("BO_Validation")]
        [XafDisplayName("A")]
        A,

        [ImageName("State_Task_Completed")]
        [XafDisplayName("R")]
        R,
    }
}
