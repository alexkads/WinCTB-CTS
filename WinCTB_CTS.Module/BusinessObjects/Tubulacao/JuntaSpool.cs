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
    public class JuntaSpool : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public JuntaSpool(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        string consumivelAcab;
        string consumivelEnch;
        string consumivelRaiz;
        DateTime dataSoldagem;
        string eps;
        string executanteResold;
        string relatorioSoldagem;
        string soldadorAcab;
        string soldadorEnch;
        string soldadorRaiz;
        string statusResold;
        DateTime dataVa;
        string executanteVa;
        string relatorioVa;
        string statusVa;
        string campoPipe;
        string norma;
        string classeInspecao;
        string materialEn;
        string materialPt;
        string spec;
        string tipoJunta;
        string schedule;
        double diametroMilimetro;
        static double espessura;
        string diametroPolegada;
        string junta;
        string linha;
        string documento;
        string sth;
        string sop;
        string progFab;
        string campoAuxiliar;
        string arranjoFisico;
        string site;
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

        [Size(100), XafDisplayName("Di_Pol")]
        public string DiametroPolegada
        {
            get => diametroPolegada;
            set => SetPropertyValue(nameof(DiametroPolegada), ref diametroPolegada, value);
        }
        [XafDisplayName("Diâmetro em Milímetros")]
        public double DiametroMilimetro
        {
            get => diametroMilimetro;
            set => SetPropertyValue(nameof(DiametroMilimetro), ref diametroMilimetro, value);
        }

        [XafDisplayName("Espessura em 'mm'")]
        public static double Relatoriod
        {
            get => espessura;
            set => SetPropertyValue(nameof(Relatoriod), ref espessura, value);
        }

        [Size(100)]
        public string Schedule
        {
            get => schedule;
            set => SetPropertyValue(nameof(Schedule), ref schedule, value);
        }

        [Size(100), XafDisplayName("Tipo de Junta")]
        public string TipoJunta
        {
            get => tipoJunta;
            set => SetPropertyValue(nameof(TipoJunta), ref tipoJunta, value);
        }

        [Size(100)]
        public string Spec
        {
            get => spec;
            set => SetPropertyValue(nameof(Spec), ref spec, value);
        }

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

        [Size(100)]
        public string Norma
        {
            get => norma;
            set => SetPropertyValue(nameof(Norma), ref norma, value);
        }

        [Size(100), XafDisplayName("Campo ou Pipe")]
        public string CampoPipe
        {
            get => campoPipe;
            set => SetPropertyValue(nameof(CampoPipe), ref campoPipe, value);
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
        public DateTime DataVa
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
        public DateTime DataSoldagem
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














    }
}