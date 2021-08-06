using ITA.Common.UI;
using System;
using System.Windows.Forms;

namespace ITA.Wizards.DatabaseWizard.Pages
{
    public class WelcomePage : ITA.WizardFramework.WelcomePage
    {
        public override bool OnValidate()
        {
            DatabaseWizard databaseWizard = ((DatabaseWizard)Wizard);
            try
            {
                if (!databaseWizard.ShowSelectProvider)
                {
                    databaseWizard.DatabaseWizardContext.DBProvider =
                        databaseWizard.DatabaseWizardContext.GetDbProviderByType(
                            databaseWizard.DatabaseWizardContext.SelectedDbProvider);
                    if (databaseWizard.DatabaseWizardContext.DBProvider == null)
                    {
                        MessageBox.Show(this, String.Format(Messages.WIZ_SRV_DB_PROVIDER_IS_NOT_AVAILABLE, databaseWizard.DatabaseWizardContext.SelectedDbProvider),
                             m_Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    databaseWizard.DatabaseWizardContext.DBProvider.CheckDbPresence();
                }
            }
            catch (Exception x)
            {
                databaseWizard.DatabaseWizardContext.DBProvider = null;
                ErrorMessageBox.Show(this, x, String.Format(Messages.WIZ_SRV_DB_PROVIDER_IS_NOT_AVAILABLE, databaseWizard.DatabaseWizardContext.SelectedDbProvider),
                                             m_Parent.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public override void OnNext(ref int NextIndex)
        {
            DatabaseWizard databaseWizard = ((DatabaseWizard)Wizard);
            
            if (!databaseWizard.ShowSelectProvider)
            {
                NextIndex++; // skip SelectProvider
                
                if (!databaseWizard.CheckExistingConfiguration)
                {                   
                    NextIndex = this.Wizard.GetPageIndex(typeof(SelectOperationPage).Name);  
                }
            }
        }
    }
}
