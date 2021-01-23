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
    public class Spool : BaseObject
    {
        public Spool(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        [Association("Spool-Juntas")]
        public XPCollection<Junta> Juntas
        {
            get
            {
                return GetCollection<Junta>(nameof(Juntas));
            }
        }


    }
}