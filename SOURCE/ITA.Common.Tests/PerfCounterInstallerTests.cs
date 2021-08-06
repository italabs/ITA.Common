using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ITA.Common.Host;
using ITA.Common.Host.Enums;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    #region test environment
    [Counter("Test custom counter 1", "Counter 1 description", PerfCounterInstallerTests.CATEGORY_NAME1, "Category description", ItaPerformanceCounterType.NumberOfItems64)]
    [Counter("Test custom counter 2", "Counter 2 description", PerfCounterInstallerTests.CATEGORY_NAME2, "Category 2 description", ItaPerformanceCounterType.AverageCount64)]
    public class PerfCounterTestClass
    {
        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        public void PublicMethod()
        {
            
        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        private void PrivateMethod()
        {
            
        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        protected void ProtectedMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        private static void StaticMethod()
        {

        }
    }

    [Counter("Test custom counter 1", "Counter 1 description", PerfCounterInstallerTests.CATEGORY_NAME2, "Category 2 description", ItaPerformanceCounterType.NumberOfItems64)]
    [Counter("Test custom counter 2", "Counter 2 description", PerfCounterInstallerTests.CATEGORY_NAME1, "Category description", ItaPerformanceCounterType.AverageCount64)]
    public class PerfCounterTestClass2
    {
        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        public void PublicMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        private void PrivateMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        protected void ProtectedMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        private static void StaticMethod()
        {

        }
    }

    [Counter("Test custom counter 1", "Counter 1 description", PerfCounterInstallerTests.CATEGORY_NAME1, "Category description", ItaPerformanceCounterType.NumberOfItems64)]
    [Counter("Test custom counter 2", "Counter 2 description", PerfCounterInstallerTests.CATEGORY_NAME1, "Category description", ItaPerformanceCounterType.AverageCount64)]
    public class PerfCounterTestClass3
    {
        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        public void PublicMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        private void PrivateMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        protected void ProtectedMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME2, ECountingMode.All)]
        private static void StaticMethod()
        {

        }
    }

    [Counter("Test custom counter 1", "Counter 1 description", PerfCounterInstallerTests.CATEGORY_NAME2, "Category 2 description", ItaPerformanceCounterType.NumberOfItems64)]
    [Counter("Test custom counter 2", "Counter 2 description", PerfCounterInstallerTests.CATEGORY_NAME1, "Category description", ItaPerformanceCounterType.AverageCount64)]
    public class PerfCounterTestClass4
    {
        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        public void PublicMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        private void PrivateMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        protected void ProtectedMethod()
        {

        }

        [MethodPerfCounted("TestPerfCounterInstallerApp", PerfCounterInstallerTests.CATEGORY_NAME1, ECountingMode.All)]
        private static void StaticMethod()
        {

        }
    }
    #endregion

    [TestFixture]
    public class PerfCounterInstallerTests : TestBase
    {
        public const string CATEGORY_NAME1 = "TestCategory{1D9D1279-FBF5-46E4-8DCA-22AF3849B014}";
        public const string CATEGORY_NAME2 = "TestCategoryNew{1D9D1279-FBF5-46E4-8DCA-22AF3849B014}";
        public const string APP_PREFIX = "TestPrefix";

        [Test, Order(1)]
        public void TestRegisteringMethods()
        {
            string[] _methodsCheckList = { "PublicMethod", "PrivateMethod", "ProtectedMethod", "StaticMethod" };

            string assemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string installLogFilePath = Path.Combine(assemblyDirectory, "install.log");

            var perfCounterInstaller = new MethodPerfCounterInstaller(typeof(PerfCounterTestClass), APP_PREFIX);

            IDictionary state = new Hashtable();
            try
            {
                //register counters
                perfCounterInstaller.Context = new InstallContext(installLogFilePath, null);
                perfCounterInstaller.Install(state);
                perfCounterInstaller.Commit(state);

                var category = PerformanceCounterCategory.GetCategories().FirstOrDefault(c => c.CategoryName.Equals(CATEGORY_NAME1));
                //check category registering
                Assert.NotNull(category, string.Format("Category '{0}' not exists", CATEGORY_NAME1));
                var counters = category.GetCounters();
                //check counters registering
                foreach (var method in _methodsCheckList)
                {
                    Assert.NotNull(
                        counters.FirstOrDefault(
                            counter =>
                                counter.CounterName.StartsWith(method, StringComparison.InvariantCultureIgnoreCase)));
                }
            }
            finally
            {
                perfCounterInstaller.Uninstall(state);
            }
        }

        [Test, Order(2)]
        public void TestCommonInstallerRegisteringCounters()
        {
            Dictionary<string, string[]> _checkList = new Dictionary<string, string[]>()
            {
                { CATEGORY_NAME1, new [] { "PublicMethod", "PrivateMethod", "ProtectedMethod", "StaticMethod" }},
                { APP_PREFIX + ": " + CATEGORY_NAME2, new [] { "Test custom counter 2" }},
                { APP_PREFIX + ": " + CATEGORY_NAME1, new [] { "Test custom counter 1" }},
            };

            string assemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string installLogFilePath = Path.Combine(assemblyDirectory, "install.log");
            List<Type> _componentTypes = new List<Type>();
            _componentTypes.Add(typeof (PerfCounterTestClass));
            var perfCounterInstaller = new CommonPerfCounterInstaller(_componentTypes, APP_PREFIX);

            IDictionary state = new Hashtable();
            try
            {
                //register counters
                perfCounterInstaller.Context = new InstallContext(installLogFilePath, null);
                perfCounterInstaller.Install(state);
                perfCounterInstaller.Commit(state);

                foreach (var pair in _checkList)
                {
                    var category = PerformanceCounterCategory.GetCategories().FirstOrDefault(c => c.CategoryName.Equals(pair.Key));
                    //check category registering
                    Assert.NotNull(category, string.Format("Category '{0}' not exists", pair.Key));
                    var counters = category.GetCounters();
                    //check counters registering
                    foreach (var method in pair.Value)
                    {
                        Assert.NotNull(
                            counters.FirstOrDefault(
                                counter =>
                                    counter.CounterName.StartsWith(method, StringComparison.InvariantCultureIgnoreCase)));
                    }
                }
            }
            finally
            {
                perfCounterInstaller.Uninstall(state);
            }
        }

        [Test, Order(3)]
        public void TestCommonInstallerRegisteringCountersOnFewComponents()
        {
            Dictionary<string, string[]> _checkList = new Dictionary<string, string[]>()
            {
                {CATEGORY_NAME1, new [] { "PublicMethod", "PrivateMethod", "ProtectedMethod", "StaticMethod" }},
                {CATEGORY_NAME2, new [] { "PublicMethod", "PrivateMethod", "ProtectedMethod", "StaticMethod" }},
                { APP_PREFIX + ": " + CATEGORY_NAME1, new [] { "Test custom counter 1", "Test custom counter 2" }},
                { APP_PREFIX + ": " + CATEGORY_NAME2, new [] { "Test custom counter 1", "Test custom counter 2" }},
            };

            string assemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string installLogFilePath = Path.Combine(assemblyDirectory, "install.log");
            List<Type> _componentTypes = new List<Type>();
            _componentTypes.Add(typeof(PerfCounterTestClass));
            _componentTypes.Add(typeof(PerfCounterTestClass2));
            var perfCounterInstaller = new CommonPerfCounterInstaller(_componentTypes, APP_PREFIX);

            IDictionary state = new Hashtable();
            try
            {
                //register counters
                perfCounterInstaller.Context = new InstallContext(installLogFilePath, null);
                perfCounterInstaller.Install(state);
                perfCounterInstaller.Commit(state);

                foreach (var pair in _checkList)
                {
                    var category = PerformanceCounterCategory.GetCategories().FirstOrDefault(c => c.CategoryName.Equals(pair.Key));
                    //check category registering
                    Assert.NotNull(category, string.Format("Category '{0}' not exists", pair.Key));
                    var counters = category.GetCounters();
                    //check counters registering
                    foreach (var method in pair.Value)
                    {
                        Assert.NotNull(
                            counters.FirstOrDefault(
                                counter =>
                                    counter.CounterName.StartsWith(method, StringComparison.InvariantCultureIgnoreCase)));
                    }
                }
            }
            finally
            {
                perfCounterInstaller.Uninstall(state);
            }
        }

        [Test, Order(4)]
        public void TestCommonInstallerThrowExceptionOnDuplicateRegisteringCounters()
        {
            string assemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string installLogFilePath = Path.Combine(assemblyDirectory, "install.log");
            List<Type> _componentTypes = new List<Type>();
            _componentTypes.Add(typeof(PerfCounterTestClass));
            _componentTypes.Add(typeof(PerfCounterTestClass3));
            var perfCounterInstaller = new CommonPerfCounterInstaller(_componentTypes, APP_PREFIX);

            IDictionary state = new Hashtable();
            //register counters
            perfCounterInstaller.Context = new InstallContext(installLogFilePath, null);
            perfCounterInstaller.Install(state);
            Assert.Throws<InstallException>(()=>perfCounterInstaller.Commit(state), "Expected InstallException was not throwed.");
        }

        [Test, Order(5)]
        public void TestCommonInstallerThrowExceptionOnDuplicateRegisteringCounters2()
        {
            string assemblyDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string installLogFilePath = Path.Combine(assemblyDirectory, "install.log");
            List<Type> _componentTypes = new List<Type>();
            _componentTypes.Add(typeof(PerfCounterTestClass));
            _componentTypes.Add(typeof(PerfCounterTestClass4));
            var perfCounterInstaller = new CommonPerfCounterInstaller(_componentTypes, APP_PREFIX);

            IDictionary state = new Hashtable();
            //register counters
            perfCounterInstaller.Context = new InstallContext(installLogFilePath, null);
            perfCounterInstaller.Install(state);
            Assert.Throws<InstallException>(() => perfCounterInstaller.Commit(state), "Expected InstallException was not throwed.");
        }
    }
}
