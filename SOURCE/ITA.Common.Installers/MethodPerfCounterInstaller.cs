using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Reflection;
using ITA.Common.Host.Windows.Extensions;
using log4net;

namespace ITA.Common.Host
{
    [RunInstaller(true)]
    public class MethodPerfCounterInstaller : Installer
    {
        private static ILog logger = Log4NetItaHelper.GetLogger(typeof(ComponentPerfCounterInstaller).Name);

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;

        private readonly Type _componentType;
        private string _appPrefix;

        public MethodPerfCounterInstaller(Type componentType, string appPrefix)
        {
            Helpers.CheckNull(componentType, "componentType");
            Helpers.CheckNullOrEmpty(appPrefix, "appPrefix");

            _componentType = componentType;
            _appPrefix = appPrefix;

            // This call is required by the Designer.
            InitializeComponent();
        }

        public Type ComponentType
        {
            get { return _componentType; }
        }

        #region Installer implementation

        public override void Commit(IDictionary savedState)
        {
            try
            {
                RegisterCounters();
            }
            catch (Exception ex)
            {
                Context.LogMessage(string.Format("Failed to install counters of '{0}': '{1}'", _componentType.FullName, ex.Message));
                throw;
            }

            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            UnregisterCounters();
            base.Uninstall(savedState);
        }
        #endregion

        #region Implementation

        private class MethodAndCounters
        {
            public MethodInfo MethodName;
            public List<MethodPerfCountedAttribute> Attributes;
        }

        private void RegisterCounters()
        {
            var lst = GetCountersByType();
            Context.LogMessage(string.Format("There are {0} counter attributes found for '{1}'", lst.Count, _componentType.Name));

            var dict = GetAttributesByCategories(lst);

            foreach (var category in dict)
            {
                var catName = category.Key;
                var catDesc = PerfCounterHelper.BuildCountersCategoryDescription(catName, logger);
                var categoryCounters = category.Value;

                DeletePerformanceCategory(catName);

                var CCDC = new CounterCreationDataCollection();

                foreach (var methodCounters in categoryCounters)
                {
                    CreateCountersDataCollection(ref CCDC, methodCounters.Key, methodCounters.Value);
                }

                PerformanceCounterCategory.Create(catName, catDesc, CCDC);
                Context.LogMessage(string.Format("Category '{0}' created. The number of installed counters is {1}", catName, CCDC.Count));
            }
        }

        private Dictionary<string, Dictionary<string, MethodPerfCountedAttribute>> GetAttributesByCategories(Dictionary<MethodInfo, MethodPerfCountedAttribute> lst)
        {
            var dict = new Dictionary<string, Dictionary<string, MethodPerfCountedAttribute>>();

            foreach (var pair in lst)
            {
                var categoryName = PerfCounterHelper.BuildCountersCategoryName(pair.Value.CategoryName ?? ComponentTypeName(), _appPrefix, logger);

                if (!dict.ContainsKey(categoryName))
                {
                    var value = new Dictionary<string, MethodPerfCountedAttribute>
                    {
                        {
                            pair.Key.Name,
                            pair.Value
                        }
                    };

                    dict.Add(categoryName, value);
                    Context.LogMessage(string.Format("New category '{0}' added to dict", categoryName));
                }
                else
                {
                    var value = dict[categoryName];
                    value.Add(pair.Key.Name, pair.Value);
                }
            }
            return dict;
        }

        private void CreateCountersDataCollection(ref CounterCreationDataCollection CCDC, string methodName, MethodPerfCountedAttribute counterAttr)
        {
            foreach (PerformanceCounterInfo counterInfo in counterAttr.CountersInfos)
            {
                var counterName = MethodPerfCountedAttribute.BuildCounterName(methodName, counterInfo.CounterName);

                var CounterData = new CounterCreationData
                {
                    CounterName = counterName,
                    CounterHelp = counterInfo.CounterDecription,
                    CounterType = counterInfo.CounterType.ToPerformanceCounterType(),
                };

                try
                {
                    Context.LogMessage(string.Format("\tCounter '{0}'", counterName));

                    CCDC.Add(CounterData);

                    CounterData = new CounterCreationData
                    {
                        CounterName = counterName + "Base",
                        CounterHelp = counterInfo.CounterDecription
                    };

                    switch (counterInfo.CounterType.ToPerformanceCounterType())
                    {
                        case PerformanceCounterType.AverageTimer32:
                        case PerformanceCounterType.AverageCount64:
                            {
                                CounterData.CounterType = PerformanceCounterType.AverageBase;
                                CCDC.Add(CounterData);
                            }
                            break;
                        case PerformanceCounterType.CounterMultiTimer:
                        case PerformanceCounterType.CounterMultiTimerInverse:
                        case PerformanceCounterType.CounterMultiTimer100Ns:
                        case PerformanceCounterType.CounterMultiTimer100NsInverse:
                            {
                                CounterData.CounterType = PerformanceCounterType.CounterMultiBase;
                                CCDC.Add(CounterData);
                            }
                            break;
                        case PerformanceCounterType.RawFraction:
                            {
                                CounterData.CounterType = PerformanceCounterType.RawBase;
                                CCDC.Add(CounterData);
                            }
                            break;
                        case PerformanceCounterType.SampleCounter:
                        case PerformanceCounterType.SampleFraction:
                            {
                                CounterData.CounterType = PerformanceCounterType.SampleBase;
                                CCDC.Add(CounterData);
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Context.LogMessage(string.Format("Failed to add counter '{0}' to counters collection: '{1}'", CounterData.CounterName, e.Message));
                }
            }
        }

        private void DeletePerformanceCategory(string catName)
        {
            if (PerformanceCounterCategory.Exists(catName))
            {
                Context.LogMessage(string.Format("Category '{0}' already exists", catName));
                PerformanceCounterCategory.Delete(catName);
                Context.LogMessage(string.Format("Category '{0}' has been deleted", catName));
            }
        }

        private Dictionary<MethodInfo, MethodPerfCountedAttribute> GetCountersByType()
        {
            var methods = _componentType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            var lst = new Dictionary<MethodInfo, MethodPerfCountedAttribute>();

            foreach (MethodInfo methodInfo in methods)
            {
                var attrs = methodInfo.GetCustomAttributes(typeof(MethodPerfCountedAttribute), false);

                foreach (MethodPerfCountedAttribute att in attrs)
                {
                    lst.Add(methodInfo, att);
                }
            }
            return lst;
        }

        private void UnregisterCounters()
        {
            var lst = GetCountersByType();
            var dict = GetAttributesByCategories(lst);

            foreach (var pair in dict)
            {
                try
                {
                    PerformanceCounterCategory.Delete(pair.Key);
                    Context.LogMessage(string.Format("Counters category '{0}' has been deleted", pair.Key));
                }
                catch (Exception ex)
                {
                    Context.LogMessage(string.Format("Unable to delete counters category '{0}'. Exception: '{1}'", pair.Key, ex.Message));
                }
            }
        }

        private string ComponentTypeName()
        {
            return _componentType.Name;
        }

        #endregion

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}