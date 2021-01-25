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
using System.Threading;
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

        private async void SimpleActionImport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            await Executar((XPObjectSpace)View.ObjectSpace);
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
            var dtSpoolsImport = new DataTable();
            var dtJuntasImport = new DataTable();

            IntefaceInitialize();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var fileStream = openFileDialog.OpenFile();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        fileStream.CopyTo(stream);
                        dtSpoolsImport = OpenXMLHelper.Excel.Reader.CreateDataTableFromStream(stream, "SGS");
                        dtJuntasImport = OpenXMLHelper.Excel.Reader.CreateDataTableFromStream(stream, "SGJ");
                        //var Import1 = OpenXMLHelper.Excel.Reader.CreateArrayFromStream(stream, "SGS");
                        //var Import2 = OpenXMLHelper.Excel.Reader.CreateArrayFromStream(stream, "SGJ");
                    }

                    fileStream.Dispose();

                    var progress = new Progress<ImportProgressReport>(value =>
                    {
                        progressBarControl.Properties.Maximum = value.TotalRows;
                        statusProgess.Text = value.MessageImport;

                        if (value.CurrentRow > 0)
                            progressBarControl.PerformStep();

                        progressBarControl.Update();
                        statusProgess.Update();
                    });

                    await Task.Run(() =>
                    {
                        StartImport(objectSpace, dtSpoolsImport, dtJuntasImport, progress);
                    });

                    form.Close();
                }
            }

           //int QuantidadeDeRegistro = dtSpoolsImport.Rows.Count;
        }

        private XtraForm form;
        private ProgressBarControl progressBarControl;
        private SimpleButton cancelProgress;
        private LabelControl statusProgess;

        private void IntefaceInitialize()
        {
            progressBarControl = new ProgressBarControl();
            cancelProgress = new SimpleButton();
            statusProgess = new LabelControl();

            form = new XtraForm();
            form.ClientSize = new System.Drawing.Size(401, 87);
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
            form.Controls.Add((Control)statusProgess);

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
            progressBarControl.Properties.Minimum = 0;

            statusProgess.Anchor = ((AnchorStyles.Bottom | AnchorStyles.Left));
            statusProgess.Location = new System.Drawing.Point(12, 50);
            statusProgess.Text = "Iniciando";

            form.Show();
        }

        private void StartImport(XPObjectSpace objectSpace, DataTable dtSpoolsImport, DataTable dtJuntasImport, IProgress<ImportProgressReport> progress)
        {
            var session = objectSpace.Session;
            var ToalRows = dtSpoolsImport.Rows.Count;

            progress.Report(new ImportProgressReport
            {
                TotalRows = ToalRows,
                CurrentRow = 0,
                MessageImport = "Inicializando importação"
            });

            session.BeginTransaction();

            for (int i = 0; i < ToalRows; i++)
            {
                var linha = dtSpoolsImport.Rows[i];
                var colunaDocumento = Convert.ToString(linha["Documento"]);
                var colunaRevSpool = Convert.ToString(linha["Revisao"] ?? null);
                //var colunaData = ConvertDateTime(linha[2]);

                var spool = objectSpace.CreateObject<Spool>();
                spool.Documento = colunaDocumento;
                spool.RevSpool = colunaRevSpool;
                //spool.DataCadastro = colunaData;
                spool.Save();

                if(i % 1000 == 0)
                {
                    try
                    {
                        session.CommitTransaction();
                    }
                    catch
                    {
                        session.RollbackTransaction();
                        throw new Exception("Process aborted by system");
                    }
                }

                progress.Report(new ImportProgressReport
                {
                    TotalRows = ToalRows,
                    CurrentRow = i,
                    MessageImport = $"Importando linha {i}/{ToalRows}"
                });
            }

            session.CommitTransaction();
            session.PurgeDeletedObjects();
            objectSpace.CommitChanges();
        }
    }

    public class ImportProgressReport
    {
        public int TotalRows { get; set; }
        public int CurrentRow { get; set; }
        public string MessageImport { get; set; }
    }
}
