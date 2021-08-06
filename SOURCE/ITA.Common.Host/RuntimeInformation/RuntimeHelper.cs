using System;
using System.Runtime.InteropServices;

namespace ITA.Common.Host.RuntimeInformation
{
    public static class RuntimeHelper
    {
        internal const string Unknown = "?";

        public static bool IsFullFramework => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);

        public static bool IsNetNative => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);

        public static bool IsNetCore => System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase);

        public static bool IsRunningInContainer => string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true");

        public static bool IsWindows() => System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux() => System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
       
        public static RuntimeMoniker GetCurrentRuntimeType()
        {
            //do not change the order of conditions because it may cause incorrect determination of runtime
            if (IsFullFramework)
                return ClrRuntime.GetCurrentVersion().RuntimeMoniker;
            if (IsNetCore)
                return CoreRuntime.GetCurrentVersion().RuntimeMoniker;

            throw new NotSupportedException("Unknown .NET Runtime");
        }
    }
}