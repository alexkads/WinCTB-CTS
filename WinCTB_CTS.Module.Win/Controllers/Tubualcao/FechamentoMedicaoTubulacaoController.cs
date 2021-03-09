//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.Persistent.Base;
//using DevExpress.XtraEditors;
//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using WinCTB_CTS.Module.ServiceProcess.Base;

//namespace WinCTB_CTS.Module.Win.Controllers
//{
//    public partial class FechamentoMedicaoTubulacaoController : WindowController
//    {
//        IObjectSpace objectSpace = null;
//        public FechamentoMedicaoTubulacaoController()
//        {
//            TargetWindowType = WindowType.Main;
//            SimpleAction ExecutarMedicao = new SimpleAction(this, "FechamentoMedicaoTubulacaoSimpleActionController", PredefinedCategory.RecordEdit)
//            {
//                Caption = "Fechar Medição de Tubulação",
//                ImageName = "WeightedPies"
//            };

//            ExecutarMedicao.Execute += ExecutarMedicao_Execute;
//        }

//        private XtraForm FormProgressImport;
//        private ProgressBarControl progressBarControl;
//        protected SimpleButton cancelProgress;
//        private LabelControl statusProgess;

//        private void InitializeInteface()
//        {
//            FormProgressImport = new XtraProgressImport();

//            progressBarControl = FormProgressImport.Controls.OfType<ProgressBarControl>().FirstOrDefault();
//            statusProgess = FormProgressImport.Controls.OfType<LabelControl>().FirstOrDefault();
//            cancelProgress = FormProgressImport.Controls.OfType<SimpleButton>().FirstOrDefault();

//            progressBarControl.Properties.ShowTitle = true;
//            progressBarControl.Properties.Step = 1;
//            progressBarControl.Properties.PercentView = true;
//            progressBarControl.Properties.Minimum = 0;

//            FormProgressImport.Show();
//        }

//        private async void ExecutarMedicao_Execute(object sender, SimpleActionExecuteEventArgs e)
//        {
//            InitializeInteface();
//            objectSpace = Application.CreateObjectSpace();

//            var progress = new Progress<ImportProgressReport>(value =>
//            {
//                progressBarControl.Properties.Maximum = value.TotalRows;
//                statusProgess.Text = value.MessageImport;

//                if (value.CurrentRow > 0)
//                    progressBarControl.PerformStep();

//                progressBarControl.Update();
//                statusProgess.Update();
//            });

//            var calculator = new Calculator.CalculoSpool(objectSpace);

//            await Task.Run(() =>
//                calculator.ExecutarCalculo(progress));

//            objectSpace.Dispose();
//            FormProgressImport.Close();
//        }

//        protected override void OnActivated()
//        {
//            base.OnActivated();
//        }
//        protected override void OnDeactivated()
//        {
//            base.OnDeactivated();
//        }
//    }
//}
