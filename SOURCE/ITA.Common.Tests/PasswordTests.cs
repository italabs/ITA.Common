using System;
using System.Linq;
using ITA.Common.Passwords;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    /// <summary>
    /// Тесты парольной политики.
    /// </summary>
    [TestFixture]
    public class PasswordTests : TestBase
    {
        [Test, Order(1)]
        public void TestGenerationValidation()
        {
            PasswordQuality[] qualities =
            {
                new PasswordQuality(),
                new PasswordQuality { Min = 5, Max = 5 },
                new PasswordQuality { Lower = 2, Upper = 2, Alpha = 5, Number = 2, Special = 2, Min = 20, Max = 25 },
                new PasswordQuality { Alpha = 7, Max = 8 },
                new PasswordQuality { Alpha = 0, Max = 8 },
                new PasswordQuality { Alpha = 0, Special = 7, Number = 1, Max = 15 },
                new PasswordQuality { Alpha = 3, Lower = 4, Upper = 2, Max = 8 },
                new PasswordQuality { Min = 8, Upper = 2, Lower = 2, Number = 2 },
                new PasswordQuality { Min = 4 }
            };

            string[] passwords = qualities.Select(PasswordGenerator.Generate).ToArray();

            for (int i = 0; i < passwords.Length; i++)
            {
                string errorMessage;
                Assert.True(PasswordQualityValidator.Validate(passwords[i], qualities[i], out errorMessage));
            }
        }

        [Test, Order(2)]
        public void TestRepeatedSymbols()
        {
            string msg;
            Assert.True(PasswordQualityValidator.Validate("abc", new PasswordQuality { Repeated = 1 }, out msg));
            Assert.False(PasswordQualityValidator.Validate("abbc", new PasswordQuality { Repeated = 1 }, out msg));
            Assert.True(PasswordQualityValidator.Validate("abbc", new PasswordQuality { Repeated = 2 }, out msg));
            Assert.True(PasswordQualityValidator.Validate("abcb", new PasswordQuality { Repeated = 1 }, out msg));
            Assert.False(PasswordQualityValidator.Validate("abbbcd", new PasswordQuality { Repeated = 2 }, out msg));
        }

        [Test, Order(3)]
        public void TestAsciiSymbols()
        {
            PasswordQuality asciiQuality = new PasswordQuality { AsciiOnly = true };
            PasswordQuality freeQuality = new PasswordQuality { AsciiOnly = false };
            string errorMessage;

            // Validate Ascii password
            string asciiPwd = PasswordSymbols.AnyAscii;
            Assert.True(PasswordQualityValidator.Validate(asciiPwd, asciiQuality, out errorMessage));

            // Validate Cyrillic password
            string nonAsciiPwd = PasswordSymbols.AnyCyr;
            Assert.False(PasswordQualityValidator.Validate(nonAsciiPwd, asciiQuality, out errorMessage));
            Assert.True(PasswordQualityValidator.Validate(nonAsciiPwd, freeQuality, out errorMessage));

            // Generated password of any quality includes only Ascii symbols
            string generatedPwd = PasswordGenerator.Generate(new PasswordQuality { Alpha = 6, Max = 12, AsciiOnly = false });
            Assert.True(PasswordQualityValidator.Validate(generatedPwd, asciiQuality, out errorMessage));
        }

        [Test, Order(4)]
        public void TestInvalidGeneration()
        {
            PasswordQuality[] badQualities =
            {
                new PasswordQuality { Min = 5, Max = 4 },
                new PasswordQuality { Lower = 0, Upper = 0, Number = 0, Special = 0 }
            };

            PasswordQuality[] goodQualities =
            {
                new PasswordQuality { Alpha = 0, Lower = 8 },
                new PasswordQuality { Alpha = 0, Upper = 3 },
                new PasswordQuality { Alpha = 5, Special = 1, Number = 1, Max = 5 }
            };

            foreach (PasswordQuality quality in badQualities)
            {
                PasswordQuality qty = quality;
                Assert.Throws<ArgumentException>(() => PasswordGenerator.Generate(qty));
            }

            foreach (PasswordQuality quality in goodQualities)
            {
                PasswordQuality qty = quality;
                Assert.DoesNotThrow(() => PasswordGenerator.Generate(qty));
            }
        }
    }
}
