using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Interfaces;
using ITA.Common.Host.Interfaces;

namespace DummyHost
{
    [ClassInterface(ClassInterfaceType.None)]
    internal class EngineServiceConfig : ITA.Common.Host.Components.ServiceConfig, IEngineServiceConfig
    {
        public static string cCfgStandAlone = "StandAlone";

        public EngineServiceConfig(IConfigManager configManager, CultureInfo defaultCulture)
            : base(configManager)
        {
            DefaultCulture = defaultCulture;
        }

        public override string Name
        {
            get { return DummyHost.ComponentName; }
        }

        public CultureInfo DefaultCulture { get; private set; }
    }

}
