using System;
using System.Text;
using System.Linq;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.ReportsV2;
using WinCTB_CTS.Module.RelatorioParametros;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao;
using WinCTB_CTS.Module.RelatorioPreDefinido;
using DevExpress.Data.Filtering;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao;

namespace WinCTB_CTS.Module
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
    public sealed partial class WinCTB_CTSModule : ModuleBase
    {
        public WinCTB_CTSModule()
        {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
        }

        public static void RegisterEnum()
        {
            EnumProcessingHelper.RegisterEnum(typeof(WinCTB_CTS.Module.BusinessObjects.Tubulacao.JuntaSpool.CampoPipe), "CampoOuPipe");

        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);

            PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);

            predefinedReportsUpdater.AddPredefinedReport<RelatorioMedicaoAnaliticoSpool>("Relatório de Medição por Spool (Analítico)", typeof(Spool), typeof(SpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioMedicaoSinteticoSpool>("Relatório de Medição por Spool (Sintético)", typeof(Spool), typeof(SpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<ResumoDeJuntas>("Resumo de Juntas", typeof(JuntaSpool), typeof(JuntaSpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<JuntaMedicao>("Juntas - Medição", typeof(JuntaSpool), typeof(JuntaSpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<SpoolMedicao>("Spool - Medição", typeof(MedicaoTubulacaoDetalhe), typeof(MedicaoTubulacaoDetalheParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<ComponentesMedicao>("STR - Componentes - (MONTAGEM)", typeof(ComponentesMedicao), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<JuntaComponentes>("STR - Mapa de Juntas (MONTAGEM)", typeof(JuntaComponentes), isInplaceReport: true);


            return new ModuleUpdater[]
            {
                updater,
                predefinedReportsUpdater
            };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            // Manage various aspects of the application UI and behavior at the module level.
        }
        public override void CustomizeTypesInfo(ITypesInfo typesInfo)
        {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }
    }
}
