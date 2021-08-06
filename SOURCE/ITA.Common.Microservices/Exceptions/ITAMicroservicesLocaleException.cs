using System;
using System.Reflection;
using System.Runtime.Serialization;
using ITA.Common;
using ITA.Common.Microservices.Exceptions;

[assembly: AssemblyResourceInfo(typeof(ITAMicroservicesLocaleException), "ITA.Common.Microservices.Messages")]

namespace ITA.Common.Microservices.Exceptions
{
    [Serializable]
    public class ITAMicroservicesLocaleException : ITAException
    {
        public ITAMicroservicesLocaleException(string id, Exception innerEx, params object[] args)
            : base(id, id, innerEx, args)
        {
        }

        public ITAMicroservicesLocaleException(string id, params object[] args)
            : base(id, id, args)
        {
        }

        public ITAMicroservicesLocaleException(string id, Exception innerEx)
            : base(id, id, innerEx)
        {
        }

        public ITAMicroservicesLocaleException(string id)
            : base(id, id)
        {
        }

        protected ITAMicroservicesLocaleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        static ITAMicroservicesLocaleException()
        {
            LocaleMessages.GlobalInstance.RegisterAssembly(Assembly.GetExecutingAssembly());
        }

        public override string Message
        {
            get
            {
                string localizedMessage = LocaleMessages.GlobalInstance.GetString(GetType(), ID, Args);

                if (string.IsNullOrEmpty(localizedMessage))
                {
                    localizedMessage = ID;
                }
                return localizedMessage;
            }
        }

        public const string E_MICROSERVICE_COMPONENT_INITIALIZE_ERROR = nameof(E_MICROSERVICE_COMPONENT_INITIALIZE_ERROR);
        public const string E_HOST_START_ERROR = nameof(E_HOST_START_ERROR);
        public const string E_HOST_COMPONENT_STOP_ERROR = nameof(E_HOST_COMPONENT_STOP_ERROR);
    }
}