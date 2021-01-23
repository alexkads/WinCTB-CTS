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

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao
{
    [DefaultClassOptions]
    public class Junta : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Junta(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string revisao;
        private string documento;
        private Spool spool;

        [Size(100)]
        public string Documento
        {
            get => documento;
            set => SetPropertyValue(nameof(Documento), ref documento, value);
        }
        
        [Size(10)]
        public string Revisao
        {
            get => revisao;
            set => SetPropertyValue(nameof(Revisao), ref revisao, value);
        }

        [Association("Spool-Juntas")]
        public Spool Spool
        {
            get => spool;
            set => SetPropertyValue(nameof(Spool), ref spool, value);
        }
    }
}