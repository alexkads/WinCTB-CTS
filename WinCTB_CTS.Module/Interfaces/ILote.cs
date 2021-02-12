using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Interfaces
{
    public enum SituacoesQuantidade
    {
        [XafDisplayName("Incompleto")]
        Incompleto = 0,
        [XafDisplayName("Completo")]
        Completo = 1,
    }

    public enum SituacoesInspecao
    {
        [XafDisplayName("Pendente")]
        Pendente = 0,
        [XafDisplayName("Aprovado")]
        Aprovado = 1
    }

    interface ILote
    {
        bool ComJuntaReprovada { get; set; }
        int ExcessoDeInspecao { get; set; }
        DateTime InicioDoCicloDoLote { get; set; }
        int JuntasNoLote { get; set; }
        int NecessidadeDeInspecao { get; set; }
        string NumeroDoLote { get; set; }
        int QuantidadeInspecionada { get; set; }
        SituacoesInspecao SituacaoInspecao { get; set; }
        SituacoesQuantidade SituacaoQuantidade { get; set; }
        DateTime TerminoDoCicloDoLote { get; set; }
    }
}
