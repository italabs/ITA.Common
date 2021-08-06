namespace ITA.Common.Host.RuntimeInformation
{
    public enum RuntimeMoniker
    {
        /// <summary>
        /// the same Runtime as the host Process (default setting)
        /// </summary>
        HostProcess = 0,
        /// <summary>
        /// not recognized, possibly a new version of .NET Core
        /// </summary>
        NotRecognized,
        /// <summary>
        /// .NET 4.6.1
        /// </summary>
        Net461,
        /// <summary>
        /// .NET 4.6.2
        /// </summary>
        Net462,
        /// <summary>
        /// .NET 4.7
        /// </summary>
        Net47,
        /// <summary>
        /// .NET 4.7.1
        /// </summary>
        Net471,
        /// <summary>
        /// .NET 4.7.2
        /// </summary>
        Net472,
        /// <summary>
        /// .NET 4.8
        /// </summary>
        Net48,
        /// <summary>
        /// .NET Core 2.0
        /// </summary>
        NetCoreApp20,
        /// <summary>
        /// .NET Core 2.1
        /// </summary>
        NetCoreApp21,
        /// <summary>
        /// .NET Core 2.2
        /// </summary>
        NetCoreApp22,
        /// <summary>
        /// .NET Core 3.0
        /// </summary>
        NetCoreApp30,
        /// <summary>
        /// .NET Core 3.1
        /// </summary>
        NetCoreApp31,
        /// <summary>
        /// .NET Core 5.0 aka ".NET 5"
        /// </summary>
        NetCoreApp50
    }
}