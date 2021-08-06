namespace ITA.Common.Passwords
{
    public static class PasswordSymbols
    {
        public const string LowerAscii = "abcdefghijklmnopqrstuvwxyz";
        public const string LowerCyr = "абвгдеёжзиклмнопрстуфхцчшщьыъэюя";
        public const string Lower = LowerAscii + LowerCyr;

        public const string UpperAscii = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string UpperCyr = "АБВГДЕЁЖЗИКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ";
        public const string Upper = UpperAscii + UpperCyr;

        public const string Number = "0123456789";
        public const string Special = @"!@#$%^&*()~'\/?><_=+-""";

        public const string AlphaAscii = LowerAscii + UpperAscii;
        public const string AlphaCyr = LowerCyr + UpperCyr;
        public const string Alpha = Lower + Upper;

        public const string AnyAscii = UpperAscii + UpperAscii + Number + Special;
        public const string AnyCyr = LowerCyr + UpperCyr + Number + Special;
        public const string Any = Upper + Lower + Number + Special;
    }
}
