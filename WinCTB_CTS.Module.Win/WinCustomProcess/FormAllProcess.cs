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
using WinCTB_CTS.Module.ServiceProcess.Importer.Estrutura;
using WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao;

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

        private void LigarToggles()
        {
            foreach (var control in this.Controls)
            {
                if (control is ToggleSwitch toggleSwitch )
                {
                    toggleSwitch.IsOn = true;
                }
            }
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

            //Importação Tabela Aulixiares Tubulação
            if (toggleSwitchImportarTabelasAuxiliaresTubulacao.IsOn)
            {
                await ImportarContrato(cts.Token, progressLocal);
                await ImportarDiametro(cts.Token, progressLocal);
                await ImportarSchedule(cts.Token, progressLocal);
                await ImportarPercInspecao(cts.Token, progressLocal);
                await ImportarProcessoDeSoldagem(cts.Token, progressLocal);
                await ImportarEAP(cts.Token, progressLocal);
            }

            //Importação Tubulçao
            if (toggleSwitchImportarTubulacao.IsOn)
            {
                await ImportarSpool(cts.Token, progressLocal);
                await ImportarJuntaSpool(cts.Token, progressLocal);
            }

            //Importação Estrutura
            if (toggleSwitchImportarEstrutura.IsOn)
            {
                await ImportarComponente(cts.Token, progressLocal);
                await ImportarJuntaComponente(cts.Token, progressLocal);
            }

            //Lotes
            if (toggleSwitchImportarLotesEstrutura.IsOn)
            {
                await GerarLotes(cts.Token, progressLocal);
                await InserirInspecao(cts.Token, progressLocal);
                await Alinhamento(cts.Token, progressLocal);
                await Balancealmento(cts.Token, progressLocal);
            }

            BtStartProcess.Enabled = true;
        }

        #region Importação Tabela Auxiliares
        private async Task ImportarContrato(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditContrato);
            var processo = new ImportContrato(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Contrato", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditContrato);
        }

        private async Task ImportarDiametro(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditDiametro);
            var processo = new ImportDiametro(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("TabDiametro", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditDiametro);
        }

        private async Task ImportarSchedule(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditSchedule);
            var processo = new ImportSchedule(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Schedule", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditSchedule);
        }

        private async Task ImportarPercInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditPercInspecao);
            var processo = new ImportPercInspecao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("PercInspecao", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditPercInspecao);
        }

        private async Task ImportarProcessoDeSoldagem(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditProcessoSoldagem);
            var processo = new ImportProcessoSoldagem(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("ProcessoSoldagem", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditProcessoSoldagem);
        }

        private async Task ImportarEAP(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditEAP);
            var processo = new ImportEAP(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("EAPPipe", "TabelasAuxiliares.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditEAP);
        }
        #endregion


        #region Importação de Tubulação
        private async Task ImportarSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditSpool);
            var processo = new ImportSpool(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("SGS", "SGSeSGJOriginal.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditSpool);
        }

        private async Task ImportarJuntaSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditJuntaSpool);
            var processo = new ImportJuntaSpool(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("SGJ", "SGSeSGJOriginal.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditJuntaSpool);
        }
        #endregion


        #region Importação de Estrutura
        private async Task ImportarComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditComponentes);
            var processo = new ImportComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Piece", "MapaMontagemEBR.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditComponentes);
        }

        private async Task ImportarJuntaComponente(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditJuntaComponente);
            var processo = new ImportJuntaComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Joints", "MapaMontagemEBR.xlsx", "");
            processo.Dispose();
            CheckEditProcessado(checkEditJuntaComponente);
        }
        #endregion


        #region Lotes de Estrutura
        private async Task GerarLotes(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress)
        {
            CheckEditEmAndamento(checkEditMontagemDeLotes);
            var processo = new GerarLote(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditMontagemDeLotes);
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
        #endregion
    }
}
