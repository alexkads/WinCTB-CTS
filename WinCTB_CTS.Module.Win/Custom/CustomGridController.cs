using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Linq;

namespace WinCTB_CTS.Module.Win.Custom
{
    public partial class CustomGridController : ViewController<ListView>
    {
        static void configColumn(GridColumn col)
        {
            col.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
            col.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (GridView != null)
            {
                if (View.Editor is GridListEditor listEditor)
                {
                    //if (listEditor.GridView.Columns.Count >= 8)
                    //    GridView.OptionsView.ColumnAutoWidth = false;

                    GridView.OptionsView.WaitAnimationOptions = DevExpress.XtraEditors.WaitAnimationOptions.Panel;

                    listEditor.GridView.OptionsFilter.AllowMultiSelectInCheckedFilterPopup = true;
                    listEditor.GridView.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;


                    foreach (GridColumn column in listEditor.GridView.Columns)
                    {
                        if (column.DisplayFormat.FormatType == DevExpress.Utils.FormatType.None)
                            configColumn(column);

                        if (column.DisplayFormat.FormatType == DevExpress.Utils.FormatType.Custom)
                            configColumn(column);

                        //column.OptionsColumn.TabStop = false;
                    }

                }

                GridView.OptionsView.EnableAppearanceEvenRow = true;
                GridControl grid = GridView.GridControl;


                //GridView.ColumnPanelRowHeight = 35;
                //GridView.OptionsView.AllowHtmlDrawHeaders = true;
                //GridView.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;


                grid.UseEmbeddedNavigator = true;
                grid.EmbeddedNavigator.Buttons.Append.Visible = false;
                grid.EmbeddedNavigator.Buttons.CancelEdit.Visible = false;
                grid.EmbeddedNavigator.Buttons.Edit.Visible = false;
                grid.EmbeddedNavigator.Buttons.EndEdit.Visible = false;
                grid.EmbeddedNavigator.Buttons.Remove.Visible = false;

                grid.EmbeddedNavigator.Buttons.Prev.Visible = false;
                grid.EmbeddedNavigator.Buttons.Next.Visible = false;
                grid.EmbeddedNavigator.Buttons.PrevPage.Visible = false;
                grid.EmbeddedNavigator.Buttons.NextPage.Visible = false;
                grid.EmbeddedNavigator.Buttons.First.Visible = false;
                grid.EmbeddedNavigator.Buttons.Last.Visible = false;

                //GridView gridViewFocusedView = (GridView)grid.FocusedView;

                //gridViewFocusedView.GroupRowExpanding += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(gridView_GroupRowExpanding);
                //gridViewFocusedView.GroupRowExpanded += new DevExpress.XtraGrid.Views.Base.RowEventHandler(gridView_GroupRowExpanded);

            }
        }

        //private void gridView_GroupRowExpanding(object sender, DevExpress.XtraGrid.Views.Base.RowAllowEventArgs e)
        //{
        //    //((GridView)sender).GridControl.UseWaitCursor = true;
        //}

        //private void gridView_GroupRowExpanded(object sender, DevExpress.XtraGrid.Views.Base.RowEventArgs e)
        //{
        //    ((GridView)sender).GridControl.UseWaitCursor = true;
        //}

        protected GridView GridView
        {
            get
            {
                if ((View is ListView) && (View.Editor is GridListEditor))
                    return (View.Editor as GridListEditor).GridView;
                return null;
            }
        }
    }
}
