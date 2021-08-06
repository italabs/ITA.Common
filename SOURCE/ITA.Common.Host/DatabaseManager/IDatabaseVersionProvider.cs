using System;

namespace ITA.Common.Host.DatabaseManager
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDatabaseVersionProvider
    {
        bool EnableVersionCheck { get; }

        /// <summary>
        /// This is constant (build dependent)
        /// </summary>
        Version CurrentDatabaseVersion { get; }

        /// <summary>
        /// Actual database version (from table)
        /// </summary>
        /// <returns></returns>
        Version GetDatabaseVersion();
    }
}
