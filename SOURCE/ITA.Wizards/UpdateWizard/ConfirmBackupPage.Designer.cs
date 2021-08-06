namespace ITA.Wizards.UpdateWizard
{
    partial class ConfirmBackupPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this._chbBackupExists = new System.Windows.Forms.CheckBox();
            this._chbBackupIsActual = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Резервное копирование базы данных";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Необходимо выполнить резервное копирование существующей базы данных";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this._chbBackupIsActual);
            this.panelPlaceholder.Controls.Add(this._chbBackupExists);
            this.panelPlaceholder.Controls.Add(this.label1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Перед началом обновления базы данных следует ОБЯЗАТЕЛЬНО\r\nвыполнить резервное копирование существующей версии базы данных.";
            // 
            // _chbBackupExists
            // 
            this._chbBackupExists.AutoSize = true;
            this._chbBackupExists.Location = new System.Drawing.Point(12, 79);
            this._chbBackupExists.Name = "_chbBackupExists";
            this._chbBackupExists.Size = new System.Drawing.Size(292, 30);
            this._chbBackupExists.TabIndex = 1;
            this._chbBackupExists.Text = "Подтверждаю, что перед обновлением базы данных\r\nбыло выполнено резервное копирование";
            this._chbBackupExists.UseVisualStyleBackColor = true;
            this._chbBackupExists.CheckedChanged += new System.EventHandler(this._chbBackupExists_CheckedChanged);
            // 
            // _chbBackupIsActual
            // 
            this._chbBackupIsActual.AutoSize = true;
            this._chbBackupIsActual.Location = new System.Drawing.Point(12, 126);
            this._chbBackupIsActual.Name = "_chbBackupIsActual";
            this._chbBackupIsActual.Size = new System.Drawing.Size(377, 17);
            this._chbBackupIsActual.TabIndex = 1;
            this._chbBackupIsActual.Text = "Подтверждаю, что сделанная резервная копия является актуальной";
            this._chbBackupIsActual.UseVisualStyleBackColor = true;
            this._chbBackupIsActual.CheckedChanged += new System.EventHandler(this._chbBackupExists_CheckedChanged);
            // 
            // ConfirmBackupPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ConfirmBackupPage";
            this.Load += new System.EventHandler(this.ConfirmBackupPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox _chbBackupExists;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _chbBackupIsActual;
    }
}
