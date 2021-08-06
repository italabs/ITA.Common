using System;
using ITA.Common.Host.ConfigManager;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    /// <summary>
    /// Тестирование работы RegistrySettingsStorage вне контекста.
    /// </summary>
    [TestFixture]
    public class RegistrySettingsStorageTests
    {
        [Test, Order(1)]
        public void WriteReadKey()
        {
            string valueToWrite = Guid.NewGuid().ToString();

            RegistrySettingsStorageHKCU regStorage = new RegistrySettingsStorageHKCU(@"Software\ITA.Common\Test");
            regStorage["Key", "Property"] = valueToWrite;
            string valueToRead = (string)regStorage["Key", "Property"];

            Assert.AreEqual(valueToWrite, valueToRead);
        }
    }
}
