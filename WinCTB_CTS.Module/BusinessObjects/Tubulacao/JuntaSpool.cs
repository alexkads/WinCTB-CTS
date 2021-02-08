using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao
{
    [DefaultClassOptions, DefaultProperty("Junta"), ImageName("BO_Contract"), NavigationItem("Tubulação")]
    [Indices("Spool;Junta")]
    public class JuntaSpool : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public JuntaSpool(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            //this.Evaluate("[<TabSchedule>][ PipingClass = ^.TabPercInspecao.Spec ].Single()");
        }

        public enum CampoPipe
        {
            PIPE = 0,
            CAMPO = 1,
        }


        TabSchedule tabSchedule;
        private TabPercInspecao tabPercInspecao;
        private TabDiametro tabDiametro;
        private string relDimFab;
        private string relRastMaterial;
        private string situacaoJunta;
        private DateTime? dataLiberacaoJunta;
        private string loteLp;
        private string loteRx;
        private string progFabJunta;
        private DateTime? dataEstanqueidade;
        private string inspetorEstanqueidade;
        private string relatorioEstanqueidade;
        private string statusEstanqueidade;
        private DateTime? dataFerrita;
        private string inspetorFerrita;
        private string relatorioFerrita;
        private string statusFerrita;
        private DateTime? dataUs;
        private string inspetorRx;
        private string inspetorUs;
        private string relatorioUs;
        private DateTime? dataRx;
        private string relatorioRx;
        private string progRx;
        private string statusRxUs;
        private DateTime? dataIdLiga;
        private string inspetorIdLiga;
        private string relIdLiga;
        private string statusIdLiga;
        private DateTime? dataRastMaterial;
        private string executanteRastMaterial;
        private string statusRastMaterial;
        private DateTime? dataDureza;
        private string inspetorDureza;
        private string relatorioDureza;
        private string statusDureza;
        private DateTime? dataTt;
        private string relatorioTt;
        private string statusTt;
        private DateTime? dataPm;
        private string inspetorPm;
        private string relatorioPm;
        private DateTime? dataLp;
        private string inspetorLp;
        private string relatorioLp;
        private string statusLpPm;
        private DateTime? dataVisualSolda;
        private string inspetorVS;
        private string relatorioVs;
        private string visualStatus;
        private double espessura;
        private string consumivelAcab;
        private string consumivelEnch;
        private string consumivelRaiz;
        private DateTime? dataSoldagem;
        private string eps;
        private string executanteResold;
        private string relatorioSoldagem;
        private string soldadorAcab;
        private string soldadorEnch;
        private string soldadorRaiz;
        private string statusResold;
        private DateTime? dataVa;
        private string executanteVa;
        private string relatorioVa;
        private string statusVa;
        private CampoPipe campoOuPipe;
        private string norma;
        private string classeInspecao;
        private string materialEn;
        private string materialPt;
        private string tipoJunta;
        private string junta;
        private string linha;
        private string documento;
        private string sth;
        private string sop;
        private string progFab;
        private string campoAuxiliar;
        private string arranjoFisico;
        private string site;
        private Spool spool;


        [Size(100)]
        public string Site
        {
            get => site;
            set => SetPropertyValue(nameof(Site), ref site, value);
        }

        [Size(100), XafDisplayName("Arranjo Físico")]
        public string ArranjoFisico
        {
            get => arranjoFisico;
            set => SetPropertyValue(nameof(ArranjoFisico), ref arranjoFisico, value);
        }

        [Size(100), XafDisplayName("Campo Auxiliar")]
        public string CampoAuxiliar
        {
            get => campoAuxiliar;
            set => SetPropertyValue(nameof(CampoAuxiliar), ref campoAuxiliar, value);
        }

        [Size(100), XafDisplayName("Prog Fab")]
        public string ProgFab
        {
            get => progFab;
            set => SetPropertyValue(nameof(ProgFab), ref progFab, value);
        }

        [Size(100)]
        public string Sop
        {
            get => sop;
            set => SetPropertyValue(nameof(Sop), ref sop, value);
        }

        [Size(100), XafDisplayName("STH")]
        public string Sth
        {
            get => sth;
            set => SetPropertyValue(nameof(Sth), ref sth, value);
        }

        [Size(100)]
        public string Documento
        {
            get => documento;
            set => SetPropertyValue(nameof(Documento), ref documento, value);
        }

        [Size(100)]
        public string Linha
        {
            get => linha;
            set => SetPropertyValue(nameof(Linha), ref linha, value);
        }

        [Association("Spool-JuntaSpools")]
        public Spool Spool
        {
            get => spool;
            set => SetPropertyValue(nameof(Spool), ref spool, value);
        }

        [Size(100)]
        public string Junta
        {
            get => junta;
            set => SetPropertyValue(nameof(Junta), ref junta, value);
        }

        [XafDisplayName("Espessura em 'mm'")]

        public double Espessura
        {
            get => espessura;
            set => SetPropertyValue(nameof(Espessura), ref espessura, value);
        }

        [Size(100), XafDisplayName("Tipo de Junta")]
        public string TipoJunta
        {
            get => tipoJunta;
            set => SetPropertyValue(nameof(TipoJunta), ref tipoJunta, value);
        }

        [VisibleInListView(false)]
        [Association("TabPercInspecao-JuntaSpools")]
        public TabPercInspecao TabPercInspecao
        {
            get => tabPercInspecao;
            set => SetPropertyValue(nameof(TabPercInspecao), ref tabPercInspecao, value);
        }

        [XafDisplayName("Percentual de Inspeção")]
        [PersistentAlias("TabPercInspecao.PercentualDeInspecao")]
        public double PercentualDeInspecao => Convert.ToDouble(EvaluateAlias("PercentualDeInspecao"));

        [XafDisplayName("Spec")]
        [PersistentAlias("TabPercInspecao.Spec")]
        public string Spec => (string)EvaluateAlias("Spec");

        [XafDisplayName("Wdi")]
        [PersistentAlias("TabDiametro.Wdi")]
        public double Wdi => Convert.ToDouble(EvaluateAlias("Wdi"));

        [Size(100), XafDisplayName("Material")]
        public string MaterialPt
        {
            get => materialPt;
            set => SetPropertyValue(nameof(MaterialPt), ref materialPt, value);
        }

        [Size(100), XafDisplayName("Material EN")]
        public string MaterialEn
        {
            get => materialEn;
            set => SetPropertyValue(nameof(MaterialEn), ref materialEn, value);
        }

        [Size(100), XafDisplayName("Classe de Inspeção")]
        public string ClasseInspecao
        {
            get => classeInspecao;
            set => SetPropertyValue(nameof(ClasseInspecao), ref classeInspecao, value);
        }

        [VisibleInListView(false)]
        [Association("TabDiametro-JuntaSpools")]
        public TabDiametro TabDiametro
        {
            get => tabDiametro;
            set => SetPropertyValue(nameof(TabDiametro), ref tabDiametro, value);
        }

       
        [VisibleInListView(false)]
        public TabSchedule TabSchedule
        {
            get => tabSchedule;
            set => SetPropertyValue(nameof(TabSchedule), ref tabSchedule, value);
        }

        [Size(100)]
        public string Norma
        {
            get => norma;
            set => SetPropertyValue(nameof(Norma), ref norma, value);
        }


        [RuleRequiredField]
        [XafDisplayName("Campo/Pipe")]
        public CampoPipe CampoOuPipe
        {
            get
            {
                return campoOuPipe;
            }
            set
            {
                SetPropertyValue("CampoOuPipe", ref campoOuPipe, value);
            }
        }

        [Size(100), XafDisplayName("Status de VA")]
        public string StatusVa
        {
            get => statusVa;
            set => SetPropertyValue(nameof(StatusVa), ref statusVa, value);
        }

        [Size(100), XafDisplayName("Relatório de VA")]
        public string RelatorioVa
        {
            get => relatorioVa;
            set => SetPropertyValue(nameof(RelatorioVa), ref relatorioVa, value);
        }

        [Size(100), XafDisplayName("Executante de VA")]
        public string ExecutanteVa
        {
            get => executanteVa;
            set => SetPropertyValue(nameof(ExecutanteVa), ref executanteVa, value);
        }
        [XafDisplayName("Data de VA")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataVa
        {
            get => dataVa;
            set => SetPropertyValue(nameof(DataVa), ref dataVa, value);
        }

        [Size(100), XafDisplayName("Status de Resold")]
        public string StatusResold
        {
            get => statusResold;
            set => SetPropertyValue(nameof(StatusResold), ref statusResold, value);
        }

        [Size(100), XafDisplayName("Soldador de Raiz")]
        public string SoldadorRaiz
        {
            get => soldadorRaiz;
            set => SetPropertyValue(nameof(SoldadorRaiz), ref soldadorRaiz, value);
        }

        [Size(100), XafDisplayName("Soldador de Enchimento")]
        public string SoldadorEnch
        {
            get => soldadorEnch;
            set => SetPropertyValue(nameof(SoldadorEnch), ref soldadorEnch, value);
        }

        [Size(100), XafDisplayName("Soldador de Acabamento")]
        public string SoldadorAcab
        {
            get => soldadorAcab;
            set => SetPropertyValue(nameof(SoldadorAcab), ref soldadorAcab, value);
        }

        [Size(100), XafDisplayName("Relatorio de Soldagem")]
        public string RelatorioSoldagem
        {
            get => relatorioSoldagem;
            set => SetPropertyValue(nameof(RelatorioSoldagem), ref relatorioSoldagem, value);
        }

        [Size(100), XafDisplayName("Executante de Resold")]
        public string ExecutanteResold
        {
            get => executanteResold;
            set => SetPropertyValue(nameof(ExecutanteResold), ref executanteResold, value);
        }

        [Size(100), XafDisplayName("EPS / IEIS")]
        public string Eps
        {
            get => eps;
            set => SetPropertyValue(nameof(Eps), ref eps, value);
        }

        [XafDisplayName("Data de Soldagem")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataSoldagem
        {
            get => dataSoldagem;
            set => SetPropertyValue(nameof(DataSoldagem), ref dataSoldagem, value);
        }

        [Size(100), XafDisplayName("Consumível de Raiz")]
        public string ConsumivelRaiz
        {
            get => consumivelRaiz;
            set => SetPropertyValue(nameof(ConsumivelRaiz), ref consumivelRaiz, value);
        }

        [Size(100), XafDisplayName("Consumível de Enchimento")]
        public string ConsumivelEnch
        {
            get => consumivelEnch;
            set => SetPropertyValue(nameof(ConsumivelEnch), ref consumivelEnch, value);
        }

        [Size(100), XafDisplayName("Consumível de Acabamento")]
        public string ConsumivelAcab
        {
            get => consumivelAcab;
            set => SetPropertyValue(nameof(ConsumivelAcab), ref consumivelAcab, value);
        }

        [Size(100), XafDisplayName("Statatus de Visual de Solda")]
        public string VisualStatus
        {
            get => visualStatus;
            set => SetPropertyValue(nameof(VisualStatus), ref visualStatus, value);
        }

        [Size(100), XafDisplayName("Relatório de Visual de Solda")]
        public string RelatorioVs
        {
            get => relatorioVs;
            set => SetPropertyValue(nameof(RelatorioVs), ref relatorioVs, value);
        }

        [Size(100), XafDisplayName("Inspetor de Visual de Solda")]
        public string InspetorVS
        {
            get => inspetorVS;
            set => SetPropertyValue(nameof(InspetorVS), ref inspetorVS, value);
        }

        [XafDisplayName("Data de Visual de Solda")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataVisualSolda
        {
            get => dataVisualSolda;
            set => SetPropertyValue(nameof(DataVisualSolda), ref dataVisualSolda, value);
        }

        [Size(100), XafDisplayName("Status LP/PM")]
        public string StatusLpPm
        {
            get => statusLpPm;
            set => SetPropertyValue(nameof(StatusLpPm), ref statusLpPm, value);
        }

        [Size(100), XafDisplayName("Relatório de LP")]
        public string RelatorioLp
        {
            get => relatorioLp;
            set => SetPropertyValue(nameof(RelatorioLp), ref relatorioLp, value);
        }

        [Size(100), XafDisplayName("Inspetor de LP")]
        public string InspetorLp
        {
            get => inspetorLp;
            set => SetPropertyValue(nameof(InspetorLp), ref inspetorLp, value);
        }

        [XafDisplayName("Data do LP")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataLp
        {
            get => dataLp;
            set => SetPropertyValue(nameof(DataLp), ref dataLp, value);
        }

        [Size(100), XafDisplayName("Relatório de PM")]
        public string RelatorioPm
        {
            get => relatorioPm;
            set => SetPropertyValue(nameof(RelatorioPm), ref relatorioPm, value);
        }

        [Size(100), XafDisplayName("Inspetor de PM")]
        public string InspetorPm
        {
            get => inspetorPm;
            set => SetPropertyValue(nameof(InspetorPm), ref inspetorPm, value);
        }

        [XafDisplayName("Data do PM")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataPm
        {
            get => dataPm;
            set => SetPropertyValue(nameof(DataPm), ref dataPm, value);
        }

        [Size(100), XafDisplayName("Status do Tratamento Térmico")]
        public string StatusTt
        {
            get => statusTt;
            set => SetPropertyValue(nameof(StatusTt), ref statusTt, value);
        }

        [Size(100), XafDisplayName("Relatório do Tratamento Térmico")]
        public string RelatorioTt
        {
            get => relatorioTt;
            set => SetPropertyValue(nameof(RelatorioTt), ref relatorioTt, value);
        }

        [XafDisplayName("Data do Tratamento Térmico")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataTt
        {
            get => dataTt;
            set => SetPropertyValue(nameof(DataTt), ref dataTt, value);
        }

        [Size(100), XafDisplayName("Status de Dureza")]
        public string StatusDureza
        {
            get => statusDureza;
            set => SetPropertyValue(nameof(StatusDureza), ref statusDureza, value);
        }

        [Size(100), XafDisplayName("Relatório de Dureza")]
        public string RelatorioDureza
        {
            get => relatorioDureza;
            set => SetPropertyValue(nameof(RelatorioDureza), ref relatorioDureza, value);
        }

        [Size(100), XafDisplayName("Inspetor de Dureza")]
        public string InspetorDureza
        {
            get => inspetorDureza;
            set => SetPropertyValue(nameof(InspetorDureza), ref inspetorDureza, value);
        }

        [XafDisplayName("Data de Dureza")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataDureza
        {
            get => dataDureza;
            set => SetPropertyValue(nameof(DataDureza), ref dataDureza, value);
        }

        [Size(100), XafDisplayName("Status Rastreabilidade de Materiais")]
        public string StatusRastMaterial
        {
            get => statusRastMaterial;
            set => SetPropertyValue(nameof(StatusRastMaterial), ref statusRastMaterial, value);
        }

        [Size(100), XafDisplayName("Relatório de Rastreabilidade de Material")]
        public string RelRastMaterial
        {
            get => relRastMaterial;
            set => SetPropertyValue(nameof(RelRastMaterial), ref relRastMaterial, value);
        }

        [Size(100), XafDisplayName("Executante Rastreabilidade de Material")]
        public string ExecutanteRastMaterial
        {
            get => executanteRastMaterial;
            set => SetPropertyValue(nameof(ExecutanteRastMaterial), ref executanteRastMaterial, value);
        }

        [XafDisplayName("Data de Rastreabilidade de Materiais")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataRastMaterial
        {
            get => dataRastMaterial;
            set => SetPropertyValue(nameof(DataRastMaterial), ref dataRastMaterial, value);
        }

        [Size(100), XafDisplayName("Status Identificação de Ligas")]
        public string StatusIdLiga
        {
            get => statusIdLiga;
            set => SetPropertyValue(nameof(StatusIdLiga), ref statusIdLiga, value);
        }

        [Size(100), XafDisplayName("Relatório de Identificação de Ligas")]
        public string RelIdLiga
        {
            get => relIdLiga;
            set => SetPropertyValue(nameof(RelIdLiga), ref relIdLiga, value);
        }

        [Size(100), XafDisplayName("Inspetor Reconhecimento de Ligas")]
        public string InspetorIdLiga
        {
            get => inspetorIdLiga;
            set => SetPropertyValue(nameof(InspetorIdLiga), ref inspetorIdLiga, value);
        }

        [XafDisplayName("Data de Reconhecimento de Ligas")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataIdLiga
        {
            get => dataIdLiga;
            set => SetPropertyValue(nameof(DataIdLiga), ref dataIdLiga, value);
        }

        [Size(100), XafDisplayName("Status RX/US")]
        public string StatusRxUs
        {
            get => statusRxUs;
            set => SetPropertyValue(nameof(StatusRxUs), ref statusRxUs, value);
        }

        [Size(100), XafDisplayName("Programação de RX")]
        public string ProgRx
        {
            get => progRx;
            set => SetPropertyValue(nameof(ProgRx), ref progRx, value);
        }

        [Size(100), XafDisplayName("Relatório de RX")]
        public string RelatorioRx
        {
            get => relatorioRx;
            set => SetPropertyValue(nameof(RelatorioRx), ref relatorioRx, value);
        }

        [Size(100), XafDisplayName("Inspetor de RX")]
        public string InspetorRx
        {
            get => inspetorRx;
            set => SetPropertyValue(nameof(InspetorRx), ref inspetorRx, value);
        }

        [XafDisplayName("Data do RX")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataRx
        {
            get => dataRx;
            set => SetPropertyValue(nameof(DataRx), ref dataRx, value);
        }

        [Size(100), XafDisplayName("Relatório de US")]
        public string RelatorioUs
        {
            get => relatorioUs;
            set => SetPropertyValue(nameof(RelatorioUs), ref relatorioUs, value);
        }

        [Size(100), XafDisplayName("Inspetor de US")]
        public string InspetorUs
        {
            get => inspetorUs;
            set => SetPropertyValue(nameof(InspetorUs), ref inspetorUs, value);
        }
        [XafDisplayName("Data do US")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]

        public DateTime? DataUs
        {
            get => dataUs;
            set => SetPropertyValue(nameof(DataUs), ref dataUs, value);
        }

        [Size(100), XafDisplayName("Status de Ferrita")]
        public string StatusFerrita
        {
            get => statusFerrita;
            set => SetPropertyValue(nameof(StatusFerrita), ref statusFerrita, value);
        }

        [Size(100), XafDisplayName("Relatório de Ferrita")]
        public string RelatorioFerrita
        {
            get => relatorioFerrita;
            set => SetPropertyValue(nameof(RelatorioFerrita), ref relatorioFerrita, value);
        }

        [Size(100), XafDisplayName("Inspetor de Ferrita")]
        public string InspetorFerrita
        {
            get => inspetorFerrita;
            set => SetPropertyValue(nameof(InspetorFerrita), ref inspetorFerrita, value);
        }
        [XafDisplayName("Data da Ferrita")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]

        public DateTime? DataFerrita
        {
            get => dataFerrita;
            set => SetPropertyValue(nameof(DataFerrita), ref dataFerrita, value);
        }

        [Size(100), XafDisplayName("Status de Estanqueidade")]
        public string StatusEstanqueidade
        {
            get => statusEstanqueidade;
            set => SetPropertyValue(nameof(StatusEstanqueidade), ref statusEstanqueidade, value);
        }

        [Size(100), XafDisplayName("Relatório de Estanqueidade")]
        public string RelatorioEstanqueidade
        {
            get => relatorioEstanqueidade;
            set => SetPropertyValue(nameof(RelatorioEstanqueidade), ref relatorioEstanqueidade, value);
        }

        [Size(100), XafDisplayName("Inspetor de Estanqueidade")]
        public string InspetorEstanqueidade
        {
            get => inspetorEstanqueidade;
            set => SetPropertyValue(nameof(InspetorEstanqueidade), ref inspetorEstanqueidade, value);
        }

        [XafDisplayName("Data de Estanqueidade")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataEstanqueidade
        {
            get => dataEstanqueidade;
            set => SetPropertyValue(nameof(DataEstanqueidade), ref dataEstanqueidade, value);
        }

        [Size(100), XafDisplayName("Relatório de Dimensional de Fabricação")]
        public string RelDimFab
        {
            get => relDimFab;
            set => SetPropertyValue(nameof(RelDimFab), ref relDimFab, value);
        }


        [Size(100), XafDisplayName("Programação de Fabricação da Junta")]
        public string ProgFabJunta
        {
            get => progFabJunta;
            set => SetPropertyValue(nameof(ProgFabJunta), ref progFabJunta, value);
        }

        [Size(100), XafDisplayName("Lote de RX")]
        public string LoteRx
        {
            get => loteRx;
            set => SetPropertyValue(nameof(LoteRx), ref loteRx, value);
        }

        [Size(100), XafDisplayName("Lote de LP")]
        public string LoteLp
        {
            get => loteLp;
            set => SetPropertyValue(nameof(LoteLp), ref loteLp, value);
        }

        [XafDisplayName("Data de Liberação da Junta")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataLiberacaoJunta
        {
            get => dataLiberacaoJunta;
            set => SetPropertyValue(nameof(DataLiberacaoJunta), ref dataLiberacaoJunta, value);
        }

        [Size(100), XafDisplayName("Situação da Junta")]
        public string SituacaoJunta
        {
            get => situacaoJunta;
            set => SetPropertyValue(nameof(SituacaoJunta), ref situacaoJunta, value);
        }
    }
}