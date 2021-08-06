using System.Data.Common;
using log4net;

namespace ITA.Wizards.UpdateWizard.Model
{
    /// <summary>
    /// Контекст обновления БД.
    /// </summary>
    public interface IUpdateContext
    {
        DbTransaction Transaction { get; }

        ILog Logger { get; }
    }

    public class UpdateContext : IUpdateContext 
    {
        public UpdateContext(DbTransaction transaction, ILog logger)
        {
            this._transaction = transaction;
            this._logger = logger;
        }

        private DbTransaction _transaction;

        public DbTransaction Transaction
        {
            get
            {
                return this._transaction;
            }
        }

        private ILog _logger;

        public ILog Logger
        {
            get
            {
                return this._logger;
            }
        }
    }
}
