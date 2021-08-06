using System;
using System.Collections.Generic;
using System.Text;
using ITA.Common.Host.Interfaces;

namespace BusinessComponent
{
    public class BusinessLogicConfig : IComponentConfig
    {
        public string Name => BusinessLogic.COMPONENT_NAME;

        private readonly IConfigManager _configManager;

        public BusinessLogicConfig(IConfigManager configManager)
        {
            _configManager = configManager;
        }

        public void CommitRequiredParameters()
        {
            if (_configManager[Name, "MyTimeout", null] == null)
            {
                _configManager[Name, "MyTimeout"] = 30;
            }

            if (_configManager[Name, "MyPath", null] == null)
            {
                _configManager[Name, "MyPath"] = @"d:\TestNetCoreApplication.txt";
            }
        }

        public int MyTimeout
        {
            get
            {
                return (int)_configManager[Name, "MyTimeout", 30];
            }
            set { _configManager[Name, "MyTimeout"] = value; }
        }

        public string TestPath
        {
            get
            {
                return (string)_configManager[Name, "MyPath", @"d:\TestNetCoreApplication.txt"];
            }
            set { _configManager[Name, "MyPath"] = value; }
        }

    }
}
