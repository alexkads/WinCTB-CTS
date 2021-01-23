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

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura
{
    [DefaultClassOptions]
    public class JuntaComponente : BaseObject
    { 
        public JuntaComponente(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private Componente componente;

        [Association("Componente-JuntaComponentes")]
        public Componente Componente
        {
            get => componente;
            set => SetPropertyValue(nameof(Componente), ref componente, value);
        }
    }
}