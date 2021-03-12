﻿using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinCTB_CTS.Module.BusinessObjects.Estrutura.Auxiliar;
using WinCTB_CTS.Module.BusinessObjects.Tubulacao.Auxiliar;
using WinCTB_CTS.Module.Helpers;
using WinCTB_CTS.Module.ServiceProcess.Base;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.Medicao;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Estrutura.ProcessoLote;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.Medicao;
using WinCTB_CTS.Module.ServiceProcess.Calculator.Tubulacao.ProcessoLote;
using WinCTB_CTS.Module.ServiceProcess.Importer.Estrutura;
using WinCTB_CTS.Module.ServiceProcess.Importer.Tubulacao;
using WinCTB_CTS.Module.Win.Services;

namespace WinCTB_CTS.Module.Win.WinCustomProcess {
    public partial class FormAllProcess : DevExpress.XtraEditors.XtraForm {
        private CancellationTokenSource _cancellationTokenSource;
        private Stopwatch stopwatch;
        private ProviderDataLayer provider;
        private UnitOfWork uow;
        private IProgress<ImportProgressReport> progressLocal;
        private readonly Font FontStandard = new Font("Tahoma", 8.25F, FontStyle.Regular);
        private readonly Font FontStrikeout = new Font("Tahoma", 8.25F, FontStyle.Strikeout);
        private readonly Font FontBold = new Font("Tahoma", 10.00F, FontStyle.Bold);
        private const string PathFileForImportTabelaAuxiliarTubulacao = "PathFileForImportTabelaAuxiliarTubulacao";
        private const string PathFileForImportTabelaAuxiliarEstrutura = "PathFileForImportTabelaAuxiliarEstrutura";
        private const string PathFileForImportTubulacao = "PathFileForImportTubulacao";
        private const string PathFileForImportEstruturaMV32 = "PathFileForImportEstruturaMV32";
        private const string PathFileForImportEstruturaSEPETIBA = "PathFileForImportEstruturaSEPETIBA";

        public FormAllProcess() {
            InitializeComponent();
            //this.Location = Screen.AllScreens[1].WorkingArea.Location;
            init();
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);
            provider = new ProviderDataLayer();
            uow = new UnitOfWork(provider.GetSimpleDataLayer());
            stopwatch = new Stopwatch();
            timerProcess.Tick += TimerProcess_Tick;
        }

        protected override void OnDeactivate(EventArgs e) {
            uow?.Dispose();
            provider?.Dispose();
            base.OnDeactivate(e);
        }

        private void init() {
            labelControlAndamentoDoProcesso.Text = string.Empty;
            BtCancelar.Enabled = false;
            LocateCheckeditToReset();
            LigarToggles();

            BtnPathImportTubulacao.EditValue = RegisterWindowsManipulation.GetRegister(PathFileForImportTubulacao);
            BtnPathImportEstruturaMV32.EditValue = RegisterWindowsManipulation.GetRegister(PathFileForImportEstruturaMV32);
            BtnPathImportEstruturaSEPETIBA.EditValue = RegisterWindowsManipulation.GetRegister(PathFileForImportEstruturaSEPETIBA);
            BtnPathImportTabAuxiliarTubulacao.EditValue = RegisterWindowsManipulation.GetRegister(PathFileForImportTabelaAuxiliarTubulacao); ;
            BtnPathImportTabAuxiliarEstrutura.EditValue = RegisterWindowsManipulation.GetRegister(PathFileForImportTabelaAuxiliarEstrutura); ;

            BtnPathImportTubulacao.EditValueChanged += BtnPathImportTubulacao_EditValueChanged;
            BtnPathImportEstruturaMV32.EditValueChanged += BtnPathImportEstruturaMV32_EditValueChanged;
            BtnPathImportEstruturaSEPETIBA.EditValueChanged += BtnPathImportEstruturaSEPETIBA_EditValueChanged; ;
            BtnPathImportTabAuxiliarTubulacao.EditValueChanged += BtnPathImportTabAuxiliarTubulacao_EditValueChanged;
            BtnPathImportTabAuxiliarEstrutura.EditValueChanged += BtnPathImportTabAuxiliarEstrutura_EditValueChanged;
        }

        private void BtnPathImportTabAuxiliarEstrutura_EditValueChanged(object sender, EventArgs e) {
            RegisterWindowsManipulation.SetRegister(PathFileForImportTabelaAuxiliarEstrutura, BtnPathImportTabAuxiliarEstrutura.Text);
        }

