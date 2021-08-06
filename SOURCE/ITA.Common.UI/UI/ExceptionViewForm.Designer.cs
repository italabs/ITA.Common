namespace ITA.Common.UI
{
    partial class ExceptionViewForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionViewForm));
            this.button1 = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.groupBoxHorizontalRule = new System.Windows.Forms.GroupBox();
            this.linkLabelClipboard = new System.Windows.Forms.LinkLabel();
            this.linkLabelReport = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanelInfo = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMessage = new System.Windows.Forms.Label();
            this.labelTimestamp = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelSource = new System.Windows.Forms.Label();
            this.labelSite = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelData = new System.Windows.Forms.Label();
            this.linkLabelURL = new System.Windows.Forms.LinkLabel();
            this.label8 = new System.Windows.Forms.Label();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelTopMessage = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.tableLayoutPanelInfo.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.groupBoxHorizontalRule);
            this.panelBottom.Controls.Add(this.linkLabelClipboard);
            this.panelBottom.Controls.Add(this.linkLabelReport);
            this.panelBottom.Controls.Add(this.button1);
            resources.ApplyResources(this.panelBottom, "panelBottom");
            this.panelBottom.Name = "panelBottom";
            // 
            // groupBoxHorizontalRule
            // 
            resources.ApplyResources(this.groupBoxHorizontalRule, "groupBoxHorizontalRule");
            this.groupBoxHorizontalRule.Name = "groupBoxHorizontalRule";
            this.groupBoxHorizontalRule.TabStop = false;
            // 
            // linkLabelClipboard
            // 
            resources.ApplyResources(this.linkLabelClipboard, "linkLabelClipboard");
            this.linkLabelClipboard.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabelClipboard.Name = "linkLabelClipboard";
            this.linkLabelClipboard.TabStop = true;
            this.linkLabelClipboard.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelClipboard_LinkClicked);
            // 
            // linkLabelReport
            // 
            resources.ApplyResources(this.linkLabelReport, "linkLabelReport");
            this.linkLabelReport.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabelReport.Name = "linkLabelReport";
            this.linkLabelReport.TabStop = true;
            this.linkLabelReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelReport_LinkClicked);
            // 
            // tableLayoutPanelInfo
            // 
            this.tableLayoutPanelInfo.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.tableLayoutPanelInfo, "tableLayoutPanelInfo");
            this.tableLayoutPanelInfo.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanelInfo.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanelInfo.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanelInfo.Controls.Add(this.labelMessage, 1, 0);
            this.tableLayoutPanelInfo.Controls.Add(this.labelTimestamp, 1, 1);
            this.tableLayoutPanelInfo.Controls.Add(this.labelType, 1, 2);
            this.tableLayoutPanelInfo.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanelInfo.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanelInfo.Controls.Add(this.labelSource, 1, 4);
            this.tableLayoutPanelInfo.Controls.Add(this.labelSite, 1, 5);
            this.tableLayoutPanelInfo.Controls.Add(this.richTextBox1, 1, 7);
            this.tableLayoutPanelInfo.Controls.Add(this.label7, 0, 6);
            this.tableLayoutPanelInfo.Controls.Add(this.labelData, 1, 6);
            this.tableLayoutPanelInfo.Controls.Add(this.linkLabelURL, 1, 3);
            this.tableLayoutPanelInfo.Controls.Add(this.label8, 0, 7);
            this.tableLayoutPanelInfo.Name = "tableLayoutPanelInfo";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Name = "label2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Name = "label4";
            // 
            // labelMessage
            // 
            this.labelMessage.AutoEllipsis = true;
            resources.ApplyResources(this.labelMessage, "labelMessage");
            this.labelMessage.MaximumSize = new System.Drawing.Size(0, 100);
            this.labelMessage.Name = "labelMessage";
            // 
            // labelTimestamp
            // 
            resources.ApplyResources(this.labelTimestamp, "labelTimestamp");
            this.labelTimestamp.Name = "labelTimestamp";
            // 
            // labelType
            // 
            this.labelType.AutoEllipsis = true;
            resources.ApplyResources(this.labelType, "labelType");
            this.labelType.Name = "labelType";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Name = "label6";
            // 
            // labelSource
            // 
            this.labelSource.AutoEllipsis = true;
            resources.ApplyResources(this.labelSource, "labelSource");
            this.labelSource.Name = "labelSource";
            // 
            // labelSite
            // 
            this.labelSite.AutoEllipsis = true;
            resources.ApplyResources(this.labelSite, "labelSite");
            this.labelSite.Name = "labelSite";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.richTextBox1, "richTextBox1");
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Name = "label7";
            // 
            // labelData
            // 
            this.labelData.AutoEllipsis = true;
            resources.ApplyResources(this.labelData, "labelData");
            this.labelData.MaximumSize = new System.Drawing.Size(0, 100);
            this.labelData.Name = "labelData";
            // 
            // linkLabelURL
            // 
            resources.ApplyResources(this.linkLabelURL, "linkLabelURL");
            this.linkLabelURL.LinkColor = System.Drawing.SystemColors.HotTrack;
            this.linkLabelURL.Name = "linkLabelURL";
            this.linkLabelURL.TabStop = true;
            this.linkLabelURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelURL_LinkClicked);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label8.Name = "label8";
            // 
            // panelTop
            // 
            this.panelTop.BackgroundImage = Properties.Resources.TopBackground;
            resources.ApplyResources(this.panelTop, "panelTop");
            this.panelTop.Controls.Add(this.labelTopMessage);
            this.panelTop.Name = "panelTop";
            // 
            // labelTopMessage
            // 
            this.labelTopMessage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelTopMessage, "labelTopMessage");
            this.labelTopMessage.Name = "labelTopMessage";
            // 
            // ExceptionViewForm
            // 
            this.AcceptButton = this.button1;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.Controls.Add(this.tableLayoutPanelInfo);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionViewForm";
            this.Load += new System.EventHandler(this.ExceptionViewForm_Load);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.tableLayoutPanelInfo.ResumeLayout(false);
            this.tableLayoutPanelInfo.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Label labelTimestamp;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.Label labelSite;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelData;
        private System.Windows.Forms.LinkLabel linkLabelURL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.LinkLabel linkLabelClipboard;
        private System.Windows.Forms.LinkLabel linkLabelReport;
        private System.Windows.Forms.Label labelTopMessage;
        private System.Windows.Forms.GroupBox groupBoxHorizontalRule;
    }
}