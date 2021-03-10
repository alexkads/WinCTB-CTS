using ClosedXML.Excel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.Helpers;

namespace WinCTB_CTS.Module.Win.Controllers {

    public partial class ExportarRastreabilidadeDeEstrutura : WindowController {
        SimpleAction ActionExportarRastreabilidade;
        public ExportarRastreabilidadeDeEstrutura() {
            TargetWindowType = WindowType.Main;
            ActionExportarRastreabilidade = new SimpleAction(this, "ActionExportarRastreabilidadeEstruturaController", PredefinedCategory.RecordEdit);
        }
        protected override void OnActivated() {
            ActionExportarRastreabilidade.Caption = "Exportar Ratreabilidade de Estrutura";
            ActionExportarRastreabilidade.ImageName = "AutomaticUpdates";
            ActionExportarRastreabilidade.Execute += ActionExportarRastreabilidade_Execute; ;
        }

        private void ActionExportarRastreabilidade_Execute(object sender, SimpleActionExecuteEventArgs e) {
            var provider = new ProviderDataLayer();
            var uow = new UnitOfWork(provider.GetSimpleDataLayer());
            var juntaComponentes = uow.QueryInTransaction<JuntaComponente>();

            using (var workbook = new XLWorkbook()) {
                var worksheet = workbook.Worksheets.Add("Juntas");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id da Junta";
                worksheet.Cell(currentRow, 2).Value = "Desenho de Montagem";
                worksheet.Cell(currentRow, 3).Value = "Peça";
                worksheet.Cell(currentRow, 4).Value = "DF1";
                worksheet.Cell(currentRow, 5).Value = "DF2";
                worksheet.Cell(currentRow, 6).Value = "Junta";
                worksheet.Cell(currentRow, 7).Value = "Status CTB";
                worksheet.Cell(currentRow, 8).Value = "Status Custom";

                foreach (var junta in juntaComponentes) {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = junta.Oid;
                    worksheet.Cell(currentRow, 2).Value = junta.Componente.DesenhoMontagem;
                    worksheet.Cell(currentRow, 3).Value = junta.Componente.Peca;
                    worksheet.Cell(currentRow, 4).Value = junta.Df1;
                    worksheet.Cell(currentRow, 5).Value = junta.Df2;
                    worksheet.Cell(currentRow, 6).Value = junta.Junta;
                    worksheet.Cell(currentRow, 7).Value = junta.StatusJunta;
                    worksheet.Cell(currentRow, 8).Value = junta.StatusCustomizadoDaJunta;
                }

                using (var stream = new MemoryStream()) {
                    var fileName = String.Empty;
                    using (SaveFileDialog sfd = new SaveFileDialog()) {
                        var agora = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                        fileName = $"Relatório de Rastrabilidade {agora}";
                        sfd.FileName = fileName;
                        sfd.Filter = "Formato Excel (*.xlsx)|*.xlsx";
                        if (sfd.ShowDialog() == DialogResult.OK) {
                            if (!String.IsNullOrWhiteSpace(sfd.FileName))
                                workbook.SaveAs(sfd.FileName);
                        }
                    }
                }
            }

            uow.Dispose();
            provider.Dispose();
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
        }
    }
}
