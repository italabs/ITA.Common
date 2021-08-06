namespace ITA.Wizards.DatabaseWizard.Pages
{
    partial class CheckExistingDatabasePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckExistingDatabasePage));
            this.label1 = new System.Windows.Forms.Label();
            this.lCheckResult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxOK = new System.Windows.Forms.PictureBox();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.pictureBoxWait = new System.Windows.Forms.PictureBox();
            this.pictureBoxDb = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDb)).BeginInit();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = "Конфигурация продукта";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Проверка существующей конфигурации";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.pictureBoxDb);
            this.panelPlaceholder.Controls.Add(this.pictureBoxOK);
            this.panelPlaceholder.Controls.Add(this.pictureBoxError);
            this.panelPlaceholder.Controls.Add(this.pictureBoxWait);
            this.panelPlaceholder.Controls.Add(this.label2);
            this.panelPlaceholder.Controls.Add(this.lCheckResult);
            this.panelPlaceholder.Controls.Add(this.label1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Проверка существующей базы данных";
            // 
            // lCheckResult
            // 
            this.lCheckResult.AutoSize = true;
            this.lCheckResult.ForeColor = System.Drawing.Color.Black;
            this.lCheckResult.Location = new System.Drawing.Point(126, 53);
            this.lCheckResult.Name = "lCheckResult";
            this.lCheckResult.Size = new System.Drawing.Size(110, 13);
            this.lCheckResult.TabIndex = 1;
            this.lCheckResult.Text = "Результат проверки";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Результат проверки:";
            // 
            // pictureBoxOK
            // 
            this.pictureBoxOK.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxOK.Image")));
            this.pictureBoxOK.Location = new System.Drawing.Point(378, 195);
            this.pictureBoxOK.Name = "pictureBoxOK";
            this.pictureBoxOK.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxOK.TabIndex = 29;
            this.pictureBoxOK.TabStop = false;
            this.pictureBoxOK.Visible = false;
            // 
            // pictureBoxError
            // 
            this.pictureBoxError.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxError.Image")));
            this.pictureBoxError.Location = new System.Drawing.Point(410, 195);
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxError.TabIndex = 28;
            this.pictureBoxError.TabStop = false;
            this.pictureBoxError.Visible = false;
            // 
            // pictureBoxWait
            // 
            this.pictureBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWait.Image")));
            this.pictureBoxWait.Location = new System.Drawing.Point(442, 195);
            this.pictureBoxWait.Name = "pictureBoxWait";
            this.pictureBoxWait.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxWait.TabIndex = 27;
            this.pictureBoxWait.TabStop = false;
            this.pictureBoxWait.Visible = false;
            // 
            // pictureBoxDb
            // 
            this.pictureBoxDb.Location = new System.Drawing.Point(221, 22);
            this.pictureBoxDb.Name = "pictureBoxDb";
            this.pictureBoxDb.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxDb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxDb.TabIndex = 30;
            this.pictureBoxDb.TabStop = false;
            // 
            // CheckExistingDatabasePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CheckExistingDatabasePage";
            this.Load += new System.EventHandler(this.CheckExistingDatabase_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDb)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lCheckResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBoxOK;
        private System.Windows.Forms.PictureBox pictureBoxError;
        private System.Windows.Forms.PictureBox pictureBoxWait;
        private System.Windows.Forms.PictureBox pictureBoxDb;
    }
}
