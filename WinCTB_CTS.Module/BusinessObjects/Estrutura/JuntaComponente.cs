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
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes;

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura
{
    [DefaultClassOptions, DefaultProperty("ConcatComponenteJunta"), ImageName("BO_Contract"), NavigationItem("Estrutura")]
    [Indices("Componente;Junta")]
    public class JuntaComponente : BaseObject
    {
        public JuntaComponente(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        private DateTime? posDf2;
        private DateTime? posDf1;
        private double percRt;
        private double percUt;
        private double percLpPm;
        private string statusJunta;
        private string statusUs;
        private double comprimentoReparoUs;
        private string inspetorUs;
        private string relatorioUs;
        private DateTime? dataUs;
        private string sampleUs;
        private string statusRx;
        private double comprimentoReparoRx;
        private string inspetorRx;
        private string relatorioRx;
        private DateTime? dataRx;
        private string sampleRx;
        private string statusPm;
        private string inspetorPm;
        private string relatorioPm;
        private DateTime? dataPm;
        private string statusLp;
        private string inspetorLp;
        private string relatorioLp;
        private DateTime? dataLP;
        private string sampleMp;
        private string statusVisualSolda;
        private string inspetorVisualSolda;
        private string relatorioVisualSolda;
        private DateTime? dataVisual;
        private string statusSolda;
        private string inspetorSoldagem;
        private string relatorioSolda;
        private string wps;
        private string consumiveis;
        private string soldadores;
        private DateTime? dataSolda;
        private string statusFitup;
        private string inspFitup;
        private string relatorioFitup;
        private DateTime? dataFitup;
        private DateTime? posiocionamento;
        private string posicaoDf2;
        private string tipoDf2;
        private double esp2;
        private string mat2;
        private string df2;
        private string posicaoDf1;
        private string tipoDf1;
        private double esp1;
        private string mat1;
        private string df1;
        private string classeInspecao;
        private double comprimento;
        private string site;
        private string tipoJunta;
        private string junta;
        private Componente componente;

        [Association("Componente-JuntaComponentes")]
        public Componente Componente
        {
            get => componente;
            set => SetPropertyValue(nameof(Componente), ref componente, value);
        }

        [Size(100)]
        public string Junta
        {
            get => junta;
            set => SetPropertyValue(nameof(Junta), ref junta, value);
        }

        [XafDisplayName("Componente-Junta")]
        [PersistentAlias("Concat(Componente,'-',Junta)")]
        public string ConcatComponenteJunta => (string)EvaluateAlias("ConcatComponenteJunta");


        [Size(100), XafDisplayName("Tipo de Junta")]
        public string TipoJunta
        {
            get => tipoJunta;
            set => SetPropertyValue(nameof(TipoJunta), ref tipoJunta, value);
        }

        [Size(100)]
        public string Site
        {
            get => site;
            set => SetPropertyValue(nameof(Site), ref site, value);
        }

        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double Comprimento
        {
            get => comprimento;
            set => SetPropertyValue(nameof(Comprimento), ref comprimento, value);
        }

        [Size(100), XafDisplayName("Classe de Inspeção")]
        public string ClasseInspecao
        {
            get => classeInspecao;
            set => SetPropertyValue(nameof(ClasseInspecao), ref classeInspecao, value);
        }

        [Size(100), XafDisplayName("DF1")]
        public string Df1
        {
            get => df1;
            set => SetPropertyValue(nameof(Df1), ref df1, value);
        }

        [Size(100), XafDisplayName("MAT_1")]
        public string Mat1
        {
            get => mat1;
            set => SetPropertyValue(nameof(Mat1), ref mat1, value);
        }

        [XafDisplayName("ESP_1")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double Esp1
        {
            get => esp1;
            set => SetPropertyValue(nameof(Esp1), ref esp1, value);
        }

        [Size(100), XafDisplayName("Tipo DF1"), ToolTip("Categora do Desenho 1")]
        public string TipoDf1
        {
            get => tipoDf1;
            set => SetPropertyValue(nameof(TipoDf1), ref tipoDf1, value);
        }

        [Size(100), XafDisplayName("Posição DF1"), ToolTip("Tipo de Estrutura do Desnho 1")]
        public string PosicaoDf1
        {
            get => posicaoDf1;
            set => SetPropertyValue(nameof(PosicaoDf1), ref posicaoDf1, value);
        }

        [Size(100), XafDisplayName("DF2")]
        public string Df2
        {
            get => df2;
            set => SetPropertyValue(nameof(Df2), ref df2, value);
        }

        [Size(100), XafDisplayName("MAT_2")]
        public string Mat2
        {
            get => mat2;
            set => SetPropertyValue(nameof(Mat2), ref mat2, value);
        }

        [XafDisplayName("ESP_2")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double Esp2
        {
            get => esp2;
            set => SetPropertyValue(nameof(Esp2), ref esp2, value);
        }

        [Size(100), XafDisplayName("Tipo do DF2"), ToolTip("Categora do Desenho 2")]
        public string TipoDf2
        {
            get => tipoDf2;
            set => SetPropertyValue(nameof(TipoDf2), ref tipoDf2, value);
        }

        [Size(100), XafDisplayName("Posição DF2"), ToolTip("Tipo de Estrutura do Desnho 2")]
        public string PosicaoDf2
        {
            get => posicaoDf2;
            set => SetPropertyValue(nameof(PosicaoDf2), ref posicaoDf2, value);
        }

        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? Posiocionamento
        {
            get => posiocionamento;
            set => SetPropertyValue(nameof(Posiocionamento), ref posiocionamento, value);
        }

        [XafDisplayName("Data do Fitup")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataFitup
        {
            get => dataFitup;
            set => SetPropertyValue(nameof(DataFitup), ref dataFitup, value);
        }

        [Size(100), XafDisplayName("Relatório de Fitup")]
        public string RelatorioFitup
        {
            get => relatorioFitup;
            set => SetPropertyValue(nameof(RelatorioFitup), ref relatorioFitup, value);
        }

        [Size(100), XafDisplayName("Inspetor de Fitup")]
        public string InspFitup
        {
            get => inspFitup;
            set => SetPropertyValue(nameof(InspFitup), ref inspFitup, value);
        }

        [Size(100), XafDisplayName("Status do Fitup")]
        public string StatusFitup
        {
            get => statusFitup;
            set => SetPropertyValue(nameof(StatusFitup), ref statusFitup, value);
        }

        [XafDisplayName("Data de Soldagem")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataSolda
        {
            get => dataSolda;
            set => SetPropertyValue(nameof(DataSolda), ref dataSolda, value);
        }

        [Size(100)]
        public string Soldadores
        {
            get => soldadores;
            set => SetPropertyValue(nameof(Soldadores), ref soldadores, value);
        }

        [Size(100), XafDisplayName("Consumíveis")]
        public string Consumiveis
        {
            get => consumiveis;
            set => SetPropertyValue(nameof(Consumiveis), ref consumiveis, value);
        }

        [Size(100), XafDisplayName("WPS")]
        public string Wps
        {
            get => wps;
            set => SetPropertyValue(nameof(Wps), ref wps, value);
        }

        [Size(100), XafDisplayName("Relatório de Soldagem")]
        public string RelatorioSolda
        {
            get => relatorioSolda;
            set => SetPropertyValue(nameof(RelatorioSolda), ref relatorioSolda, value);
        }

        [Size(100), XafDisplayName("Inspetor de Soldagem")]
        public string InspetorSoldagem
        {
            get => inspetorSoldagem;
            set => SetPropertyValue(nameof(InspetorSoldagem), ref inspetorSoldagem, value);
        }

        [Size(100), XafDisplayName("Status de Solda")]
        public string StatusSolda
        {
            get => statusSolda;
            set => SetPropertyValue(nameof(StatusSolda), ref statusSolda, value);
        }

        [XafDisplayName("Data de Vidual de Solda")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataVisual
        {
            get => dataVisual;
            set => SetPropertyValue(nameof(DataVisual), ref dataVisual, value);
        }

        [Size(100), XafDisplayName("Relatório de Visual de Solda")]
        public string RelatorioVisualSolda
        {
            get => relatorioVisualSolda;
            set => SetPropertyValue(nameof(RelatorioVisualSolda), ref relatorioVisualSolda, value);
        }

        [Size(100), XafDisplayName("Inspetor de Visual de Solda")]
        public string InspetorVisualSolda
        {
            get => inspetorVisualSolda;
            set => SetPropertyValue(nameof(InspetorVisualSolda), ref inspetorVisualSolda, value);
        }


        [Size(100), XafDisplayName("Status Visual de Solda")]
        public string StatusVisualSolda
        {
            get => statusVisualSolda;
            set => SetPropertyValue(nameof(StatusVisualSolda), ref statusVisualSolda, value);
        }

        [Size(100), XafDisplayName("SAMPLE_MP")]
        public string SampleMp
        {
            get => sampleMp;
            set => SetPropertyValue(nameof(SampleMp), ref sampleMp, value);
        }

        [XafDisplayName("Data LP")]
        public DateTime? DataLP
        {
            get => dataLP;
            set => SetPropertyValue(nameof(DataLP), ref dataLP, value);
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

        [Size(100), XafDisplayName("Status de LP")]
        public string StatusLp
        {
            get => statusLp;
            set => SetPropertyValue(nameof(StatusLp), ref statusLp, value);
        }

        [XafDisplayName("Data PM")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataPm
        {
            get => dataPm;
            set => SetPropertyValue(nameof(DataPm), ref dataPm, value);
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

        [Size(100), XafDisplayName("Status de PM")]
        public string StatusPm
        {
            get => statusPm;
            set => SetPropertyValue(nameof(StatusPm), ref statusPm, value);
        }

        [Size(100), XafDisplayName("SAMPLE_RX")]
        public string SampleRx
        {
            get => sampleRx;
            set => SetPropertyValue(nameof(SampleRx), ref sampleRx, value);
        }

        [XafDisplayName("Data do RX")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataRx
        {
            get => dataRx;
            set => SetPropertyValue(nameof(DataRx), ref dataRx, value);
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

        [XafDisplayName("Comprimento do Reparo")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double ComprimentoReparoRx
        {
            get => comprimentoReparoRx;
            set => SetPropertyValue(nameof(ComprimentoReparoRx), ref comprimentoReparoRx, value);
        }

        [Size(100), XafDisplayName("Status do RX")]
        public string StatusRx
        {
            get => statusRx;
            set => SetPropertyValue(nameof(StatusRx), ref statusRx, value);
        }

        [Size(100), XafDisplayName("SAMPLE_US")]
        public string SampleUs
        {
            get => sampleUs;
            set => SetPropertyValue(nameof(SampleUs), ref sampleUs, value);
        }

        [XafDisplayName("Data do US")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataUs
        {
            get => dataUs;
            set => SetPropertyValue(nameof(DataUs), ref dataUs, value);
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

        [XafDisplayName("Comprimento de Reparo de US")]
        public double ComprimentoReparoUs
        {
            get => comprimentoReparoUs;
            set => SetPropertyValue(nameof(ComprimentoReparoUs), ref comprimentoReparoUs, value);
        }

        public double PercLpPm
        {
            get => percLpPm;
            set => SetPropertyValue(nameof(PercLpPm), ref percLpPm, value);
        }

        [Size(100), XafDisplayName("Status de US")]
        public string StatusUs
        {
            get => statusUs;
            set => SetPropertyValue(nameof(StatusUs), ref statusUs, value);
        }

        [Size(100), XafDisplayName("Status da Junta")]
        public string StatusJunta
        {
            get => statusJunta;
            set => SetPropertyValue(nameof(StatusJunta), ref statusJunta, value);
        }

        public double PercUt
        {
            get => percUt;
            set => SetPropertyValue(nameof(PercUt), ref percUt, value);
        }

        public double PercRt
        {
            get => percRt;
            set => SetPropertyValue(nameof(PercRt), ref percRt, value);
        }

        [XafDisplayName("Posicionamento Df1")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? PosDf1
        {
            get => posDf1;
            set => SetPropertyValue(nameof(PosDf1), ref posDf1, value);
        }

        [XafDisplayName("Posicionamento Df2")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? PosDf2
        {
            get => posDf2;
            set => SetPropertyValue(nameof(PosDf2), ref posDf2, value);
        }


        #region Lotes
        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false)]
        [Association("JuntaComponente-LoteLPPMJuntaEstruturas")]
        public XPCollection<LoteLPPMJuntaEstrutura> LoteLPPMJuntaEstruturas
            => GetCollection<LoteLPPMJuntaEstrutura>(nameof(LoteLPPMJuntaEstruturas));

        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false)]
        [Association("JuntaComponente-LoteRXJuntaEstruturas")]
        public XPCollection<LoteRXJuntaEstrutura> LoteRXJuntaEstruturas
            => GetCollection<LoteRXJuntaEstrutura>(nameof(LoteRXJuntaEstruturas));

        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false)]
        [Association("JuntaComponente-LoteUSJuntaEstruturas")]
        public XPCollection<LoteUSJuntaEstrutura> LoteUSJuntaEstruturas
            => GetCollection<LoteUSJuntaEstrutura>(nameof(LoteUSJuntaEstruturas));
        #endregion



    }
}