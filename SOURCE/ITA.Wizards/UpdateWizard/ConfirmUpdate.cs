using System;
using ITA.WizardFramework;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.Wizards.UpdateWizard
{
    /// <summary>
    /// Страница с результатом обновления базы данных
    /// </summary>
    public partial class ConfirmUpdate : CustomPage
    {
        public ConfirmUpdate()
        {
            InitializeComponent();
			this.labelHint.Text = Messages.WIZ_DB_UPDATING;
			this.labelDescription.Text = Messages.WIZ_DB_IS_UPDATED;
			this.label1.Text = Messages.WIZ_DB_IS_UPDATED_TO_VERSION;
        }

        public override void OnActive()
        {
            base.OnActive();

            Wizard.DisableButton(Wizard.EButtons.BackButton);

            UpdateDatabaseWizardContext updateContext = (UpdateDatabaseWizardContext)this.Wizard.Context[UpdateDatabaseWizardContext.ClassName];            
            Version newVersion = updateContext.NewVersion;
            this._lblNewVersion.Text = newVersion != null ? newVersion.ToString() : string.Empty;
        }

        public override void OnNext(ref int NextIndex)
        {
            DatabaseWizardContext databaseContext = (DatabaseWizardContext)this.Wizard.Context[DatabaseWizardContext.ClassName];

            if (!databaseContext.DBProvider.CreateNewDatabase)
            {
                // skip progress page
                NextIndex++;
            }

            base.OnNext(ref NextIndex);
        }
    }
}
