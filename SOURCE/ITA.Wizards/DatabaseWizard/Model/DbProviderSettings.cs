using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace ITA.Wizards.DatabaseWizard.Model
{
	public class DbProviderSettings
	{
		private const string SERVER_CREDENTIAL_TYPE_KEY = "ServerCredentialType";
		private const string CREATE_NEW_DATABASE_KEY = "CreateNewDatabase";
		private const string CREATE_NEW_DATABASE_LOGIN_KEY = "CreateNewDatabaseLogin";
		private const string DATABASE_CREDENTIAL_TYPE_KEY = "DatabaseCredentialType";
		private const string SERVER_NAME_KEY = "ServerName";
		private const string SERVER_LOGIN_KEY = "ServerLogin";
		private const string SERVER_PASSWORD_KEY = "ServerPassword";
		private const string DATABASE_NAME_KEY = "DatabaseName";
		private const string DATABASE_LOGIN_KEY = "DatabaseLogin";
		private const string DATABASE_PASSWORD_KEY = "DatabasePassword";
		private const string UNATTENDED_MODE_KEY = "UnattendedMode";
		private const string CONNECTION_STRING_KEY = "ConnectionString";
		private const string SERVER_DATABASE_PORT_KEY = "ServerDatabasePort";
		private const string SERVER_DATABASE_SID_KEY = "ServerDatabaseSID";
		private const string DB_PROVIDER_TYPE_KEY = "DbProviderType";
		
		private const string SERVER_DATABASE_PORT_DEFAULT = "1521";

		private bool validationResult = true;

		public DbProviderSettings(string[] commandLineArgs)
		{
			if (commandLineArgs == null)
				throw new ArgumentNullException("commandLineArgs", "List of command line arguments cannot be empty.");
			Initialize(commandLineArgs);
		}

		public static string GetUsageMessage(string wizardName)
		{
			return String.Format(Messages.WIZ_USAGE_MESSAGE_TEMPLATE,
				wizardName,
				String.Join(",", Enum.GetNames(typeof(DbProviderType))), DbProviderType.MSSQL,
				DbProviderType.MSSQL,
				String.Join(",", Enum.GetNames(typeof(ELoginType))), ELoginType.NT,
				String.Join(",", Enum.GetNames(typeof(ELoginType))), ELoginType.NT,
				DbProviderType.Oracle,
				wizardName);
		}

		private void Initialize(string[] args)
		{
			Arguments initParams = new Arguments(args);

			if (initParams[SERVER_CREDENTIAL_TYPE_KEY] != null)
				ServerCredentialType = (ELoginType?)Enum.Parse(typeof(ELoginType), initParams[SERVER_CREDENTIAL_TYPE_KEY], true);

			if (initParams[CREATE_NEW_DATABASE_KEY] != null)
				CreateNewDatabase = Convert.ToBoolean(initParams[CREATE_NEW_DATABASE_KEY]);

			if (initParams[CREATE_NEW_DATABASE_LOGIN_KEY] != null)
				CreateNewDatabaseLogin = Convert.ToBoolean(initParams[CREATE_NEW_DATABASE_LOGIN_KEY]);

			if (initParams[DATABASE_CREDENTIAL_TYPE_KEY] != null)
				DatabaseCredentialType = (ELoginType?)Enum.Parse(typeof(ELoginType), initParams[DATABASE_CREDENTIAL_TYPE_KEY], true);

			if (initParams[SERVER_NAME_KEY] != null)
				ServerName = initParams[SERVER_NAME_KEY];

			if (initParams[SERVER_LOGIN_KEY] != null)
				ServerLogin = initParams[SERVER_LOGIN_KEY];

			if (initParams[SERVER_PASSWORD_KEY] != null)
				ServerPassword = initParams[SERVER_PASSWORD_KEY];

			if (initParams[DATABASE_NAME_KEY] != null)
				DatabaseName = initParams[DATABASE_NAME_KEY];

			if (initParams[DATABASE_LOGIN_KEY] != null)
				DatabaseLogin = initParams[DATABASE_LOGIN_KEY];

			if (initParams[DATABASE_PASSWORD_KEY] != null)
				DatabasePassword = initParams[DATABASE_PASSWORD_KEY];

			if (initParams[UNATTENDED_MODE_KEY] != null)
				UnattendedMode = Convert.ToBoolean(initParams[UNATTENDED_MODE_KEY]);

			if (initParams[CONNECTION_STRING_KEY] != null)
				ConnectionString = initParams[CONNECTION_STRING_KEY];

			if (initParams[SERVER_DATABASE_PORT_KEY] != null)
				ServerDatabasePort = initParams[SERVER_DATABASE_PORT_KEY];
			else
				ServerDatabasePort = SERVER_DATABASE_PORT_DEFAULT;

			if (initParams[SERVER_DATABASE_SID_KEY] != null)
				ServerDatabaseSID = initParams[SERVER_DATABASE_SID_KEY];

			if (initParams[DB_PROVIDER_TYPE_KEY] != null)
				DbProviderType = (DbProviderType)Enum.Parse(typeof(DbProviderType), initParams[DB_PROVIDER_TYPE_KEY], false);
		}

		public event EventHandler<SettinsValidationArgs> ValidationError;
		public void OnValidationError(SettinsValidationArgs args)
		{
			validationResult = false;
			if (ValidationError != null)
				ValidationError(this, args);
		}
		private void RaiseValidationError(string parameterName)
		{
			OnValidationError(new SettinsValidationArgs(String.Format(Messages.WIZ_PARAMETER_VALIDATION_ERROR, parameterName)));
		}
		public bool Validate()
		{
			validationResult = true;

			if (String.IsNullOrEmpty(ServerName))
				RaiseValidationError(ServerName);
			if (String.IsNullOrEmpty(DatabaseName))
				RaiseValidationError(DatabaseName);

			if (UnattendedMode)
			{
				if(this.DbProviderType == DbProviderType.MSSQL)
				{
					if(ServerCredentialType == ELoginType.SQL)
					{
						if (String.IsNullOrEmpty(ServerLogin))
							RaiseValidationError(ServerLogin);
						if (String.IsNullOrEmpty(ServerPassword))
							RaiseValidationError(ServerPassword);
					}
					if (DatabaseCredentialType == ELoginType.SQL && String.IsNullOrEmpty(ConnectionString))
					{
						if (String.IsNullOrEmpty(DatabaseLogin))
							RaiseValidationError(DatabaseLogin);
						if (String.IsNullOrEmpty(DatabasePassword))
							RaiseValidationError(DatabasePassword);
					}
				}
				else if (DbProviderType == DbProviderType.Oracle)
				{
					if(String.IsNullOrEmpty(ServerDatabaseSID))
						RaiseValidationError(ServerDatabaseSID);
					
					if(String.IsNullOrEmpty(ServerDatabasePort))
						RaiseValidationError(ServerDatabasePort);
                    
					if (String.IsNullOrEmpty(ServerLogin))
						RaiseValidationError(ServerLogin);
					
					if (String.IsNullOrEmpty(ServerPassword))
						RaiseValidationError(ServerPassword);

					if (String.IsNullOrEmpty(DatabaseLogin))
						RaiseValidationError(DatabaseLogin);

					if (String.IsNullOrEmpty(DatabasePassword))
						RaiseValidationError(DatabasePassword);
				}
                else if (DbProviderType == DbProviderType.MySQL)
                {
                    if (String.IsNullOrEmpty(ServerDatabasePort))
                        RaiseValidationError(ServerDatabasePort);

                    if (String.IsNullOrEmpty(ServerLogin))
                        RaiseValidationError(ServerLogin);

                    if (String.IsNullOrEmpty(ServerPassword))
                        RaiseValidationError(ServerPassword);

                    if (String.IsNullOrEmpty(DatabaseLogin))
                        RaiseValidationError(DatabaseLogin);

                    if (String.IsNullOrEmpty(DatabasePassword))
                        RaiseValidationError(DatabasePassword);
                }
                else if (DbProviderType == DbProviderType.AzureSQL)
                {
                    if (String.IsNullOrEmpty(ServerName))
                        RaiseValidationError(ServerName);

                    if (String.IsNullOrEmpty(DatabaseName))
                        RaiseValidationError(DatabaseName);

                    if (String.IsNullOrEmpty(DatabaseLogin))
                        RaiseValidationError(DatabaseLogin);

                    if (String.IsNullOrEmpty(DatabasePassword))
                        RaiseValidationError(DatabasePassword);
                }
                else
                {
                    throw new NotSupportedException(String.Format("Invalid provider type ({0}).", this.DbProviderType));
                }
			}

			return validationResult;
		}

		public DbProviderType DbProviderType { get; set; }
		public string ServerDatabaseSID { get; set; }
		public string ServerDatabasePort { get; set; }
		public string ConnectionString { get; set; }
		public bool UnattendedMode { get; set; }
		public string DatabasePassword { get; set; }
		public string DatabaseLogin { get; set; }
		public string DatabaseName { get; set; }
		public string ServerName { get; set; }
		public string ServerLogin { get; set; }
		public string ServerPassword { get; set; }
		public bool? CreateNewDatabaseLogin { get; set; }
		public bool? CreateNewDatabase { get; set; }
		public ELoginType? ServerCredentialType { get; set; }
		public ELoginType? DatabaseCredentialType { get; set; }

		/// <summary>
		/// <![CDATA[http://www.codeproject.com/KB/recipes/command_line.aspx]]>
		/// </summary>
		public class Arguments
		{
			// Variables

			private StringDictionary Parameters;

			// Constructor

			public Arguments(string[] Args)
			{
				Parameters = new StringDictionary();
				Regex Spliter = new Regex(@"^-{1,2}|^/|=",
										  RegexOptions.IgnoreCase | RegexOptions.Compiled);

				Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
										  RegexOptions.IgnoreCase | RegexOptions.Compiled);

				string Parameter = null;
				string[] Parts;

				// Valid parameters forms:

				// {-,/,--}param{ ,=,:}((",')value(",'))

				// Examples: 

				// -param1 value1 --param2 /param3:"Test-:-work" 

				//   /param4=happy -param5 '--=nice=--'

				foreach (string Txt in Args)
				{
					// Look for new parameters (-,/ or --) and a

					// possible enclosed value (=,:)

					Parts = Spliter.Split(Txt, 3);

					switch (Parts.Length)
					{
						// Found a value (for the last parameter 

						// found (space separator))

						case 1:
							if (Parameter != null)
							{
								if (!Parameters.ContainsKey(Parameter))
								{
									Parts[0] =
										Remover.Replace(Parts[0], "$1");

									Parameters.Add(Parameter, Parts[0]);
								}
								Parameter = null;
							}
							// else Error: no parameter waiting for a value (skipped)

							break;

						// Found just a parameter

						case 2:
							// The last parameter is still waiting. 

							// With no value, set it to true.

							if (Parameter != null)
							{
								if (!Parameters.ContainsKey(Parameter))
									Parameters.Add(Parameter, "true");
							}
							Parameter = Parts[1];
							break;

						// Parameter with enclosed value

						case 3:
							// The last parameter is still waiting. 

							// With no value, set it to true.

							if (Parameter != null)
							{
								if (!Parameters.ContainsKey(Parameter))
									Parameters.Add(Parameter, "true");
							}

							Parameter = Parts[1];

							// Remove possible enclosing characters (",')

							if (!Parameters.ContainsKey(Parameter))
							{
								Parts[2] = Remover.Replace(Parts[2], "$1");
								Parameters.Add(Parameter, Parts[2]);
							}

							Parameter = null;
							break;
					}
				}
				// In case a parameter is still waiting

				if (Parameter != null)
				{
					if (!Parameters.ContainsKey(Parameter))
						Parameters.Add(Parameter, "true");
				}
			}

			// Retrieve a parameter value if it exists 

			// (overriding C# indexer property)

			public string this[string Param]
			{
				get
				{
					return (Parameters[Param]);
				}
			}
		}
	}

	public class SettinsValidationArgs : EventArgs
	{
		private readonly string _errorMessage;

		public string ErrorMessage
		{
			get { return _errorMessage; }
		}

		public SettinsValidationArgs(string errorMessage)
		{
			_errorMessage = errorMessage;
		}
	}
}