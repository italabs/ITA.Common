namespace ITA.Common.UI
{
    partial class ErrorMessageBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorMessageBox));
            this.panelContent = new System.Windows.Forms.Panel();
            this.flowLayoutPanelErrors = new System.Windows.Forms.FlowLayoutPanel();
            this.panelBottom2 = new System.Windows.Forms.Panel();
            this.groupBoxHorizontalRule = new System.Windows.Forms.GroupBox();
            this.pictureBoxDetails = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.linkLabelDetails = new System.Windows.Forms.LinkLabel();
            this.imageListDetails = new System.Windows.Forms.ImageList(this.components);
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelMessage = new System.Windows.Forms.Label();
            this.pictureBoxIcon = new System.Windows.Forms.PictureBox();
            this.panelContent.SuspendLayout();
            this.panelBottom2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetails)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.panelContent, "panelContent");
            this.panelContent.Controls.Add(this.flowLayoutPanelErrors);
            this.panelContent.Name = "panelContent";
            // 
            // flowLayoutPanelErrors
            // 
            this.flowLayoutPanelErrors.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.flowLayoutPanelErrors, "flowLayoutPanelErrors");
            this.flowLayoutPanelErrors.Name = "flowLayoutPanelErrors";
            // 
            // panelBottom2
            // 
            this.panelBottom2.BackColor = System.Drawing.SystemColors.Control;
            this.panelBottom2.Controls.Add(this.groupBoxHorizontalRule);
            this.panelBottom2.Controls.Add(this.pictureBoxDetails);
            this.panelBottom2.Controls.Add(this.panel2);
            this.panelBottom2.Controls.Add(this.linkLabelDetails);
            resources.ApplyResources(this.panelBottom2, "panelBottom2");
            this.panelBottom2.Name = "panelBottom2";
            // 
            // groupBoxHorizontalRule
            // 
            resources.ApplyResources(this.groupBoxHorizontalRule, "groupBoxHorizontalRule");
            this.groupBoxHorizontalRule.Name = "groupBoxHorizontalRule";
            this.groupBoxHorizontalRule.TabStop = false;
            // 
            // pictureBoxDetails
            // 
            resources.ApplyResources(this.pictureBoxDetails, "pictureBoxDetails");
            this.pictureBoxDetails.Name = "pictureBoxDetails";
            this.pictureBoxDetails.TabStop = false;
            this.pictureBoxDetails.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            this.pictureBoxDetails.Click += new System.EventHandler(this.pictureBoxDetails_Click);
            this.pictureBoxDetails.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.btn2);
            this.panel2.Controls.Add(this.btn1);
            this.panel2.Controls.Add(this.btn3);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // btn2
            // 
            this.btn2.BackColor = System.Drawing.SystemColors.Control;
            this.btn2.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.btn2, "btn2");
            this.btn2.Name = "btn2";
            this.btn2.UseVisualStyleBackColor = true;
            // 
            // btn1
            // 
            this.btn1.BackColor = System.Drawing.SystemColors.Control;
            this.btn1.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.btn1, "btn1");
            this.btn1.Name = "btn1";
            this.btn1.UseVisualStyleBackColor = true;
            // 
            // btn3
            // 
            this.btn3.BackColor = System.Drawing.SystemColors.Control;
            this.btn3.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.btn3, "btn3");
            this.btn3.Name = "btn3";
            this.btn3.UseVisualStyleBackColor = true;
            // 
            // linkLabelDetails
            // 
            resources.ApplyResources(this.linkLabelDetails, "linkLabelDetails");
            this.linkLabelDetails.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabelDetails.Name = "linkLabelDetails";
            this.linkLabelDetails.TabStop = true;
            this.linkLabelDetails.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            this.linkLabelDetails.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            this.linkLabelDetails.MouseEnter += new System.EventHandler(this.pictureBox2_MouseEnter);
            // 
            // imageListDetails
            // 
            this.imageListDetails.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListDetails.ImageStream")));
            this.imageListDetails.TransparentColor = System.Drawing.Color.White;
            this.imageListDetails.Images.SetKeyName(0, "DownDetails.bmp");
            this.imageListDetails.Images.SetKeyName(1, "DownDetailsFocused.bmp");
            this.imageListDetails.Images.SetKeyName(2, "UpDetails.bmp");
            this.imageListDetails.Images.SetKeyName(3, "UpDetailsFocused.bmp");
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.Transparent;
            this.panelTop.BackgroundImage = Properties.Resources.TopBackground;
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Controls.Add(this.labelMessage);
            this.panelTop.Controls.Add(this.pictureBoxIcon);
            this.panelTop.Name = "panelTop";
            // 
            // labelMessage
            // 
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.Name = "labelMessage";
            // 
            // pictureBoxIcon
            // 
            resources.ApplyResources(this.pictureBoxIcon, "pictureBoxIcon");
            this.pictureBoxIcon.Name = "pictureBoxIcon";
            this.pictureBoxIcon.TabStop = false;
            // 
            // ErrorMessageBox
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelBottom2);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox";
            this.Load += new System.EventHandler(this.ErrorMessageBox2_Load);
            this.SizeChanged += new System.EventHandler(this.ErrorMessageBox2_SizeChanged);
            this.Shown += new System.EventHandler(this.ErrorMessageBox2_Shown);
            this.panelContent.ResumeLayout(false);
            this.panelBottom2.ResumeLayout(false);
            this.panelBottom2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDetails)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBoxIcon;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelErrors;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Panel panelBottom2;
        private System.Windows.Forms.GroupBox groupBoxHorizontalRule;
        private System.Windows.Forms.LinkLabel linkLabelDetails;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBoxDetails;
        private System.Windows.Forms.ImageList imageListDetails;
    }
}