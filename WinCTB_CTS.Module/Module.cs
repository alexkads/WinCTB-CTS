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
using WinCTB_CTS.Module.Interfaces;
using WinCTB_CTS.Module.Comum.ViewCloner;
using WinCTB_CTS.Module.BusinessObjects.Estrutura;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Medicao;
//using WinCTB_CTS.Module.Action;

namespace WinCTB_CTS.Module {
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppModuleBasetopic.aspx.
    public sealed partial class WinCTB_CTSModule : ModuleBase {
        public WinCTB_CTSModule() {
            InitializeComponent();
            BaseObject.OidInitializationMode = OidInitializationMode.AfterConstruction;
            RegisterEnum();
        }

        public static void RegisterEnum() {
            EnumProcessingHelper.RegisterEnum(typeof(WinCTB_CTS.Module.BusinessObjects.Tubulacao.JuntaSpool.CampoPipe), "CampoOuPipe");
            EnumProcessingHelper.RegisterEnum(typeof(WinCTB_CTS.Module.Interfaces.ENDS), nameof(ENDS));
            EnumProcessingHelper.RegisterEnum(typeof(WinCTB_CTS.Module.Interfaces.SituacoesInspecao), nameof(SituacoesInspecao));
            EnumProcessingHelper.RegisterEnum(typeof(WinCTB_CTS.Module.Interfaces.SituacoesQuantidade), nameof(SituacoesQuantidade));
        }

        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList moduleTypeList = base.GetRequiredModuleTypesCore();
            moduleTypeList.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
            moduleTypeList.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            return moduleTypeList;
        }

        public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
            base.AddGeneratorUpdaters(updaters);
            updaters.Add(new ModelViewClonerUpdater());
            //updaters.Add(new CustomDetailViewItemsGenarator());
            //updaters.Add(new CustomModelViewsUpdater());
            //updaters.Add(new CustomBOModelUpdater());
            //updaters.Add(new CustomBOModelMemberUpdater());
            //updaters.Add(new CustomDetailViewLayoutGenarator());
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);

            PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);

            //predefinedReportsUpdater.AddPredefinedReport<RelatorioMedicaoAnaliticoSpool>("Relatório de Medição por Spool (Analítico)", typeof(Spool), typeof(SpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioMedicaoSinteticoSpool>("PIPE - SINTÉTICO SPOOL", typeof(MedicaoTubulacaoDetalhe), typeof(MedicaoTubulacaoDetalheParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioMedicaoSinteticoComponente>("STRUCTURE - SINTÉTICO COMPONENTE", typeof(MedicaoEstruturaDetalhe), typeof(MedicaoEstruturaDetalheParameters), isInplaceReport: true);

            predefinedReportsUpdater.AddPredefinedReport<RelatorioResumoDeJuntas>("Resumo de Juntas", typeof(JuntaSpool), typeof(JuntaSpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioJuntaMedicao>("PIPE - Juntas Medição", typeof(JuntaSpool), typeof(JuntaSpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioSpoolMedicao>("PIPE - Spool Medição", typeof(MedicaoTubulacaoDetalhe), typeof(MedicaoTubulacaoDetalheParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioSpool>("Relatório de Spool", typeof(Spool), typeof(SpoolParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioComponentes>("Relatório de Componentes - (MONTAGEM)", typeof(Componente), typeof(ComponenteParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioComponentesMedicao>("STR - Componentes - (MONTAGEM)", typeof(MedicaoEstruturaDetalhe), typeof(MedicaoEstruturaDetalheParameters), isInplaceReport: true);
            predefinedReportsUpdater.AddPredefinedReport<RelatorioJuntaComponentes>("STR - Mapa de Juntas (MONTAGEM)", typeof(JuntaComponente), typeof(JuntaComponenteParameters), isInplaceReport: true);


            return new ModuleUpdater[]
            {
                updater,
                predefinedReportsUpdater
            };
        }
        public override void Setup(XafApplication application) {
            base.Setup(application);
            application.OptimizedControllersCreation = true;
            // Manage various aspects of the application UI and behavior at the module level.
        }

        public override void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            base.ExtendModelInterfaces(extenders);
            //extenders.Add<IModelMember, IModelMemberExtender>();
            //extenders.Add<IModelPropertyEditor, IModelPropertyEditorClassMemberExtender>();
            //extenders.Add<IModelLayoutGroup, IModelLayoutGroupExtender>();
        }


        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
            //CustomizeAppearanceDemoObject(typesInfo.FindTypeInfo(typeof(FeatureCenter.Module.ConditionalAppearance.ConditionalAppearanceHideShowEditorsObject)));
            //ITypeInfo typeInfo = typesInfo.FindTypeInfo(typeof(FeatureCenter.Module.Messages.Messages));
            //typeInfo.FindMember("Win").AddAttribute(new ExpandObjectMembersAttribute(ExpandObjectMembers.InDetailView));
            //typeInfo.FindMember("Web").AddAttribute(new ExpandObjectMembersAttribute(ExpandObjectMembers.InDetailView));
        }
    }
}
