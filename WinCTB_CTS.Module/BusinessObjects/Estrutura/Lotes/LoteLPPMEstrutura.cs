using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using WinCTB_CTS.Module.BusinessObjects.Comum;
using WinCTB_CTS.Module.Interfaces;

namespace WinCTB_CTS.Module.BusinessObjects.Estrutura.Lotes
{
    [DefaultProperty(nameof(NumeroDoLote)), Persistent("WT_LoteLPPMEstrutura"), ImageName("BO_Contract"), NavigationItem("Lote")]
    [Appearance("", AppearanceItemType = nameof(Action), TargetItems = "CloneObject, New", Visibility = ViewItemVisibility.Hide)]
    [XafDisplayName("Lote de LP/PM Estrutura")]
    [FriendlyKeyProperty(nameof(NumeroDoLote))]
    public class LoteLPPMEstrutura : XPBaseObject, ILote
    {
        private string _Area;
        private bool _ComJuntaReprovada;
        private int _ExcessoDeInspecao;
        private Guid _GuidAreaEstrutura;
        private Guid _guidDesenhoDeDetalhamento;
        private DateTime _InicioDoCicloDoLote;
        private int _JuntasNoLote;
        private int _NecessidadeDeInspecao;
        private string _NumeroDoDesenho;
        private string _NumeroDoLote;
        private double _percentualNivelDeInspecao;

        // Fields...
        private int _QuantidadeInspecionada;
        private int _QuantidadeNecessaria;
        private SituacoesInspecao _SituacaoInspecao;
        private SituacoesQuantidade _SituacaoQuantidade;
        private DateTime _TerminoDoCicloDoLote;

