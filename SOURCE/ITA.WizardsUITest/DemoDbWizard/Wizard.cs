using ITA.Wizards.DatabaseWizard;
using ITA.Wizards.DatabaseWizard.Model;

namespace ITA.WizardsUITest.DemoDbWizard
{
    public class Wizard : DatabaseWizard
    {
        public Wizard() : base(new DummyDbProviderFactory(false, false), "testDb", "testUser")
        {
        }

        public Wizard(IDatabaseProviderFactory dbProviderFactory, string defaultDatabaseName, string defaultUserName) : base(dbProviderFactory, defaultDatabaseName, defaultUserName)
        {
        }
    }
}