        private void BtnPathImportTabAuxiliarTubulacao_EditValueChanged(object sender, EventArgs e) {
            RegisterWindowsManipulation.SetRegister(PathFileForImportTabelaAuxiliarTubulacao, BtnPathImportTabAuxiliarTubulacao.Text);
        }

        private void BtnPathImportEstruturaMV32_EditValueChanged(object sender, EventArgs e) {
            RegisterWindowsManipulation.SetRegister(PathFileForImportEstruturaMV32, BtnPathImportEstruturaMV32.Text);
        }

        private void BtnPathImportEstruturaSEPETIBA_EditValueChanged(object sender, EventArgs e) {
            RegisterWindowsManipulation.SetRegister(PathFileForImportEstruturaSEPETIBA, BtnPathImportEstruturaSEPETIBA.Text);
        }

        private void BtnPathImportTubulacao_EditValueChanged(object sender, EventArgs e) {
            RegisterWindowsManipulation.SetRegister(PathFileForImportTubulacao, BtnPathImportTubulacao.Text);
        }

        public void LogTrace(ImportProgressReport value) {
            progressBarControlGeral.Properties.Maximum = value.TotalRows;
            labelControlAndamentoDoProcesso.Text = value.MessageImport;

            if (value.CurrentRow > 0)
                progressBarControlGeral.EditValue = value.CurrentRow;

            progressBarControlGeral.Update();
            labelControlAndamentoDoProcesso.Update();
        }

        private void LigarToggles(bool IsOn = false) {
            var controls = GetAllControl(this, typeof(ToggleSwitch));
            foreach (var control in controls) {
                if (control is ToggleSwitch toggle)
                    toggle.IsOn = IsOn;
            }
        }

        public IEnumerable<Control> GetAllControl(Control control, Type type) {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControl(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        private void LocateCheckeditToReset() {
            var controls = GetAllControl(this, typeof(CheckEdit));
            foreach (var control in controls) {
                if (control is CheckEdit checkEdit)
                    resetCheckEdit(checkEdit, FontStandard);
            }
        }


        private Action<CheckEdit, Font> resetCheckEdit = (checkEdit, font) => {
            checkEdit.Checked = false;
            checkEdit.Font = font;
            checkEdit.ForeColor = Color.Black;
        };

        private void CheckEditEmAndamento(CheckEdit checkEdit) {
            checkEdit.Checked = false;
            checkEdit.Font = FontBold;
            checkEdit.ForeColor = System.Drawing.Color.Blue;

        }

        private void CheckEditProcessado(CheckEdit checkEdit) {
            checkEdit.Checked = true;
            checkEdit.Font = FontStrikeout;
            checkEdit.ForeColor = System.Drawing.Color.Red;
        }

        private void BtCancelar_Click(object sender, EventArgs e) {
            _cancellationTokenSource?.Cancel();
        }

        private async void BtStartProcess_Click(object sender, EventArgs e) {
            
            LocateCheckeditToReset();
            BtStartProcess.Enabled = false;
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            progressLocal = new Progress<ImportProgressReport>(LogTrace);
            
            BtCancelar.Enabled = !BtStartProcess.Enabled;
            
           
            StartTimer();

            //Importação Tabela Aulixiares Tubulação
            if (toggleSwitchImportarTabelasAuxiliaresTubulacao.IsOn) {
                await ImportarContratoTubulacao(cancellationToken, progressLocal);
                await ImportarDiametro(cancellationToken, progressLocal);
                await ImportarSchedule(cancellationToken, progressLocal);
                await ImportarPercInspecao(cancellationToken, progressLocal);
                await ImportarProcessoDeSoldagem(cancellationToken, progressLocal);
                await ImportarEAPTubulacao(cancellationToken, progressLocal);
            }

            //Importação Tubulçao
            if (toggleSwitchImportarTubulacao.IsOn) {
                await ImportarSpool(cancellationToken, progressLocal);
                await ImportarJuntaSpool(cancellationToken, progressLocal);
            }

            //Medição de Tubulação
            if (toggleSwitchMedicaoTubulacao.IsOn) {
                await MedicaoTubulacao(cancellationToken, progressLocal);
            }

            //Tabela Auxiliar Estrutura
            if (toggleSwitchImportarTabelasAuxiliaresEstrutura.IsOn) {
                await ImportarContratoEstrutura(cancellationToken, progressLocal);
                await ImportarEAPEstrutura(cancellationToken, progressLocal);
            }

            //Importação Estrutura
            if (toggleSwitchImportarEstruturaMV32.IsOn) {
                await ImportarComponenteMV32(cancellationToken, progressLocal);
                await ImportarJuntaComponenteMV32(cancellationToken, progressLocal);
                await AtualizacaoStatusJuntaMV32(cancellationToken, progressLocal);

            }

            //Importação Estrutura
            if (toggleSwitchImportarEstruturaSepetiba.IsOn) {
                await ImportarComponenteSepetiba(cancellationToken, progressLocal);
                await ImportarJuntaComponenteSepetiba(cancellationToken, progressLocal);
                await AtualizacaoStatusJuntaSepetiba(cancellationToken, progressLocal);
            }

            //Lotes
            if (toggleSwitchImportarLotesEstrutura.IsOn) {
                await GerarLotes(cancellationToken, progressLocal);
                await InserirInspecao(cancellationToken, progressLocal);
                await Alinhamento(cancellationToken, progressLocal);
                await Balancealmento(cancellationToken, progressLocal);
                await AtualizacaoStatusJuntaMV32(cancellationToken, progressLocal);
                await AtualizacaoStatusJuntaSepetiba(cancellationToken, progressLocal);
            }

            //Medição de Estrutura
            if (toggleSwitchMedicaoEstrutura.IsOn) {
                await MedicaoEstrutura(cancellationToken, progressLocal);
            }

            BtStartProcess.Enabled = true;
            BtCancelar.Enabled = !BtStartProcess.Enabled;
            StopTimer();
        }

        #region Cronometro
        private void StartTimer() {
            timerProcess.Interval = 100;
            timerProcess.Start();            
            stopwatch.Reset();
            stopwatch.Start();
        }

        private void StopTimer() {
            timerProcess.Stop();
            stopwatch.Stop();
        }
        private void TimerProcess_Tick(object sender, EventArgs e) {
            BeginInvoke(new Action(() => textBoxCronometro.Text = stopwatch.Elapsed.ToString()));
        }
        #endregion


        private async Task ImportarContratoEstrutura(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditContratoEstrutura);
            var processo = new ImportContratoEstrutura(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Contrato", "TabelaAuxiliarEstrutura.xlsx", BtnPathImportTabAuxiliarEstrutura.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditContratoEstrutura);
        }

        private async Task ImportarEAPEstrutura(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditEAPEstrutura);
            var processo = new ImportEAPEstrutura(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("EAPEst", "TabelaAuxiliarEstrutura.xlsx", BtnPathImportTabAuxiliarEstrutura.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditEAPEstrutura);
        }

        #region Importação Tabela Auxiliares
        private async Task ImportarContratoTubulacao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditContrato);
            var processo = new ImportContratoTubulacao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Contrato", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditContrato);
        }

