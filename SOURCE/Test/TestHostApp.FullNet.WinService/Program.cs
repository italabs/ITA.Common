using System;
using System.Reflection;
using ConsoleApp;
using ITA.Common;
using ITA.Common.Host.Interfaces;
using ITA.Common.Host.Windows;
using Unity;
using log4net;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(DummyHost.DummyHost).Name);

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("AppDomain Error: ", (Exception)e.ExceptionObject);
        }

        private static void Main(string[] args)
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
                    CommonWinService winService = new CommonWinService(applicationHost, null);
                    winService.Run();
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
