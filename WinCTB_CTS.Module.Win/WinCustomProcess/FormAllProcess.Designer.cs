
namespace WinCTB_CTS.Module.Win.WinCustomProcess
{
    partial class FormAllProcess
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAllProcess));
            this.progressBarControlGeral = new DevExpress.XtraEditors.ProgressBarControl();
            this.BtStartProcess = new DevExpress.XtraEditors.SimpleButton();
            this.labelControlAndamentoDoProcesso = new DevExpress.XtraEditors.LabelControl();
            this.checkEdirMontagemDeLotes = new DevExpress.XtraEditors.CheckEdit();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.checkEditInspecaoEmLotes = new DevExpress.XtraEditors.CheckEdit();
            this.checkEditBalanceamento = new DevExpress.XtraEditors.CheckEdit();
            this.checkEditAlinhamentoDeLotes = new DevExpress.XtraEditors.CheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControlGeral.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdirMontagemDeLotes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditInspecaoEmLotes.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditBalanceamento.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditAlinhamentoDeLotes.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBarControlGeral
            // 
            this.progressBarControlGeral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarControlGeral.Location = new System.Drawing.Point(12, 12);
            this.progressBarControlGeral.Name = "progressBarControlGeral";
            this.progressBarControlGeral.Properties.FlowAnimationEnabled = true;
            this.progressBarControlGeral.Properties.ShowTitle = true;
            this.progressBarControlGeral.Properties.Step = 1;
            this.progressBarControlGeral.Size = new System.Drawing.Size(656, 24);
            this.progressBarControlGeral.TabIndex = 0;
            // 
            // BtStartProcess
            // 
            this.BtStartProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtStartProcess.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtStartProcess.Appearance.Options.UseFont = true;
            this.BtStartProcess.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("BtStartProcess.ImageOptions.Image")));
            this.BtStartProcess.Location = new System.Drawing.Point(537, 439);
            this.BtStartProcess.Name = "BtStartProcess";
            this.BtStartProcess.Size = new System.Drawing.Size(131, 40);
            this.BtStartProcess.TabIndex = 1;
            this.BtStartProcess.Text = "Inicializar";
            this.BtStartProcess.Click += new System.EventHandler(this.BtStartProcess_Click);
            // 
            // labelControlAndamentoDoProcesso
            // 
            this.labelControlAndamentoDoProcesso.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.labelControlAndamentoDoProcesso.Appearance.Options.UseFont = true;
            this.labelControlAndamentoDoProcesso.Location = new System.Drawing.Point(12, 42);
            this.labelControlAndamentoDoProcesso.Name = "labelControlAndamentoDoProcesso";
            this.labelControlAndamentoDoProcesso.Size = new System.Drawing.Size(164, 19);
            this.labelControlAndamentoDoProcesso.TabIndex = 2;
            this.labelControlAndamentoDoProcesso.Text = "AndamentoDoProcesso";
            // 
            // checkEdirMontagemDeLotes
            // 
            this.checkEdirMontagemDeLotes.Location = new System.Drawing.Point(12, 96);
            this.checkEdirMontagemDeLotes.Name = "checkEdirMontagemDeLotes";
            this.checkEdirMontagemDeLotes.Properties.Caption = "Montagem de Lotes";
            this.checkEdirMontagemDeLotes.Properties.ReadOnly = true;
            this.checkEdirMontagemDeLotes.Size = new System.Drawing.Size(147, 19);
            this.checkEdirMontagemDeLotes.TabIndex = 3;
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(12, 67);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(656, 23);
            this.separatorControl1.TabIndex = 4;
            // 
            // checkEditInspecaoEmLotes
            // 
            this.checkEditInspecaoEmLotes.Location = new System.Drawing.Point(12, 121);
            this.checkEditInspecaoEmLotes.Name = "checkEditInspecaoEmLotes";
            this.checkEditInspecaoEmLotes.Properties.Caption = "Inspeção em Lotes";
            this.checkEditInspecaoEmLotes.Properties.ReadOnly = true;
            this.checkEditInspecaoEmLotes.Size = new System.Drawing.Size(147, 19);
            this.checkEditInspecaoEmLotes.TabIndex = 5;
            // 
            // checkEditBalanceamento
            // 
            this.checkEditBalanceamento.Location = new System.Drawing.Point(12, 171);
            this.checkEditBalanceamento.Name = "checkEditBalanceamento";
            this.checkEditBalanceamento.Properties.Caption = "Balanceamento de Lotes";
            this.checkEditBalanceamento.Properties.ReadOnly = true;
            this.checkEditBalanceamento.Size = new System.Drawing.Size(147, 19);
            this.checkEditBalanceamento.TabIndex = 6;
            // 
            // checkEditAlinhamentoDeLotes
            // 
            this.checkEditAlinhamentoDeLotes.Location = new System.Drawing.Point(12, 146);
            this.checkEditAlinhamentoDeLotes.Name = "checkEditAlinhamentoDeLotes";
            this.checkEditAlinhamentoDeLotes.Properties.Caption = "Alinhamento de Lotes";
            this.checkEditAlinhamentoDeLotes.Properties.ReadOnly = true;
            this.checkEditAlinhamentoDeLotes.Size = new System.Drawing.Size(147, 19);
            this.checkEditAlinhamentoDeLotes.TabIndex = 7;
            // 
            // FormAllProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 491);
            this.Controls.Add(this.checkEditAlinhamentoDeLotes);
            this.Controls.Add(this.checkEditBalanceamento);
            this.Controls.Add(this.checkEditInspecaoEmLotes);
            this.Controls.Add(this.separatorControl1);
            this.Controls.Add(this.checkEdirMontagemDeLotes);
            this.Controls.Add(this.labelControlAndamentoDoProcesso);
            this.Controls.Add(this.BtStartProcess);
            this.Controls.Add(this.progressBarControlGeral);
            this.Name = "FormAllProcess";
            this.Text = "Importação de dados";
            ((System.ComponentModel.ISupportInitialize)(this.progressBarControlGeral.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEdirMontagemDeLotes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditInspecaoEmLotes.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditBalanceamento.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkEditAlinhamentoDeLotes.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl progressBarControlGeral;
        private DevExpress.XtraEditors.SimpleButton BtStartProcess;
        private DevExpress.XtraEditors.LabelControl labelControlAndamentoDoProcesso;
        private DevExpress.XtraEditors.CheckEdit checkEdirMontagemDeLotes;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.CheckEdit checkEditInspecaoEmLotes;
        private DevExpress.XtraEditors.CheckEdit checkEditBalanceamento;
        private DevExpress.XtraEditors.CheckEdit checkEditAlinhamentoDeLotes;
    }
}