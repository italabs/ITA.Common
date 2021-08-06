using ITA.Wizards.DatabaseWizard.Controls;

namespace ITA.WizardsUITest
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chAzureSql = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chMySql = new System.Windows.Forms.CheckBox();
            this.chOracle = new System.Windows.Forms.CheckBox();
            this.chMSSQL = new System.Windows.Forms.CheckBox();
            this.rdbUpdateMySql = new System.Windows.Forms.RadioButton();
            this.cmbCulture = new System.Windows.Forms.ComboBox();
            this.chkbEnableChangingDatabaseLogin = new System.Windows.Forms.CheckBox();
            this.chkbShowSelectOperation = new System.Windows.Forms.CheckBox();
            this.chkbChangeUserCredentialType = new System.Windows.Forms.CheckBox();
            this.chkbChangeServerCredentialType = new System.Windows.Forms.CheckBox();
            this.chbSelectProvider = new System.Windows.Forms.CheckBox();
            this.rdbUpdateOracle = new System.Windows.Forms.RadioButton();
            this.chbxInvalidUpdateRules = new System.Windows.Forms.CheckBox();
            this.chbxError = new System.Windows.Forms.CheckBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.rdbUpdateSqlServer = new System.Windows.Forms.RadioButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.rbnSQL = new System.Windows.Forms.RadioButton();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rbnNT = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelLogin = new System.Windows.Forms.Label();
            this.chkbCreateNewDB = new System.Windows.Forms.CheckBox();
            this.groupBoxCredentials = new System.Windows.Forms.GroupBox();
            this.textBoxDBName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.rdbtNTDatabase = new System.Windows.Forms.RadioButton();
            this.checkBoxCreateNewUser = new System.Windows.Forms.CheckBox();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxUserID = new ITA.Wizards.DatabaseWizard.Controls.TextBoxWithValidator();
            this.progressText = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cbDefaultDbProvider = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBoxCredentials.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(312, 389);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Запуск мастера БД";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 397);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(306, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(326, 397);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Авторазвертывание";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(3, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 502);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(472, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "UI Режим";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(312, 418);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(154, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Показать диалог";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbDefaultDbProvider);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.chAzureSql);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.chMySql);
            this.groupBox1.Controls.Add(this.chOracle);
            this.groupBox1.Controls.Add(this.chMSSQL);
            this.groupBox1.Controls.Add(this.rdbUpdateMySql);
            this.groupBox1.Controls.Add(this.cmbCulture);
            this.groupBox1.Controls.Add(this.chkbEnableChangingDatabaseLogin);
            this.groupBox1.Controls.Add(this.chkbShowSelectOperation);
            this.groupBox1.Controls.Add(this.chkbChangeUserCredentialType);
            this.groupBox1.Controls.Add(this.chkbChangeServerCredentialType);
            this.groupBox1.Controls.Add(this.chbSelectProvider);
            this.groupBox1.Controls.Add(this.rdbUpdateOracle);
            this.groupBox1.Controls.Add(this.chbxInvalidUpdateRules);
            this.groupBox1.Controls.Add(this.chbxError);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.rdbUpdateSqlServer);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(466, 377);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры";
            // 
            // chAzureSql
            // 
            this.chAzureSql.AutoSize = true;
            this.chAzureSql.Checked = true;
            this.chAzureSql.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAzureSql.Location = new System.Drawing.Point(382, 22);
            this.chAzureSql.Name = "chAzureSql";
            this.chAzureSql.Size = new System.Drawing.Size(74, 17);
            this.chAzureSql.TabIndex = 14;
            this.chAzureSql.Text = "AzureSQL";
            this.chAzureSql.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Поддерживаемые БД:";
            // 
            // chMySql
            // 
            this.chMySql.AutoSize = true;
            this.chMySql.Checked = true;
            this.chMySql.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chMySql.Location = new System.Drawing.Point(306, 22);
            this.chMySql.Name = "chMySql";
            this.chMySql.Size = new System.Drawing.Size(55, 17);
            this.chMySql.TabIndex = 12;
            this.chMySql.Text = "MySql";
            this.chMySql.UseVisualStyleBackColor = true;
            // 
            // chOracle
            // 
            this.chOracle.AutoSize = true;
            this.chOracle.Checked = true;
            this.chOracle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chOracle.Location = new System.Drawing.Point(227, 22);
            this.chOracle.Name = "chOracle";
            this.chOracle.Size = new System.Drawing.Size(57, 17);
            this.chOracle.TabIndex = 12;
            this.chOracle.Text = "Oracle";
            this.chOracle.UseVisualStyleBackColor = true;
            // 
            // chMSSQL
            // 
            this.chMSSQL.AutoSize = true;
            this.chMSSQL.Checked = true;
            this.chMSSQL.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chMSSQL.Location = new System.Drawing.Point(142, 22);
            this.chMSSQL.Name = "chMSSQL";
            this.chMSSQL.Size = new System.Drawing.Size(75, 17);
            this.chMSSQL.TabIndex = 12;
            this.chMSSQL.Text = "Sql Server";
            this.chMSSQL.UseVisualStyleBackColor = true;
            // 
            // rdbUpdateMySql
            // 
            this.rdbUpdateMySql.AutoSize = true;
            this.rdbUpdateMySql.Location = new System.Drawing.Point(9, 124);
            this.rdbUpdateMySql.Name = "rdbUpdateMySql";
            this.rdbUpdateMySql.Size = new System.Drawing.Size(245, 17);
            this.rdbUpdateMySql.TabIndex = 11;
            this.rdbUpdateMySql.TabStop = true;
            this.rdbUpdateMySql.Text = "Задать тестовую конфигурацию БД (MySql)";
            this.rdbUpdateMySql.UseVisualStyleBackColor = true;
            // 
            // cmbCulture
            // 
            this.cmbCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCulture.FormattingEnabled = true;
            this.cmbCulture.Items.AddRange(new object[] {
            "ru-RU",
            "en-US"});
            this.cmbCulture.Location = new System.Drawing.Point(9, 332);
            this.cmbCulture.Name = "cmbCulture";
            this.cmbCulture.Size = new System.Drawing.Size(224, 21);
            this.cmbCulture.TabIndex = 10;
            // 
            // chkbEnableChangingDatabaseLogin
            // 
            this.chkbEnableChangingDatabaseLogin.AutoSize = true;
            this.chkbEnableChangingDatabaseLogin.Checked = true;
            this.chkbEnableChangingDatabaseLogin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbEnableChangingDatabaseLogin.Location = new System.Drawing.Point(8, 285);
            this.chkbEnableChangingDatabaseLogin.Name = "chkbEnableChangingDatabaseLogin";
            this.chkbEnableChangingDatabaseLogin.Size = new System.Drawing.Size(340, 17);
            this.chkbEnableChangingDatabaseLogin.TabIndex = 9;
            this.chkbEnableChangingDatabaseLogin.Text = "Разрешать менять тип пользователя (новый, существующий)";
            this.chkbEnableChangingDatabaseLogin.UseVisualStyleBackColor = true;
            // 
            // chkbShowSelectOperation
            // 
            this.chkbShowSelectOperation.AutoSize = true;
            this.chkbShowSelectOperation.Checked = true;
            this.chkbShowSelectOperation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbShowSelectOperation.Location = new System.Drawing.Point(8, 308);
            this.chkbShowSelectOperation.Name = "chkbShowSelectOperation";
            this.chkbShowSelectOperation.Size = new System.Drawing.Size(240, 17);
            this.chkbShowSelectOperation.TabIndex = 8;
            this.chkbShowSelectOperation.Text = "Показывать экран выбора типа операции";
            this.chkbShowSelectOperation.UseVisualStyleBackColor = true;
            // 
            // chkbChangeUserCredentialType
            // 
            this.chkbChangeUserCredentialType.AutoSize = true;
            this.chkbChangeUserCredentialType.Checked = true;
            this.chkbChangeUserCredentialType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbChangeUserCredentialType.Location = new System.Drawing.Point(8, 262);
            this.chkbChangeUserCredentialType.Name = "chkbChangeUserCredentialType";
            this.chkbChangeUserCredentialType.Size = new System.Drawing.Size(387, 17);
            this.chkbChangeUserCredentialType.TabIndex = 7;
            this.chkbChangeUserCredentialType.Text = "Разрешать менять тип пользовательского подключения к серверу БД";
            this.chkbChangeUserCredentialType.UseVisualStyleBackColor = true;
            // 
            // chkbChangeServerCredentialType
            // 
            this.chkbChangeServerCredentialType.AutoSize = true;
            this.chkbChangeServerCredentialType.Checked = true;
            this.chkbChangeServerCredentialType.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbChangeServerCredentialType.Location = new System.Drawing.Point(9, 239);
            this.chkbChangeServerCredentialType.Name = "chkbChangeServerCredentialType";
            this.chkbChangeServerCredentialType.Size = new System.Drawing.Size(388, 17);
            this.chkbChangeServerCredentialType.TabIndex = 6;
            this.chkbChangeServerCredentialType.Text = "Разрешать менять тип административного подключения к серверу БД";
            this.chkbChangeServerCredentialType.UseVisualStyleBackColor = true;
            // 
            // chbSelectProvider
            // 
            this.chbSelectProvider.AutoSize = true;
            this.chbSelectProvider.Checked = true;
            this.chbSelectProvider.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbSelectProvider.Location = new System.Drawing.Point(9, 216);
            this.chbSelectProvider.Name = "chbSelectProvider";
            this.chbSelectProvider.Size = new System.Drawing.Size(206, 17);
            this.chbSelectProvider.TabIndex = 5;
            this.chbSelectProvider.Text = "Показывать выбор провайдера БД";
            this.chbSelectProvider.UseVisualStyleBackColor = true;
            // 
            // rdbUpdateOracle
            // 
            this.rdbUpdateOracle.AutoSize = true;
            this.rdbUpdateOracle.Location = new System.Drawing.Point(9, 101);
            this.rdbUpdateOracle.Name = "rdbUpdateOracle";
            this.rdbUpdateOracle.Size = new System.Drawing.Size(247, 17);
            this.rdbUpdateOracle.TabIndex = 4;
            this.rdbUpdateOracle.Text = "Задать тестовую конфигурацию БД (Oracle)";
            this.rdbUpdateOracle.UseVisualStyleBackColor = true;
            // 
            // chbxInvalidUpdateRules
            // 
            this.chbxInvalidUpdateRules.AutoSize = true;
            this.chbxInvalidUpdateRules.Location = new System.Drawing.Point(9, 194);
            this.chbxInvalidUpdateRules.Name = "chbxInvalidUpdateRules";
            this.chbxInvalidUpdateRules.Size = new System.Drawing.Size(295, 17);
            this.chbxInvalidUpdateRules.TabIndex = 3;
            this.chbxInvalidUpdateRules.Text = "Эмуляция загрузки невалидного списка обновлений";
            this.chbxInvalidUpdateRules.UseVisualStyleBackColor = true;
            // 
            // chbxError
            // 
            this.chbxError.AutoSize = true;
            this.chbxError.Location = new System.Drawing.Point(9, 170);
            this.chbxError.Name = "chbxError";
            this.chbxError.Size = new System.Drawing.Size(194, 17);
            this.chbxError.TabIndex = 2;
            this.chbxError.Text = "Эмуляция сбоя при создании БД";
            this.chbxError.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(9, 147);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(281, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Запустить мастер БД без тестовой конфигурации";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // rdbUpdateSqlServer
            // 
            this.rdbUpdateSqlServer.AutoSize = true;
            this.rdbUpdateSqlServer.Checked = true;
            this.rdbUpdateSqlServer.Location = new System.Drawing.Point(9, 79);
            this.rdbUpdateSqlServer.Name = "rdbUpdateSqlServer";
            this.rdbUpdateSqlServer.Size = new System.Drawing.Size(265, 17);
            this.rdbUpdateSqlServer.TabIndex = 0;
            this.rdbUpdateSqlServer.TabStop = true;
            this.rdbUpdateSqlServer.Text = "Задать тестовую конфигурацию БД (Sql Server)";
            this.rdbUpdateSqlServer.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.chkbCreateNewDB);
            this.tabPage2.Controls.Add(this.groupBoxCredentials);
            this.tabPage2.Controls.Add(this.progressText);
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.progressBar1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(472, 476);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Авторазвертывание SQL Server";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxPassword);
            this.groupBox2.Controls.Add(this.textBoxLogin);
            this.groupBox2.Controls.Add(this.rbnSQL);
            this.groupBox2.Controls.Add(this.txtServerName);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.rbnNT);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(5, 7);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(442, 197);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Настройки подключения к серверу БД";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Enabled = false;
            this.textBoxPassword.Location = new System.Drawing.Point(244, 150);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(181, 20);
            this.textBoxPassword.TabIndex = 20;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Enabled = false;
            this.textBoxLogin.Location = new System.Drawing.Point(244, 102);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(181, 20);
            this.textBoxLogin.TabIndex = 19;
            // 
            // rbnSQL
            // 
            this.rbnSQL.AutoSize = true;
            this.rbnSQL.Location = new System.Drawing.Point(15, 114);
            this.rbnSQL.Name = "rbnSQL";
            this.rbnSQL.Size = new System.Drawing.Size(145, 17);
            this.rbnSQL.TabIndex = 18;
            this.rbnSQL.Text = "Средствами &SQL Server";
            this.rbnSQL.UseVisualStyleBackColor = true;
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(244, 19);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(181, 20);
            this.txtServerName.TabIndex = 0;
            this.txtServerName.Text = ".\\sqlexpress";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 21);
            this.label1.TabIndex = 6;
            this.label1.Text = "Укажите &сервер БД";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // rbnNT
            // 
            this.rbnNT.AutoSize = true;
            this.rbnNT.Checked = true;
            this.rbnNT.Location = new System.Drawing.Point(15, 90);
            this.rbnNT.Name = "rbnNT";
            this.rbnNT.Size = new System.Drawing.Size(134, 17);
            this.rbnNT.TabIndex = 17;
            this.rbnNT.TabStop = true;
            this.rbnNT.Text = "Средствами &Windows";
            this.rbnNT.UseVisualStyleBackColor = true;
            this.rbnNT.CheckedChanged += new System.EventHandler(this.rbnNT_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(15, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(410, 31);
            this.label2.TabIndex = 21;
            this.label2.Text = "Укажите способ проверки подлинности для административного соединения:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelPassword);
            this.panel1.Controls.Add(this.labelLogin);
            this.panel1.Location = new System.Drawing.Point(9, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 126);
            this.panel1.TabIndex = 22;
            // 
            // labelPassword
            // 
            this.labelPassword.Location = new System.Drawing.Point(232, 71);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(100, 16);
            this.labelPassword.TabIndex = 1;
            this.labelPassword.Text = "&Пароль:";
            this.labelPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelLogin
            // 
            this.labelLogin.Location = new System.Drawing.Point(232, 29);
            this.labelLogin.Name = "labelLogin";
            this.labelLogin.Size = new System.Drawing.Size(100, 16);
            this.labelLogin.TabIndex = 0;
            this.labelLogin.Text = "&Логин:";
            this.labelLogin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkbCreateNewDB
            // 
            this.chkbCreateNewDB.AutoSize = true;
            this.chkbCreateNewDB.Checked = true;
            this.chkbCreateNewDB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkbCreateNewDB.Location = new System.Drawing.Point(9, 371);
            this.chkbCreateNewDB.Name = "chkbCreateNewDB";
            this.chkbCreateNewDB.Size = new System.Drawing.Size(168, 17);
            this.chkbCreateNewDB.TabIndex = 18;
            this.chkbCreateNewDB.Text = "Создать новую базу данных";
            this.chkbCreateNewDB.UseVisualStyleBackColor = true;
            // 
            // groupBoxCredentials
            // 
            this.groupBoxCredentials.Controls.Add(this.textBoxDBName);
            this.groupBoxCredentials.Controls.Add(this.label3);
            this.groupBoxCredentials.Controls.Add(this.label4);
            this.groupBoxCredentials.Controls.Add(this.label9);
            this.groupBoxCredentials.Controls.Add(this.radioButton1);
            this.groupBoxCredentials.Controls.Add(this.rdbtNTDatabase);
            this.groupBoxCredentials.Controls.Add(this.checkBoxCreateNewUser);
            this.groupBoxCredentials.Controls.Add(this.txtDBPassword);
            this.groupBoxCredentials.Controls.Add(this.label5);
            this.groupBoxCredentials.Controls.Add(this.label6);
            this.groupBoxCredentials.Controls.Add(this.textBoxUserID);
            this.groupBoxCredentials.Location = new System.Drawing.Point(9, 204);
            this.groupBoxCredentials.Name = "groupBoxCredentials";
            this.groupBoxCredentials.Size = new System.Drawing.Size(438, 161);
            this.groupBoxCredentials.TabIndex = 17;
            this.groupBoxCredentials.TabStop = false;
            this.groupBoxCredentials.Text = "Настройки подключения сервера к БД";
            // 
            // textBoxDBName
            // 
            this.textBoxDBName.Location = new System.Drawing.Point(244, 30);
            this.textBoxDBName.Name = "textBoxDBName";
            this.textBoxDBName.Size = new System.Drawing.Size(181, 20);
            this.textBoxDBName.TabIndex = 13;
            this.textBoxDBName.Text = "test";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Location = new System.Drawing.Point(13, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 21);
            this.label3.TabIndex = 12;
            this.label3.Text = "Укажите способ проверки подлинности";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(15, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "Укажите имя БД";
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(15, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(98, 17);
            this.label9.TabIndex = 0;
            this.label9.Text = "Укажите имя БД";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.BackColor = System.Drawing.Color.Transparent;
            this.radioButton1.Location = new System.Drawing.Point(19, 101);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(145, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.Text = "Средствами &SQL Server";
            this.radioButton1.UseVisualStyleBackColor = false;
            // 
            // rdbtNTDatabase
            // 
            this.rdbtNTDatabase.AutoSize = true;
            this.rdbtNTDatabase.BackColor = System.Drawing.Color.Transparent;
            this.rdbtNTDatabase.Checked = true;
            this.rdbtNTDatabase.Location = new System.Drawing.Point(19, 74);
            this.rdbtNTDatabase.Name = "rdbtNTDatabase";
            this.rdbtNTDatabase.Size = new System.Drawing.Size(134, 17);
            this.rdbtNTDatabase.TabIndex = 1;
            this.rdbtNTDatabase.TabStop = true;
            this.rdbtNTDatabase.Text = "Средствами &Windows";
            this.rdbtNTDatabase.UseVisualStyleBackColor = false;
            this.rdbtNTDatabase.CheckedChanged += new System.EventHandler(this.rdbtNTDatabase_CheckedChanged);
            // 
            // checkBoxCreateNewUser
            // 
            this.checkBoxCreateNewUser.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxCreateNewUser.Checked = true;
            this.checkBoxCreateNewUser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCreateNewUser.Enabled = false;
            this.checkBoxCreateNewUser.Location = new System.Drawing.Point(18, 124);
            this.checkBoxCreateNewUser.Name = "checkBoxCreateNewUser";
            this.checkBoxCreateNewUser.Size = new System.Drawing.Size(192, 24);
            this.checkBoxCreateNewUser.TabIndex = 3;
            this.checkBoxCreateNewUser.Text = "&Создать новый логин";
            this.checkBoxCreateNewUser.UseVisualStyleBackColor = false;
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Enabled = false;
            this.txtDBPassword.Location = new System.Drawing.Point(244, 128);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.Size = new System.Drawing.Size(181, 20);
            this.txtDBPassword.TabIndex = 5;
            this.txtDBPassword.UseSystemPasswordChar = true;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Location = new System.Drawing.Point(241, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(180, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "&Логин:";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Location = new System.Drawing.Point(241, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(184, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "&Пароль:";
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.BackColor = System.Drawing.Color.Transparent;
            this.textBoxUserID.Enabled = false;
            this.textBoxUserID.IsValid = true;
            this.textBoxUserID.Location = new System.Drawing.Point(18, 73);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(406, 47);
            this.textBoxUserID.TabIndex = 4;
            // 
            // progressText
            // 
            this.progressText.AutoSize = true;
            this.progressText.Location = new System.Drawing.Point(6, 439);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(0, 13);
            this.progressText.TabIndex = 5;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(472, 476);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Авторазвертывание MySQL";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(17, 28);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(185, 23);
            this.button4.TabIndex = 0;
            this.button4.Text = "Авторазвертывание";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(140, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Провайдер по-умолчанию:";
            // 
            // cbDefaultDbProvider
            // 
            this.cbDefaultDbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDefaultDbProvider.FormattingEnabled = true;
            this.cbDefaultDbProvider.Items.AddRange(new object[] {
            "MSSQL",
            "Oracle",
            "MySQL",
            "AzureSQL"});
            this.cbDefaultDbProvider.Location = new System.Drawing.Point(152, 45);
            this.cbDefaultDbProvider.Name = "cbDefaultDbProvider";
            this.cbDefaultDbProvider.Size = new System.Drawing.Size(226, 21);
            this.cbDefaultDbProvider.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 504);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "DB Wizard Tests";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBoxCredentials.ResumeLayout(false);
            this.groupBoxCredentials.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chbSelectProvider;
        private System.Windows.Forms.RadioButton rdbUpdateOracle;
        private System.Windows.Forms.CheckBox chbxInvalidUpdateRules;
        private System.Windows.Forms.CheckBox chbxError;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton rdbUpdateSqlServer;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label progressText;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxCredentials;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton rdbtNTDatabase;
        private System.Windows.Forms.CheckBox checkBoxCreateNewUser;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private TextBoxWithValidator textBoxUserID;
        private System.Windows.Forms.CheckBox chkbCreateNewDB;
        private System.Windows.Forms.TextBox textBoxDBName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.RadioButton rbnSQL;
        private System.Windows.Forms.RadioButton rbnNT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelLogin;
		private System.Windows.Forms.CheckBox chkbChangeServerCredentialType;
		private System.Windows.Forms.CheckBox chkbChangeUserCredentialType;
		private System.Windows.Forms.CheckBox chkbShowSelectOperation;
		private System.Windows.Forms.CheckBox chkbEnableChangingDatabaseLogin;
		private System.Windows.Forms.ComboBox cmbCulture;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.RadioButton rdbUpdateMySql;
        private System.Windows.Forms.CheckBox chMySql;
        private System.Windows.Forms.CheckBox chOracle;
        private System.Windows.Forms.CheckBox chMSSQL;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chAzureSql;
        private System.Windows.Forms.ComboBox cbDefaultDbProvider;
        private System.Windows.Forms.Label label8;
    }
}