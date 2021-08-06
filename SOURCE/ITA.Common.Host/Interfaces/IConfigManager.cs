namespace ITA.Common.Host.Interfaces
{
	/// <summary>
	/// Summary description for IConfigManager.
	/// </summary>
	public interface IConfigManager
	{
		object this [ string Component, string Property, object Default ]
		{
			get;
		}

		object this [ string Component, string Property ]
		{
			get; set;
		}
	
		IComponentConfig GetConfig ( string Name );

        void AddConfig(IComponentConfig Config, ISettingsStorage storage);
        
        void DelConfig(string Name);
	}
}
