using System;
using System.Collections.Generic;

namespace ITA.Common.Microservices.Helpers
{
    /// <summary>
    /// Static helper class for checking argument values.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws an exception <see cref="ArgumentNullException"/>, if value equals is `NULL`.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void NotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentException"/>, if the value
        /// of the specified argument is an empty string.
        /// </summary>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void NotNullOrWhiteSpace(string argumentValue, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(argumentValue))
            {
                throw new ArgumentException(@"String cannot be empty.", argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentNullException"/>, if the value of the
        /// specified argument is `NULL`. If the argument value is an empty list,
        /// then an exception <see cref = "ArgumentException" /> is raised.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void NotNullOrEmpty<T>(IReadOnlyList<T> argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argumentValue.Count == 0)
            {
                throw new ArgumentException(@"The list cannot be empty.", argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if the value
        /// of the specified argument is less than or equal to <paramref name = "value" />.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="value">Value for equals.</param>
        public static void GreaterThan<TArg>(TArg argumentValue, TArg value, string argumentName)
            where TArg : IComparable
        {
            if (argumentValue.CompareTo(value) != 1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if
        /// the value of the specified argument is less than or equal to zero.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void GreaterThanZero<TArg>(TArg argumentValue, string argumentName)
            where TArg : struct, IComparable
        {
            if (argumentValue.CompareTo(default(TArg)) != 1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if
        /// the value of the specified argument is less than <paramref name = "value" />.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="value">Value for equals.</param>
        public static void GreaterThanOrEqualTo<TArg>(TArg argumentValue, TArg value, string argumentName)
            where TArg : IComparable
        {
            if (argumentValue.CompareTo(value) == -1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if
        /// the value of the specified argument is less than zero.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void GreaterThanOrEqualToZero<TArg>(TArg argumentValue, string argumentName)
            where TArg : struct, IComparable
        {
            if (argumentValue.CompareTo(default(TArg)) == -1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if
        /// the value of the specified argument is greater than or equal to <paramref name = "value" />.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="value">Value for equals.</param>
        public static void LessThan<TArg>(TArg argumentValue, TArg value, string argumentName)
            where TArg : IComparable
        {
            if (argumentValue.CompareTo(value) != -1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if
        /// the value of the specified argument is greater than or equal to zero.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void LessThanZero<TArg>(TArg argumentValue, string argumentName)
            where TArg : struct, IComparable
        {
            if (argumentValue.CompareTo(default(TArg)) != -1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if the
        /// value of the specified argument is greater than <paramref name = "value" />.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <param name="value">Value for equals.</param>
        public static void LessThanOrEqualTo<TArg>(TArg argumentValue, TArg value, string argumentName)
            where TArg : IComparable
        {
            if (argumentValue.CompareTo(value) == 1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentException"/>, if the specified period is incorrect.
        /// </summary>
        /// <param name="startDateTime">Start date.</param>
        /// <param name="endDateTime">End date.</param>
        /// <param name="startDateTimeArgumentName">Argument name for start date.</param>
        /// <param name="endDateTimeArgumentName">Argument name for end date.</param>
        public static void Range(DateTime? startDateTime, DateTime? endDateTime, string startDateTimeArgumentName, string endDateTimeArgumentName)
        {
            if (startDateTime > endDateTime)
            {
                throw new ArgumentException($"The period is incorrect `{startDateTimeArgumentName}` - `{endDateTimeArgumentName}`.");
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentOutOfRangeException"/>, if the value of the specified argument is greater than zero.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void LessThanOrEqualToZero<TArg>(TArg argumentValue, string argumentName)
            where TArg : struct, IComparable
        {
            if (argumentValue.CompareTo(default(TArg)) == 1)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception <see cref="ArgumentException"/>, if the value of the specified argument is the default.
        /// </summary>
        /// <typeparam name="TArg">Argument type.</typeparam>
        /// <param name="argumentValue">Argument value.</param>
        /// <param name="argumentName">Argument name.</param>
        public static void NotDefault<TArg>(TArg argumentValue, string argumentName)
        {
            if (argumentValue == null || argumentValue.Equals(default(TArg)))
            {
                throw new ArgumentException(@"The value cannot be the default", argumentName);
            }
        }
    }
}
