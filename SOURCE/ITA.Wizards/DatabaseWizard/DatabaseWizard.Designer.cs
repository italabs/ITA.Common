namespace ITA.Wizards.DatabaseWizard
{
    partial class DatabaseWizard
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
            this.SuspendLayout();
            // 
            // DatabaseWizard
            // 
            this.HorizontalBanner = Properties.Resources.HorizontalBanner;
            this.Name = "DatabaseWizard";
            this.Text = Messages.WIZ_DB_TITLE;
            this.VerticalBanner = Properties.Resources.VerticalBanner;
            this.ResumeLayout(false);

        }

        #endregion
    }
}
