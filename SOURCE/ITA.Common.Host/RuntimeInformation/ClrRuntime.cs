﻿using System;

namespace ITA.Common.Host.RuntimeInformation
{
    public class ClrRuntime : Runtime, IEquatable<ClrRuntime>
    {
        public static readonly ClrRuntime Net461 = new ClrRuntime(RuntimeMoniker.Net461, "net461", ".NET 4.6.1");
        public static readonly ClrRuntime Net462 = new ClrRuntime(RuntimeMoniker.Net462, "net462", ".NET 4.6.2");
        public static readonly ClrRuntime Net47 = new ClrRuntime(RuntimeMoniker.Net47, "net47", ".NET 4.7");
        public static readonly ClrRuntime Net471 = new ClrRuntime(RuntimeMoniker.Net471, "net471", ".NET 4.7.1");
        public static readonly ClrRuntime Net472 = new ClrRuntime(RuntimeMoniker.Net472, "net472", ".NET 4.7.2");
        public static readonly ClrRuntime Net48 = new ClrRuntime(RuntimeMoniker.Net48, "net48", ".NET 4.8");

        public string Version { get; }

        private ClrRuntime(RuntimeMoniker runtimeMoniker, string msBuildMoniker, string displayName, string version = null)
            : base(runtimeMoniker, msBuildMoniker, displayName)
        {
            Version = version;
        }
        
        public override bool Equals(object obj) => obj is ClrRuntime other && Equals(other);

        public bool Equals(ClrRuntime other) => other != null && base.Equals(other) && Version == other.Version;

        public override int GetHashCode() => base.GetHashCode() ^ (Version?.GetHashCode() ?? 0);

        internal static ClrRuntime GetCurrentVersion()
        {
            if (!RuntimeHelper.IsWindows())
            {
                throw new NotSupportedException("Full .NET Framework supports Windows OS only.");
            }

            var version = FrameworkVersionHelper.GetFrameworkReleaseVersion(); // .NET Developer Pack is not installed

            switch (version)
            {
                case "4.6.1": return Net461;
                case "4.6.2": return Net462;
                case "4.7": return Net47;
                case "4.7.1": return Net471;
                case "4.7.2": return Net472;
                case "4.8": return Net48;
                default: // unlikely to happen but theoretically possible
                    return new ClrRuntime(RuntimeMoniker.NotRecognized, $"net{version.Replace(".", null)}", $".NET {version}");
            }
        }
    }
}