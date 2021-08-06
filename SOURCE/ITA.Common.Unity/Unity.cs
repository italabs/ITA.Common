using System;
using Microsoft.Practices.Unity.Configuration;
using Unity;

namespace ITA.Common.Unity
{
	public static class Unity
	{
		public static readonly IUnityContainer Container;

		static Unity()
		{
			var oContainer = Container = new UnityContainer();

            //if you want to log diagnostic info uncomment the line below
		    //Container.AddExtension(new Diagnostic());

            var logger = Log4NetItaHelper.GetLogger(typeof(Unity).Name);

			try
			{
				oContainer.LoadConfiguration();
			}
			catch (Exception e)
			{
				logger.Fatal("Error has occurred while Unity-container initialization.", e);
				throw;
			}
		}
	}
}
