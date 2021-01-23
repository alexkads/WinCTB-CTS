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

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao
{
    [DefaultClassOptions]
    public class Spool : BaseObject
    {
        public Spool(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }



        string progPintura;
        string elevacao;
        string progMontagem;
        DateTime dataPintFab;
        string relPiRevUnico;
        DateTime dataPiRevUnico;
        string relIndPintAcabamento;
        string relPintAcabamento;
        string inspPintAcabamento;
        DateTime dataPiAcabamento;
        string relIndIntermediaria;
        string relPiIntermediaria;
        string tagSpool;
        string inspPiIntermediaria;
        string inspPintIntermediaria;
        DateTime dataPiIntermediaria;
        string relIndFundo;
        string relatorioPinFundo;
        string inspPinturaFundo;
        DateTime dataPiFundo;
        DateTime dataEndFab;
        string inspetorDf;
        string relatorioDf;
        DateTime dataDfFab;
        DateTime dataVsFab;
        DateTime dataSoldaFab;
        DateTime dataVaFab;
        DateTime dataCorte;
        DateTime dataProgFab;
        string nrProgFab;
        DateTime dataCadastro;
        int totaldeJuntasPipe;
        int totaldeJuntas;
        int quantidadeIsolamento;
        string espIsolamento;
        string area;
        double pesoFabricacao;
        double comprimento;
        string condicaoPintura;
        string tipoIsolamento;
        string fluido;
        string pNumber;
        string espec;
        double espessura;
        string diametroPolegada;
        double diametro;
        string norma;
        string mat;
        string revIso;
        string revSpool;
        string isometrico;
        string siteFabricante;
        string linha;
        string sth;
        string areaFisica;
        string subSop;
        private string campoAuxiliar;
        private string documento;
        private string arranjoFisico;
        private string contrato;

        [Size(100)]
        public string Contrato
        {
            get => contrato;
            set => SetPropertyValue(nameof(Contrato), ref contrato, value);
        }


        [Size(100), XafDisplayName("Arranjo Físico")]
        public string ArranjoFisico
        {
            get => arranjoFisico;
            set => SetPropertyValue(nameof(ArranjoFisico), ref arranjoFisico, value);
        }

        [Size(100)]
        public string Documento
        {
            get => documento;
            set => SetPropertyValue(nameof(Documento), ref documento, value);
        }

        [Size(100)]
        public string CampoAuxiliar
        {
            get => campoAuxiliar;
            set => SetPropertyValue(nameof(CampoAuxiliar), ref campoAuxiliar, value);
        }

        [Size(100)]
        public string SubSop
        {
            get => subSop;
            set => SetPropertyValue(nameof(SubSop), ref subSop, value);
        }

        [Size(100), XafDisplayName("Área Física")]
        public string AreaFisica
        {
            get => areaFisica;
            set => SetPropertyValue(nameof(AreaFisica), ref areaFisica, value);
        }

        [Size(100), XafDisplayName("STH")]
        public string Sth
        {
            get => sth;
            set => SetPropertyValue(nameof(Sth), ref sth, value);
        }

        [Size(100)]
        public string Linha
        {
            get => linha;
            set => SetPropertyValue(nameof(Linha), ref linha, value);
        }

        [Size(100), XafDisplayName("Site Fabricante")]
        public string SiteFabricante
        {
            get => siteFabricante;
            set => SetPropertyValue(nameof(SiteFabricante), ref siteFabricante, value);
        }

        [Size(100), XafDisplayName("Isométrico")]
        [RuleRequiredField]
        public string Isometrico
        {
            get => isometrico;
            set => SetPropertyValue(nameof(Isometrico), ref isometrico, value);
        }

        [Size(100), XafDisplayName("Spool"), RuleRequiredField]
        public string TagSpool
        {
            get => tagSpool;
            set => SetPropertyValue(nameof(TagSpool), ref tagSpool, value);
        }

        [Size(100), XafDisplayName("Rev. Spool"), ToolTip("Revisão do Spool")]
        public string RevSpool
        {
            get => revSpool;
            set => SetPropertyValue(nameof(RevSpool), ref revSpool, value);
        }

        [Size(100), XafDisplayName("Rev. Iso"), ToolTip("Revisão do Isométrico")]
        public string RevIso
        {
            get => revIso;
            set => SetPropertyValue(nameof(RevIso), ref revIso, value);
        }

        [Size(100), XafDisplayName("Mat"), ToolTip("Mateial do Spool")]
        public string Material
        {
            get => mat;
            set => SetPropertyValue(nameof(Material), ref mat, value);
        }

        [Size(100)]
        public string Norma
        {
            get => norma;
            set => SetPropertyValue(nameof(Norma), ref norma, value);
        }

        [XafDisplayName("Diâmetro"), ToolTip("Diâmetro em Polegada exibido em decimal")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double Diametro
        {
            get => diametro;
            set => SetPropertyValue(nameof(Diametro), ref diametro, value);
        }

        [Size(100), XafDisplayName("Di_Pol"), ToolTip("Diâmetro em Polegada")]
        public string DiametroPolegada
        {
            get => diametroPolegada;
            set => SetPropertyValue(nameof(DiametroPolegada), ref diametroPolegada, value);
        }

        [ToolTip("Espessura em 'mm'")]
        [ModelDefault("DisplayFormat", "n3"), ModelDefault("EditMask", "n3")]
        public double Espessura
        {
            get => espessura;
            set => SetPropertyValue(nameof(Espessura), ref espessura, value);
        }

        [Size(100)]
        public string Espec
        {
            get => espec;
            set => SetPropertyValue(nameof(Espec), ref espec, value);
        }

        [Size(100)]
        public string PNumber
        {
            get => pNumber;
            set => SetPropertyValue(nameof(PNumber), ref pNumber, value);
        }

        [Size(100)]
        public string Fluido
        {
            get => fluido;
            set => SetPropertyValue(nameof(Fluido), ref fluido, value);
        }

        [Size(100), XafDisplayName("Tipo de Isolamento")]
        public string TipoIsolamento
        {
            get => tipoIsolamento;
            set => SetPropertyValue(nameof(TipoIsolamento), ref tipoIsolamento, value);
        }

        [Size(100), XafDisplayName("Condição de Pintura")]
        public string CondicaoPintura
        {
            get => condicaoPintura;
            set => SetPropertyValue(nameof(CondicaoPintura), ref condicaoPintura, value);
        }

        [XafDisplayName("Comprimento de Solda")]

        public double Comprimento
        {
            get => comprimento;
            set => SetPropertyValue(nameof(Comprimento), ref comprimento, value);
        }

        [XafDisplayName("Peso de Fabricação"), ToolTip("Peso dos Componentes de Fabricação")]
        [RuleRange(DefaultContexts.Save, 0.001, 500000)]
        public double PesoFabricacao
        {
            get => pesoFabricacao;
            set => SetPropertyValue(nameof(PesoFabricacao), ref pesoFabricacao, value);
        }

        [Size(100), XafDisplayName("Área")]
        public string Area
        {
            get => area;
            set => SetPropertyValue(nameof(Area), ref area, value);
        }

        [Size(100), XafDisplayName("Esp IT")]
        public string EspIsolamento
        {
            get => espIsolamento;
            set => SetPropertyValue(nameof(EspIsolamento), ref espIsolamento, value);
        }

        [XafDisplayName("Qtd IT")]
        public int QuantidadeIsolamento
        {
            get => quantidadeIsolamento;
            set => SetPropertyValue(nameof(QuantidadeIsolamento), ref quantidadeIsolamento, value);
        }

        [XafDisplayName("Total de Juntas")]
        public int TotaldeJuntas
        {
            get => totaldeJuntas;
            set => SetPropertyValue(nameof(TotaldeJuntas), ref totaldeJuntas, value);
        }

        [XafDisplayName("Total de Juntas Pipe")]
        public int TotaldeJuntasPipe
        {
            get => totaldeJuntasPipe;
            set => SetPropertyValue(nameof(TotaldeJuntasPipe), ref totaldeJuntasPipe, value);
        }

        [XafDisplayName("Data de Cadastro")]
        public DateTime DataCadastro
        {
            get => dataCadastro;
            set => SetPropertyValue(nameof(DataCadastro), ref dataCadastro, value);
        }

        [Size(100), XafDisplayName("Nr Programação de Fabricação")]
        public string NrProgFab
        {
            get => nrProgFab;
            set => SetPropertyValue(nameof(NrProgFab), ref nrProgFab, value);
        }

        [XafDisplayName("Data Programação de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataProgFab
        {
            get => dataProgFab;
            set => SetPropertyValue(nameof(DataProgFab), ref dataProgFab, value);
        }

        [XafDisplayName("Data de Corte")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataCorte
        {
            get => dataCorte;
            set => SetPropertyValue(nameof(DataCorte), ref dataCorte, value);
        }

        [XafDisplayName("Data de VA de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataVaFab
        {
            get => dataVaFab;
            set => SetPropertyValue(nameof(DataVaFab), ref dataVaFab, value);
        }

        [XafDisplayName("Data de Solda de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataSoldaFab
        {
            get => dataSoldaFab;
            set => SetPropertyValue(nameof(DataSoldaFab), ref dataSoldaFab, value);
        }

        [XafDisplayName("Data de Visual de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataVsFab
        {
            get => dataVsFab;
            set => SetPropertyValue(nameof(DataVsFab), ref dataVsFab, value);
        }

        [XafDisplayName("Data de Dimensional de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataDfFab
        {
            get => dataDfFab;
            set => SetPropertyValue(nameof(DataDfFab), ref dataDfFab, value);
        }

        [Size(100), XafDisplayName("Relatório DF")]
        public string RelatorioDf
        {
            get => relatorioDf;
            set => SetPropertyValue(nameof(RelatorioDf), ref relatorioDf, value);
        }

        [Size(100), XafDisplayName("Inspetor DF")]
        public string InspetorDf
        {
            get => inspetorDf;
            set => SetPropertyValue(nameof(InspetorDf), ref inspetorDf, value);
        }

        [XafDisplayName("Data END de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataEndFab
        {
            get => dataEndFab;
            set => SetPropertyValue(nameof(DataEndFab), ref dataEndFab, value);
        }

        [XafDisplayName("Data de Pintura de Fundo")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataPiFundo
        {
            get => dataPiFundo;
            set => SetPropertyValue(nameof(DataPiFundo), ref dataPiFundo, value);
        }

        [Size(100), XafDisplayName("Inspetor Pintura de Fundo")]
        public string InspPinturaFundo
        {
            get => inspPinturaFundo;
            set => SetPropertyValue(nameof(InspPinturaFundo), ref inspPinturaFundo, value);
        }

        [Size(100), XafDisplayName("Relatório de Pintura de Fundo")]
        public string RelatorioPinFundo
        {
            get => relatorioPinFundo;
            set => SetPropertyValue(nameof(RelatorioPinFundo), ref relatorioPinFundo, value);
        }

        [Size(100), XafDisplayName("Relatório Individual de Fundo")]
        public string RelIndFundo
        {
            get => relIndFundo;
            set => SetPropertyValue(nameof(RelIndFundo), ref relIndFundo, value);
        }

        [XafDisplayName("Data de Pintura Intermediária")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataPiIntermediaria
        {
            get => dataPiIntermediaria;
            set => SetPropertyValue(nameof(DataPiIntermediaria), ref dataPiIntermediaria, value);
        }


        [Size(100), XafDisplayName("Inspetor de Pintura Intermediária")]
        public string InspPiIntermediaria
        {
            get => inspPiIntermediaria;
            set => SetPropertyValue(nameof(InspPiIntermediaria), ref inspPiIntermediaria, value);
        }

        [Size(100), XafDisplayName("Relatório de Pintura Intermediária")]
        public string RelPiIntermediaria
        {
            get => relPiIntermediaria;
            set => SetPropertyValue(nameof(RelPiIntermediaria), ref relPiIntermediaria, value);
        }

        [Size(100), XafDisplayName("Relatório Individual Pintura Intermeriária")]
        public string RelIndIntermediaria
        {
            get => relIndIntermediaria;
            set => SetPropertyValue(nameof(RelIndIntermediaria), ref relIndIntermediaria, value);
        }

        [XafDisplayName("Data Pintura de Acabamento")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        public DateTime DataPiAcabamento
        {
            get => dataPiAcabamento;
            set => SetPropertyValue(nameof(DataPiAcabamento), ref dataPiAcabamento, value);
        }

        [Size(100), XafDisplayName("Inspetor de Pintura de Acabamento")]
        public string InspPintAcabamento
        {
            get => inspPintAcabamento;
            set => SetPropertyValue(nameof(InspPintAcabamento), ref inspPintAcabamento, value);
        }

        [Size(100), XafDisplayName("Relatório de Pintura de Acabamento")]
        public string RelPintAcabamento
        {
            get => relPintAcabamento;
            set => SetPropertyValue(nameof(RelPintAcabamento), ref relPintAcabamento, value);
        }

        [Size(100), XafDisplayName("Relatório Individual Pintura de Acabamento")]
        public string RelIndPintAcabamento
        {
            get => relIndPintAcabamento;
            set => SetPropertyValue(nameof(RelIndPintAcabamento), ref relIndPintAcabamento, value);
        }

        [XafDisplayName("Data PI Rev Unico")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]

        public DateTime DataPiRevUnico
        {
            get => dataPiRevUnico;
            set => SetPropertyValue(nameof(DataPiRevUnico), ref dataPiRevUnico, value);
        }

        [Size(100), XafDisplayName("Relatório Pintura Rev. Unico")]

        public string RelPiRevUnico
        {
            get => relPiRevUnico;
            set => SetPropertyValue(nameof(RelPiRevUnico), ref relPiRevUnico, value);
        }

        [XafDisplayName("Data de Pintura de Fabricação")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]

        public DateTime DataPintFab
        {
            get => dataPintFab;
            set => SetPropertyValue(nameof(DataPintFab), ref dataPintFab, value);
        }

        [Size(100), XafDisplayName("Programação de Montagem - Semana")]
        public string ProgMontagem
        {
            get => progMontagem;
            set => SetPropertyValue(nameof(ProgMontagem), ref progMontagem, value);
        }

        [Size(100), XafDisplayName("Elevação")]
        public string Elevacao
        {
            get => elevacao;
            set => SetPropertyValue(nameof(Elevacao), ref elevacao, value);
        }
        
        [Size(100), XafDisplayName("Programação de Pintura")]
        public string ProgPintura
        {
            get => progPintura;
            set => SetPropertyValue(nameof(ProgPintura), ref progPintura, value);
        }




        [Association("Spool-Juntas")]
        public XPCollection<Junta> Juntas
        {
            get
            {
                return GetCollection<Junta>(nameof(Juntas));
            }
        }


    }
}