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
    [DefaultClassOptions, DefaultProperty("Eps"), ImageName("BO_Contract"), NavigationItem("Tabela Auxiliar")]
    [Indices("Eps")]
    public class TabProcessoSoldagem : BaseObject
    { 
        public TabProcessoSoldagem(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string ench;
        private string raiz;
        private string eps;

        [Size(100)]
        public string Eps
        {
            get => eps;
            set => SetPropertyValue(nameof(Eps), ref eps, value);
        }

        [Size(100)]
        public string Raiz
        {
            get => raiz;
            set => SetPropertyValue(nameof(Raiz), ref raiz, value);
        }
        
        [Size(100)]
        public string Ench
        {
            get => ench;
            set => SetPropertyValue(nameof(Ench), ref ench, value);
        }
    }
}