namespace ITA.Common.Host.Interfaces
{
	public interface ICompaundService //IHostService?  IEngineService?
	{
		EComponentStatus Status { get; set; }
		string InstanceName { get; }
		string EventLogName { get; }
		IComponent Host { get; }
		IService Service { get; }
	}

	public interface ICommandContext
	{
		EComponentStatus Status { get; set; }
		string InstanceName { get; }
		string EventLogName { get; }
		IComponent Host { get; }
		IService Service { get; }

		T GetComponent<T>() where T : class;
        T GetComponent<T>(string name) where T : class;

		IComponent[] GetComponents();
	}
}