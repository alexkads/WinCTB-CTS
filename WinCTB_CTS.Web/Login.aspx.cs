using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates;
using System;

public partial class LoginPage : BaseXafPage
{
    public override System.Web.UI.Control InnerContentPlaceHolder
    {
        get
        {
            return Content;
        }
    }
}
