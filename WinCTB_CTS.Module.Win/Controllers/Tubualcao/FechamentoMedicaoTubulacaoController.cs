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
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;
using WinCTB_CTS.Module.Comum;
using WinCTB_CTS.Module.Importer;

namespace WinCTB_CTS.Module.Win.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class FechamentoMedicaoTubulacaoController : ViewController
    {
        public FechamentoMedicaoTubulacaoController()
        {
            TargetObjectType = typeof(MedicaoTubulacao);

            SimpleAction ExecutarMedicao = new SimpleAction(this, "FechamentoMedicaoTubulacaoSimpleActionController", PredefinedCategory.RecordEdit)
            {
                Caption = "Executar Medição",
                Id = nameof(MedicaoTubulacao),
                TargetObjectType = typeof(MedicaoTubulacao),
                TargetViewType = ViewType.ListView,
                TargetViewNesting = Nesting.Any,
                ToolTip = nameof(MedicaoTubulacao),
                SelectionDependencyType = SelectionDependencyType.Independent,
                ImageName = "Action_Debug_Step"
            };

            ExecutarMedicao.Execute += ExecutarMedicao_Execute;
        }

        private XtraForm FormProgressImport;
        private ProgressBarControl progressBarControl;
        private SimpleButton cancelProgress;
        private LabelControl statusProgess;

        private void InitializeInteface()
        {
            FormProgressImport = new XtraProgressImport();

            progressBarControl = FormProgressImport.Controls.OfType<ProgressBarControl>().FirstOrDefault();
            statusProgess = FormProgressImport.Controls.OfType<LabelControl>().FirstOrDefault();
            cancelProgress = FormProgressImport.Controls.OfType<SimpleButton>().FirstOrDefault();

            progressBarControl.Properties.ShowTitle = true;
            progressBarControl.Properties.Step = 1;
            progressBarControl.Properties.PercentView = true;
            progressBarControl.Properties.Minimum = 0;

            FormProgressImport.Show();
        }

        private async void ExecutarMedicao_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            InitializeInteface();

            var progress = new Progress<ImportProgressReport>(value =>
            {
                progressBarControl.Properties.Maximum = value.TotalRows;
                statusProgess.Text = value.MessageImport;

                if (value.CurrentRow > 0)
                    progressBarControl.PerformStep();

                progressBarControl.Update();
                statusProgess.Update();
            });

            var calculator = new Calculator.CalculoSpool(View.ObjectSpace);

            await Task.Run(() =>
                calculator.ExecutarCalculo(progress));

            View.ObjectSpace.Refresh();
            FormProgressImport.Close();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
