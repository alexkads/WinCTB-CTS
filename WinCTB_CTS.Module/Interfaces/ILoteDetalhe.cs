using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Comum;

namespace WinCTB_CTS.Module.Interfaces
{
    interface ILoteDetalhe
    {
        bool AprovouLote { get; set; }
        int CicloTermico { get; set; }
        DateTime DataInclusao { get; set; }
        DateTime DataInspecao { get; set; }
        bool InspecaoExcesso { get; set; }
        InspecaoLaudo? Laudo { get; set; }
        string NumeroDoRelatorio { get; set; }
    }
}
