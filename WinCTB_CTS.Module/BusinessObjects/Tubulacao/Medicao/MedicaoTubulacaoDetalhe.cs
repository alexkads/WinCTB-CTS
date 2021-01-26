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

namespace WinCTB_CTS.Module.BusinessObjects.Tubulacao.Medicao
{
    //[DefaultClassOptions]
    public class MedicaoTubulacaoDetalhe : BaseObject
    { 
        public MedicaoTubulacaoDetalhe(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }


        MedicaoTubulacao medicaoTubulacao;
        private double avancoDeMontagem;
        private double avancoDeFabricacao;
        private int quantidadeDeJuntaMontagem;
        private int quantidadeDeJuntaFabricacao;
        private Spool spool;

        [Association("Spool-MedicaoTubulacaoDetalhes")]
        public Spool Spool
        {
            get => spool;
            set => SetPropertyValue(nameof(Spool), ref spool, value);
        }

        [XafDisplayName("Quantidade de Junta Fabricação")]
        public int QuantidadeDeJuntaFabricacao
        {
            get => quantidadeDeJuntaFabricacao;
            set => SetPropertyValue(nameof(QuantidadeDeJuntaFabricacao), ref quantidadeDeJuntaFabricacao, value);
        }

        public int QuantidadeDeJuntaMontagem
        {
            get => quantidadeDeJuntaMontagem;
            set => SetPropertyValue(nameof(QuantidadeDeJuntaMontagem), ref quantidadeDeJuntaMontagem, value);
        }

        [XafDisplayName("Acanço de Fabricação")]
        public double AvancoDeFabricacao
        {
            get => avancoDeFabricacao;
            set => SetPropertyValue(nameof(AvancoDeFabricacao), ref avancoDeFabricacao, value);
        }


        [XafDisplayName("Avanço de Montagem")]
        public double AvancoDeMontagem
        {
            get => avancoDeMontagem;
            set => SetPropertyValue(nameof(AvancoDeMontagem), ref avancoDeMontagem, value);
        }

        
        [Association("MedicaoTubulacao-MedicaoTubulacaoDetalhes")]
        public MedicaoTubulacao MedicaoTubulacao
        {
            get => medicaoTubulacao;
            set => SetPropertyValue(nameof(MedicaoTubulacao), ref medicaoTubulacao, value);
        }
    }
}