using System;
using System.Linq;

namespace ITA.Common.Passwords
{
    /// <summary>
    /// Utility class responsible for validating passwords against quality rules.
    /// </summary>
    public class PasswordQualityValidator
    {
        /// <summary>
        /// Validates the specified password over the specified quality rules.
        /// </summary>
        public static bool Validate(string password, PasswordQuality quality, out string errorMessage)
        {
            Helpers.CheckNull(password, "password");
            Helpers.CheckNull(quality, "quality");
            if (!quality.Validate())
            {
                throw new ArgumentException(PasswordQualityMessages.E_INVALID_QUALITY_PARAMS);
            }

            if (quality.AsciiOnly && !IsAscii(password))
            {
                errorMessage = PasswordQualityMessages.ASCII_ONLY;
                return false;
            }

            if (quality.Repeated.HasValue && quality.Repeated.Value > 0 && HasRepeatedSymbolsCount(password, quality.Repeated.Value))
            {
                errorMessage = string.Format(PasswordQualityMessages.REPEATED_NOT_MORE_THAN, quality.Repeated.Value);
                return false;
            }

            if (quality.Max.HasValue && password.Length > quality.Max.Value)
            {
                errorMessage = string.Format(PasswordQualityMessages.LENGTH_NOT_MORE_THAN, quality.Max.Value);
                return false;
            }

            if (!ValidateSymbols(password, quality.Alpha, PasswordSymbols.Alpha, PasswordQualityMessages.NO_ALPHA, 
                PasswordQualityMessages.ALPHA_NOT_LESS_THAN, out errorMessage))
            {
                return false;
            }

            if (!ValidateSymbols(password, quality.Lower, PasswordSymbols.Lower, PasswordQualityMessages.NO_LOWER,
                PasswordQualityMessages.LOWER_NOT_LESS_THAN, out errorMessage))
            {
                return false;
            }

            if (!ValidateSymbols(password, quality.Upper, PasswordSymbols.Upper, PasswordQualityMessages.NO_UPPER,
                PasswordQualityMessages.UPPER_NOT_LESS_THAN, out errorMessage))
            {
                return false;
            }

            if (!ValidateSymbols(password, quality.Number, PasswordSymbols.Number, PasswordQualityMessages.NO_NUMBER,
               PasswordQualityMessages.NUMBER_NOT_LESS_THAN, out errorMessage))
            {
                return false;
            }

            if (!ValidateSymbols(password, quality.Special, PasswordSymbols.Special, PasswordQualityMessages.NO_SPECIAL,
                  PasswordQualityMessages.SPECIAL_NOT_LESS_THAN, out errorMessage))
            {
                return false;
            }

            if (quality.Min.HasValue && password.Length < quality.Min.Value)
            {
                errorMessage = string.Format(PasswordQualityMessages.LENGTH_NOT_LESS_THAN, quality.Min.Value);
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Checks the specified password for ASCII symbols.
        /// </summary>
        private static bool IsAscii(string password)
        {
            return password.All(symbol => (int) symbol <= 127);
        }

        /// <summary>
        /// Checks the specified password for existense of the specified symbols.
        /// </summary>
        private static bool ValidateSymbols(string password, int? minCount, string symbolsToCheck,
            string noSymbolRuleMessage, string lackSymbolRuleMessage, out string errorMessage)
        {
            if (minCount.HasValue)
            {
                if (minCount.Value == 0 && password.IndexOfAny(symbolsToCheck.ToCharArray()) >= 0)
                {
                    errorMessage = noSymbolRuleMessage;
                    return false;
                }

                int symbolCount = password.Count(symbolsToCheck.Contains);

                if (minCount.Value > 0 && symbolCount < minCount.Value)
                {
                    errorMessage = string.Format(lackSymbolRuleMessage, minCount.Value);
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Checks repeted symbols in the specified password.
        /// </summary>
        private static bool HasRepeatedSymbolsCount(string password, int repeated)
        {
            if (password.Length > 0)
            {
                char s = password[0];
                int r = 1;

                for (int i = 1; i < password.Length; i++)
                {
                    r = password[i] == s ? r + 1 : 1;
                    if (r > repeated)
                    {
                        return true;
                    }

                    s = password[i];
                }
            }
            else
            {
                return false;
            }

            return false;
        }
    }
}
