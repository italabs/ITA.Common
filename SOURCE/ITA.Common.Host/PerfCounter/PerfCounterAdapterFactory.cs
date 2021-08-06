using System;
using System.IO;
using System.Reflection;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.RuntimeInformation;
using log4net;

namespace ITA.Common.Host.PerfCounter
{
    internal class PerfCounterAdapterFactory : IPerCounterAdapterFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PerfCounterAdapter));

        private const string WindowsAssemblyFileName = "ITA.Common.Host.Windows.dll";
        private const string WindowsAssemblyName = "ITA.Common.Host.Windows";
        private const string WindowsAdapterTypeName = "ITA.Common.Host.Windows.WinPerformanceCounterAdapter";

        private const string EventCounterAssemblyFileName = "ITA.Common.Host.EventCounters.dll";
        private const string EventCounterAssemblyName = "ITA.Common.Host.EventCounters";
        private const string EventCounterAdapterTypeName = "ITA.Common.Host.EventCounters.EventCounterAdapter";

        private static IPerformanceCounterAdapter _adapter;

        #region Implementation of IPerCounterAdapterFactory

        public IPerformanceCounterAdapter CreateAdapter() => _adapter ?? (_adapter = InternalCreate());

        #endregion

        protected virtual IPerformanceCounterAdapter InternalCreate()
        {
            var type = GetPerfCounterType();

            Log.Debug($"Initializing performance counter implementation '{type}'");

            switch (type)
            {
                case PerfCounterType.WindowsClassic:
                    return LoadAdapterFromLibrary(WindowsAssemblyName, WindowsAssemblyFileName, WindowsAdapterTypeName);
                case PerfCounterType.ModernCore:
                    return LoadAdapterFromLibrary(EventCounterAssemblyName, EventCounterAssemblyFileName, EventCounterAdapterTypeName);
                case PerfCounterType.InMemory:
                    return new CrossPlatformPerformanceCounterAdapter();
                default:
                    throw new NotSupportedException($"Counter type '{type}' is not supported.");
            }
        }

        protected virtual PerfCounterType GetPerfCounterType()
        {
            var runtime = RuntimeHelper.GetCurrentRuntimeType();

            if (RuntimeHelper.IsLinux())
            {
                Log.Debug($"Linux runtime '{runtime}'");

                return PerfCounterType.ModernCore;
            }

            if (RuntimeHelper.IsWindows())
            {
                if (RuntimeHelper.IsFullFramework)
                {
                    Log.Debug($"Windows full framework runtime '{runtime}'");

                    return PerfCounterType.WindowsClassic;
                }

                if (RuntimeHelper.IsNetCore)
                {
                    Log.Debug($"Windows .NET Core runtime '{runtime}'");

                    if (runtime >= RuntimeMoniker.NetCoreApp30)
                    {
                        return PerfCounterType.ModernCore;
                    }
                }
            }

            Log.Debug($"Initializing default (in-memory) PerfCounter implementation. Runtime '{runtime}'");

            return PerfCounterType.InMemory;
        }

        #region Assembly loading

        private IPerformanceCounterAdapter LoadAdapterFromLibrary(string assemblyName, string assemblyFileName, string adapterTypeName)
        {
            try
            {
                var assembly = LoadLibrary(assemblyName, assemblyFileName);
                if (assembly == null)
                    throw new Exception($"Couldn't load assembly '{assemblyFileName}'");

                return CreateAdapter(assembly, adapterTypeName);
            }
            catch (FileNotFoundException ex)
            {
                // не считаем за ошибку отсутствие сборки с реализацией счётчиков
                // в этом случае будет использоваться дефолтная реализация
                Log.Warn($"Couldn't find assembly '{assemblyName}' with PerfCounter implementation", ex);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"Couldn't load PerfCounter implementation from the assembly '{assemblyName}'", ex);
                throw;
            }
        }

        private IPerformanceCounterAdapter CreateAdapter(Assembly assembly, string adapterTypeName)
        {
            var type = assembly.GetType(adapterTypeName);
            if (type == null)
                throw new ArgumentException($"Couldn't load type {adapterTypeName} from the assembly '{assembly.FullName}'", nameof(adapterTypeName));

            var adapterInstance = Activator.CreateInstance(type) as IPerformanceCounterAdapter;
            if (adapterInstance == null)
                throw new Exception($"'{type.FullName}' from the assembly '{assembly.FullName}' cannot be cast to IPerformanceCounterAdapter");

            return adapterInstance;
        }

        private Assembly LoadLibrary(string assemblyName, string assemblyFileName)
        {
            var curAssembly = Assembly.GetExecutingAssembly();
            if (curAssembly.GlobalAssemblyCache)
            {
                // если сборка ITA.Common.Host находится в GAC, то пытаемся загрузить сборку со счетчиками
                // по полному имени, используя версию, культуру и токен текущей сборки

                var curAssemblyName = curAssembly.GetName();
                var curAssemblyPublicKeyToken = BitConverter.ToString(curAssemblyName.GetPublicKeyToken()).Replace("-", "");
                var curAssemblyCultureName = !string.IsNullOrEmpty(curAssemblyName.CultureName)
                    ? curAssemblyName.CultureName
                    : "neutral";

                return Assembly.Load($"{assemblyName}, Version={curAssemblyName.Version}, Culture={curAssemblyCultureName}, PublicKeyToken={curAssemblyPublicKeyToken}");
            }
            else
            {
                // загружаем сборку со счетчиками по пути, в котором находится текущая сборка
                var curDir = Path.GetDirectoryName(curAssembly.Location);
                return Assembly.LoadFrom(Path.Combine(curDir, assemblyFileName));
            }
        }

        #endregion
    }
}