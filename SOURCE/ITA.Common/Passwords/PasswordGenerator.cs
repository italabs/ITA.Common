using System;
using System.Collections.Generic;

namespace ITA.Common.Passwords
{
    /// <summary>
    /// Utility class responsible for generating passwords according to the quality rules.
    /// </summary>
    public static class PasswordGenerator
    {
        private const int MAX_GENERATION_ATTEMPTS = 100;
        private const int DEFAULT_MAX_LENGTH = 10;
        private const int DEFAULT_MIN_LENGTH = 4;

        private static readonly Random _rand = new Random();

        public static string Generate(PasswordQuality quality)
        {
            Helpers.CheckNull(quality, "quality");
            if (!quality.Validate())
            {
                throw new ArgumentException(PasswordQualityMessages.E_INVALID_QUALITY_PARAMS);
            }

            string password = string.Empty;

            for (int genAttempt = 0; genAttempt < MAX_GENERATION_ATTEMPTS; genAttempt++)
            {
                List<char> symbols = new List<char>();

                // If specified, generate Lower symbols.
                if (quality.Alpha != 0 && quality.Lower.HasValue)
                {
                    symbols.AddRange(GetSymbols(PasswordSymbols.LowerAscii, quality.Lower.Value));
                }

                // If specified, generate Upper symbols.
                if (quality.Alpha != 0 && quality.Upper.HasValue)
                {
                    symbols.AddRange(GetSymbols(PasswordSymbols.UpperAscii, quality.Upper.Value));
                }

                // If specified, generate more Alpha (Lower + Upper) symbols.
                if (quality.Alpha.HasValue && quality.Alpha.Value != 0 && symbols.Count < quality.Alpha.Value)
                {
                    symbols.AddRange(GetSymbols(PasswordSymbols.AlphaAscii, quality.Alpha.Value - symbols.Count));
                }

                // If specified, generate Number symbols.
                if (quality.Number.HasValue)
                {
                    symbols.AddRange(GetSymbols(PasswordSymbols.Number, quality.Number.Value));
                }

                // If specified, generate Special symbols.
                if (quality.Special.HasValue)
                {
                    symbols.AddRange(GetSymbols(PasswordSymbols.Special, quality.Special.Value));
                }

                // Calculate remaining symbols count.
                int minLength = Math.Max(symbols.Count, quality.Min ?? 0);
                minLength = minLength == 0 ? DEFAULT_MIN_LENGTH : minLength;

                int maxlength = Math.Max(symbols.Count, quality.Max ?? 0);
                maxlength = Math.Max(maxlength == 0 ? DEFAULT_MAX_LENGTH : maxlength, minLength);

                int remainingSymbolsCount = _rand.Next(minLength, maxlength + 1) - symbols.Count;

                // Prepare remaining symbols dictionary.
                string remainingSymbols =
                    ((quality.Alpha != 0 && quality.Lower != 0) ? PasswordSymbols.LowerAscii : string.Empty) +
                    ((quality.Alpha != 0 && quality.Upper != 0) ? PasswordSymbols.UpperAscii : string.Empty) +
                    ((quality.Number != 0) ? PasswordSymbols.Number : string.Empty);
                if (remainingSymbols.Length == 0)
                {
                    remainingSymbols = PasswordSymbols.Special;
                }

                // Generate remaining symbols of choosen type (Lower, Upper, Number of Special).
                symbols.AddRange(GetSymbols(remainingSymbols, remainingSymbolsCount));

                // Mix generated symbols.
                int resultCount = symbols.Count;
                char[] result = new char[resultCount];
                for (int i = 0; i < resultCount; i++)
                {
                    int index = _rand.Next(0, symbols.Count);
                    result[i] = symbols[index];
                    symbols.RemoveAt(index);
                }
                password = new string(result);

                // Perform self-check
                string errorMessage;
                if (PasswordQualityValidator.Validate(password, quality, out errorMessage))
                {
                    break;
                }
            }

            return password;
        }

        private static List<char> GetSymbols(string symbols, int count)
        {
            List<char> result = new List<char>();
            int actualCount = 0;
            while (actualCount < count)
            {
                result.Add(symbols[_rand.Next(0, symbols.Length)]);
                actualCount++;
            }
            return result;
        }
    }
}
