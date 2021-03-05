using System.ComponentModel;

namespace WinCTB_CTS.Module.Interfaces
{
    public interface IEtapasImportEstrutura
    {
        bool ConcluidoComponente { get; set; }
        bool ConcluidoJuntas { get; set; }
    }
}