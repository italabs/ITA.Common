using System;
using ITA.Common.Host.Interfaces;

namespace ITA.Common.Host
{
    /// <summary>
    /// Base universal host implementation class
    /// </summary>
    public class BaseApplicationHost : IApplicationHost
    {
        //
        // Command line parsing stuff
        //
        public const char cSeparator = '=';
        public const string cDebug = "debug";
        public const string cInstance = "instance";
        public const uint ATTACH_PARENT_PROCESS = uint.MaxValue;

        public static string cDefaultInstance = "Default";
        //
        // Runtime properties
        //
        //
        // User defined properties
        //
        private IComponentWithEvents m_Component;
        private bool m_bDebug;

        private string m_szCaption = "";
        private string m_szCopyright = "";
        private string m_szInstanceName = cDefaultInstance;
        private string m_szServiceName = "";
        private string m_szUsage = "";

        public IComponentWithEvents Component
        {
            get { return m_Component; }
            set { m_Component = value; }
        }

        public event EventHandler UpdateFieldsHandler;

        public string InstanceName
        {
            get { return m_szInstanceName; }
            set
            {
                m_szInstanceName = value;
                OnUpdateFields();
            }
        }

        public string ServiceName
        {
            get { return m_szServiceName; }
            set
            {
                m_szServiceName = value;
                OnUpdateFields();
            }
        }

        public bool Debug
        {
            get { return m_bDebug; }
        }

        public string Caption
        {
            get { return m_szCaption; }
            set { m_szCaption = value; }
        }

        public string Copyright
        {
            get { return m_szCopyright; }
            set { m_szCopyright = value; }
        }

        public string Usage
        {
            get { return m_szUsage; }
            set { m_szUsage = value; }
        }

        public void Start()
        {
            m_Component.Initialize(); //means first startup
        }

        public void Stop()
        {
            m_Component.Shutdown(); //means full stop
        }

        public void Pause()
        {
            m_Component.Pause();
        }

        public void Continue()
        {
            m_Component.Continue();
        }

        public void ParseArgs(string[] args)
        {
            try
            {
                foreach (string s in args)
                {
                    string[] Parts = s.Split(cSeparator);
                    if (Parts.Length != 2)
                    {
                        continue;
                    }

                    Parts[0] = Parts[0].Trim();
                    Parts[1] = Parts[1].Trim();

                    if (Parts[0].ToLower() == cDebug) m_bDebug = bool.Parse(Parts[1]);
                    else if (Parts[0].ToLower() == cInstance) InstanceName = Parts[1];
                }
            }
            catch
            {
                throw new Exception("Invalid command line parameters.");
            }
        }

        private void OnError(IComponent Source, Exception x)
        {
            Console.WriteLine("[{0}] Error has occurred: {1}", Source == null ? "Unknown component" : Source.Name,
                              x.Message);
            if (x.InnerException != null)
            {
                OnError(Source, x.InnerException);
            }
        }

        public virtual void Run()
        {

        }

        public virtual void RunDebug()
        {
            Component.OnError += OnError;

            Console.WriteLine(Copyright);

            Console.WriteLine("Instance name: {0}", InstanceName);

            Console.WriteLine("Initializing...");
            Start();
            Console.WriteLine("Ready.");

            Console.WriteLine();
            Console.WriteLine("Press ENTER to shut down...");
            Console.ReadLine();

            Console.WriteLine("Shutting down...");
            Stop();
            Console.WriteLine("Stopped.");
        }

        private void OnUpdateFields()
        {
            UpdateFieldsHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}
