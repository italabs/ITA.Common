using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using ITA.Common.UI;
using ITA.Wizards.DatabaseWizard.Model;
using ITA.WizardsUITest.DemoDbWizard;
using log4net;
using log4net.Config;

namespace ITA.WizardsUITest
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

		    var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(logRepository);

			DbProviderSettings settings = null;
			using (Wizard wzd = new Wizard())
			{
				if (args != null && args.Length > 0)
					Console.WriteLine(DbProviderSettings.GetUsageMessage(AppDomain.CurrentDomain.FriendlyName));

				settings = wzd.SetDbProviderSetings(args);
				// try run in unattended mode)
				if (settings.UnattendedMode)
					wzd.RunUnattended(args);
			}

			if (!settings.UnattendedMode)
				Application.Run(new Form1(args));
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = (Exception)e.ExceptionObject;
			ErrorMessageBox.Show(ex, "Произошла неожиданная ошибка", "Тест", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			ErrorMessageBox.Show(e.Exception, "Произошла неожиданная ошибка", "Тест", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
