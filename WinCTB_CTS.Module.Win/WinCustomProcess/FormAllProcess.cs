using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCTB_CTS.Module.ServiceProcess.Base;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.ProcessoLote;

namespace WinCTB_CTS.Module.Win.WinCustomProcess
{
    public partial class FormAllProcess : DevExpress.XtraEditors.XtraForm
    {
        private IProgress<ImportProgressReport> progressLocal;
        private readonly Font FontStandard = new Font("Tahoma", 8.25F, FontStyle.Regular);
        private readonly Font FontStrikeout = new Font("Tahoma", 8.25F, FontStyle.Strikeout);
        private readonly Font FontBold = new Font("Tahoma", 8.25F, FontStyle.Bold);

        public FormAllProcess()
        {
            InitializeComponent();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            labelControlAndamentoDoProcesso.Text = string.Empty;
            resetCheckEdit();
        }

        public void LogTrace(ImportProgressReport value)
        {
            progressBarControlGeral.Properties.Maximum = value.TotalRows;
            labelControlAndamentoDoProcesso.Text = value.MessageImport;

            if (value.CurrentRow > 0)
                progressBarControlGeral.EditValue = value.CurrentRow;

            progressBarControlGeral.Update();
            labelControlAndamentoDoProcesso.Update();
        }

        private void resetCheckEdit()
        {
            foreach (var control in this.Controls)
            {
                if (control is CheckEdit checkEdit)
                {
                    checkEdit.Checked = false;
                    checkEdit.Font = FontStandard;
                    checkEdit.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void CheckEditEmAndamento(CheckEdit checkEdit)
        {
            checkEdit.Checked = true;
            checkEdit.Font = FontStandard;
            checkEdit.ForeColor = System.Drawing.Color.Blue;

        }

        private void CheckEditProcessado(CheckEdit checkEdit)
        {
            checkEdit.Checked = true;
            checkEdit.Font = FontStrikeout;
            checkEdit.ForeColor = System.Drawing.Color.Red;
        }

        private async void BtStartProcess_Click(object sender, EventArgs e)
        {
            resetCheckEdit();
            BtStartProcess.Enabled = false;
            var cts = new CancellationTokenSource();
            progressLocal = new Progress<ImportProgressReport>(LogTrace);

            //Importação Estrutura


            //Lotes
            await GerarLotes(cts.Token, progressLocal);
            await InserirInspecao(cts.Token, progressLocal);
            await Alinhamento(cts.Token, progressLocal);
            await Balancealmento(cts.Token, progressLocal);

            BtStartProcess.Enabled = true;
        }

        private async Task GerarLotes(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEdirMontagemDeLotes);
            var processo = new GerarLote(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEdirMontagemDeLotes);
        }

        private async Task InserirInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditInspecaoEmLotes);
            var processo = new LotesDeEstruturaInspecao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditInspecaoEmLotes);
        }

        private async Task Alinhamento(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditAlinhamentoDeLotes);
            var processo = new LotesDeEstruturaAlinhamento(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditAlinhamentoDeLotes);
        }

        private async Task Balancealmento(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditBalanceamento);
            var processo = new BalanceamentoDeLotesEstrutura(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditBalanceamento);
        }
    }
}
