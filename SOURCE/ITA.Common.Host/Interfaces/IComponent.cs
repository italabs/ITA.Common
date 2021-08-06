using System;
using System.Diagnostics;

namespace ITA.Common.Host.Interfaces
{   
    /// <summary>
    /// Summary description for IComponent.
    /// </summary>
    public interface IComponentConfig
    {
        string Name { get; }
    }

    /// <summary>
    /// Базовый контракт для копонентов системы.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Имя конмпонента.
        /// </summary>
        string Name { get; }

        string PublishedAs { get; }

        /// <summary>
        /// Конфигурация компонента.
        /// </summary>
        IComponentConfig Config { get; }

        /// <summary>
        /// Статус компонента (Error, Running,... )
        /// </summary>
        EComponentStatus Status { get; }

        /// <summary>
        /// Инициализирует компонент.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Прекращает работу компонента.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Запускает компонент.
        /// </summary>
        void Start();   

        /// <summary>
        /// Остановливает компонент.
        /// </summary>
        void Stop();

        /// <summary>
        /// Приостанавливает работу компонента.
        /// </summary>
        void Pause();

        /// <summary>
        /// Продолжает работу компонента.
        /// </summary>
        void Continue();
    }

    public delegate void OnIComponentError(IComponent Source, Exception x);

    public delegate void OnIComponentEvent(IComponent Source, string ID, EEventType Type, params object[] Args);

    /// <summary>
    /// Компоненты с событиями
    /// </summary>
    public interface IComponentWithEvents : IComponent
    {
        /// <summary>
        /// Список событий компонента.
        /// </summary>
        string[] Events { get; }

        /// <summary>
        /// Генерирует событие <see cref="OnEvent"/>.
        /// </summary>
        /// <param name="ID">Идентификатор события (OnStarting, OnStarted,...).</param>
        /// <param name="Type">Тип события (Information, Warning,...)</param>
        /// <param name="Args">Дополнительные аргумены.</param>
        void FireEvent(string ID, EEventType Type, params object[] Args);

        /// <summary>
        /// Генерирует событие <see cref="OnError"/>.
        /// </summary>
        /// <param name="ex">Экземпляр исключения.</param>
        void FireError(Exception ex);

        /// <summary>
        /// Гененрирует событие <see cref="OnFatalError"/>.
        /// </summary>
        /// <param name="ex">Экземпляр исключения.</param>
        void FireFatalError(Exception ex);

        /// <summary>
        /// Событие, возникающее в случае ошибки.
        /// </summary>
        event OnIComponentError OnError;

        /// <summary>
        /// Произвольное событие компонента.
        /// </summary>
        event OnIComponentEvent OnEvent;

        /// <summary>
        /// Событие, возникающее в случае фатальной ошибки.
        /// </summary>
        event OnIComponentError OnFatalError;
    }

}