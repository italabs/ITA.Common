using System;
using System.Runtime.InteropServices;

namespace ITA.Common
{
    /// <summary>
    /// Класс-хелпер для определения разрядности процесса (x86, x64)
    /// </summary>
    public static class ProcessMode
    {
        private const int c64BitProcessPointerSize = 8;

        /// <summary>
        /// Возвращает true, если процесс 64-разрядный
        /// </summary>
        /// <returns>Boolean</returns>
        public static Boolean Is64Bit()
        {
            return Marshal.SizeOf(typeof(IntPtr)) == c64BitProcessPointerSize;
        }
    }
}
