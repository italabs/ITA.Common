using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.UpdateWizard
{
    /// <summary>
    /// Информация о необходимости выполнить обновление базы данных
    /// </summary>
    public partial class UpdateNecessityPage : CustomPage
    {
        public UpdateNecessityPage()
        {
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_DB_UPDATING;
			this.labelDescription.Text = Messages.I_ITA_DATABASE_IS_NOT_ACTUAL;
			this.label1.Text = Messages.WIZ_DB_NEED_TO_BE_UPDATED;
			this._lblCurrentVersion.Text = Messages.WIZ_DB_VERSION_CURRENT;
			this._lblActualVersion.Text = Messages.WIZ_DB_VERSION_ACTUAL;
			this.label2.Text = Messages.WIZ_NEED_BACKUP_DESCRIPTION;
			this.label3.Text = Messages.WIZ_DB_CURRENT_NAME;
        }

        public override void OnActive()
        {
            base.OnActive();

            Wizard.EnableButton(Wizard.EButtons.BackButton);
            Wizard.EnableButton(Wizard.EButtons.NextButton);

            DatabaseWizardContext databaseWizardContext = (DatabaseWizardContext)this.Wizard.Context[DatabaseWizardContext.ClassName];
            UpdateDatabaseWizardContext updateWizardContext = (UpdateDatabaseWizardContext)this.Wizard.Context[UpdateDatabaseWizardContext.ClassName];

            this._lblDatabaseNameOut.Text = databaseWizardContext.DBProvider.DatabaseName;
            this._lblCurrentVersionOut.Text = updateWizardContext.CurrentDatabaseVersion.ToString();
            this._lblActualVersionOut.Text = updateWizardContext.Manager.GetActualDatabaseVersion().ToString();
        }

    }
}
