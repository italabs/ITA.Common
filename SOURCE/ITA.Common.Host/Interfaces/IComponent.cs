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
    /// ������� �������� ��� ���������� �������.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// ��� �����������.
        /// </summary>
        string Name { get; }

        string PublishedAs { get; }

        /// <summary>
        /// ������������ ����������.
        /// </summary>
        IComponentConfig Config { get; }

        /// <summary>
        /// ������ ���������� (Error, Running,... )
        /// </summary>
        EComponentStatus Status { get; }

        /// <summary>
        /// �������������� ���������.
        /// </summary>
        void Initialize();

        /// <summary>
        /// ���������� ������ ����������.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// ��������� ���������.
        /// </summary>
        void Start();   

        /// <summary>
        /// ������������� ���������.
        /// </summary>
        void Stop();

        /// <summary>
        /// ���������������� ������ ����������.
        /// </summary>
        void Pause();

        /// <summary>
        /// ���������� ������ ����������.
        /// </summary>
        void Continue();
    }

    public delegate void OnIComponentError(IComponent Source, Exception x);

    public delegate void OnIComponentEvent(IComponent Source, string ID, EEventType Type, params object[] Args);

    /// <summary>
    /// ���������� � ���������
    /// </summary>
    public interface IComponentWithEvents : IComponent
    {
        /// <summary>
        /// ������ ������� ����������.
        /// </summary>
        string[] Events { get; }

        /// <summary>
        /// ���������� ������� <see cref="OnEvent"/>.
        /// </summary>
        /// <param name="ID">������������� ������� (OnStarting, OnStarted,...).</param>
        /// <param name="Type">��� ������� (Information, Warning,...)</param>
        /// <param name="Args">�������������� ��������.</param>
        void FireEvent(string ID, EEventType Type, params object[] Args);

        /// <summary>
        /// ���������� ������� <see cref="OnError"/>.
        /// </summary>
        /// <param name="ex">��������� ����������.</param>
        void FireError(Exception ex);

        /// <summary>
        /// ����������� ������� <see cref="OnFatalError"/>.
        /// </summary>
        /// <param name="ex">��������� ����������.</param>
        void FireFatalError(Exception ex);

        /// <summary>
        /// �������, ����������� � ������ ������.
        /// </summary>
        event OnIComponentError OnError;

        /// <summary>
        /// ������������ ������� ����������.
        /// </summary>
        event OnIComponentEvent OnEvent;

        /// <summary>
        /// �������, ����������� � ������ ��������� ������.
        /// </summary>
        event OnIComponentError OnFatalError;
    }

}