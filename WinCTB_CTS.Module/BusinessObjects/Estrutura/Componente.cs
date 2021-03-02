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

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura
{
    [DefaultClassOptions, DefaultProperty("Peca"), ImageName("BO_Contract"), NavigationItem("Estrutura")]
    [Indices("DesenhoMontagem;Peca")]
    public class Componente : BaseObject
    { 
        public Componente(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



        private string statusPeca;
        private DateTime? dataPintura;
        private string inspPintura;
        private string relPintura;
        private string progPintura;
        private string relatorioDimensional;
        private DateTime? dataDimensional;
        private DateTime? dataPosicionamento;
        private string progNdt;
        private string progWeld;
        private string progFitup;
        private DateTime? dataRecebimento;
        private string relatorioRecebimento;
        private double areaPintura;
        private double pesoTotal;
        private string elevacao;
        private string dwg;
        private string posicao;
        private string tipoEstrutura;
        private string revisao;
        private string peca;
        private string transmital;
        private string desenhoMontagem;
        private string documentoReferencia;
        private string modulo;

        [Size(100), XafDisplayName("Módulo")]
        public string Modulo
        {
            get => modulo;
            set => SetPropertyValue(nameof(Modulo), ref modulo, value);
        }


        [Size(100), XafDisplayName("Documento de Referência")]
        public string DocumentoReferencia
        {
            get => documentoReferencia;
            set => SetPropertyValue(nameof(DocumentoReferencia), ref documentoReferencia, value);
        }

        [Size(100), XafDisplayName("Desenho de Montagem")]
        public string DesenhoMontagem
        {
            get => desenhoMontagem;
            set => SetPropertyValue(nameof(DesenhoMontagem), ref desenhoMontagem, value);
        }

        [Size(100)]
        public string Transmital
        {
            get => transmital;
            set => SetPropertyValue(nameof(Transmital), ref transmital, value);
        }

        [Size(100), XafDisplayName("Peça")]
        public string Peca
        {
            get => peca;
            set => SetPropertyValue(nameof(Peca), ref peca, value);
        }

        [Size(100), XafDisplayName("Revisão")]
        public string Revisao
        {
            get => revisao;
            set => SetPropertyValue(nameof(Revisao), ref revisao, value);
        }

        [Size(100), XafDisplayName("Tipo de Estrutura")]
        public string TipoEstrutura
        {
            get => tipoEstrutura;
            set => SetPropertyValue(nameof(TipoEstrutura), ref tipoEstrutura, value);
        }

        [Size(100), XafDisplayName("Posição")]
        public string Posicao
        {
            get => posicao;
            set => SetPropertyValue(nameof(Posicao), ref posicao, value);
        }

        [Size(100), XafDisplayName("DWG")]
        public string Dwg
        {
            get => dwg;
            set => SetPropertyValue(nameof(Dwg), ref dwg, value);
        }

        [Size(100), XafDisplayName("Elevação")]
        public string Elevacao
        {
            get => elevacao;
            set => SetPropertyValue(nameof(Elevacao), ref elevacao, value);
        }

        [XafDisplayName("Peso Total")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double PesoTotal
        {
            get => pesoTotal;
            set => SetPropertyValue(nameof(PesoTotal), ref pesoTotal, value);
        }

        [XafDisplayName("Área de Pintura")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double AreaPintura
        {
            get => areaPintura;
            set => SetPropertyValue(nameof(AreaPintura), ref areaPintura, value);
        }

        [Size(100), XafDisplayName("Relatório de Recebimento")]
        public string RelatorioRecebimento
        {
            get => relatorioRecebimento;
            set => SetPropertyValue(nameof(RelatorioRecebimento), ref relatorioRecebimento, value);
        }

        [XafDisplayName("Data de Recebimento")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataRecebimento
        {
            get => dataRecebimento;
            set => SetPropertyValue(nameof(DataRecebimento), ref dataRecebimento, value);
        }

        [Size(100), XafDisplayName("Programação de Fitup")]
        public string ProgFitup
        {
            get => progFitup;
            set => SetPropertyValue(nameof(ProgFitup), ref progFitup, value);
        }

        [Size(100), XafDisplayName("Programação de Solda")]
        public string ProgWeld
        {
            get => progWeld;
            set => SetPropertyValue(nameof(ProgWeld), ref progWeld, value);
        }

        [Size(100), XafDisplayName("Programação de END")]
        public string ProgNdt
        {
            get => progNdt;
            set => SetPropertyValue(nameof(ProgNdt), ref progNdt, value);
        }

        [XafDisplayName("Data de Posicionamento")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataPosicionamento
        {
            get => dataPosicionamento;
            set => SetPropertyValue(nameof(DataPosicionamento), ref dataPosicionamento, value);
        }

        [XafDisplayName("Data do Dimensional")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataDimensional
        {
            get => dataDimensional;
            set => SetPropertyValue(nameof(DataDimensional), ref dataDimensional, value);
        }

        [Size(100), XafDisplayName("Relatório Dimensional")]
        public string RelatorioDimensional
        {
            get => relatorioDimensional;
            set => SetPropertyValue(nameof(RelatorioDimensional), ref relatorioDimensional, value);
        }

        [Size(100), XafDisplayName("Programação de Pintura")]
        public string ProgPintura
        {
            get => progPintura;
            set => SetPropertyValue(nameof(ProgPintura), ref progPintura, value);
        }

        [Size(100), XafDisplayName("Relatório de Pintura")]
        public string RelPintura
        {
            get => relPintura;
            set => SetPropertyValue(nameof(RelPintura), ref relPintura, value);
        }

        [Size(100), XafDisplayName("Inspetor de Pintura")]
        public string InspPintura
        {
            get => inspPintura;
            set => SetPropertyValue(nameof(InspPintura), ref inspPintura, value);
        }

        [XafDisplayName("Data de Pintura")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime? DataPintura
        {
            get => dataPintura;
            set => SetPropertyValue(nameof(DataPintura), ref dataPintura, value);
        }
        
        [Size(100), XafDisplayName("Status da Peça")]
        public string StatusPeca
        {
            get => statusPeca;
            set => SetPropertyValue(nameof(StatusPeca), ref statusPeca, value);
        }



        [Association("Componente-JuntaComponentes")]
        public XPCollection<JuntaComponente> JuntaComponentes
        {
            get
            {
                return GetCollection<JuntaComponente>(nameof(JuntaComponentes));
            }
        }
    }
}