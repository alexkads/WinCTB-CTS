//using System.ComponentModel;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Model;
//using DevExpress.Persistent.Base;
//using DevExpress.Xpo;

//namespace WinCTB_CTS.Module.Action
//{
//	interface ICustomLayoutInfo
//	{
//	}
//	[NonPersistent]
//	public class DemoItem
//	{
//		private string item;
//		public DemoItem(string item)
//		{
//			this.item = item;
//		}
//		public string Item
//		{
//			get { return item; }
//		}
//	}
//	[NonPersistent]
//	[AutoCreatableObject]
//	public abstract class ActionBaseRootObject : INotifyPropertyChanged, ICustomLayoutInfo
//	{
//		private string logItems;
//		private BindingList<DemoItem> selectionDependency;
//		private ActionBase cmIsEmpty;
//		private ActionBase cmNotEmpty;
//		public ActionBaseRootObject()
//		{
//			selectionDependency = new BindingList<DemoItem>();
//			selectionDependency.Add(new DemoItem(ActionsDemoStrings.DemoItemText + " 1"));
//			selectionDependency.Add(new DemoItem(ActionsDemoStrings.DemoItemText + " 2"));
//			selectionDependency.AllowNew = false;
//			selectionDependency.AllowRemove = false;
//			cmIsEmpty = new SimpleAction();
//			cmNotEmpty = new SimpleAction();
//		}
//		[ModelDefault(CustomDetailViewItemsGenarator.TabPageNameAttribute, "SelectionDependency")]
//		[Index(10)]
//		public BindingList<DemoItem> SelectionDependency
//		{
//			get { return selectionDependency; }
//		}
//		[ModelDefault(CustomDetailViewItemsGenarator.TabPageNameAttribute, "ConfirmationMessage")]
//		[Index(20)]
//		[ModelDefault(CustomDetailViewItemsGenarator.VisiblePropertiesAttribute, "ConfirmationMessage")]
//		public ActionBase ConfirmationMessageIsEmpty
//		{
//			get { return cmIsEmpty; }
//		}
//		[ModelDefault(CustomDetailViewItemsGenarator.TabPageNameAttribute, "ConfirmationMessage")]
//		[ModelDefault(CustomDetailViewItemsGenarator.VisiblePropertiesAttribute, "ConfirmationMessage")]
//		[Index(30)]
//		public ActionBase ConfirmationMessageNotEmpty
//		{
//			get { return cmNotEmpty; }
//		}
//		[Size(SizeAttribute.Unlimited)]
//		[ModelDefault("RowCount", "5")]
//		[ModelDefault(CustomDetailViewItemsGenarator.FooterAttribute, "True")]
//		public string Log
//		{
//			get { return logItems; }
//		}
//		public void LogTrace(string message)
//		{
//			logItems = message + "\r\n" + logItems;
//			if (PropertyChanged != null)
//			{
//				PropertyChanged(this, new PropertyChangedEventArgs("Log"));
//			}
//		}
//		public void LogExecuteAction(ActionBase action, params object[] parameter)
//		{
//			string message = ActionsDemoStrings.LogTraceHeader + action.Caption + ActionsDemoStrings.LogTraceBody;
//			if (parameter.Length > 0)
//			{
//				string parameterValue = "null";
//				if (parameter[0] != null)
//				{
//					parameterValue = parameter[0].ToString();
//				}
//				message += ActionsDemoStrings.LogTraceParameterHeader + parameterValue + ActionsDemoStrings.LogTraceParameterTail;
//			}
//			LogTrace(message + ActionsDemoStrings.LogTraceTail);
//		}
//		public event PropertyChangedEventHandler PropertyChanged;
//	}
//}
