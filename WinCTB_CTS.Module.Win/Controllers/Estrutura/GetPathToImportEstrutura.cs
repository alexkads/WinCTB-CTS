//using DevExpress.Data.Filtering;
//using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Actions;
//using DevExpress.ExpressApp.Editors;
//using DevExpress.ExpressApp.Layout;
//using DevExpress.ExpressApp.Model.NodeGenerators;
//using DevExpress.ExpressApp.SystemModule;
//using DevExpress.ExpressApp.Templates;
//using DevExpress.ExpressApp.Utils;
//using DevExpress.Persistent.Base;
//using DevExpress.Persistent.Validation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using WinCTB_CTS.Module.Importer.Estrutura;
//using WinCTB_CTS.Module.Win.Services;

//namespace WinCTB_CTS.Module.Win.Controllers.Estrutura
//{
//    public partial class GetPathToImportEstrutura : ViewController
//    {
//        private ParametrosImportComponentEJunta currentObject;
//        public GetPathToImportEstrutura()
//        {
//            TargetViewType = ViewType.DetailView;
//            TargetObjectType = typeof(ParametrosImportComponentEJunta);

//            SimpleAction GetPathToImportEstruturaAction = new SimpleAction(this, "GetPathToImportEstruturaAction", "DetailViewActions");
//            GetPathToImportEstruturaAction.Caption = "Informar Arquivo";
//            GetPathToImportEstruturaAction.ImageName = "Import";
//            GetPathToImportEstruturaAction.Execute += GetPathToImportEstruturaActionn_Execute;
//        }

//        private void GetPathToImportEstruturaActionn_Execute(object sender, SimpleActionExecuteEventArgs e)
//        {
//            using (OpenFileDialog dialog = new OpenFileDialog())
//            {
//                dialog.CheckFileExists = true;
//                dialog.CheckPathExists = true;
//                dialog.DereferenceLinks = true;
//                dialog.Multiselect = false;
//                //dialog.Filter = "ver como é o filtro";
//                if (dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK)
//                {
//                    currentObject.PathFileForImport = dialog.FileName;
//                    RegisterWindowsManipulation.SetRegister("PathFileForImportEstrutura", dialog.FileName);
//                }
//            }
           
//        }

//        protected override void OnViewControlsCreated()
//        {
//            base.OnViewControlsCreated();
//            currentObject = (ParametrosImportComponentEJunta)View.CurrentObject;
//        }        
//    }
//}
