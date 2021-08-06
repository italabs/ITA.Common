using System;
using ITA.WizardFramework;

namespace ITA.Wizards.UpdateWizard
{
    /// <summary>
    /// Предупреждение о необходимости сделать бекап БД
    /// </summary>
    public partial class ConfirmBackupPage : CustomPage
    {
        public ConfirmBackupPage()
        {
            InitializeComponent();

			this.labelHint.Text = Messages.WIZ_BACKUP;
			this.labelDescription.Text = Messages.WIZ_NEED_BACKUP;
			this.label1.Text = Messages.WIZ_NEED_BACKUP_MESSAGE;
			this._chbBackupExists.Text = Messages.WIZ_BACKUP_CONFIRM;
			this._chbBackupIsActual.Text = Messages.WIZ_CONFIRM_BACKUP_IS_ACTUAL;
        }

        private void ConfirmBackupPage_Load(object sender, EventArgs e)
        {
            this.Wizard.OnRepeatPage += new EventHandler(Wizard_OnRepeatPage);
        }

        private void Wizard_OnRepeatPage(object sender, EventArgs e)
        {
            this._chbBackupExists.Checked = false;
            this._chbBackupIsActual.Checked = false;
        }

        public override void OnActive()
        {
            base.OnActive();

            this._chbBackupExists_CheckedChanged(this, EventArgs.Empty);
        }

        private void _chbBackupExists_CheckedChanged(object sender, EventArgs e)
        {
            bool isNext = this._chbBackupExists.Checked && this._chbBackupIsActual.Checked;

            if (isNext)
            {
                Wizard.EnableButton(Wizard.EButtons.NextButton);
            }
            else
            {
                Wizard.DisableButton(Wizard.EButtons.NextButton);
            }
        }      
    }
}
