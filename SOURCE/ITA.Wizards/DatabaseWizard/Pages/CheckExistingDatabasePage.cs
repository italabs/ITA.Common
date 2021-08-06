using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Remoting.Messaging;
using ITA.Common;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.UpdateWizard;
using ITA.Wizards.UpdateWizard.Model;
using log4net;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    public enum DbState
    {
        Valid = 0,
        NeedUpdate = 1,
        NotExists = 2
    }

    public partial class CheckExistingDatabasePage : CustomPage
    {

        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(CheckExistingDatabasePage).Name);

        public CheckExistingDatabasePage()
        {
            InitializeComponent();
			
			this.labelHint.Text = Messages.WIZ_PRODUCT_CONFIGURATION;
			this.labelDescription.Text = Messages.WIZ_CONFIG_VALIDATION;
			this.label1.Text = Messages.WIZ_DB_VALIDATION;
			this.lCheckResult.Text = Messages.WIZ_VALIDATION_RESULT;
			this.label2.Text = Messages.WIZ_VALIDATION_RESULT2;
        }

        private DbState m_DbState = DbState.NotExists;
        private delegate bool AsyncMethodCaller(ref DbState State, ref Exception Error);
        private AsyncMethodCaller m_CheckDatabaseAsync;

        private UpdateManager UpdateManager
        {
            get
            {
                return ((DatabaseWizard)Wizard).DatabaseWizardContext.DBProvider.UpdateManager;
            }
        }

        public override void OnActive()
        {
            CheckDatabase();

            base.OnActive();
        }

        public override void OnPrev(ref int Steps)
        {
            Wizard.EnableButton(Wizard.EButtons.NextButton);

            base.OnPrev(ref Steps);
        }

        public override void OnNext(ref int NextIndex)
        {
            switch (m_DbState)
            {
                case DbState.Valid:                                       
                    NextIndex = this.Wizard.GetPageIndex(typeof(FinishPage).Name);
                    break;
                case DbState.NeedUpdate:
                    //launch Db Update Wizard
                    NextIndex = this.Wizard.GetPageIndex(typeof(UpdateNecessityPage).Name);                    
                    break;
                case DbState.NotExists:
                    break;
            }

            base.OnNext(ref NextIndex);
        }

        private delegate void SyncDelegate();
        private delegate void SyncImplDelegate(SyncDelegate d);

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
            if (this.InvokeRequired)
            {
                this.Invoke(new SyncImplDelegate(this.SyncImpl), d);
            }
            else
            {
                this.SyncImpl(d);
            }
        }

        private bool CheckDatabaseAsyncProc(ref DbState State, ref Exception Error)
        {
            try
            {
                logger.Debug("Begin CheckDatabase async proc");

                string connString = Wizard.Context.ValueOf<string>("ExistingConnString");

                UpdateDatabaseWizardContext updateWizardContext = new UpdateDatabaseWizardContext(connString, UpdateManager);

                if (updateWizardContext.IsUpdateRequired)
                {
                    //Необходимо выполнить обновление
                    State = DbState.NeedUpdate;

                    this.Wizard.Context[UpdateDatabaseWizardContext.ClassName] = updateWizardContext;

                    DatabaseWizardContext DbWizardContext = (DatabaseWizardContext)this.Wizard.Context[DatabaseWizardContext.ClassName];
                    DbWizardContext.DBProvider.CreateNewDatabase = false;
                }
                else
                {
                    State = DbState.Valid;
                }

                return true;
            }
            catch (Exception x)
            {
                Error = x;

                logger.Error("Error at check database", x);

                State = DbState.NotExists;

                return false;
            }
        }

        private void CheckDatabase()
        {
            logger.Debug("CheckDatabase begin");

            Wizard.DisableButton(Wizard.EButtons.NextButton);
            Wizard.DisableButton(Wizard.EButtons.BackButton);

            if (m_CheckDatabaseAsync == null)
            {
                logger.Debug("CheckDatabase: async method caller creating");

                m_CheckDatabaseAsync = new AsyncMethodCaller(CheckDatabaseAsyncProc);

                Exception x = null;
                DbState state = DbState.NotExists;

                lCheckResult.Text = ITA.Wizards.Messages.I_ITA_RESULT_WAIT;
                lCheckResult.ForeColor = Color.Black;

                pictureBoxDb.Image = pictureBoxWait.Image;

                logger.Debug("CheckDatabase: begin async method caller invoke");

                m_CheckDatabaseAsync.BeginInvoke(ref state, ref x, new AsyncCallback(OnCheckComplete), this);
            }
        }

        private void CheckExistingDatabase_Load(object sender, EventArgs e)
        {
            this.Wizard.OnRepeatPage += new EventHandler(Wizard_OnRepeatPage);
        }

        private void Wizard_OnRepeatPage(object sender, EventArgs e)
        {
            lCheckResult.Text = ITA.Wizards.Messages.I_ITA_RESULT_WAIT;
            lCheckResult.ForeColor = Color.Black;
        }

        private void OnCheckComplete(IAsyncResult ar)
        {
            try
            {
                logger.Debug("Begin OnCheckDatabaseComlete");

                AsyncResult result = (AsyncResult)ar;
                AsyncMethodCaller caller = (AsyncMethodCaller)result.AsyncDelegate;

                Exception Error = null;
                DbState State = DbState.NotExists;
                bool Result = caller.EndInvoke(ref State, ref Error, ar);

                Sync(delegate()
                {
                    switch (State)
                    {
                        case DbState.Valid:
                            lCheckResult.Text = ITA.Wizards.Messages.I_ITA_DATABASE_IS_ACTUAL;
                            lCheckResult.ForeColor = Color.Green;

                            Wizard.EnableButton(Wizard.EButtons.NextButton);
                            pictureBoxDb.Image = pictureBoxOK.Image;
                            break;
                        case DbState.NeedUpdate:
                            lCheckResult.Text = ITA.Wizards.Messages.I_ITA_DATABASE_IS_NOT_ACTUAL;
                            lCheckResult.ForeColor = Color.Blue;

                            Wizard.EnableButton(Wizard.EButtons.NextButton);
                            pictureBoxDb.Image = pictureBoxOK.Image;
                            break;
                        case DbState.NotExists:
                            lCheckResult.Text = ITA.Wizards.Messages.E_ITA_DATABASE_IS_UNAVAILABLE;
                            lCheckResult.ForeColor = Color.Red;

                            Wizard.DisableButton(Wizard.EButtons.NextButton);
                            pictureBoxDb.Image = pictureBoxError.Image;
                            break;
                    }

                    Wizard.EnableButton(Wizard.EButtons.BackButton);

                });

                m_DbState = State;
            }
            catch (Exception x)
            {
                logger.Error("Error at OnCheckComplete", x);

                Sync(delegate()
                {
                    ErrorMessageBox.Show(this, x, ITA.Wizards.Messages.E_ITA_DATABASE_VERSION_ERROR, this.Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
            //
            // Allow doing async search again
            //
            m_CheckDatabaseAsync = null;
        }
    }
}
