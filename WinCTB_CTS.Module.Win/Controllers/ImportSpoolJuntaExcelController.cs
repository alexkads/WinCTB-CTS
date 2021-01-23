using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportSpoolJuntaExcelController : ViewController
    {
        public ImportSpoolJuntaExcelController()
        {
            TargetObjectType = typeof(Spool);

            SimpleAction simpleActionImport = new SimpleAction(this, "PopupWindowShowActionImportSpoolJuntaExcelController", PredefinedCategory.RecordEdit)
            {
                Caption = "Importar",
                Id = nameof(Spool),
                TargetObjectType = typeof(Spool),
                TargetViewType = ViewType.ListView,
                TargetViewNesting = Nesting.Any,
                ToolTip = nameof(Spool),
                SelectionDependencyType = SelectionDependencyType.Independent,
                ImageName = "Action_Debug_Step"
            };

            simpleActionImport.Execute += SimpleActionImport_Execute;

        }

        private void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Executar((XPObjectSpace)View.ObjectSpace);
        }

        static void cancelProgress_Click(object sender, EventArgs e)
        {
            ((Form)(((SimpleButton)sender).Parent)).Close();
            throw new Exception("Process aborted by user.");
        }

        static private Func<object, DateTime> ConvertDateTime = (obj) =>
        {
            return DateTime.FromOADate(double.Parse(obj.ToString()));
        };

        public static double ConvertStringToDouble(string value)
        {
            if (Double.TryParse(value, NumberStyles.Number, new NumberFormatInfo { NumberDecimalSeparator = "." }, out double number))
                return number;
            else
                return 0;
        }


        public async Task Executar(XPObjectSpace objectSpace)
        {
            var session = objectSpace.Session;
            var dtPlanilha = new DataTable();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var fileStream = openFileDialog.OpenFile())
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            fileStream.CopyTo(stream);
                            dtSpoolsImport = OpenXMLHelper.Excel.Reader.CreateDataTableFromStream(stream, "SGS");
                            dtJuntasImport = OpenXMLHelper.Excel.Reader.CreateDataTableFromStream(stream, "SGJ");
                        }
                    }

                    await StartImport(objectSpace, dtSpoolsImport, dtJuntasImport);
                }
            }

            int QuantidadeDeRegistro = dtPlanilha.Rows.Count;

        private async Task StartImport(XPObjectSpace objectSpace, DataTable dtSpoolsImport, DataTable dtJuntasImport)
        {
            var session = objectSpace.Session;
            int QuantidadeDeRegistro = dtSpoolsImport.Rows.Count;
            using (XtraForm form = new XtraForm())
            {
                using (ProgressBarControl progressBarControl = new ProgressBarControl())
                {
                    SimpleButton cancelProgress = new SimpleButton();

                    form.ClientSize = new System.Drawing.Size(401, 57);
                    form.TopMost = true;
                    form.CancelButton = cancelProgress;
                    form.Text = "Importação";
                    form.Name = "Importação";
                    form.FormBorderEffect = FormBorderEffect.Default;
                    form.ControlBox = false;
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.Controls.Add((Control)progressBarControl);
                    form.Controls.Add((Control)cancelProgress);

                    cancelProgress.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Right));
                    cancelProgress.Location = new System.Drawing.Point(314, 12);
                    cancelProgress.Name = "simpleButton1";
                    cancelProgress.Size = new System.Drawing.Size(75, 33);
                    cancelProgress.TabIndex = 1;
                    cancelProgress.Text = "Cancel";
                    cancelProgress.Click += cancelProgress_Click;

                    progressBarControl.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left));
                    progressBarControl.Location = new System.Drawing.Point(12, 12);
                    progressBarControl.Size = new System.Drawing.Size(289, 33);
                    progressBarControl.TabIndex = 0;
                    progressBarControl.Properties.ShowTitle = true;
                    progressBarControl.Properties.Step = 1;
                    progressBarControl.Properties.PercentView = true;
                    progressBarControl.Properties.Maximum = QuantidadeDeRegistro;
                    progressBarControl.Properties.Minimum = 0;

                    form.Show();
                    session.BeginTransaction();

                    var observable = Observable.Create<DataRow>(observer =>
                    {
                        foreach (DataRow row in dtSpoolsImport.Rows)
                        {
                            observer.OnNext(row);
                        }

                        observer.OnCompleted();
                        return Disposable.Empty;
                    });

                    observable.SubscribeOn(Scheduler.CurrentThread)
                        .Subscribe(row =>
                        {
                            var ParNomeDoMunicipio = Convert.ToString(row[0]);
                            var ParNomeDoEstado = Convert.ToString(row[1]);
                            var ParSiglaEstado = Convert.ToString(row[2]);

                            try
                            {
                                session.CommitTransaction();
                            }
                            catch
                            {
                                session.RollbackTransaction();
                                ((Form)(cancelProgress.Parent)).Close();
                                throw new Exception("Process aborted by WeldTrace");
                            }

                            progressBarControl.PerformStep();
                            cancelProgress.Update();
                            System.Windows.Forms.Application.DoEvents();
                        });                   
                }

                session.PurgeDeletedObjects();
                objectSpace.CommitChanges();
                form.Close();
            }
        }

    }
}
