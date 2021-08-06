
#include "comdef.h"
#include "ITA.Common.Host.Failover.tlh"

using namespace ITA_Common_Host_Failover;

namespace ITA { namespace Common { namespace Transport { namespace Native
{
	class ITAControlManager
	{
	public:
		ITAControlManager(BSTR url, BSTR windowsServiceName, long waitTimeout = 30000);
		~ITAControlManager(void);

		bool Open();
		bool Online();
		bool Offline();
		bool IsAlive();
		bool LooksAlive();
		bool Terminate();

	private:		
		BSTR url;
		BSTR windowsServiceName;		
		long waitTimeout;
		
		bool initialized;
		IFailoverClusterResourcePtr resource;

		bool CheckHRESULT(HRESULT hres, bool throwIfError = false);

	protected:
		bool Init();
		void Release();
	};

}}}}
