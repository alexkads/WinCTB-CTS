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
    [DefaultClassOptions, DefaultProperty("DiametroPolegada"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]

    public class TabDiametro : BaseObject
    { 
        public TabDiametro(Session session)
            : base(session)
        {
        }


        private int diametroMilimetro;
        private string wdi;
        private string diametroPolegada;

        [Size(100), XafDisplayName("Polegada")]
        public string DiametroPolegada
        {
            get => diametroPolegada;
            set => SetPropertyValue(nameof(DiametroPolegada), ref diametroPolegada, value);
        }

        //[Size(100), XafDisplayName("mm")]
        //public string DiametroMilimetro
        //{
        //    get => diametroMilimetro;
        //    set => SetPropertyValue(nameof(DiametroMilimetro), ref diametroMilimetro, value);
        //}


        [Size(100), XafDisplayName("mm")]
        public int DiametroMilimetro
        {
            get => diametroMilimetro;
            set => SetPropertyValue(nameof(DiametroMilimetro), ref diametroMilimetro, value);
        }



        [Size(100), XafDisplayName("WDI")]
        public string Wdi
        {
            get => wdi;
            set => SetPropertyValue(nameof(Wdi), ref wdi, value);
        }

        [Association("TabDiametro-JuntaSpools")]
        public XPCollection<JuntaSpool> JuntaSpools
        {
            get
            {
                return GetCollection<JuntaSpool>(nameof(JuntaSpools));
            }
        }

        [Association("TabDiametro-TabSchedules")]
        public XPCollection<TabSchedule> TabSchedules
        {
            get
            {
                return GetCollection<TabSchedule>(nameof(TabSchedules));
            }
        }
    }
}