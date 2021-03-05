using DevExpress.ExpressApp.DC;

namespace WinCTB_CTS.Module.Interfaces
{
    public interface IEtapasFormacaoLotes
    {
        [XafDisplayName("Concluído LPPM")]
        bool ConcluidoLPPM { get; set; }
        [XafDisplayName("Concluído RX")]
        bool ConcluidoRX { get; set; }
        [XafDisplayName("Concluído US")]
        bool ConcluidoUS { get; set; }
        [XafDisplayName("Concluído Inspeção de LPPM")]
        bool ConcluidoInspecaoLPPM { get; set; }
        [XafDisplayName("Concluído Inspeção de RX")]
        bool ConcluidoInspecaoRX { get; set; }
        [XafDisplayName("Concluído Inspeção de US")]
        bool ConcluidoInspecaoUS { get; set; }
        [XafDisplayName("Concluído Alinhamento de Lotes")]
        bool ConcluidoAlinhamentoDeLotes { get; set; }
        [XafDisplayName("Concluído Balanceamento de Lotes")]
        bool ConcluidoBalanceamentoDeLotes { get; set; }
    }
}