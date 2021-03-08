//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.ExpressApp.Win.Editors;
//using DevExpress.ExpressApp.Win.Templates.ActionContainers;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using DevExpress.XtraEditors;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;

//namespace WinCTB_CTS.Module.Win.Controllers.Experimental
//{
//    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
//    public partial class CustomizeWinActionContainerViewItemController : ViewController<DetailView>
//    {
//        public CustomizeWinActionContainerViewItemController()
//        {
//            TargetObjectType = typeof(ParametrosImportComponentEJunta);
//        }
//        protected override void OnActivated()
//        {
//            base.OnActivated();
//            foreach (WinActionContainerViewItem item in View.GetItems<WinActionContainerViewItem>())
//            {
//                if (item.Id == "DetailViewActions")
//                    item.ControlCreated += Item_ControlCreated;
//            }
//        }

//        private void Item_ControlCreated(object sender, EventArgs e)
//        {
//            ((ButtonsContainer)((ViewItem)(sender)).Control).ActionItemAdding += CustomizeWinActionContainerViewItemController_ActionItemAdding;
//        }

//        void CustomizeWinActionContainerViewItemController_ActionItemAdding(object sender, ActionItemEventArgs e)
//        {
//            if (e.Item.Action.Id == "SimpleAction")
//            {
//                SimpleButton button = (((ButtonsContainersSimpleActionItem)e.Item).Control) as SimpleButton;
//                if (button != null)
//                {
//                    if (!string.IsNullOrEmpty(e.Item.Action.Model.ImageName))
//                        button.Image = ImageLoader.Instance.GetLargeImageInfo(e.Item.Action.Model.ImageName).Image;
//                    button.Font = new Font(button.Font, FontStyle.Bold);
//                }
//            }
//        }

//        protected override void OnViewControlsCreated()
//        {
//            base.OnViewControlsCreated();
//            // Access and customize the target View control.
//        }
//        protected override void OnDeactivated()
//        {
//            // Unsubscribe from previously subscribed events and release other references and resources.
//            base.OnDeactivated();
//        }
//    }
//}