        public LoteLPPMEstrutura(Session session)
            : base(session)
        {
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();
            new HashSet<LoteLPPMJuntaEstrutura>(LoteLPPMjuntaEstruturas)
                .ToObservable(Scheduler.CurrentThread)
                .Subscribe(juntasDoLote =>
                {
                    Session.Delete(juntasDoLote);
                }, (ex) => new InvalidOperationException($"Erro na exlusão do registro {ex.Message}"));
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            NumeroDoLote = "LO" + DistributedIdGeneratorHelper.Generate(this.Session.DataLayer, this.GetType().FullName, string.Empty).ToString().PadLeft(5, '0');
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Área")]
        public string Area
        {
            get => _Area;
            set => SetPropertyValue(nameof(Area), ref _Area, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Com Junta Reprovada")]
        public bool ComJuntaReprovada
        {
            get => _ComJuntaReprovada;
            set => SetPropertyValue(nameof(ComJuntaReprovada), ref _ComJuntaReprovada, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Excesso De Inspeção")]
        public int ExcessoDeInspecao
        {
            get => _ExcessoDeInspecao;
            set => SetPropertyValue(nameof(ExcessoDeInspecao), ref _ExcessoDeInspecao, value);
        }

        [VisibleInListView(false)]
        [Indexed(Unique = false)]
        [VisibleInDetailViewAttribute(false)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Guid Area Estrutura")]
        public Guid GuidAreaEstrutura
        {
            get => _GuidAreaEstrutura;
            set => SetPropertyValue(nameof(GuidAreaEstrutura), ref _GuidAreaEstrutura, value);
        }

        [VisibleInListView(false)]
        [Indexed(Unique = false)]
        [VisibleInDetailViewAttribute(false)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Guid Desenho de Detalhamento")]
        public Guid GuidDesenhoDeDetalhamento
        {
            get => _guidDesenhoDeDetalhamento;
            set => SetPropertyValue(nameof(GuidDesenhoDeDetalhamento), ref _guidDesenhoDeDetalhamento, value);
        }


        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Início do Ciclo do Lote")]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        public DateTime InicioDoCicloDoLote
        {
            get => _InicioDoCicloDoLote;
            set => SetPropertyValue(nameof(InicioDoCicloDoLote), ref _InicioDoCicloDoLote, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Juntas no Lote")]
        public int JuntasNoLote
        {
            get => _JuntasNoLote;
            set => SetPropertyValue(nameof(JuntasNoLote), ref _JuntasNoLote, value);
        }

        [Association("LoteLPPMEstrutura-LoteLPPMjuntaEstruturas"), DevExpress.Xpo.Aggregated]
        public XPCollection<LoteLPPMJuntaEstrutura> LoteLPPMjuntaEstruturas => GetCollection<LoteLPPMJuntaEstrutura>(nameof(LoteLPPMjuntaEstruturas));

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Necessidade De Inspeção")]
        public int NecessidadeDeInspecao
        {
            get => _NecessidadeDeInspecao;
            set => SetPropertyValue(nameof(NecessidadeDeInspecao), ref _NecessidadeDeInspecao, value);
        }


        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Número do Desenho")]
        public string NumeroDoDesenho
        {
            get => _NumeroDoDesenho;
            set => SetPropertyValue(nameof(NumeroDoDesenho), ref _NumeroDoDesenho, value);
        }

        [Key(false)]
        [VisibleInListView(true)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Número do Lote")]
        public string NumeroDoLote
        {
            get => _NumeroDoLote;
            set => SetPropertyValue(nameof(NumeroDoLote), ref _NumeroDoLote, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [ModelDefault("DisplayFormat", "P0")]
        [XafDisplayName("% Inspeção")]
        public double PercentualNivelDeInspecao
        {
            get => _percentualNivelDeInspecao;
            set => SetPropertyValue(nameof(PercentualNivelDeInspecao), ref _percentualNivelDeInspecao, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Quantidade Inspecionada")]
        public int QuantidadeInspecionada
        {
            get => _QuantidadeInspecionada;
            set => SetPropertyValue(nameof(QuantidadeInspecionada), ref _QuantidadeInspecionada, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Quantidade Necessária")]
        public int QuantidadeNecessaria
        {
            get => _QuantidadeNecessaria;
            set => SetPropertyValue("NecessidadeDeInpecaoPrevista", ref _QuantidadeNecessaria, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Situação Inspeção")]
        public SituacoesInspecao SituacaoInspecao
        {
            get => _SituacaoInspecao;
            set => SetPropertyValue(nameof(SituacaoInspecao), ref _SituacaoInspecao, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [Indexed(Unique = false)]
        [XafDisplayName("Situação Quantidade")]
        public SituacoesQuantidade SituacaoQuantidade
        {
            get => _SituacaoQuantidade;
            set => SetPropertyValue(nameof(SituacaoQuantidade), ref _SituacaoQuantidade, value);
        }

        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Término do Ciclo do Lote")]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        public DateTime TerminoDoCicloDoLote
        {
            get => _TerminoDoCicloDoLote;
            set => SetPropertyValue(nameof(TerminoDoCicloDoLote), ref _TerminoDoCicloDoLote, value);
        }
    }

    [Persistent("WT_LoteLPPMJuntaEstrutura")]
    [Appearance("", AppearanceItemType = nameof(Action), TargetItems = "CloneObject, New", Visibility = ViewItemVisibility.Hide)]
    [Appearance("", AppearanceItemType = nameof(Action), TargetItems = nameof(Delete), Visibility = ViewItemVisibility.Hide)]
    public class LoteLPPMJuntaEstrutura : BaseObject, ILoteDetalhe
    {
        private JuntaComponente juntaComponente;
        private bool _AprovouLote;
        private int _CicloTermico;
        private DateTime _DataInclusao;
        private DateTime _DataInspecao;

        // Fields...
        private bool _InspecaoExcesso;
        private InspecaoLaudo? _Laudo;
        private LoteLPPMEstrutura _loteLPPMEstrutura;
        private string _NumeroDoRelatorio;
        private double _percentualNivelDeInspecao;

        public LoteLPPMJuntaEstrutura(Session session)
            : base(session) { }

        protected override void OnDeleting()
        {
            base.OnDeleting();
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Aprovou o Lote")]
        public bool AprovouLote
        {
            get => _AprovouLote;
            set => SetPropertyValue(nameof(AprovouLote), ref _AprovouLote, value);
        }

        [RuleRequiredField]
        [Indexed(Unique = false)]
        [XafDisplayName("Ciclo Térmico")]
        [ModelDefault("AllowEdit", "False")]
        public int CicloTermico
        {
            get => _CicloTermico;
            set => SetPropertyValue(nameof(CicloTermico), ref _CicloTermico, value);
        }

        [XafDisplayName(nameof(Componente))]
        [VisibleInLookupListView(true)]
        [PersistentAlias("JuntaEstrutura.IdentificacaoDoComponentes")]
        public string Componente => (string)EvaluateAlias(nameof(Componente));

        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Data de Inclusão")]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        public DateTime DataInclusao
        {
            get => _DataInclusao;
            set => SetPropertyValue(nameof(DataInclusao), ref _DataInclusao, value);
        }

        [ModelDefault("EditMask", "G")]
        [ModelDefault("DisplayFormat", "G")]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Data de Inspeção")]
        [ValueConverter(typeof(UtcDateTimeConverter))]
        public DateTime DataInspecao
        {
            get => _DataInspecao;
            set => SetPropertyValue(nameof(DataInspecao), ref _DataInspecao, value);
        }

        [XafDisplayName("Desenho de Detalhamento")]
        [PersistentAlias("JuntaEstrutura.Desenho")]
        public string Desenho => (string)EvaluateAlias(nameof(Desenho));

        [XafDisplayName("Inspeção em excesso")]
        [ModelDefault("AllowEdit", "False")]
        public bool InspecaoExcesso
        {
            get => _InspecaoExcesso;
            set => SetPropertyValue(nameof(InspecaoExcesso), ref _InspecaoExcesso, value);
        }

        [XafDisplayName("Junta")]
        [Indexed(Unique = false)]
        [Association("JuntaComponente-LoteLPPMJuntaEstruturas")]
        [ModelDefault("AllowEdit", "False")]
        public JuntaComponente JuntaComponente
        {
            get => juntaComponente;
            set => SetPropertyValue(nameof(JuntaComponente), ref juntaComponente, value);
        }


        [ModelDefault("AllowEdit", "False")]
        public InspecaoLaudo? Laudo
        {
            get => _Laudo;
            set => SetPropertyValue(nameof(Laudo), ref _Laudo, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Lote LP/PM Estrutura")]
        [Association("LoteLPPMEstrutura-LoteLPPMjuntaEstruturas")]
        public LoteLPPMEstrutura LoteLPPMEstrutura
        {
            get => _loteLPPMEstrutura;
            set => SetPropertyValue(nameof(LoteLPPMEstrutura), ref _loteLPPMEstrutura, value);
        }
               
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Número do Relatório")]
        public string NumeroDoRelatorio
        {
            get => _NumeroDoRelatorio;
            set => SetPropertyValue(nameof(NumeroDoRelatorio), ref _NumeroDoRelatorio, value);
        }

        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("DisplayFormat", "P0")]
        [ModelDefault("EditMask", "n1")]
        [XafDisplayName("% Inspeção")]
        [VisibleInListView(false)]
        public double PercentualNivelDeInspecao
        {
            get => _percentualNivelDeInspecao;
            set => SetPropertyValue(nameof(PercentualNivelDeInspecao), ref _percentualNivelDeInspecao, value);
        }
    }
}