        private async Task ImportarDiametro(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditDiametro);
            var processo = new ImportDiametro(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("TabDiametro", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditDiametro);
        }

        private async Task ImportarSchedule(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditSchedule);
            var processo = new ImportSchedule(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Schedule", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditSchedule);
        }

        private async Task ImportarPercInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditPercInspecao);
            var processo = new ImportPercInspecao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("PercInspecao", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditPercInspecao);
        }

        private async Task ImportarProcessoDeSoldagem(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditProcessoSoldagem);
            var processo = new ImportProcessoSoldagem(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("ProcessoSoldagem", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditProcessoSoldagem);
        }

        private async Task ImportarEAPTubulacao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditEAPTubulacao);
            var processo = new ImportEAPTubulacao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("EAPPipe", "TabelaAuxiliarTubulacao.xlsx", BtnPathImportTabAuxiliarTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditEAPTubulacao);
        }
        #endregion

        #region Importação de Tubulação
        private async Task ImportarSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditSpool);
            var processo = new ImportSpool(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("SGS", "SGSeSGJ.xlsx", BtnPathImportTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditSpool);
        }

        private async Task ImportarJuntaSpool(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditJuntaSpool);
            var processo = new ImportJuntaSpool(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("SGJ", "SGSeSGJ.xlsx", BtnPathImportTubulacao.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditJuntaSpool);
        }
        #endregion

        #region Importação de Estrutura
        private async Task ImportarComponenteMV32(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditComponentesMV32);
            var processo = new ImportComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Piece", "MapaMontagemEBR_MV32.xlsx", BtnPathImportEstruturaMV32.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditComponentesMV32);
        }

        private async Task ImportarJuntaComponenteMV32(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditJuntaComponenteMV32);
            var processo = new ImportJuntaComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Joints", "MapaMontagemEBR_MV32.xlsx", BtnPathImportEstruturaMV32.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditJuntaComponenteMV32);
        }

        private async Task ImportarComponenteSepetiba(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditComponentesSepetiba);
            var processo = new ImportComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Piece", "MapaMontagemEBR_SEPETIBA.xlsx", BtnPathImportEstruturaMV32.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditComponentesSepetiba);
        }

        private async Task ImportarJuntaComponenteSepetiba(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditJuntaComponenteSepetiba);
            var processo = new ImportJuntaComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaWithStream("Joints", "MapaMontagemEBR_SEPETIBA.xlsx", BtnPathImportEstruturaMV32.Text);
            processo.Dispose();
            CheckEditProcessado(checkEditJuntaComponenteSepetiba);
        }
        #endregion

        #region Lotes de Estrutura
        private async Task GerarLotes(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditMontagemDeLotes);
            var processo = new GerarLote(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditMontagemDeLotes);
        }

        private async Task InserirInspecao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditInspecaoEmLotes);
            var processo = new LotesDeEstruturaInspecao(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditInspecaoEmLotes);
        }

        private async Task Alinhamento(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditAlinhamentoDeLotes);
            var processo = new LotesDeEstruturaAlinhamento(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditAlinhamentoDeLotes);
        }

        private async Task Balancealmento(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditBalanceamento);
            var processo = new BalanceamentoDeLotesEstrutura(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditBalanceamento);
        }
        #endregion

        #region Medições
        private async Task MedicaoEstrutura(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditMedicaoComponentes);
            var processo = new CalculoComponente(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditMedicaoComponentes);
        }

        private async Task MedicaoTubulacao(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditMedicaoSpool);
            var processo = new CalculoSpool(cancellationToken, progressLocal);
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditMedicaoSpool);
        }
        #endregion


        private async Task AtualizacaoStatusJuntaMV32(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditAtualizacaoStatusJuntaComponenteMV32);
            var processo = new AtualizarStatusJuntaComponente(cancellationToken, progressLocal, "EBR-MV32");
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditAtualizacaoStatusJuntaComponenteMV32);
        }

        private async Task AtualizacaoStatusJuntaSepetiba(CancellationToken cancellationToken, IProgress<ImportProgressReport> progress) {
            CheckEditEmAndamento(checkEditAtualizacaoStatusJuntaComponenteSepetiba);
            var processo = new AtualizarStatusJuntaComponente(cancellationToken, progressLocal, "EBR-SEPETIBA");
            await processo.ProcessarTarefaSimples();
            processo.Dispose();
            CheckEditProcessado(checkEditAtualizacaoStatusJuntaComponenteSepetiba);
        }

        #region Proteção da tabela auxiliar
        private void toggleSwitchImportarTubulacao_Toggled(object sender, EventArgs e) {
            if (!uow.QueryInTransaction<TabEAPPipe>().Any()) {
                toggleSwitchImportarTabelasAuxiliaresTubulacao.IsOn = toggleSwitchImportarTubulacao.IsOn;

                if (toggleSwitchImportarTubulacao.IsOn)
                    toggleSwitchImportarTabelasAuxiliaresTubulacao.ReadOnly = true;
                else
                    toggleSwitchImportarTabelasAuxiliaresTubulacao.ReadOnly = false;
            }
        }

        private void toggleSwitchImportarEstruturaSepetiba_Toggled(object sender, EventArgs e) {
            if (!uow.QueryInTransaction<TabEAPEst>().Any()) {
                var validade = toggleSwitchImportarEstruturaSepetiba.IsOn || toggleSwitchImportarEstruturaMV32.IsOn;
                toggleSwitchImportarTabelasAuxiliaresEstrutura.IsOn = validade;

                if (validade)
                    toggleSwitchImportarTabelasAuxiliaresEstrutura.ReadOnly = true;
                else
                    toggleSwitchImportarTabelasAuxiliaresEstrutura.ReadOnly = false;
            }
        }

        private void toggleSwitchImportarEstruturaMV32_Toggled(object sender, EventArgs e) {
            if (!uow.QueryInTransaction<TabEAPEst>().Any()) {
                var validade = toggleSwitchImportarEstruturaSepetiba.IsOn || toggleSwitchImportarEstruturaMV32.IsOn;
                toggleSwitchImportarTabelasAuxiliaresEstrutura.IsOn = validade;

                if (validade)
                    toggleSwitchImportarTabelasAuxiliaresEstrutura.ReadOnly = true;
                else
                    toggleSwitchImportarTabelasAuxiliaresEstrutura.ReadOnly = false;
            }
        }
        #endregion


    }
}
