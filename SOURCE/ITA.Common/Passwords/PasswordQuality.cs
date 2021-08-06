using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITA.Common.Passwords
{
    /// <summary>
    /// Password quality parameters.
    /// NULL - do not check
    /// 0 - forbidden
    /// 1..N - check symbols count
    /// </summary>
    [DataContract]
    public class PasswordQuality : IEquatable<PasswordQuality>
    {
        /// <summary>
        /// If defined, specifies minimum password length.
        /// </summary>
        [DataMember]
        public int? Min { get; set; }

        /// <summary>
        /// If defined, specifies maximum password length.
        /// </summary>
        [DataMember]
        public int? Max { get; set; }

        /// <summary>
        /// If defined, specifies minimum number of alphanumeric symbols.
        /// </summary>
        [DataMember]
        public int? Alpha { get; set; }

        /// <summary>
        /// If defined, specifies minimum number of upper alphanumeric symbols.
        /// </summary>
        [DataMember]
        public int? Upper { get; set; }

        /// <summary>
        /// If defined, specifies minimum number of lower alphanumeric symbols.
        /// </summary>
        [DataMember]
        public int? Lower { get; set; }

        /// <summary>
        /// If defined, specifies minimum number of numeric symbols.
        /// </summary>
        [DataMember]
        public int? Number { get; set; }

        /// <summary>
        /// If defined, specifies minimum number of special symbols.
        /// </summary>
        [DataMember]
        public int? Special { get; set; }

        /// <summary>
        /// If defined and greater that zero, specifies maximum number of repeated symbols in a row.
        /// </summary>
        [DataMember]
        public int? Repeated { get; set; }

        /// <summary>
        /// Indicates if password should contain only ASCII symbols.
        /// </summary>
        [DataMember]
        public bool AsciiOnly { get; set; }

        /// <summary>
        /// Validates the password quality parameters consistency.
        /// </summary>
        public bool Validate()
        {
            if (Upper == 0 && Lower == 0 && Number == 0 && Special == 0)
                return false;
            if (Min.HasValue && Max.HasValue && Min.Value > Max.Value)
                return false;
            if (Alpha.HasValue && (Upper == 0 || Lower == 0) && Alpha != 0)
                return false;
            if (Max.HasValue && Max.Value < ((Upper ?? 0) + (Lower ?? 0) + (Number ?? 0) + (Special ?? 0)))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the text representation of the password quality.
        /// </summary>
        public List<string> GetQualityRules()
        {
            List<string> rules = new List<string>();

            if (AsciiOnly)
            {
                rules.Add(PasswordQualityMessages.ASCII_ONLY);
            }
            if (Min.HasValue)
            {
                rules.Add(string.Format(PasswordQualityMessages.LENGTH_NOT_LESS_THAN, Min.Value));
            }
            if (Max.HasValue)
            {
                rules.Add(string.Format(PasswordQualityMessages.LENGTH_NOT_MORE_THAN, Max.Value));
            }

            AddQualityRule(rules, Alpha, PasswordQualityMessages.NO_ALPHA, PasswordQualityMessages.ALPHA_NOT_LESS_THAN);
            AddQualityRule(rules, Lower, PasswordQualityMessages.NO_LOWER, PasswordQualityMessages.LOWER_NOT_LESS_THAN);
            AddQualityRule(rules, Upper, PasswordQualityMessages.NO_UPPER, PasswordQualityMessages.UPPER_NOT_LESS_THAN);
            AddQualityRule(rules, Number, PasswordQualityMessages.NO_NUMBER, PasswordQualityMessages.NUMBER_NOT_LESS_THAN);
            AddQualityRule(rules, Special, PasswordQualityMessages.NO_SPECIAL, PasswordQualityMessages.SPECIAL_NOT_LESS_THAN);

            if (Repeated.HasValue && Repeated.Value > 0)
            {
                rules.Add(string.Format(PasswordQualityMessages.REPEATED_NOT_MORE_THAN, Repeated.Value));
            }

            return rules;
        }

        private static void AddQualityRule(List<string> rules, int? rule, string forbidden, string req)
        {
            if (rule.HasValue)
            {
                rules.Add(rule.Value == 0 ? forbidden : string.Format(req, rule.Value));
            }
        }

        public bool Equals(PasswordQuality other)
        {
            return bool.Equals(AsciiOnly, other.AsciiOnly) &&
                   Nullable.Equals(Alpha, other.Alpha) && 
                   Nullable.Equals(Lower, other.Lower) &&
                   Nullable.Equals(Max, other.Max) &&
                   Nullable.Equals(Min, other.Min) &&
                   Nullable.Equals(Number, other.Number) &&
                   Nullable.Equals(Repeated, other.Repeated) &&
                   Nullable.Equals(Special, other.Special) &&
                   Nullable.Equals(Upper, other.Upper);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((PasswordQuality)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 17;
                hashCode = (hashCode * 23) + AsciiOnly.GetHashCode();
                hashCode = (hashCode * 23) + Alpha.GetHashCode();
                hashCode = (hashCode * 23) + Lower.GetHashCode();
                hashCode = (hashCode * 23) + Max.GetHashCode();
                hashCode = (hashCode * 23) + Min.GetHashCode();
                hashCode = (hashCode * 23) + Number.GetHashCode();
                hashCode = (hashCode * 23) + Repeated.GetHashCode();
                hashCode = (hashCode * 23) + Special.GetHashCode();
                hashCode = (hashCode * 23) + Upper.GetHashCode();
                return hashCode;
            }
        }       
    }
}
