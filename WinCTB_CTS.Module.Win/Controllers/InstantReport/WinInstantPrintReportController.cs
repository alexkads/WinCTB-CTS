using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Caching;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinCTB_CTS.Module.Win.Controllers.InstantReport
{
    public class WinInstantPrintReportController : ObjectViewController<ObjectView, IReportDataV2>
    {
        SimpleAction printAction;
        IReportDataV2 currentReport;
        public WinInstantPrintReportController()
        {
            printAction = new SimpleAction(this, "PrintInXLSX", PredefinedCategory.Reports)
            {
                Caption = "Impressão no Xlsx",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                ImageName = "Action_SaveScript"
            };

            printAction.Execute += PrintAction_Execute;
        }

        private void PrintAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            currentReport = (IReportDataV2)e.CurrentObject;

            if (currentReport.ParametersObjectType != null)
            {
                ShowParametersDetailView(currentReport.ParametersObjectType);
            }
            else
            {
                PrintReport(null);
            }
        }

        private void ShowParametersDetailView(Type parametersObjectType)
        {
            ReportParametersObjectBase reportParametersObject = CreateReportParametersObject(parametersObjectType, ApplicationReportObjectSpaceProvider.Instance);
            if (reportParametersObject != null)
            {
                DetailView parametersDetailView = CreateParametersDetailView(reportParametersObject);
                if (parametersDetailView != null && reportParametersObject != null)
                {
                    parametersDetailView.ViewEditMode = ViewEditMode.Edit;
                    parametersDetailView.Caption = currentReport.DisplayName;
                    DialogController controller = CreatePreviewReportDialogController();
                    controller.Tag = currentReport.DisplayName;
                    controller.Accepting += Controller_Accepting;
                    ShowViewParameters showViewParameters = new ShowViewParameters();
                    showViewParameters.Controllers.Add(controller);
                    showViewParameters.CreatedView = parametersDetailView;
                    showViewParameters.TargetWindow = TargetWindow.NewWindow;
                    showViewParameters.Context = TemplateContext.PopupWindow;
                    Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
                }
            }
        }

        private void Controller_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            string reportContainerHandle = (string)((WindowController)sender).Tag;
            ((DialogController)sender).Accepting -= Controller_Accepting;

            ReportParametersObjectBase reportParametersObject = (ReportParametersObjectBase)e.AcceptActionArgs.CurrentObject;
            PrintReport(reportParametersObject);
        }

        protected virtual DialogController CreatePreviewReportDialogController()
        {
            return Application.CreateController<PreviewReportDialogController>();
        }

        protected ReportParametersObjectBase CreateReportParametersObject(Type parametersObjectType, IObjectSpaceCreator objectSpaceProvider)
        {
            Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
            ReportParametersObjectBase reportParametersObject;
            if (typeof(ReportParametersObjectBase).IsAssignableFrom(parametersObjectType))
                reportParametersObject = (ReportParametersObjectBase)TypeHelper.CreateInstance(parametersObjectType, objectSpaceProvider);
            else
                reportParametersObject = null;
            return reportParametersObject;
        }

        protected DetailView CreateParametersDetailView(ReportParametersObjectBase reportParametersObject)
        {
            Guard.ArgumentNotNull(reportParametersObject, "reportParametersObject");

            DetailView detailView = Application.CreateDetailView(
                reportParametersObject.ObjectSpace, reportParametersObject, false);

            if (detailView != null && detailView.Items.Count == 0)
            {
                detailView.Dispose();
                detailView = null;
            }
            return detailView;
        }

        private void PrintReport(ReportParametersObjectBase GetReportParametersObject)
        {
            var fileNameAddress = String.Empty;
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    var agora = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                    var fileName = $"{currentReport.DisplayName} {agora}";

                    sfd.FileName = fileName;
                    sfd.Filter = "Formato Excel (*.xlsx)|*.xlsx";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        CriteriaOperator filter = string.Empty;
                        if (GetReportParametersObject != null)
                            filter = XpoObjectInCriteriaProcessingHelper.ParseCriteria(((XPObjectSpace)ObjectSpace).Session, GetReportParametersObject.GetCriteria().LegacyToString());
                        else
                            filter = string.Empty;

                        XtraReport report = ReportDataProvider.ReportsStorage.LoadReport(currentReport);
                        ReportsModuleV2 reportsModule = ReportsModuleV2.FindReportsModule(Application.Modules);

                        if (reportsModule != null && reportsModule.ReportsDataSourceHelper != null)
                        {
                            if (GetReportParametersObject == null)
                                reportsModule.ReportsDataSourceHelper.SetupBeforePrint(report, null, null, true, null, true);
                            else
                                reportsModule.ReportsDataSourceHelper.SetupBeforePrint(report, null, filter, true, null, true);

                            XtraForm form = new XtraForm()
                            {
                                FormBorderStyle = FormBorderStyle.None,
                                Size = new System.Drawing.Size(400, 20),
                                ShowInTaskbar = false,
                                StartPosition = FormStartPosition.CenterScreen,
                                TopMost = true
                            };

                            ProgressBarControl progressBar = new ProgressBarControl();
                            ReflectorBar reflectorBar = new ReflectorBar(progressBar);

                            form.Controls.Add(progressBar);
                            progressBar.Dock = DockStyle.Fill;

                            XlsxExportOptions options = new XlsxExportOptions { ExportMode = XlsxExportMode.SingleFile, ShowGridLines = true, RawDataMode = false };

                            form.Show();
                            report.PrintingSystem.ProgressReflector = reflectorBar;
                            report.ExportToXlsx(sfd.FileName, options);
                            report.PrintingSystem.ResetProgressReflector();
                            form.Close();
                            form.Dispose();
                            fileNameAddress = sfd.FileName;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            FileInfo fi = new FileInfo(fileNameAddress);
            if (fi.Exists)
            {
                System.Diagnostics.Process.Start(fileNameAddress);
            }
        }

    }
}
