#include "stdafx.h"
#include "ITAControlManager.h"

using namespace ITA_Common_Host_Failover;
using namespace ITA::Common::Transport::Native;

ITAControlManager::ITAControlManager(BSTR url, BSTR windowsServiceName, long waitTimeout): 
	initialized(false), 
	url(url), 
	windowsServiceName(windowsServiceName), 
	waitTimeout(waitTimeout)
{
	initialized = Init();
}

ITAControlManager::~ITAControlManager(void)
{	
	Release();
}

bool ITAControlManager::Open()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->Open(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::Terminate()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->Terminate(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::Online()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->Online(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::Offline()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->Offline(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::IsAlive()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->IsAlive(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::LooksAlive()
{
	VARIANT_BOOL res = VARIANT_FALSE;

	HRESULT hres = resource->LooksAlive(&res);

	CheckHRESULT(hres, true);

	return res == VARIANT_TRUE;
}

bool ITAControlManager::Init()
{
	HRESULT hres = CoInitialize(NULL); 
	if (FAILED(hres))
	{
		_com_error error = _com_error(hres);

		printf("Init: COM error code='%d' message='%s'\n", hres, error.ErrorMessage());

		return false;		
	}	

	printf("Init: COM initialized success\n");

	hres = resource.CreateInstance(CLSID_FailoverClusterResource);

	printf("Init: Create COM IFailoverClusterResourcePtr\n");
	
	printf("Init: url='%S' waitTimeout='%d' windowsServiceName='%S'\n", url, waitTimeout, windowsServiceName);

	resource->put_Url(url);
	resource->put_WaitTimeout(waitTimeout);
	resource->put_WindowsService(windowsServiceName);

	return true;
}

void ITAControlManager::Release()
{
	resource = NULL;

	if (initialized == true)
	{
		CoUninitialize();
		printf("Release COM release success\n");
	}
	else
	{
		printf("Release COM was not initialized\n");
	}
}

bool ITAControlManager::CheckHRESULT(HRESULT hres, bool throwIfError)
{
	if (FAILED(hres))
	{
		_com_error error = _com_error(hres);

		printf("CheckHRESULT COM error code='%d' message='%s'\n", hres, error.ErrorMessage());

		if (throwIfError)
		{
			throw error;
		}

		return false;		
	}

	return true;
}