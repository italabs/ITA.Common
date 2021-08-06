namespace ITA.Common.UI
{
    partial class LanguageIndicator
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
            this.labelLanguage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelLanguage
            // 
            this.labelLanguage.BackColor = System.Drawing.Color.Transparent;
            this.labelLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLanguage.Location = new System.Drawing.Point(0, 0);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(22, 20);
            this.labelLanguage.TabIndex = 0;
            this.labelLanguage.Text = "en";
            this.labelLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LanguageIndicator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.RoyalBlue;
            this.Controls.Add(this.labelLanguage);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "LanguageIndicator";
            this.Size = new System.Drawing.Size(22, 20);
            this.Load += new System.EventHandler(this.LanguageIndicator_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelLanguage;
    }
}
