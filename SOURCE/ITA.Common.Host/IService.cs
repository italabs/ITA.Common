namespace ITA.Common.Host
{
    /// <summary>
    /// Параметры сервиса
    /// </summary>
    public interface IService
    {
        string ServiceName { get; }
        
        string ServiceDisplayName { get; }
        
        bool Debug { get; }
       
        /// <summary>
        /// Сервер стартуется автостартом или же пользователем вручную
        /// </summary>
        bool IsAutoStart { get;}

        /// <summary>
        /// Имя экземпляра сервиса (default).
        /// </summary>
        string InstanceName { get; }
    }
}
