using System;
using System.Net;
using System.Collections;
//using System.Runtime.Remoting;
//using System.Runtime.Remoting.Channels;
//using System.Runtime.Remoting.Channels.Tcp;
//using System.Runtime.Remoting.Channels.Http;
using System.Runtime.InteropServices;

namespace ITA.Common.Host
{
	/// <summary>
	/// Summary description for ComponentPublisher
	/// </summary>
	//[ClassInterface(ClassInterfaceType.None)]
	//public class ComponentPublisher : ObjRef
	//{
	//	private bool m_bOwnChannel = false;
	//	private IChannel m_Channel = null;
	//	private ObjRef m_Ref = null;

	//	private int m_Port = 0;
	//	private System.Net.IPAddress m_Address = IPAddress.None;

	//	public ComponentPublisher()
	//	{
	//	}

	//	public int Port
	//	{
	//		get { return m_Port; }
	//	}

	//	public IPAddress Address
	//	{
	//		get { return m_Address; }
	//	}

	//	public void Publish(ComponentWithEvents Published, System.Net.IPAddress NetAddress, int Port, string Url, EChannelType ChannelType, string InstanceName)
	//	{
	//		Publish(Published, Published.GetType(), NetAddress, Port, Url, ChannelType, InstanceName);
	//	}

	//	public void Publish(ComponentWithEvents Published, System.Type PublishType, System.Net.IPAddress NetAddress, int Port, string Url, EChannelType ChannelType, string InstanceName)
	//	{
	//		if (m_Ref == null)
	//		{
	//			BinaryServerFormatterSinkProvider BinProvider = new BinaryServerFormatterSinkProvider();
	//			SoapServerFormatterSinkProvider SoapProvider = new SoapServerFormatterSinkProvider();

	//			BinProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
	//			SoapProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

	//			IChannel Ch = ChannelServices.GetChannel(Published.Name + "-" + InstanceName);
	//			if (Ch == null)
	//			{
	//				IDictionary Props = new Hashtable();
	//				Props["name"] = Published.Name + "-" + InstanceName;
	//				Props["port"] = Port;
	//				Props["bindTo"] = NetAddress.ToString();

	//				switch (ChannelType)
	//				{
	//					case EChannelType.tcp:
	//						Ch = new TcpServerChannel(Props, BinProvider);
	//						break;
	//					case EChannelType.http:
	//						Ch = new HttpServerChannel(Props, BinProvider);
	//						break;
	//				}

	//				ChannelServices.RegisterChannel(Ch);

	//				m_Address = NetAddress;
	//				m_Port = Port;
	//				m_bOwnChannel = true;
	//			}

	//			m_Channel = Ch;
	//			m_Ref = RemotingServices.Marshal(Published, Url, PublishType);
	//		}
	//	}


	//	public void Unpublish(ComponentWithEvents Published)
	//	{
	//		try
	//		{
	//			if (m_Ref != null)
	//			{
	//				RemotingServices.Unmarshal(m_Ref, true);
	//				m_Ref = null;
	//			}
	//		}
	//		catch (Exception x)
	//		{
	//			Published.FireError(x);
	//		}

	//		try
	//		{
	//			m_Port = 0;
	//			m_Address = IPAddress.None;
	//			if (m_Channel != null && m_bOwnChannel)
	//			{
	//				ChannelServices.UnregisterChannel(m_Channel);
	//				m_Channel = null;
	//			}

	//			GC.Collect();
	//			GC.WaitForPendingFinalizers();
	//		}
	//		catch (Exception x)
	//		{
	//			Published.FireError(x);
	//		}
	//	}
	//}
}
