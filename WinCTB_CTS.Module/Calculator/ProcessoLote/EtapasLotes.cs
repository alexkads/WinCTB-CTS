//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.DC;
//using DevExpress.ExpressApp.Model;
//using DevExpress.ExpressApp.Xpo;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.BaseImpl;
//using DevExpress.Persistent.Validation;
//using DevExpress.Xpo;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.BusinessObjects.Padrao;
//using WinCTB_CTS.Module.Importer;
//using WinCTB_CTS.Module.Interfaces;

//namespace WinCTB_CTS.Module.Calculator.ProcessoLote
//{
//    [DomainComponent, NonPersistent]
//    public class EtapasLotes : IEtapasFormacaoLotes
//    {
//        private bool concluidoLPPM;
//        private bool concluidoRX;
//        private bool concluidoUS;
//        private bool concluidoInspecaoLPPM;
//        private bool concluidoInspecaoRX;
//        private bool concluidoInspecaoUS;
//        private bool concluidoAlinhamentoDeLotes;
//        private bool concluidoBalanceamentoDeLotes;
//        public EtapasLotes() { }

        

//        #region EventRegister
//        public event PropertyChangedEventHandler PropertyLotesChanged;

//        protected void OnLotesPropertyChanged(String propertyName) => PropertyLotesChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        #endregion
//    }
//}
