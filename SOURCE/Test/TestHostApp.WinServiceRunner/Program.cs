using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ConsoleApp;
using ITA.Common;
using ITA.Common.Host;
using ITA.Common.Host.Windows;
using log4net;

namespace TestHostApp.WinServiceRunner
{
    class Program
    {
        private static readonly ILog logger = Log4NetItaHelper.GetLogger(typeof(global::DummyHost.DummyHost).Name);

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Log4NetHelper.ConfigureWithProcessId();

            try
            {
                Console.WriteLine(RuntimeEnvironment.GetSystemVersion());
                Console.ReadLine();
                string engineInfo = String.Format("TestNetCore Engine v{0} (c) ITA 2016-{1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now.Year);

                logger.Info(engineInfo);

                logger.Info("Creating Host object");

                BaseApplicationHost Host = new BaseApplicationHost();

                logger.Info("Initializing Host object");


                Host.Caption = "Test NetCoreApp Engine";
                Host.Copyright = engineInfo;
                Host.Usage = "Usage: dotnet ConsoleApp instance=<instance name> [debug=<false|true>]";
                Host.ServiceName = "TestNetCoreEngineSvc";

                logger.Info("Parsing arguments");

                logger.Info(string.Format("Nargs:{0}", args.Length));

                foreach (string arg in args)
                {
                    logger.Info(arg);
                }

                Host.ParseArgs(args);

                logger.Info("Creating DummyHost object");

                EventLog eventlog = new EventLog();

                var host = new DummyHost.DummyHost(Host, eventlog);

                Host.Component = host;

                logger.Info("Running the DummyHost");
                
                CommonWinService winService = new CommonWinService(Host, null);
                winService.Run();
            }
            catch (Exception x)
            {
                logger.Error("Exception caught:", x);
                Console.WriteLine(x.Message);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            logger.Error("AppDomain Error: ", (Exception)e.ExceptionObject);
        }
    }
}
