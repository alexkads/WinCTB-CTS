
namespace WinCTB_CTS.Module.Win.Controllers
{
    partial class XtraProgressImport
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
            this.ProgressImport = new DevExpress.XtraEditors.ProgressBarControl();
            this.StatusImport = new DevExpress.XtraEditors.LabelControl();
            this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressImport.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ProgressImport
            // 
            this.ProgressImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressImport.Location = new System.Drawing.Point(12, 12);
            this.ProgressImport.Name = "ProgressImport";
            this.ProgressImport.Size = new System.Drawing.Size(385, 25);
            this.ProgressImport.TabIndex = 0;
            // 
            // StatusImport
            // 
            this.StatusImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusImport.Location = new System.Drawing.Point(12, 54);
            this.StatusImport.Name = "StatusImport";
            this.StatusImport.Size = new System.Drawing.Size(55, 13);
            this.StatusImport.TabIndex = 1;
            this.StatusImport.Text = "Iniciando...";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(322, 44);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancelar";
            // 
            // XtraProgressImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(409, 78);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.StatusImport);
            this.Controls.Add(this.ProgressImport);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Glow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XtraProgressImport";
            this.Opacity = 0.8D;
            this.Text = "XtraProgressImport";
            ((System.ComponentModel.ISupportInitialize)(this.ProgressImport.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.ProgressBarControl ProgressImport;
        private DevExpress.XtraEditors.LabelControl StatusImport;
        private DevExpress.XtraEditors.SimpleButton cancelButton;
    }
}