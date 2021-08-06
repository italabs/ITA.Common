using System;

namespace ITA.Common.Host.Interfaces
{
    public interface IApplicationHost : IDebugRunner
    {
        void Start();

        void Stop();

        void Continue();

        void Pause();

        void ParseArgs(string[] args);

        string InstanceName { get; set; }

        string ServiceName { get; set; }

        bool Debug { get; }
        string Copyright { get; set; }
        string Caption { get; set; }
        string Usage { get; set; }

        IComponentWithEvents Component { get; set; }

        event EventHandler UpdateFieldsHandler;
    }
}
