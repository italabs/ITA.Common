using System;
using System.Text;

namespace ITA.Common.UI
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Информация о произошедшем исключении для отображения в ГУИ
        /// </summary>
        /// <returns></returns>
        public static string GetExceptionMessage(Exception ex)
        {
            var messages = new StringBuilder();

            string delimeter = " :: ";

            if (ex != null)
            {
                IErrorSource source = null;

                if (ex is ITARemoteException)
                    source = new ExceptionDetailSource(((ITARemoteException) ex).Detail);
                else
                    source = new ExceptionSource(ex);

                messages.Append(source.Message);

                IErrorSource innerSource = source;

                if (innerSource != null)
                {
                    while (true)
                    {
                        innerSource = innerSource.InnerSource;

                        if (innerSource == null)
                            break;

                        messages.Append(delimeter);
                        messages.Append(innerSource.Message);
                    }
                }
            }

            return messages.ToString();
        }


        /// <summary>
        /// Терминация строки - установка на конце точки, если нет знаков . ! ? и т.д.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string TerminateMessage(string msg)
        {
            if (msg == null)
                return null;

            msg = msg.TrimEnd();

            if (string.IsNullOrEmpty(msg))
                return msg;

            if (!msg.EndsWith(".") && !msg.EndsWith("?") && !msg.EndsWith("!"))
                return msg + ".";

            return msg;
        }
    }
}