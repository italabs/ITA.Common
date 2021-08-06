using System;
using System.Collections.Generic;
using ITA.Common.Host;
using ITA.Common.Host.Interfaces;
using log4net;
using Unity;

namespace ITA.Common.Unity
{
	public class CommandContext : ICommandContext
	{
		private static readonly ILog m_logger = Log4NetItaHelper.GetLogger(typeof(CommandContext).Name);

		private ICompaundService m_service = null;

		public CommandContext(ICompaundService service)
		{
			m_service = service;
            ITA.Common.Unity.Unity.Container.RegisterInstance(typeof(ICommandContext), this);
		}

		#region ICommandContext Members

		public T GetComponent<T>() where T: class
		{
			try
			{
			    return ITA.Common.Unity.Unity.Container.Resolve<T>();

			}
			catch (Exception e)
			{
				m_logger.Error(string.Format("Error has occurred while getting component '{0}'", typeof(T).Name), e);

				return null;
			}
		}

        public T GetComponent<T>(string name) where T : class
        {
            try
            {
                return ITA.Common.Unity.Unity.Container.Resolve<T>(name);
            }
            catch (Exception e)
            {
                m_logger.Error(string.Format("Error has occurred while getting component '{0}' with specific name '{1}'", typeof(T).Name, name), e);
                return null;
            }
        }

		public IComponent[] GetComponents()
		{
			List<IComponent> components = new List<IComponent>();

			foreach (var reg in Unity.Container.Registrations)
			{
			    string logMsg = String.Format("{0}-{1}-{2}", reg.Name, reg.RegisteredType.Name, reg.MappedToType.Name);
				Console.WriteLine(logMsg);
			    m_logger.Debug(logMsg);                

				object obj = null;

			    if (!string.IsNullOrEmpty(reg.Name))
			        obj = ITA.Common.Unity.Unity.Container.Resolve(reg.RegisteredType, reg.Name);
			    else
			        obj = ITA.Common.Unity.Unity.Container.Resolve(reg.RegisteredType); 

				if (obj is CompaundComponent || obj is UnboundCompaundComponent)
					continue;

				if (obj is IComponent)
				{
					components.Add((IComponent)obj);
				}
			}

			return components.ToArray();
		}

		#endregion

		#region ICommandContextBase Members

		public EComponentStatus Status
		{
			get { return m_service.Status; }
			set { m_service.Status = value; }
		}

		public string InstanceName
		{
			get { return m_service.InstanceName; }
		}

		public string EventLogName
		{
			get { return m_service.EventLogName; }
		}

		public IComponent Host
		{
			get { return m_service.Host; }
		}

		public IService Service
		{
			get { return m_service.Service; }
		}

		#endregion
	}
}
