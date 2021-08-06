using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections.Generic;
using System.Data.Sql;
using ITA.Common;
using Microsoft.Win32;

namespace ITA.Wizards.DatabaseWizard.Model
{
	public class ServerEnumerator
	{
		//declare the DLL import functions 
		[DllImport("netapi32.dll", EntryPoint = "NetServerEnum")]
		public static extern int NetServerEnum([MarshalAs(UnmanagedType.LPWStr)] string servername,
		   int level,
		   out IntPtr bufptr,
		   int prefmaxlen,
		   ref int entriesread,
		   ref int totalentries,
		   SV_101_TYPES servertype,
		   [MarshalAs(UnmanagedType.LPWStr)]string domain,
		   int resume_handle);


		[DllImport("netapi32.dll", EntryPoint = "NetApiBufferFree")]
		public static extern int NetApiBufferFree(IntPtr buffer);

		//declare the structures to hold info
		public enum SV_101_TYPES : uint
		{
			SV_TYPE_WORKSTATION = 0x00000001,
			SV_TYPE_SERVER = 0x00000002,
			SV_TYPE_SQLSERVER = 0x00000004,
			SV_TYPE_DOMAIN_CTRL = 0x00000008,
			SV_TYPE_DOMAIN_BAKCTRL = 0x00000010,
			SV_TYPE_TIME_SOURCE = 0x00000020,
			SV_TYPE_AFP = 0x00000040,
			SV_TYPE_NOVELL = 0x00000080,
			SV_TYPE_DOMAIN_MEMBER = 0x00000100,
			SV_TYPE_PRINTQ_SERVER = 0x00000200,
			SV_TYPE_DIALIN_SERVER = 0x00000400,
			SV_TYPE_XENIX_SERVER = 0x00000800,
			SV_TYPE_SERVER_UNIX = SV_TYPE_XENIX_SERVER,
			SV_TYPE_NT = 0x00001000,
			SV_TYPE_WFW = 0x00002000,
			SV_TYPE_SERVER_MFPN = 0x00004000,
			SV_TYPE_SERVER_NT = 0x00008000,
			SV_TYPE_POTENTIAL_BROWSER = 0x00010000,
			SV_TYPE_BACKUP_BROWSER = 0x00020000,
			SV_TYPE_MASTER_BROWSER = 0x00040000,
			SV_TYPE_DOMAIN_MASTER = 0x00080000,
			SV_TYPE_SERVER_OSF = 0x00100000,
			SV_TYPE_SERVER_VMS = 0x00200000,
			SV_TYPE_WINDOWS = 0x00400000, SV_TYPE_DFS = 0x00800000, SV_TYPE_CLUSTER_NT = 0x01000000, SV_TYPE_TERMINALSERVER = 0x02000000, SV_TYPE_CLUSTER_VS_NT = 0x04000000, SV_TYPE_DCE = 0x10000000, SV_TYPE_ALTERNATE_XPORT = 0x20000000, SV_TYPE_LOCAL_LIST_ONLY = 0x40000000, SV_TYPE_DOMAIN_ENUM = 0x80000000,
			SV_TYPE_ALL = 0xFFFFFFFF
		}

		/// <summary>
        /// Here's some possible error codes
		/// </summary>
		public enum NERR
		{
			NERR_Success = 0, /* Success */
			ERROR_MORE_DATA = 234,// dderror
			ERROR_NO_BROWSER_SERVERS_FOUND = 6118,
			/// <summary>
			/// The system call level is not correct.
			/// </summary>
			ERROR_INVALID_LEVEL = 124,
			/// <summary>
			/// Access is denied.
			/// </summary>
			ERROR_ACCESS_DENIED = 5,
			/// <summary>
			/// The parameter is incorrect.
			/// </summary>
			ERROR_INVALID_PARAMETER = 87,
			/// <summary>
			/// Not enough storage to process this command.
			/// </summary>
			ERROR_NOT_ENOUGH_MEMORY = 8,
			/// <summary>
			/// The network is busy.
			/// </summary>
			ERROR_NETWORK_BUSY = 54,
			/// <summary>
			/// The network path was not found.
			/// </summary>
			ERROR_BAD_NETPATH = 53,
			/// <summary>
			/// The network is not present or not started.
			/// </summary>
			ERROR_NO_NETWORK = 1222,
			/// <summary>
			/// Handle is in an invalid state.
			/// </summary>
			ERROR_INVALID_HANDLE_STATE = 1609,
			/// <summary>
			/// An extended error has occurred.
			/// </summary>
			ERROR_EXTENDED_ERROR = 1208
		}


