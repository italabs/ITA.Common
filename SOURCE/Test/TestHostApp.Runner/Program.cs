using System;
using System.Reflection;
using ConsoleApp;
using ITA.Common;
using ITA.Common.Host.Interfaces;
using Unity;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestHostApp.Runner
{
    class Program
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(global::DummyHost.DummyHost).Name);

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("AppDomain Error: ", (Exception)e.ExceptionObject);
        }

        private static async System.Threading.Tasks.Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Log4NetHelper.ConfigureWithProcessId();

                IApplicationHost applicationHost = null;

                applicationHost = CreateHost(args);

                if (applicationHost.Debug)
                {
                    //run console version of application
                    applicationHost.RunDebug();
                }
                else
                {
                    IServiceFactory serviceFactory = ITA.Common.Unity.Unity.Container.Resolve<IServiceFactory>();
                    //run daemon
                    IHost host = new HostBuilder()
                        .ConfigureServices((hostContext, services) =>
                        {
                            //add to DI container our host implementation
                            services.AddSingleton(typeof(IApplicationHost), applicationHost);

                            //add to DI container system host implementation
                            services.AddSingleton(typeof(IHostedService), serviceFactory.GetHostedServiceType());

                            logger.Info("Running the Host");

                        })
                        .Build();

                    await host.RunAsync();
                }
            }
            catch (Exception x)
            {
                logger.Error("Exception caught:", x);
                Console.WriteLine(x.Message);
            }
        }

        private static IApplicationHost CreateHost(string[] args)
        {
            string engineInfo = String.Format("TestNetCore Engine v{0} (c) ITA 2016-{1}",
                Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now.Year);

            logger.Info(engineInfo);

            logger.Info("Creating Host object");

            IApplicationHost applicationHost = ITA.Common.Unity.Unity.Container.Resolve<IApplicationHost>("ApplicationHost");

            logger.Info("Initializing Host object");


            applicationHost.Caption = "Test NetCoreApp Engine";
            applicationHost.Copyright = engineInfo;
            applicationHost.Usage = "Usage: dotnet ConsoleApp instance=<instance name> [debug=<false|true>]";
            applicationHost.ServiceName = "TestNetCoreEngineSvc";

            logger.Info("Parsing arguments");

            logger.Info(string.Format("Nargs:{0}", args.Length));

            foreach (string arg in args)
            {
                logger.Info(arg);
            }

            applicationHost.ParseArgs(args);

            IEventLog eventlog = ITA.Common.Unity.Unity.Container.Resolve<IEventLog>();

            logger.Info("Creating Host object");

            var dummyHost = new DummyHost.DummyHost(applicationHost, eventlog);
            applicationHost.Component = dummyHost;

            return applicationHost;
        }
    }
}
