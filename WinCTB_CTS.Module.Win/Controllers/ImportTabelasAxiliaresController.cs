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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ImportTabelasAxiliaresController : WindowController
    {
        SimpleAction ActionAtualizarTabelasAuxiliares;
        IObjectSpace objectSpace;
        public ImportTabelasAxiliaresController()
        {
            TargetWindowType = WindowType.Main;
            ActionAtualizarTabelasAuxiliares = new SimpleAction(this, "SimpleActionImportTabelasAxiliaresController", PredefinedCategory.RecordEdit)
            {
                Caption = "Atualizar Tabelas Auxiliares",
                ImageName = "UpdateTableOfContents"
            };

            ActionAtualizarTabelasAuxiliares.Execute += ActionAtualizarTabelasAuxiliares_Execute;
        }

        private void ActionAtualizarTabelasAuxiliares_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();
            var param = objectSpace.CreateObject<ParametrosAtualizacaoTabelasAuxiliares>();
            DetailView view = Application.CreateDetailView(objectSpace, param);
            view.ViewEditMode = ViewEditMode.Edit;

            e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            e.ShowViewParameters.CreatedView = view;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.Controllers.Add(dialogControllerAcceptingImportarPlanilha());
        }

        private DialogController dialogControllerAcceptingImportarPlanilha()
        {
            DialogController dialogControllerImportarPlanilha = Application.CreateController<DialogController>();
            dialogControllerImportarPlanilha.Accepting += DialogControllerImportarPlanilha_Accepting;
            //dc.CancelAction.Active["NoAccept"] = false;
            return dialogControllerImportarPlanilha;
        }

        private void DialogControllerImportarPlanilha_Accepting(object sender, DialogControllerAcceptingEventArgs e)
        {
            var parametros = (ParametrosAtualizacaoTabelasAuxiliares)e.AcceptActionArgs.SelectedObjects[0];
            MemoryStream stream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            var arquivo = parametros.Padrao;
            arquivo.SaveToStream(stream);

            //Executar importação da planilha anexo em stream
            ImportAndAddToSpreadsheetStream(stream);
        }

        DataTableCollection dtcollectionImport;

        private void ImportAndAddToSpreadsheetStream(MemoryStream stream)
        {
            //var dtPlanilha = OpenXMLHelper.Excel.Reader.Read(stream);
            //var NestedObjectSpace = View.ObjectSpace.CreateNestedObjectSpace();
            //var sessionimp = ((XPObjectSpace)NestedObjectSpace).Session;

            stream.Seek(0, SeekOrigin.Begin);

            using (var excelReader = new ExcelDataReaderHelper.Excel.Reader(stream))
            {
                dtcollectionImport = excelReader.CreateDataTableCollection(false);
            }


            ImportarSchedule(dtcollectionImport["Schedule"]);
        }

        private void ImportarSchedule(DataTable dtSchedule)
        {
            //Converter Pivot em Linhas
            var schedules = ConvertListFromPivot(dtSchedule);

            foreach (var item in schedules)
            {
                objectSpace.CreateObject<TabSchedule>

            }
        }

        static private Func<DataTable, IEnumerable<ScheduleMapping>> ConvertListFromPivot = (dt) =>
         {
             var result = new List<ScheduleMapping>();

             for (int idxrow = 0; idxrow < dt.Rows.Count; idxrow++)
             {
                 var row = dt.Rows[idxrow];

                 if (idxrow > 0)
                 {
                     for (int idxcol = 2; idxcol < row.ItemArray.Length; idxcol++)
                     {
                         result.Add(new ScheduleMapping
                         {
                             numeroLinha = idxrow,
                             pipingClass = row[0].ToString(),
                             material = row[1].ToString(),
                             diametro = ((dt.Rows[0])[idxcol]).ToString(),
                             scheduleTag = row[idxcol].ToString()
                         });
                     }
                 }
             }

             return result;
         };

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }

    class ScheduleMapping
    {
        public int numeroLinha { get; set; }
        public string pipingClass { get; set; }
        public string material { get; set; }
        public string diametro { get; set; }
        public string scheduleTag { get; set; }

    }
}
