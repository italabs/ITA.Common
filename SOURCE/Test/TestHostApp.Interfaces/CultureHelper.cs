using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Interfaces
{
    public static class CultureHelper
    {
        public static void SetCurrentCulture(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            //Также устанавливаем культуру для всех новых потоков в приложении.
            typeof(CultureInfo).GetField("s_userDefaultUICulture", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic).SetValue(Thread.CurrentThread.CurrentUICulture, cultureInfo);
            typeof(CultureInfo).GetField("s_userDefaultCulture", BindingFlags.GetField | BindingFlags.Static | BindingFlags.NonPublic).SetValue(Thread.CurrentThread.CurrentCulture, cultureInfo);
        }
    }

}
