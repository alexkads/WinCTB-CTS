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
    [DefaultClassOptions, DefaultProperty("PipingClass"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
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


        private TabDiametro tabDiametro;
        private string scheduleTag;
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

        [Association("TabDiametro-TabSchedules")]
        public TabDiametro TabDiametro
        {
            get => tabDiametro;
            set => SetPropertyValue(nameof(TabDiametro), ref tabDiametro, value);
        }

        [XafDisplayName("WDI")]
        [PersistentAlias("TabDiametro.Wdi")]
        public string Wdi => (string)EvaluateAlias("Wdi");

        [Size(50)]
        public string ScheduleTag
        {
            get => scheduleTag;
            set => SetPropertyValue(nameof(ScheduleTag), ref scheduleTag, value);
        }
    }
}