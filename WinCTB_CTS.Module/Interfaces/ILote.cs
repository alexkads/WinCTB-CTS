using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Interfaces
{
    public enum SituacoesQuantidade
    {
        [XafDisplayName("Incompleto")]
        Incompleto = 0,
        [XafDisplayName("Completo")]
        Completo = 1,
    }

    public enum SituacoesInspecao
    {
        [XafDisplayName("Pendente")]
        Pendente = 0,
        [XafDisplayName("Aprovado")]
        Aprovado = 1
    }

    public enum ENDS
    {
        [XafDisplayName("LP/PM")]
        LPPM = 0,
        [XafDisplayName("Radiografia")]
        RX = 1,
        [XafDisplayName("US")]
        US = 2
    }
}
