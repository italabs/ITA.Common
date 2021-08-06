using System;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ITA.Common;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using log4net;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    public class ProgressPage : CustomPage
    {
        #region Delegates

        public delegate void DatabaseStepDelegate();


        #endregion

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(ProgressPage).Name);
        private readonly IContainer components;
        private Font OldFont;
        private DbConnection _connection;

        private Label label1;
        private PictureBox pictureBoxError;
        private PictureBox pictureBoxOK;
        private PictureBox pictureBoxWait;

        private Panel stagePanel;

        public ProgressPage()
        {
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_DB_CREATING;
			this.labelDescription.Text = Messages.WIZ_PERFORMING_OPERATION;
			this.label1.Text = Messages.WIZ_CREATING_AND_CONFIGURATING;
        }

        public DatabaseWizardContext Context
        {
            get { return Wizard.Context.ValueOf<DatabaseWizardContext>(DatabaseWizardContext.ClassName); }
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

        public override void OnActive()
        {
            Wizard.DisableButton(Wizard.EButtons.CancelButton);
            Wizard.DisableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.NextButton);
            Wizard.DisableButton(Wizard.EButtons.FinishButton);

            Wizard.bCanClose = false;

            base.OnActive();

            var T = new Thread(Execute);
            T.Start();
        }


        /// <summary>
        /// UI-thread safe invoke
        /// </summary>
        /// <param name="d"></param>
        private void SyncImpl(SyncDelegate d)
        {
            d.Invoke();
        }

        /// <summary>
        /// Synchronize UI and work threads
        /// </summary>
        /// <param name="d"></param>
        private void Sync(SyncDelegate d)
        {
            if (InvokeRequired)
            {
                Invoke(new SyncImplDelegate(SyncImpl), d);
            }
            else
            {
                SyncImpl(d);
            }
        }

        /// <summary>
        /// Create database objects in new thread
        /// </summary>
        public void Execute()
        {
            try
            {
                Sync(delegate
                         {
                             Cursor = Cursors.WaitCursor;
                             stagePanel.Controls.Clear();
                             stagePanel.Invalidate();
                         });

                var offset = 10;
                var height = 23;
                var labelWidth = stagePanel.Bounds.Width - 4 * offset - 2 * height;
                var x = offset;
                var y = offset;

                foreach (var S in Context.DBProvider.GetStages())
                {
                    Sync(delegate
                             {
                                 S.Label.SetBounds(x, y, labelWidth, height);
                                 S.Pic.SetBounds(S.Label.Bounds.Left + S.Label.Width + offset, y, height, height);

                                 stagePanel.Controls.Add(S.Label);
                                 S.Pic.Image = null;
                                 stagePanel.Controls.Add(S.Pic);

                                 y += offset + height;

                             });
                }


                using (_connection = Context.DBProvider.GetConnection(Context.GetServerConnectionString()))
                {
                    _connection.Open();

                    foreach (var S in Context.DBProvider.GetStages())
                    {
                        Sync(delegate
                        {
                            OldFont = S.Label.Font;
                            S.Pic.Image = pictureBoxWait.Image;
                            stagePanel.ScrollControlIntoView(S.Pic);
                        });

                        try
                        {
                            if (S.Step != null)
                            {
                                //execute step
                                S.Step.Invoke();
                            }

                            Thread.Sleep(300);

                            Sync(delegate
                            {
                                S.Label.Font = OldFont;
                                S.Pic.Image = pictureBoxOK.Image;
                            });
                        }
                        catch
                        {
                            Sync(delegate
                            {
                                S.Pic.Image = pictureBoxError.Image;
                                S.Label.Font = OldFont;
                            });
                            throw;
                        }
                    }
                }

                Sync(delegate { Wizard.EnableButton(Wizard.EButtons.NextButton); });
            }
            catch (Exception x)
            {
                Sync(delegate
                         {
                             ErrorMessageBox.Show(this, x, Messages.E_DBWIZARD_DATABASE_SETUP_FAILED, Wizard.Text,
                                                  MessageBoxButtons.OK, MessageBoxIcon.Error);

                             Wizard.EnableButton(Wizard.EButtons.CancelButton);
                             Wizard.EnableButton(Wizard.EButtons.BackButton);
                         });
            }
            finally
            {
                Sync(delegate
                         {
                             Cursor = Cursors.Default;
                             Wizard.bCanClose = true;
                         });
            }
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressPage));
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxOK = new System.Windows.Forms.PictureBox();
            this.pictureBoxError = new System.Windows.Forms.PictureBox();
            this.pictureBoxWait = new System.Windows.Forms.PictureBox();
            this.stagePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.panelPlaceholder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOK)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).BeginInit();
            this.SuspendLayout();
            // 
            // labelHint
            // 
            this.labelHint.Text = Messages.WIZ_DB_CREATING;
            // 
            // labelDescription
            // 
            this.labelDescription.Text = Messages.WIZ_PERFORMING_OPERATION;
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Location = new System.Drawing.Point(433, 0);
            // 
            // panelPlaceholder
            // 
            this.panelPlaceholder.Controls.Add(this.stagePanel);
            this.panelPlaceholder.Size = new System.Drawing.Size(500, 281);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(475, 23);
            this.label1.TabIndex = 18;
            this.label1.Text = Messages.WIZ_CREATING_AND_CONFIGURATING;
            // 
            // pictureBoxOK
            // 
            this.pictureBoxOK.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxOK.Image")));
            this.pictureBoxOK.Location = new System.Drawing.Point(400, 288);
            this.pictureBoxOK.Name = "pictureBoxOK";
            this.pictureBoxOK.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxOK.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxOK.TabIndex = 26;
            this.pictureBoxOK.TabStop = false;
            this.pictureBoxOK.Visible = false;
            // 
            // pictureBoxError
            // 
            this.pictureBoxError.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxError.Image")));
            this.pictureBoxError.Location = new System.Drawing.Point(432, 288);
            this.pictureBoxError.Name = "pictureBoxError";
            this.pictureBoxError.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxError.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxError.TabIndex = 26;
            this.pictureBoxError.TabStop = false;
            this.pictureBoxError.Visible = false;
            // 
            // pictureBoxWait
            // 
            this.pictureBoxWait.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxWait.Image")));
            this.pictureBoxWait.Location = new System.Drawing.Point(464, 288);
            this.pictureBoxWait.Name = "pictureBoxWait";
            this.pictureBoxWait.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxWait.TabIndex = 26;
            this.pictureBoxWait.TabStop = false;
            this.pictureBoxWait.Visible = false;
            // 
            // stagePanel
            // 
            this.stagePanel.AutoScroll = true;
            this.stagePanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.stagePanel.Location = new System.Drawing.Point(12, 46);
            this.stagePanel.Name = "stagePanel";
            this.stagePanel.Size = new System.Drawing.Size(475, 195);
            this.stagePanel.TabIndex = 0;
            // 
            // ProgressPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxOK);
            this.Controls.Add(this.pictureBoxError);
            this.Controls.Add(this.pictureBoxWait);
            this.Name = "ProgressPage";
            this.Controls.SetChildIndex(this.panelPlaceholder, 0);
            this.Controls.SetChildIndex(this.pictureBoxWait, 0);
            this.Controls.SetChildIndex(this.pictureBoxError, 0);
            this.Controls.SetChildIndex(this.pictureBoxOK, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.panelPlaceholder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOK)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWait)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Nested type: Stage

        public struct Stage
        {
            public readonly string SqlResourceName;
            public readonly DbParameter[] Parameters;
            public Label Label;
            public PictureBox Pic;
            public DatabaseStepDelegate Step;

            public Stage(string displayName, DatabaseStepDelegate step)
            {
                SqlResourceName = null;
                Parameters = null;
                Label = new Label() { Text = displayName };
                Pic = new PictureBox();
                Step = step;
            }

            public Stage(string displayName, string sqlResourceName, params DbParameter[] parameters)
            {
                SqlResourceName = sqlResourceName;
                Parameters = parameters;
                Label = new Label() { Text = displayName };
                Pic = new PictureBox();
                Step = null;
            }

        }

        #endregion

        #region Nested type: SyncDelegate

        private delegate void SyncDelegate();

        #endregion

        #region Nested type: SyncImplDelegate

        private delegate void SyncImplDelegate(SyncDelegate d);

        #endregion
    }
}