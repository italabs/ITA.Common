using System;
using System.ComponentModel;
using System.Windows.Forms;
using ITA.Common.UI;

namespace ITA.Common.ErrorUITest
{
    /// <summary>
    /// Форма для отладки различных режимов диалога показа ошибок.
    /// </summary>
    public class Form1 : Form
    {
        private Button button3;
        private RadioButton radioButtonImageNone;
        private GroupBox groupBox1;
        private RadioButton radioButtonImageError;
        private RadioButton radioButtonImageWarning;
        private RadioButton radioButtonImageQuestion;
        private RadioButton radioButtonImageInformation;
        private GroupBox groupBox2;
        private RadioButton radioButtonAbortRetryIgnore;
        private RadioButton radioButtonYesNoCancel;
        private RadioButton radioButtonYesNo;
        private RadioButton radioButtonOKCancel;
        private RadioButton radioButtonOK;
        private RadioButton radioButtonRetryCancel;
        private GroupBox groupBox3;
        private RadioButton radioButtonDef3;
        private RadioButton radioButtonDef2;
        private RadioButton radioButtonDef1;
        private NumericUpDown numericUpDown1;
        private Label label1;
        private Label label2;
        private Button button4;
        private TextBox textBoxCaption;
        private Label label3;
        private TextBox textBoxMessage;
        private Label label4;
        private Label label5;
		private TextBox textBoxError;
		private ImageList imageList;
		private GroupBox groupBox4;
		private RadioButton radioButtonPic2;
		private RadioButton radioButtonPic1;
        private Label label6;
        private TextBox textBoxDetails;
        private CheckBox checkBoxDetails;
        private Button btnFolderSelectDialog;
		private IContainer components;

