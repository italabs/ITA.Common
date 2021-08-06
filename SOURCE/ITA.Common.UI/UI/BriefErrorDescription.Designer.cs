namespace ITA.Common.UI
{
    partial class BriefErrorDescription
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BriefErrorDescription));
            this.panelContents = new System.Windows.Forms.Panel();
            this.linkLabelDetails = new System.Windows.Forms.LinkLabel();
            this.labelMessage = new System.Windows.Forms.Label();
            this.panelIdent = new System.Windows.Forms.Panel();
            this.pictureBoxArrow = new System.Windows.Forms.PictureBox();
            this.panelContents.SuspendLayout();
            this.panelIdent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContents
            // 
            this.panelContents.Controls.Add(this.linkLabelDetails);
            this.panelContents.Controls.Add(this.labelMessage);
            resources.ApplyResources(this.panelContents, "panelContents");
            this.panelContents.Name = "panelContents";
            // 
            // linkLabelDetails
            // 
            resources.ApplyResources(this.linkLabelDetails, "linkLabelDetails");
            this.linkLabelDetails.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabelDetails.Name = "linkLabelDetails";
            this.linkLabelDetails.TabStop = true;
            this.linkLabelDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.SizeChanged += new System.EventHandler(this.label1_SizeChanged);
            // 
            // panelIdent
            // 
            this.panelIdent.Controls.Add(this.pictureBoxArrow);
            resources.ApplyResources(this.panelIdent, "panelIdent");
            this.panelIdent.Name = "panelIdent";
            // 
            // pictureBoxArrow
            // 
            resources.ApplyResources(this.pictureBoxArrow, "pictureBoxArrow");
            this.pictureBoxArrow.Image = Properties.Resources.Arrow;
            this.pictureBoxArrow.Name = "pictureBoxArrow";
            this.pictureBoxArrow.TabStop = false;
            // 
            // BriefErrorDescription
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContents);
            this.Controls.Add(this.panelIdent);
            this.Name = "BriefErrorDescription";
            this.Load += new System.EventHandler(this.BriefErrorDescription_Load);
            this.ParentChanged += new System.EventHandler(this.BriefErrorDescription_ParentChanged);
            this.panelContents.ResumeLayout(false);
            this.panelContents.PerformLayout();
            this.panelIdent.ResumeLayout(false);
            this.panelIdent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxArrow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContents;
        private System.Windows.Forms.PictureBox pictureBoxArrow;
        private System.Windows.Forms.Panel panelIdent;
        private System.Windows.Forms.LinkLabel linkLabelDetails;
        private System.Windows.Forms.Label labelMessage;
    }
}
