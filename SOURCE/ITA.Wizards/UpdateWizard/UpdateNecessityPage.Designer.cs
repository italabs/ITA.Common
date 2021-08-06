namespace ITA.Wizards.UpdateWizard
{
    partial class UpdateNecessityPage
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
            this._lblCurrentVersion = new System.Windows.Forms.Label();
            this._lblActualVersion = new System.Windows.Forms.Label();
            this._lblCurrentVersionOut = new System.Windows.Forms.Label();
            this._lblActualVersionOut = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._lblDatabaseNameOut = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Size = new System.Drawing.Size(388, 16);
            this.labelHint.Text = "Обновление базы данных";
            // 
            // labelDescription
            // 
            this.labelDescription.Text = "Необходимо выполнить обновление базы данных";
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.label2);
            this.panelPlaceholder.Controls.Add(this._lblActualVersion);
            this.panelPlaceholder.Controls.Add(this._lblActualVersionOut);
            this.panelPlaceholder.Controls.Add(this._lblDatabaseNameOut);
            this.panelPlaceholder.Controls.Add(this._lblCurrentVersionOut);
            this.panelPlaceholder.Controls.Add(this.label3);
            this.panelPlaceholder.Controls.Add(this._lblCurrentVersion);
            this.panelPlaceholder.Controls.Add(this.label1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(375, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "Для продолжения настройки сервера требуется выполнить обновление\r\nвыбранной базы данных до актуальной версии: ";
            // 
            // _lblCurrentVersion
            // 
            this._lblCurrentVersion.AutoSize = true;
            this._lblCurrentVersion.Location = new System.Drawing.Point(33, 92);
            this._lblCurrentVersion.Name = "_lblCurrentVersion";
            this._lblCurrentVersion.Size = new System.Drawing.Size(162, 13);
            this._lblCurrentVersion.TabIndex = 2;
            this._lblCurrentVersion.Text = "Версия текущей базы данных:";
            // 
            // _lblActualVersion
            // 
            this._lblActualVersion.AutoSize = true;
            this._lblActualVersion.Location = new System.Drawing.Point(33, 117);
            this._lblActualVersion.Name = "_lblActualVersion";
            this._lblActualVersion.Size = new System.Drawing.Size(168, 13);
            this._lblActualVersion.TabIndex = 2;
            this._lblActualVersion.Text = "Версия, требуемая для работы:";
            // 
            // _lblCurrentVersionOut
            // 
            this._lblCurrentVersionOut.AutoSize = true;
            this._lblCurrentVersionOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lblCurrentVersionOut.Location = new System.Drawing.Point(233, 92);
            this._lblCurrentVersionOut.Name = "_lblCurrentVersionOut";
            this._lblCurrentVersionOut.Size = new System.Drawing.Size(61, 13);
            this._lblCurrentVersionOut.TabIndex = 2;
            this._lblCurrentVersionOut.Text = "1.0.0.110";
            // 
            // _lblActualVersionOut
            // 
            this._lblActualVersionOut.AutoSize = true;
            this._lblActualVersionOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lblActualVersionOut.Location = new System.Drawing.Point(233, 117);
            this._lblActualVersionOut.Name = "_lblActualVersionOut";
            this._lblActualVersionOut.Size = new System.Drawing.Size(47, 13);
            this._lblActualVersionOut.TabIndex = 2;
            this._lblActualVersionOut.Text = "1.5.0.1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(379, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Перед началом обновления базы данных следует ОБЯЗАТЕЛЬНО\r\nвыполнить резервное копирование существующей версии базы данных.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Имя текущей базы данных:";
            // 
            // _lblDatabaseNameOut
            // 
            this._lblDatabaseNameOut.AutoSize = true;
            this._lblDatabaseNameOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._lblDatabaseNameOut.Location = new System.Drawing.Point(233, 66);
            this._lblDatabaseNameOut.Name = "_lblDatabaseNameOut";
            this._lblDatabaseNameOut.Size = new System.Drawing.Size(98, 13);
            this._lblDatabaseNameOut.TabIndex = 2;
            this._lblDatabaseNameOut.Text = "DB_XXXX";
            // 
            // UpdateNecessityPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "UpdateNecessityPage";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            this.panelPlaceholder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lblActualVersion;
        private System.Windows.Forms.Label _lblCurrentVersion;
        private System.Windows.Forms.Label _lblActualVersionOut;
        private System.Windows.Forms.Label _lblCurrentVersionOut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label _lblDatabaseNameOut;
        private System.Windows.Forms.Label label3;
    }
}
