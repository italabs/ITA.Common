namespace ITA.Wizards.DatabaseWizard.Controls
{
    partial class TextBoxWithValidator
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
            this.labelHint = new System.Windows.Forms.Label();
            this.textBoxToValidate = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHint.AutoSize = true;
            this.labelHint.BackColor = System.Drawing.Color.Gold;
            this.labelHint.ForeColor = System.Drawing.SystemColors.WindowText;
            this.labelHint.Location = new System.Drawing.Point(561, 23);
            this.labelHint.Name = "labelHint";
            this.labelHint.Size = new System.Drawing.Size(0, 13);
            this.labelHint.TabIndex = 15;
            this.labelHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxToValidate
            // 
            this.textBoxToValidate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxToValidate.Location = new System.Drawing.Point(3, 0);
            this.textBoxToValidate.Name = "textBoxToValidate";
            this.textBoxToValidate.Size = new System.Drawing.Size(561, 20);
            this.textBoxToValidate.TabIndex = 14;
            this.textBoxToValidate.EnabledChanged += new System.EventHandler(this.textBoxToValidate_EnabledChanged);
            this.textBoxToValidate.TextChanged += new System.EventHandler(this.textBoxConfirm_TextChanged);
            this.textBoxToValidate.Enter += new System.EventHandler(this.textBoxConfirm_Enter);
            // 
            // TextBoxWithValidator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.labelHint);
            this.Controls.Add(this.textBoxToValidate);
            this.Name = "TextBoxWithValidator";
            this.Size = new System.Drawing.Size(564, 39);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHint;
        private System.Windows.Forms.TextBox textBoxToValidate;
    }
}
