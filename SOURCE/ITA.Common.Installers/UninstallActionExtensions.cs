using System.ComponentModel;
using ITA.Common.Host;

namespace ITA.Common.Installers
{
    public static class UninstallActionExtensions
    {
        public static System.Configuration.Install.UninstallAction ToUninstallAction(this UninstallAction uninstallAction)
        {
            switch (uninstallAction)
            {
                case UninstallAction.Remove: return System.Configuration.Install.UninstallAction.Remove;
                case UninstallAction.NoAction: return System.Configuration.Install.UninstallAction.NoAction;
            }

            throw new InvalidEnumArgumentException(nameof(uninstallAction));
        }
    }
}
