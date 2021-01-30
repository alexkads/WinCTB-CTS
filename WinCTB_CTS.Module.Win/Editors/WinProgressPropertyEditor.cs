using DevExpress.Accessibility;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using System;
using WinCTB_CTS.Module.BusinessObjects.Padrao;

namespace WinCTB_CTS.Module.Win.Editors
{
    //[PropertyEditor(typeof(double), "ASPxCustomPropertyProgressBar", false)]
    [PropertyEditor(typeof(double), EditorsProviders.ProgressPropertyAlias, false)]
    public class WinProgressPropertyEditor : DXPropertyEditor
    {
        public WinProgressPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        protected override object CreateControlCore() => new TaskProgressBarControl();
        protected override RepositoryItem CreateRepositoryItem() => new RepositoryItemTaskProgressBarControl();
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            RepositoryItemTaskProgressBarControl repositoryItem = (RepositoryItemTaskProgressBarControl)item;
            //repositoryItem.Maximum = 100;
            repositoryItem.Minimum = 0;
            base.SetupRepositoryItem(item);
        }
    }

    public class TaskProgressBarControl : ProgressBarControl
    {
        static TaskProgressBarControl() => RepositoryItemTaskProgressBarControl.Register();

        protected override object ConvertCheckValue(object val) => val;

        public override string EditorTypeName => RepositoryItemTaskProgressBarControl.EditorName;
    }

    public class RepositoryItemTaskProgressBarControl : RepositoryItemProgressBar
    {
        protected internal const string EditorName = "TaskProgressBarControl";

        static RepositoryItemTaskProgressBarControl() => Register();
        public RepositoryItemTaskProgressBarControl()
        {
            //Maximum = 100;
            Minimum = 0;
            ShowTitle = true;
            Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }

        protected override int ConvertValue(object val)
        {
            try
            {
                float number = Convert.ToSingle(val);
                return (int)(Minimum + number * Maximum);
            }
            catch { }
            return Minimum;
        }

        protected internal static void Register()
        {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName))
            {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(TaskProgressBarControl),
                    typeof(RepositoryItemTaskProgressBarControl), typeof(ProgressBarViewInfo),
                    new ProgressBarPainter(), true, EditImageIndexes.ProgressBarControl, typeof(ProgressBarAccessible)));
            }
        }

        public override string EditorTypeName => EditorName;
    }
}
