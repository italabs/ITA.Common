using System;

namespace ITA.Wizards.UpdateWizard.Model
{
    internal class UpdateDatabaseWizardContext
    {
        public const string ClassName = "UpdateDatabaseWizardContext";

        public UpdateDatabaseWizardContext(string connectionString, UpdateManager updateManager)
        {
            this._manager = updateManager;
            updateManager.DatabaseProvider.ConnectionString = connectionString;
        }

        public Version NewVersion {get;set;}  

        private UpdateManager _manager;

        public UpdateManager Manager
        {
            get
            {
                return this._manager;
            }
        }

        public bool IsUpdateRequired
        {
            get
            {
                return this._manager.IsUpdateRequired;
            }
        }

        public Version CurrentDatabaseVersion
        {
            get
            {
                return this._manager.GetDatabaseVersion();
            }
        }
    }
}
