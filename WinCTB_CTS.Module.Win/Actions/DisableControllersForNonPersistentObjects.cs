using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;

namespace WinCTB_CTS.Module.Win.Actions
{
	public class DisableControllersForNonPersistentObjects : DisableControllersByConditionViewController
	{
		protected override string DisableReason { get { return "NonPersistent"; } }
		protected override bool GetIsDisabled()
		{
			return (View is ObjectView) && ((ObjectView)View).ObjectTypeInfo.IsPersistent;
		}
	}
	public abstract class DisableControllersByConditionViewController : ViewController
	{
		protected List<Type> controllersToDeactivate = new List<Type>();
		protected virtual string DisableReason { get { return "ByCondition"; } }
		private void SetNonPersistentFlag()
		{
			foreach (Type controllerType in controllersToDeactivate)
			{
				foreach (Controller controller in Frame.Controllers)
				{
					if (controllerType.IsAssignableFrom(controller.GetType()))
					{
						controller.Active.SetItemValue(DisableReason, !GetIsDisabled());
						break;
					}
				}
			}
		}
		private void RemoveNonPersistentFlag()
		{
			foreach (Type controllerType in controllersToDeactivate)
			{
				foreach (Controller controller in Frame.Controllers)
				{
					if (controllerType.IsAssignableFrom(controller.GetType()))
					{
						controller.Active.RemoveItem(DisableReason);
						break;
					}
				}
			}
		}
		protected abstract bool GetIsDisabled();
		protected override void OnActivated()
		{
			base.OnActivated();
			SetNonPersistentFlag();
		}
		protected override void OnDeactivated()
		{
			RemoveNonPersistentFlag();
			base.OnDeactivated();
		}
		public DisableControllersByConditionViewController()
		{
			controllersToDeactivate.Add(typeof(ModificationsController));
			controllersToDeactivate.Add(typeof(DeleteObjectsViewController));
			controllersToDeactivate.Add(typeof(NewObjectViewController));
			controllersToDeactivate.Add(typeof(RecordsNavigationController));
			controllersToDeactivate.Add(typeof(RefreshController));
			controllersToDeactivate.Add(typeof(ListViewProcessCurrentObjectController));
			controllersToDeactivate.Add(typeof(ResetViewSettingsController));
			controllersToDeactivate.Add(typeof(ExportAnalysisController));
		}
	}
}
