using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ITA.Common;
using log4net;

namespace ITA.Wizards.DatabaseWizard.Model
{
	/// <summary>
	/// Helps execute sql script
	/// </summary>
	public sealed class SqlScriptHelper
	{
	    private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(SqlScriptHelper).Name);

		private static readonly Regex _NonWhiteRegex = new Regex(@"\S+", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Multiline);

		private Regex _goRegex = new Regex(@"^\s*(g|G)(o|O)\s*$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Multiline);

        public SqlScriptHelper(Regex goRegex)
        {
            if (goRegex == null)
                throw new ArgumentNullException("goRegex");

            _goRegex = goRegex;
        }

        private void DumpConnectionString(string connectionString)
        {
            try
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = connectionString;
                if (builder.Remove("Password"))
                {
                    builder.Add("Password", "**********");
                }
                
                logger.DebugFormat("connection string:'{0}'.", builder.ConnectionString);
            }
            catch
            {
            
            }
        }

	    /// <summary>
		/// Execute SQL script</summary>
		/// <param name="command"></param>
		/// <param name="script">Sql script text</param>
		/// <remarks>Split script body into batches</remarks>        
		private void InternalExecuteScript(DbCommand command, string script)
		{
			const string operationName = "InternalExecuteScript";
			logger.InfoFormat("{0} begin.", operationName);

			if (command == null || command.Connection == null)
			{
				throw new ArgumentNullException();
			}

			logger.DebugFormat("script:'{0}'.", script);

			string[] commands = _goRegex.Split(script);

	        DumpConnectionString(command.Connection.ConnectionString);			
			//open connection if required
			bool isNeedCloseConnection;
			if (command.Connection.State == ConnectionState.Closed)
			{
				logger.Debug("Ñommand.Connection.State is closed.");
				logger.Debug("Opening connection...");

				command.Connection.Open();
				isNeedCloseConnection = true;
				logger.Debug("Connection is open.");
			}
			else
			{
				isNeedCloseConnection = false;
			}
			try
			{
				logger.Debug("Executing commands...");
				// for all commands...
				foreach (string commandText in commands)
				{
					logger.DebugFormat("commandText:'{0}'.", commandText);
					if (!_NonWhiteRegex.IsMatch(commandText))
					{
						logger.Debug("Skip command.");
						//or not
						continue;
					}
					// execute command (single batch)
					command.CommandText = commandText;
					command.ExecuteNonQuery();
				}
			}
			catch (Exception e)
			{
				logger.Error(e);
				throw;
			}
			finally
			{
                command.Parameters.Clear();
				if (isNeedCloseConnection)
				{
					logger.Debug("Closing connection...");
					command.Connection.Close();
					logger.Debug("Connection is closed.");
				}
				logger.InfoFormat("{0} end.", operationName);
			}
		}
        
        /// <summary>
		/// Execute SQL script</summary>
		/// <param name="command"></param>
		/// <param name="script">Sql script text</param>
		/// <remarks>Split script body into batches</remarks>        
        private object InternalExecuteScalarScript(DbCommand command, string script)
		{
            const string operationName = "InternalExecuteScalarScript";
			logger.InfoFormat("{0} begin.", operationName);

			if (command == null || command.Connection == null)
			{
				throw new ArgumentNullException();
			}

			logger.DebugFormat("script:'{0}'.", script);

			string[] commands = _goRegex.Split(script);