		[StructLayout(LayoutKind.Sequential)]
		public struct SERVER_INFO_101
		{
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
			public UInt32 sv101_platform_id;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
			public string sv101_name;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
			public UInt32 sv101_version_major;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
			public UInt32 sv101_version_minor;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
			public UInt32 sv101_type;
			[MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
			public string sv101_comment;
		}

		public enum PLATFORM_ID
		{
			PLATFORM_ID_DOS = 300,
			PLATFORM_ID_OS2 = 400,
			PLATFORM_ID_NT = 500,
			PLATFORM_ID_OSF = 600,
			PLATFORM_ID_VMS = 700
		}

        /// <summary>
		/// ѕеречисление списка локальных серверов + из сети
		/// </summary>
		/// <returns></returns>
        public static string[] EnumServers()
        {
            string[] networkServers = EnumNetworkServers();
            string[] localServers = EnumLocalServers();

            return new List<string>(Utils.Union(networkServers, localServers)).ToArray();
        }

        /// <summary>
        /// ѕолучение списка экземпл€ров локальных серверов MS SQL - из реестра
        /// </summary>
        /// <returns></returns>
        public static string[] EnumLocalServers()
        {
            try
            {
                List<string> instances = new List<string>();

                string workstationNetBIOSName = Environment.MachineName.ToUpper();

                Microsoft.Win32.RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Microsoft SQL Server\Instance Names\SQL\", false);

                if (regKey != null)
                {
                    if (regKey.ValueCount > 0)
                    {
                        foreach (string instance in regKey.GetValueNames())
                        {
                            if (string.Compare(instance, "MSSQLSERVER", false) == 0)
                            {
                                //default local instance - add without instance name
                                instances.Add(workstationNetBIOSName);
                            }
                            else
                            {
                                //non default local instance
                                instances.Add(workstationNetBIOSName + @"\" + instance.ToUpper());
                            }
                        }
                    }
                }

                return instances.ToArray();
            }
            catch
            {
                return new string[] { };
            }
        }

		/// <summary>
		/// ѕеречисление серверов при помощи стандартных классов
		/// </summary>
		/// <returns></returns>
		public static string[] EnumNetworkServers()
		{
            //ѕолучени€ списка MSSQL серверов версии начина€ с 9 (2005)
            //GetDataSources возвращает сервера начина€ с 8 (2000) версии - 
            //отсекаем, добавл€ем сервера, дл€ которых верси€ не определена
			DataTable serverTable = SqlDataSourceEnumerator.Instance.GetDataSources();
			DataRow[] serverRows = serverTable.Select("Version IS NULL OR Version NOT LIKE '8.%'", "ServerName");

			List<string> servers = new List<string>();

			foreach (DataRow serverRow in serverRows)
			{
				string serverName = (string)serverRow["ServerName"];
				string instanceName = serverRow["InstanceName"] is string ? (string)serverRow["InstanceName"] : string.Empty;

				if (!string.IsNullOrEmpty(instanceName))
					serverName = serverName + @"\" + instanceName;

				servers.Add(serverName.ToUpper());
			}
			return servers.ToArray();
		}

		//now let's do it!        
        public static string[] EnumNetworkServers(SV_101_TYPES SrvType)
		{

			ArrayList ServerNames = new ArrayList();

			SERVER_INFO_101 si;
			IntPtr ppSVINFO = new IntPtr();
			int etriesread = 0;
			int totalentries = 0;
			try
			{
				if (NetServerEnum(null,
					101,
					out ppSVINFO,
					-1,
					ref etriesread,
					ref totalentries,
					SrvType,
					null,
					0) == 0)
				{
					Int32 ptr = ppSVINFO.ToInt32();

					for (int i = 0; i < etriesread; i++)
					{
						si = (SERVER_INFO_101)Marshal.PtrToStructure(new IntPtr(ptr), typeof(SERVER_INFO_101));

						ServerNames.Add(si.sv101_name);

						ptr += Marshal.SizeOf(si);
					}
				}
			}
			catch
			{
			}
			finally
			{
				NetApiBufferFree(ppSVINFO);
			}

			return ServerNames.ToArray(typeof(string)) as string[];
		}
	}
}