        public Form1()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button3 = new System.Windows.Forms.Button();
            this.radioButtonImageNone = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonImageError = new System.Windows.Forms.RadioButton();
            this.radioButtonImageWarning = new System.Windows.Forms.RadioButton();
            this.radioButtonImageQuestion = new System.Windows.Forms.RadioButton();
            this.radioButtonImageInformation = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonRetryCancel = new System.Windows.Forms.RadioButton();
            this.radioButtonAbortRetryIgnore = new System.Windows.Forms.RadioButton();
            this.radioButtonYesNoCancel = new System.Windows.Forms.RadioButton();
            this.radioButtonYesNo = new System.Windows.Forms.RadioButton();
            this.radioButtonOKCancel = new System.Windows.Forms.RadioButton();
            this.radioButtonOK = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonDef3 = new System.Windows.Forms.RadioButton();
            this.radioButtonDef2 = new System.Windows.Forms.RadioButton();
            this.radioButtonDef1 = new System.Windows.Forms.RadioButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxError = new System.Windows.Forms.TextBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButtonPic2 = new System.Windows.Forms.RadioButton();
            this.radioButtonPic1 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDetails = new System.Windows.Forms.TextBox();
            this.checkBoxDetails = new System.Windows.Forms.CheckBox();
            this.btnFolderSelectDialog = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(447, 263);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 23);
            this.button3.TabIndex = 7;
            this.button3.Text = "Test";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // radioButtonImageNone
            // 
            this.radioButtonImageNone.AutoSize = true;
            this.radioButtonImageNone.Location = new System.Drawing.Point(15, 19);
            this.radioButtonImageNone.Name = "radioButtonImageNone";
            this.radioButtonImageNone.Size = new System.Drawing.Size(51, 17);
            this.radioButtonImageNone.TabIndex = 0;
            this.radioButtonImageNone.Text = "None";
            this.radioButtonImageNone.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonImageError);
            this.groupBox1.Controls.Add(this.radioButtonImageWarning);
            this.groupBox1.Controls.Add(this.radioButtonImageQuestion);
            this.groupBox1.Controls.Add(this.radioButtonImageInformation);
            this.groupBox1.Controls.Add(this.radioButtonImageNone);
            this.groupBox1.Location = new System.Drawing.Point(12, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(121, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Icon";
            // 
            // radioButtonImageError
            // 
            this.radioButtonImageError.AutoSize = true;
            this.radioButtonImageError.Location = new System.Drawing.Point(15, 111);
            this.radioButtonImageError.Name = "radioButtonImageError";
            this.radioButtonImageError.Size = new System.Drawing.Size(47, 17);
            this.radioButtonImageError.TabIndex = 4;
            this.radioButtonImageError.Text = "Error";
            this.radioButtonImageError.UseVisualStyleBackColor = true;
            // 
            // radioButtonImageWarning
            // 
            this.radioButtonImageWarning.AutoSize = true;
            this.radioButtonImageWarning.Location = new System.Drawing.Point(15, 88);
            this.radioButtonImageWarning.Name = "radioButtonImageWarning";
            this.radioButtonImageWarning.Size = new System.Drawing.Size(65, 17);
            this.radioButtonImageWarning.TabIndex = 3;
            this.radioButtonImageWarning.Text = "Warning";
            this.radioButtonImageWarning.UseVisualStyleBackColor = true;
            // 
            // radioButtonImageQuestion
            // 
            this.radioButtonImageQuestion.AutoSize = true;
            this.radioButtonImageQuestion.Location = new System.Drawing.Point(15, 65);
            this.radioButtonImageQuestion.Name = "radioButtonImageQuestion";
            this.radioButtonImageQuestion.Size = new System.Drawing.Size(67, 17);
            this.radioButtonImageQuestion.TabIndex = 2;
            this.radioButtonImageQuestion.Text = "Question";
            this.radioButtonImageQuestion.UseVisualStyleBackColor = true;
            // 
            // radioButtonImageInformation
            // 
            this.radioButtonImageInformation.AutoSize = true;
            this.radioButtonImageInformation.Checked = true;
            this.radioButtonImageInformation.Location = new System.Drawing.Point(15, 42);
            this.radioButtonImageInformation.Name = "radioButtonImageInformation";
            this.radioButtonImageInformation.Size = new System.Drawing.Size(77, 17);
            this.radioButtonImageInformation.TabIndex = 1;
            this.radioButtonImageInformation.TabStop = true;
            this.radioButtonImageInformation.Text = "Information";
            this.radioButtonImageInformation.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonRetryCancel);
            this.groupBox2.Controls.Add(this.radioButtonAbortRetryIgnore);
            this.groupBox2.Controls.Add(this.radioButtonYesNoCancel);
            this.groupBox2.Controls.Add(this.radioButtonYesNo);
            this.groupBox2.Controls.Add(this.radioButtonOKCancel);
            this.groupBox2.Controls.Add(this.radioButtonOK);
            this.groupBox2.Location = new System.Drawing.Point(139, 14);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 164);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Buttons";
            // 
            // radioButtonRetryCancel
            // 
            this.radioButtonRetryCancel.AutoSize = true;
            this.radioButtonRetryCancel.Location = new System.Drawing.Point(15, 134);
            this.radioButtonRetryCancel.Name = "radioButtonRetryCancel";
            this.radioButtonRetryCancel.Size = new System.Drawing.Size(86, 17);
            this.radioButtonRetryCancel.TabIndex = 5;
            this.radioButtonRetryCancel.TabStop = true;
            this.radioButtonRetryCancel.Text = "Retry Cancel";
            this.radioButtonRetryCancel.UseVisualStyleBackColor = true;
            // 
            // radioButtonAbortRetryIgnore
            // 
            this.radioButtonAbortRetryIgnore.AutoSize = true;
            this.radioButtonAbortRetryIgnore.Location = new System.Drawing.Point(15, 111);
            this.radioButtonAbortRetryIgnore.Name = "radioButtonAbortRetryIgnore";
            this.radioButtonAbortRetryIgnore.Size = new System.Drawing.Size(111, 17);
            this.radioButtonAbortRetryIgnore.TabIndex = 4;
            this.radioButtonAbortRetryIgnore.TabStop = true;
            this.radioButtonAbortRetryIgnore.Text = "Abort Retry Ignore";
            this.radioButtonAbortRetryIgnore.UseVisualStyleBackColor = true;
            // 
            // radioButtonYesNoCancel
            // 
            this.radioButtonYesNoCancel.AutoSize = true;
            this.radioButtonYesNoCancel.Location = new System.Drawing.Point(15, 88);
            this.radioButtonYesNoCancel.Name = "radioButtonYesNoCancel";
            this.radioButtonYesNoCancel.Size = new System.Drawing.Size(96, 17);
            this.radioButtonYesNoCancel.TabIndex = 3;
            this.radioButtonYesNoCancel.TabStop = true;
            this.radioButtonYesNoCancel.Text = "Yes No Cancel";
            this.radioButtonYesNoCancel.UseVisualStyleBackColor = true;
            // 
            // radioButtonYesNo
            // 
            this.radioButtonYesNo.AutoSize = true;
            this.radioButtonYesNo.Location = new System.Drawing.Point(15, 65);
            this.radioButtonYesNo.Name = "radioButtonYesNo";
            this.radioButtonYesNo.Size = new System.Drawing.Size(60, 17);
            this.radioButtonYesNo.TabIndex = 2;
            this.radioButtonYesNo.TabStop = true;
            this.radioButtonYesNo.Text = "Yes No";
            this.radioButtonYesNo.UseVisualStyleBackColor = true;
            // 
            // radioButtonOKCancel
            // 
            this.radioButtonOKCancel.AutoSize = true;
            this.radioButtonOKCancel.Checked = true;
            this.radioButtonOKCancel.Location = new System.Drawing.Point(15, 42);
            this.radioButtonOKCancel.Name = "radioButtonOKCancel";
            this.radioButtonOKCancel.Size = new System.Drawing.Size(76, 17);
            this.radioButtonOKCancel.TabIndex = 1;
            this.radioButtonOKCancel.TabStop = true;
            this.radioButtonOKCancel.Text = "OK Cancel";
            this.radioButtonOKCancel.UseVisualStyleBackColor = true;
            // 
            // radioButtonOK
            // 
            this.radioButtonOK.AutoSize = true;
            this.radioButtonOK.Location = new System.Drawing.Point(15, 19);
            this.radioButtonOK.Name = "radioButtonOK";
            this.radioButtonOK.Size = new System.Drawing.Size(40, 17);
            this.radioButtonOK.TabIndex = 0;
            this.radioButtonOK.TabStop = true;
            this.radioButtonOK.Text = "OK";
            this.radioButtonOK.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonDef3);
            this.groupBox3.Controls.Add(this.radioButtonDef2);
            this.groupBox3.Controls.Add(this.radioButtonDef1);
            this.groupBox3.Location = new System.Drawing.Point(282, 14);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 164);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Default";
            // 
            // radioButtonDef3
            // 
            this.radioButtonDef3.AutoSize = true;
            this.radioButtonDef3.Location = new System.Drawing.Point(15, 65);
            this.radioButtonDef3.Name = "radioButtonDef3";
            this.radioButtonDef3.Size = new System.Drawing.Size(65, 17);
            this.radioButtonDef3.TabIndex = 2;
            this.radioButtonDef3.TabStop = true;
            this.radioButtonDef3.Text = "Button 3";
            this.radioButtonDef3.UseVisualStyleBackColor = true;
            // 
            // radioButtonDef2
            // 
            this.radioButtonDef2.AutoSize = true;
            this.radioButtonDef2.Location = new System.Drawing.Point(15, 42);
            this.radioButtonDef2.Name = "radioButtonDef2";
            this.radioButtonDef2.Size = new System.Drawing.Size(65, 17);
            this.radioButtonDef2.TabIndex = 1;
            this.radioButtonDef2.TabStop = true;
            this.radioButtonDef2.Text = "Button 2";
            this.radioButtonDef2.UseVisualStyleBackColor = true;
            // 
            // radioButtonDef1
            // 
            this.radioButtonDef1.AutoSize = true;
            this.radioButtonDef1.Location = new System.Drawing.Point(15, 19);
            this.radioButtonDef1.Name = "radioButtonDef1";
            this.radioButtonDef1.Size = new System.Drawing.Size(65, 17);
            this.radioButtonDef1.TabIndex = 0;
            this.radioButtonDef1.TabStop = true;
            this.radioButtonDef1.Text = "Button 1";
            this.radioButtonDef1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(70, 243);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(42, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 245);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Build";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 245);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "nested exceptions";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(447, 294);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 23);
            this.button4.TabIndex = 8;
            this.button4.Text = "Exit";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.Location = new System.Drawing.Point(68, 184);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.Size = new System.Drawing.Size(351, 20);
            this.textBoxCaption.TabIndex = 4;
            this.textBoxCaption.Text = "TestUI App";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 274);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Message";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(68, 271);
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(351, 20);
            this.textBoxMessage.TabIndex = 5;
            this.textBoxMessage.Text = "Test exception message";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Caption";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 300);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Error";
            // 
            // textBoxError
            // 
            this.textBoxError.Location = new System.Drawing.Point(68, 297);
            this.textBoxError.Name = "textBoxError";
            this.textBoxError.Size = new System.Drawing.Size(351, 20);
            this.textBoxError.TabIndex = 6;
            this.textBoxError.Text = "Error";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "DownDetails_test.bmp");
            this.imageList.Images.SetKeyName(1, "DownDetailsFocused_test.bmp");
            this.imageList.Images.SetKeyName(2, "TopBackground_test.bmp");
            this.imageList.Images.SetKeyName(3, "UpDetails_test.bmp");
            this.imageList.Images.SetKeyName(4, "UpDetailsFocused_test.bmp");
            this.imageList.Images.SetKeyName(5, "Arrow_test.gif");
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.radioButtonPic2);
            this.groupBox4.Controls.Add(this.radioButtonPic1);
            this.groupBox4.Location = new System.Drawing.Point(437, 14);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(106, 82);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pictures";
            // 
            // radioButtonPic2
            // 
            this.radioButtonPic2.AutoSize = true;
            this.radioButtonPic2.Location = new System.Drawing.Point(10, 43);
            this.radioButtonPic2.Name = "radioButtonPic2";
            this.radioButtonPic2.Size = new System.Drawing.Size(60, 17);
            this.radioButtonPic2.TabIndex = 0;
            this.radioButtonPic2.Text = "Custom";
            this.radioButtonPic2.UseVisualStyleBackColor = true;
            // 
            // radioButtonPic1
            // 
            this.radioButtonPic1.AutoSize = true;
            this.radioButtonPic1.Checked = true;
            this.radioButtonPic1.Location = new System.Drawing.Point(10, 20);
            this.radioButtonPic1.Name = "radioButtonPic1";
            this.radioButtonPic1.Size = new System.Drawing.Size(59, 17);
            this.radioButtonPic1.TabIndex = 0;
            this.radioButtonPic1.TabStop = true;
            this.radioButtonPic1.Text = "Default";
            this.radioButtonPic1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 213);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Details";
            // 
            // textBoxDetails
            // 
            this.textBoxDetails.Location = new System.Drawing.Point(68, 210);
            this.textBoxDetails.Name = "textBoxDetails";
            this.textBoxDetails.Size = new System.Drawing.Size(351, 20);
            this.textBoxDetails.TabIndex = 4;
            this.textBoxDetails.Text = "TestUI Details";
            // 
            // checkBoxDetails
            // 
            this.checkBoxDetails.AutoSize = true;
            this.checkBoxDetails.Location = new System.Drawing.Point(437, 114);
            this.checkBoxDetails.Name = "checkBoxDetails";
            this.checkBoxDetails.Size = new System.Drawing.Size(130, 17);
            this.checkBoxDetails.TabIndex = 10;
            this.checkBoxDetails.Text = "Allow technical details";
            this.checkBoxDetails.UseVisualStyleBackColor = true;
            // 
            // btnFolderSelectDialog
            // 
            this.btnFolderSelectDialog.Location = new System.Drawing.Point(447, 182);
            this.btnFolderSelectDialog.Name = "btnFolderSelectDialog";
            this.btnFolderSelectDialog.Size = new System.Drawing.Size(112, 23);
            this.btnFolderSelectDialog.TabIndex = 11;
            this.btnFolderSelectDialog.Text = "FolderSelectDialog";
            this.btnFolderSelectDialog.Click += new System.EventHandler(this.btnFolderSelectDialog_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.button4;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(580, 341);
            this.Controls.Add(this.btnFolderSelectDialog);
            this.Controls.Add(this.checkBoxDetails);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.textBoxError);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.textBoxDetails);
            this.Controls.Add(this.textBoxCaption);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "ErrorMessageBox Test";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private void BuildException(string Text, decimal RequestedLevel, int Level)
        {
            if (Level < RequestedLevel - 1)
            {
                try
                {
                    BuildException(Text, RequestedLevel, Level + 1);
                }
                catch (Exception X)
                {
                    throw new Exception(string.Format("{0}. level {1}", Text, Level), X);
                }
            }
            else
            {
                // Trivial case
                throw new Exception(string.Format("{0}. level {1}", Text, Level));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBoxIcon Icon = MessageBoxIcon.None;
            if (radioButtonImageNone.Checked) Icon = MessageBoxIcon.None;
            else if (radioButtonImageInformation.Checked) Icon = MessageBoxIcon.Information;
            else if (radioButtonImageQuestion.Checked) Icon = MessageBoxIcon.Question;
            else if (radioButtonImageWarning.Checked) Icon = MessageBoxIcon.Warning;
            else if (radioButtonImageError.Checked) Icon = MessageBoxIcon.Error;

            MessageBoxButtons Buttons = MessageBoxButtons.OK;
            if (radioButtonOK.Checked) Buttons = MessageBoxButtons.OK;
            else if (radioButtonOKCancel.Checked) Buttons = MessageBoxButtons.OKCancel;
            else if (radioButtonYesNo.Checked) Buttons = MessageBoxButtons.YesNo;
            else if (radioButtonYesNoCancel.Checked) Buttons = MessageBoxButtons.YesNoCancel;
            else if (radioButtonRetryCancel.Checked) Buttons = MessageBoxButtons.RetryCancel;
            else if (radioButtonAbortRetryIgnore.Checked) Buttons = MessageBoxButtons.AbortRetryIgnore;

            MessageBoxDefaultButton DefButton = MessageBoxDefaultButton.Button1;
            if (radioButtonDef1.Checked) DefButton = MessageBoxDefaultButton.Button1;
            else if (radioButtonDef2.Checked) DefButton = MessageBoxDefaultButton.Button2;
            else if (radioButtonDef3.Checked) DefButton = MessageBoxDefaultButton.Button3;

			if (radioButtonPic1.Checked)
			{
				ErrorMessageBox.SetPicture(EPictureType.Arrow, null);
				ErrorMessageBox.SetPicture(EPictureType.DownDetails, null);
				ErrorMessageBox.SetPicture(EPictureType.DownDetailsFocused, null);
				ErrorMessageBox.SetPicture(EPictureType.UpDetails, null);
				ErrorMessageBox.SetPicture(EPictureType.UpDetailsFocused, null);
				ErrorMessageBox.SetPicture(EPictureType.TopBackground, null);
			}
			else if (radioButtonPic2.Checked)
			{
				ErrorMessageBox.SetPicture(EPictureType.Arrow, imageList.Images["Arrow_test.gif"]);
				ErrorMessageBox.SetPicture(EPictureType.DownDetails, imageList.Images["DownDetails_test.bmp"]);
				ErrorMessageBox.SetPicture(EPictureType.DownDetailsFocused, imageList.Images["DownDetailsFocused_test.bmp"]);
				ErrorMessageBox.SetPicture(EPictureType.UpDetails, imageList.Images["UpDetails_test.bmp"]);
				ErrorMessageBox.SetPicture(EPictureType.UpDetailsFocused, imageList.Images["UpDetailsFocused_test.bmp"]);
				ErrorMessageBox.SetPicture(EPictureType.TopBackground, imageList.Images["TopBackground_test.bmp"]);
			}

            ErrorMessageBox.ShowTechnicalDetails = checkBoxDetails.Checked;

            Exception Error = null;
            try
            {
                if (numericUpDown1.Value > 0)
                {
                    BuildException(textBoxError.Text, numericUpDown1.Value, 0);
                }
            }
            catch (Exception X)
            {
                Error = X;
                Error.HelpLink = "http://www.it-assist.ru";
            }

            DialogResult Result;

            if (Error == null)
            {
                Result = RichMessageBox.Show(this,
                                              textBoxDetails.Text,
                                              textBoxMessage.Text,
                                              textBoxCaption.Text,
                                              Buttons,
                                              Icon,
                                              DefButton);
            }
            else
            {
                Result = ErrorMessageBox.Show(this,
                                              Error,
                                              textBoxMessage.Text,
                                              textBoxCaption.Text,
                                              Buttons,
                                              Icon,
                                              DefButton);
            }
            
            MessageBox.Show(this,Result.ToString(), "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFolderSelectDialog_Click(object sender, EventArgs e)
        {
            FolderSelectDialog frmDialog = new FolderSelectDialog();
            frmDialog.InitialDirectory = "C:\\";
            frmDialog.Title = "Test dialog";
            frmDialog.ShowDialog();
        }
    }
}
