using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar
{
    [DefaultClassOptions]
    public class TabSchedule : BaseObject
    {
        public TabSchedule(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        private string scheduleTag;
        private string wDI;
        private string material;
        private string pipingClass;

        [Size(100)]
        public string PipingClass
        {
            get => pipingClass;
            set => SetPropertyValue(nameof(PipingClass), ref pipingClass, value);
        }

        [Size(100)]
        public string Material
        {
            get => material;
            set => SetPropertyValue(nameof(Material), ref material, value);
        }


        [Size(50)]
        public string WDI
        {
            get => wDI;
            set => SetPropertyValue(nameof(WDI), ref wDI, value);
        }

        
        [Size(50)]
        public string ScheduleTag
        {
            get => scheduleTag;
            set => SetPropertyValue(nameof(ScheduleTag), ref scheduleTag, value);
        }
    }
}