            DumpConnectionString(command.Connection.ConnectionString);
			//open connection if required
			bool isNeedCloseConnection;
			if (command.Connection.State == ConnectionState.Closed)
			{
				logger.Debug("Command.Connection.State is closed.");
				logger.Debug("Opening connection...");

				command.Connection.Open();
				isNeedCloseConnection = true;
				logger.Debug("Connection is open.");
			}
			else
			{
				isNeedCloseConnection = false;
			}
            object result = null;
			try
			{
				logger.Debug("Executing commands...");
				// for all commands...
				foreach (string commandText in commands)
				{
					logger.DebugFormat("commandText:'{0}'.", commandText);
					if (!_NonWhiteRegex.IsMatch(commandText))
					{
						logger.Debug("Skip command.");
						//or not
						continue;
					}
					// execute command (single batch)
					command.CommandText = commandText;
					result =  command.ExecuteScalar();
                    logger.DebugFormat("result:'{0}'", result);
				}
                return result;
			}
			catch (Exception e)
			{
				logger.Error(e);
				throw;
			}
			finally
			{
                command.Parameters.Clear();
				if (isNeedCloseConnection)
				{
					logger.Debug("Closing connection...");
					command.Connection.Close();
					logger.Debug("Connection is closed.");
				}
				logger.InfoFormat("{0} end.", operationName);
			}
		}
          
        /// <summary>
		/// Execute SQL script</summary>
		/// <param name="transaction">Valid transaction</param>
		/// <param name="script">Sql script text</param>        
		public void ExecuteScript(DbTransaction transaction, int commandTimeout, string script, params DbParameter[] parameters)
		{
			CheckParameters(script, transaction);

            DbCommand command = transaction.Connection.CreateCommand();
			command.Connection = transaction.Connection;
			command.Transaction = transaction;

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            if (commandTimeout > 0)
                command.CommandTimeout = commandTimeout;

			InternalExecuteScript(command, script);
		}      

		/// <summary>
		/// Execute SQL script</summary>
        /// <param name="connection">Valid connection</param>
		/// <param name="script">Sql script text</param>        
		public void ExecuteScript(DbConnection connection, string script)
		{
			CheckParameters(script, connection);
			DbCommand command = connection.CreateCommand();
			command.Connection = connection;
			InternalExecuteScript(command, script);
		}
        
        /// <summary>
		/// Execute SQL script</summary>
		/// <param name="transaction">Valid connection</param>
		/// <param name="script">Sql script text</param>        
		public void ExecuteScript(DbConnection connection, string script, params DbParameter[] parameters)
		{
			CheckParameters(script, connection);
            using (DbCommand command = connection.CreateCommand())
            {
                command.Connection = connection;
                command.Parameters.AddRange(parameters);
                InternalExecuteScript(command, script);
            }
		}

        /// <summary>
        /// Execute SQL script</summary>
        /// <param name="connection">Valid connection</param>
        /// <param name="script">Sql script text</param>        
        public void ExecuteScript(DbConnection connection, int commandTimeout, string script, params DbParameter[] parameters)
        {
            CheckParameters(script, connection);
            using (DbCommand command = connection.CreateCommand())
            {
                command.Connection = connection;

                if (commandTimeout > 0)
                    command.CommandTimeout = commandTimeout;
                
                if (parameters != null && parameters.Length > 0)
                    command.Parameters.AddRange(parameters);

                InternalExecuteScript(command, script);
            }
        }
      	
        public static string GetScript(string resourceName, string ns)
        {
            return GetScript(Assembly.GetExecutingAssembly(), resourceName, ns);
        }
        public static string GetScript(Assembly resourceAssembly, string resourceName, string ns)
        {
            if (resourceAssembly == null)
                throw new ArgumentNullException("resourceAssembly");
            
            if (resourceName == null)
                throw new ArgumentNullException("resourceName");

            Stream stream = resourceAssembly.GetManifestResourceStream(ns + "." + resourceName);
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd().Replace("\r","");
            }
        }

		private static void CheckParameters(string script, DbTransaction transaction)
		{
			if (transaction == null)
				throw new ArgumentNullException("transaction");
			if (script == null)
				throw new ArgumentNullException("script");
		}

		private static void CheckParameters(string script, DbConnection connection)
		{
			if (connection == null)
                throw new ArgumentNullException("connection");
			if (script == null)
				throw new ArgumentNullException("script");
		}
	}
}
