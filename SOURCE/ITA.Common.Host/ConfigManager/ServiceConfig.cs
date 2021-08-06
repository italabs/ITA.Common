using System;
using System.Runtime.InteropServices;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host.Components
{
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class ServiceConfig : MarshalByRefObject, IServiceConfig, IComponentConfig
	{	
		public static string cCfgAutoStart = "AutoStart";
		public static string cCfgEnableSuspend = "EnableSuspend";
		public static string cCfgBatteryAction = "OnBatteryAction";
        public static string cCfgLowBatteryAction = "OnLowBatteryAction";
        public static string cCfgSuspendAction = "OnSuspendAction";
        public static string cCfgID = "ID";

		protected IConfigManager m_ConfigManager = null;

		public ServiceConfig ( IConfigManager ConfigManager )
		{
			m_ConfigManager = ConfigManager;
		}

		#region IServiceConfig Members

		public bool AutoStart
		{
			get
			{
				return 0 != ( int ) m_ConfigManager [ Name, cCfgAutoStart, 0 ];
			}
			set
			{
				m_ConfigManager [ Name, cCfgAutoStart ] = value ? 1 : 0;
			}
		}

		public bool EnableSuspend
		{
			get
			{
				return 0 != ( int ) m_ConfigManager [ Name, cCfgEnableSuspend, 1 ];
			}
			set
			{
				m_ConfigManager [ Name, cCfgEnableSuspend ] = value ? 1 : 0;
			}
		}

		public EOnPowerEventBehaviour OnBatteryAction
		{
			get
			{				 
				return ( EOnPowerEventBehaviour ) Enum.Parse ( typeof ( EOnPowerEventBehaviour ), m_ConfigManager [ Name, cCfgBatteryAction, EOnPowerEventBehaviour.Ignore.ToString () ] as string, true );
			}
			set
			{ 
				m_ConfigManager [ Name, cCfgBatteryAction ] = value.ToString ();
			}
		}

        public EOnPowerEventBehaviour OnLowBatteryAction
		{
			get
			{				 
				return ( EOnPowerEventBehaviour ) Enum.Parse ( typeof ( EOnPowerEventBehaviour ), m_ConfigManager [ Name, cCfgLowBatteryAction, EOnPowerEventBehaviour.Pause.ToString () ] as string, true );
			}
			set
			{ 
				m_ConfigManager [ Name, cCfgLowBatteryAction ] = value.ToString ();
			}
		}

        public EOnPowerEventBehaviour OnSuspendAction
        {
            get
            {
                return (EOnPowerEventBehaviour)Enum.Parse(typeof(EOnPowerEventBehaviour), m_ConfigManager[Name, cCfgSuspendAction, EOnPowerEventBehaviour.Stop.ToString()] as string, true);
            }
            set
            {
                m_ConfigManager[Name, cCfgSuspendAction] = value.ToString();
            }
        }

		public string ID
		{
			get
			{
				return m_ConfigManager [ Name, cCfgID, "" ] as string;
			}
			set
			{
				m_ConfigManager [ Name, cCfgID ] = value;
			}
		}

		#endregion

		#region IComponentConfig Members

		public abstract string Name
		{
			get;
		}

		#endregion
	}

}