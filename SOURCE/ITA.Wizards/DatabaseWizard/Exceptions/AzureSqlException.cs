using System;

namespace ITA.Wizards.DatabaseWizard.Exceptions
{
    public class AzureSqlException : Exception
    {
        public AzureSqlException(string message)
            : base(message)
        {
        }

        public AzureSqlException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}