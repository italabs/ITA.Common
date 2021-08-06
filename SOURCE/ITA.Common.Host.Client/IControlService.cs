using System.ServiceModel;

namespace ITA.Common.Host
{
    /// <summary>
    /// Service management interface.
    /// </summary>
    [ServiceContract]
    public interface IControlService
    {
        [OperationContract]
        [FaultContract(typeof(ServiceExceptionDetail))]
        void Start();

        [OperationContract]
        [FaultContract(typeof(ServiceExceptionDetail))]
        void Stop();

        [OperationContract]
        [FaultContract(typeof(ServiceExceptionDetail))]
        void Pause();

        [OperationContract]
        [FaultContract(typeof(ServiceExceptionDetail))]
        void Continue();

        bool AutoStart
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            set;
        }

        bool EnableSuspend
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            set;
        }

        EOnPowerEventBehaviour OnBatteryAction
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            set;
        }

        EOnPowerEventBehaviour OnLowBatteryAction
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            set;
        }

        EOnPowerEventBehaviour OnSuspendAction
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            set;
        }

        string InstanceID
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
        }

        string InstanceName
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
        }

        string ServiceName
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
        }

        string ServiceDisplayName
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
        }

        EComponentStatus ServiceStatus
        {
            [OperationContract]
            [FaultContract(typeof(ServiceExceptionDetail))]
            get;
        }
    }
}
