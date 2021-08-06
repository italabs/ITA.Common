namespace ITA.Wizards.UpdateWizard
{
    partial class UpdateProgress
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
            this._updateProgressBar = new System.Windows.Forms.ProgressBar();
            this._lblCurrentAction = new System.Windows.Forms.Label();
            this._lblLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Обновление базы данных";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Дождитесь завершения обновления базы данных";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this._lblLog);
            this.panelPlaceholder.Controls.Add(this._updateProgressBar);
            this.panelPlaceholder.Controls.Add(this._lblCurrentAction);
            // 
            // _updateProgressBar
            // 
            this._updateProgressBar.Location = new System.Drawing.Point(12, 39);
            this._updateProgressBar.Name = "_updateProgressBar";
            this._updateProgressBar.Size = new System.Drawing.Size(473, 23);
            this._updateProgressBar.Step = 5;
            this._updateProgressBar.TabIndex = 5;
            // 
            // _lblCurrentAction
            // 
            this._lblCurrentAction.AutoSize = true;
            this._lblCurrentAction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lblCurrentAction.Location = new System.Drawing.Point(12, 13);
            this._lblCurrentAction.Name = "_lblCurrentAction";
            this._lblCurrentAction.Size = new System.Drawing.Size(144, 13);
            this._lblCurrentAction.TabIndex = 3;
            this._lblCurrentAction.Text = "Прогресс выполнения:";
            // 
            // _lblLog
            // 
            this._lblLog.BackColor = System.Drawing.SystemColors.Window;
            this._lblLog.Location = new System.Drawing.Point(12, 83);
            this._lblLog.Multiline = true;
            this._lblLog.Name = "_lblLog";
            this._lblLog.ReadOnly = true;
            this._lblLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._lblLog.Size = new System.Drawing.Size(473, 146);
            this._lblLog.TabIndex = 6;
            // 
            // UpdateProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UpdateProgress";
            this.Load += new System.EventHandler(this.UpdateProgress_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar _updateProgressBar;
        private System.Windows.Forms.Label _lblCurrentAction;
        private System.Windows.Forms.TextBox _lblLog;
    }
}
