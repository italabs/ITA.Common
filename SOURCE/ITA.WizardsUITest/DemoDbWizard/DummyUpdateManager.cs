using System;
using ITA.Wizards.UpdateWizard.Model;

namespace ITA.WizardsUITest.DemoDbWizard
{
    /// <summary>
    /// Тестовый апдейтер, которые возвращает текущую и актуальную версии БД
    /// </summary>
    public class DummyUpdateManager : UpdateManager
    {
        public DummyUpdateManager(string resourceNamespace)
            : base(resourceNamespace)
        {
        }

        public override Version GetDatabaseVersion()
        {
            // пробуем открыть соединение 
            // если не вышло - исключение, которое обработается визардом - "База данных недоступна или ее версия не поддерживается"
            //using (var conn = )
            DatabaseProvider.ConnectionString = String.IsNullOrEmpty(DatabaseProvider.ConnectionString)
                                                    ? DatabaseProvider.GetConnectionString(false, DatabaseProvider.DatabaseName)
                                                    : DatabaseProvider.ConnectionString;
            using (var conn = this.DatabaseProvider.GetConnection())
            {
                conn.Open();
            }
            return new Version(1, 0, 0, 0);
        }

        protected override Version GetDatabaseVersion(IUpdateContext context)
        {
            return GetDatabaseVersion();
        }

        protected override void SetDatabaseVersion(IUpdateContext context, Version version)
        {
            
        }

        public override Version GetActualDatabaseVersion()
        {
            return new Version(1, 5, 1, 2);
        }
    }
}
