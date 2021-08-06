using ITA.Common.ETWTest.Components.Services;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace ITA.Common.ETWTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentCultureName = args == null || args.Length < 1 
                ? Thread.CurrentThread.CurrentCulture.Name 
                : args[0]; 

            var savedCulture = Thread.CurrentThread.CurrentCulture;

            try
            {
                var culture = new CultureInfo(currentCultureName);
                SetDefaultCulture(culture);
                Console.WriteLine("Run test with culture '{0}'", culture);

                var thread = new Thread(RunTest)
                {
                    CurrentCulture = culture,
                    CurrentUICulture = culture
                };
                thread.Start();
                thread.Join();

                Console.WriteLine("Test done");
                SetDefaultCulture(savedCulture);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                SetDefaultCulture(savedCulture);
            }
            Console.Read();
        }

        private static void RunTest()
        {
            try
            {
                var service = new MainService();

                var customer = service.CreateCustomer("TestCustomer");

                var order1 = service.CreateOrder("1", "Description1");
                customer = service.AttachOrderToCustomer(customer, order1);

                var order2 = service.CreateOrder("2", "Description2");
                customer = service.AttachOrderToCustomer(customer, order2);

                int count = 0;
                var customers = service.GetCustomers(out count);

                service.MainServiceCall();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }       


        private static void SetDefaultCulture(CultureInfo cultureInfo)
        {
            typeof(CultureInfo)
                .GetField("s_userDefaultCulture", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(Thread.CurrentThread.CurrentCulture, cultureInfo);
            typeof(CultureInfo)
                .GetField("s_userDefaultUICulture",
                    BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(Thread.CurrentThread.CurrentUICulture, cultureInfo);
        }
    }
}
