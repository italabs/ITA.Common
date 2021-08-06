using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using ITA.Common.UI;
using ITA.WizardFramework;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.UpdateWizard
{
    /// <summary>
    /// ѕрогресс выполнени€ обновление базы данных
    /// </summary>
    public partial class UpdateProgress : CustomPage
    {
        public UpdateProgress()
        {
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_DB_UPDATING;
			this.labelDescription.Text = Messages.WIZ_WAIT_UPDATING_COMPLETE;
			this._lblCurrentAction.Text = Messages.WIZ_UPDATING_PROGRESS;
        }

        private UpdateDatabaseWizardContext UpdateWizardContext
        {
            get
            {
                UpdateDatabaseWizardContext updateWizardContext = (UpdateDatabaseWizardContext)this.Wizard.Context[UpdateDatabaseWizardContext.ClassName];
                return updateWizardContext;
            }
        }

        private void UpdateProgress_Load(object sender, EventArgs e)
        {
            Wizard.OnRepeatPage += new EventHandler(Wizard_OnRepeatPage);

            UpdateWizardContext.Manager.UpdateEvent += new UpdateEventHandler(manager_UpdateEvent);
        }

        private void Wizard_OnRepeatPage(object sender, EventArgs e)
        {            
            this._updateProgressBar.Value = 0;
            this._lblLog.Clear();
        }

        public override void OnActive()
        {
            base.OnActive();

            //ѕерехода на шаг назад пока не было
            this._prev = false;
                      
            ThreadPool.QueueUserWorkItem(new WaitCallback( delegate(object target) {
                UpdateWizardContext.NewVersion = UpdateWizardContext.Manager.ExecuteUpdates();
                                        }));

            Wizard.DisableButton(Wizard.EButtons.BackButton);
            Wizard.DisableButton(Wizard.EButtons.NextButton);
            Wizard.EnableButton(Wizard.EButtons.CancelButton);
        }

        private bool _prev = false;

        public override void OnPrev(ref int Steps)
        {
            if (this._prev == false)
            {
                this._prev = true;      
     
                this.Wizard.RepeatPage();

                base.Wizard.PrevPage();
                base.Wizard.PrevPage();
                base.Wizard.PrevPage();
                base.Wizard.NextPage();
            }
        }

        private void manager_UpdateEvent(object sender, UpdateEventArg arg)
        {
            this.Invoke(new EventHandler(delegate(object target, EventArgs a)
            {
                switch (arg.Type)
                {
                    case UpdateEventType.StepStarted:
                        {
                            this._lblCurrentAction.Text = arg.Step.Description;
                            break;
                        }
                    case UpdateEventType.StepFinished:
                        {
                            this._lblLog.AppendText(arg.Step.Description + "..." + "OK" + Environment.NewLine);
                            this._updateProgressBar.Value = arg.Progress;
                            break;
                        }
                    case UpdateEventType.Finished:
                        {
                            this._lblCurrentAction.Text = Messages.I_ITA_FINISHED;
                            this._lblCurrentAction.ForeColor = Color.Green;
                            this._updateProgressBar.Value = 100;

                            Wizard.EnableButton(Wizard.EButtons.NextButton);
                            Wizard.DisableButton(Wizard.EButtons.CancelButton);
                            break;
                        }
                    case UpdateEventType.Failed:
                        {
                            if (arg.Step != null)
                            {
                                this._lblLog.AppendText(arg.Step.Description + "..." + Messages.E_ITA_ERROR + Environment.NewLine);
                            }
                            else
                            {
                                this._lblLog.AppendText(Messages.E_ITA_UPDATE_FATAL_ERROR + Environment.NewLine);
                            }

                            this._lblCurrentAction.Text = Messages.E_ITA_FINISHED_WITH_ERRORS;
                            this._lblCurrentAction.ForeColor = Color.Red;

                            Wizard.EnableButton(Wizard.EButtons.BackButton);

                            ErrorMessageBox.Show(this, arg.Exception, Messages.E_ITA_INITIAL_WIZARD_DATABASE_UPDATE_ERROR, Wizard.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    default:
                        break;
                }
            }));
        }        
    }
}
