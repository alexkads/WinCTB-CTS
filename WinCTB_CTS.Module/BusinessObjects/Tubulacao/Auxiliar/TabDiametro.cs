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

    public class TabDiametro : BaseObject
    { 
        public TabDiametro(Session session)
            : base(session)
        {
        }


        private string wdi;
        private string diametroMilimetro;
        private string diametroPolegada;

        [Size(100), XafDisplayName("Polegada")]
        public string DiametroPolegada
        {
            get => diametroPolegada;
            set => SetPropertyValue(nameof(DiametroPolegada), ref diametroPolegada, value);
        }

        [Size(100), XafDisplayName("mm")]
        public string DiametroMilimetro
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
    }
}