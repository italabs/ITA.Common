// TestFailover.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ITAControlManager.h"

using namespace ITA_Common_Host_Failover;
using namespace ITA::Common::Transport::Native;

int _tmain(int argc, _TCHAR* argv[])
{
	try
	{
		printf("Begin\n\n");

		BOOL init = FALSE;
		VARIANT_BOOL controlWindowsService = 0;
		BSTR url = ::SysAllocString(L"net.pipe://localhost/ITAEngine/default");
		BSTR windowsServiceName = ::SysAllocString(L"ITAEngineSvc_default");

		try	
		{	
			ITAControlManager *manager = new ITAControlManager(url, windowsServiceName);

			bool result = manager->IsAlive();
			printf("IsAlive: %s\n", result ? "TRUE" : "FALSE");	

			result = manager->Offline(); // Logical stop
			printf("Call Offline: %s\n", result ? "TRUE" : "FALSE");			

			result = manager->IsAlive();
			printf("IsAlive: %s\n", result ? "TRUE" : "FALSE");

			result = manager->Online(); // Logical start
			printf("Call Online: %s\n", result ? "TRUE" : "FALSE");			

			result = manager->IsAlive();
			printf("IsAlive: %s\n", result ? "TRUE" : "FALSE");

			result = manager->Terminate(); // Service stop
			printf("Call Terminate: %s\n", result ? "TRUE" : "FALSE");			

			result = manager->IsAlive();
			printf("IsAlive: %s\n", result ? "TRUE" : "FALSE");

			result = manager->Online(); // Logical start
			printf("Call Online: %s\n", result ? "TRUE" : "FALSE");			

			result = manager->IsAlive();
			printf("IsAlive: %s\n", result ? "TRUE" : "FALSE");

			delete manager;
		}
		catch(_com_error& ex)
		{
			printf("ERROR: %s", ex.Description());			
		}

		::SysFreeString(url);
		::SysFreeString(windowsServiceName);
	}
	catch(const char * str)
	{
		printf("Error: %s", str);
		return 1;
	}
	return 0;
}

