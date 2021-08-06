using System;

namespace ITA.Common
{
    /// <summary>
    /// Contains helper methods.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Checks if the given <paramref name="value"/> is null.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">if the given <paramref name="value"/> is null.</exception>
        /// 
        /// <param name="value">Value for validation.</param>
        /// <param name="parameterName">Parameter name.</param>
        public static void CheckNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Checks if the given <paramref name="value"/> is null or an empty string.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">If the given <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">If t he given <paramref name="value"/> is an empty string.</exception>
        /// 
        /// <param name="value">Value for validation.</param>
        /// <param name="parameterName">Parameter name.</param>
        public static void CheckNullOrEmpty(string value, string parameterName)
        {
            CheckNull(value, parameterName);
            if (value.Trim().Length == 0)
                throw new ArgumentException("Argument cannot be an empty string.", parameterName);
        }
    }
}
