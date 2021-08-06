using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ITA.Common.Host.Enums;
using ITA.Common.Host.Windows.Extensions;
using log4net;

namespace ITA.Common.Host
{
    /// <summary>
    /// Common installer for registering all counter types - marked by CounterAttribute and MethodPerfCountedAttribute
    /// Installer registering all counters from all components at one time
    /// </summary>
    [RunInstaller(true)]
    public class CommonPerfCounterInstaller : Installer
    {
        private static ILog logger = Log4NetItaHelper.GetLogger(typeof(ComponentPerfCounterInstaller).Name);

        /// <summary>
        /// Inner auxiliary class
        /// </summary>
        private class CounterInfo
        {
            /// <summary>
            /// Counter name
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Counter type
            /// </summary>
            public ItaPerformanceCounterType Type { get; set; }
            /// <summary>
            /// Counter description
            /// </summary>
            public string Description { get; set; }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;

        private readonly List<Type> _componentTypes;

        private string _appPrefix;

        public CommonPerfCounterInstaller(List<Type> componentTypes, string appPrefix)
        {
            Helpers.CheckNull(componentTypes, "componentTypes");
            Helpers.CheckNullOrEmpty(appPrefix, "appPrefix");

            _componentTypes = componentTypes;
            _appPrefix = appPrefix;

            // This call is required by the Designer.
            InitializeComponent();
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
                Context.LogMessage(string.Format("Failed to install counters of '{0}': '{1}'", _appPrefix, ex.Message));
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

        /// <summary>
        /// Appends <see cref="categoryPrefix"/> to the counter category value if prefix is defined.
        /// </summary>
        /// <param name="counter">Instance of <see cref="CounterAttribute"/> object, describing performance counter.</param>
        /// <returns>
        /// Counter category with or without application prefix. 
        /// If prefix is defined, resulting pattern is this: {prefix}: {origCategoryValue}.
        /// </returns>
        private string ComposeCategory(string categoryName)
        {
            string result = categoryName;
            if (!String.IsNullOrEmpty(_appPrefix))
                result = string.Format("{0}: {1}", _appPrefix, result);
            result = result.Length > 80 ? result.Substring(0, 80) : result;
            return result;
        }

        private Dictionary<MethodInfo, MethodPerfCountedAttribute> GetCountersByType(Type componentType)
        {
            var methods = componentType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

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

        private Dictionary<string, List<CounterInfo>> GetCategoriesForMethodDefinedCounters(Type componentType)
        {
            Dictionary<MethodInfo, MethodPerfCountedAttribute> lst = GetCountersByType(componentType);
            Context.LogMessage(string.Format("There are {0} method counter attributes found for '{1}'", lst.Count, componentType.Name));

            var result = new Dictionary<string, List<CounterInfo>>();

            foreach (var pair in lst)
            {
                var categoryName = PerfCounterHelper.BuildCountersCategoryName(
                    string.IsNullOrEmpty(pair.Value.CategoryName) ? componentType.Name : pair.Value.CategoryName,
                    _appPrefix, logger);

                List<CounterInfo> list = null;
                if (result.ContainsKey(categoryName))
                {
                    list = result[categoryName];
                }
                else
                {
                    list = new List<CounterInfo>();
                    result.Add(categoryName, list);
                    Context.LogMessage(string.Format("New category '{0}' added to method attributed result list.", categoryName));
                }

                if (list != null)
                {
                    foreach (PerformanceCounterInfo counterInfo in pair.Value.CountersInfos)
                    {
                        list.Add(new CounterInfo()
                        {
                            Name = MethodPerfCountedAttribute.BuildCounterName(pair.Key.Name, counterInfo.CounterName),
                            Type = counterInfo.CounterType,
                            Description = counterInfo.CounterDecription,
                        });
                    }
                }
            }

            return result;
        }

        private Dictionary<string, List<CounterInfo>> GetCategoriesForClassDefinedCounters(Type componentType)
        {
            var categorySortedCollection = new Dictionary<string, List<CounterInfo>>();

            object[] counterObjects = componentType.GetCustomAttributes(typeof(CounterAttribute), true);

            Context.LogMessage(string.Format("There are {0} class counter attributes found for '{1}'", counterObjects.Length, componentType.Name));

            foreach (CounterAttribute counter in counterObjects)
            {
                List<CounterInfo> counterList = null;
                //TODO: code below must be uncommented after fix issue #41984
                //string category = string.IsNullOrEmpty(counter.CategoryName) 
                //? PerfCounterHelper.BuildCountersCategoryName(componentType.Name, _appPrefix,logger)
                //: counter.CategoryName;

                string category = ComposeCategory(counter.CategoryName);//TODO: this line must be uncommented after fix issue #41984

                if (categorySortedCollection.ContainsKey(category))
                {
                    counterList = categorySortedCollection[category];
                }
                else
                {
                    counterList = new List<CounterInfo>();
                    categorySortedCollection.Add(category, counterList);
                    Context.LogMessage(string.Format("New category '{0}' added to class attributed result list.", category));
                }

                if (counterList != null) counterList.Add(new CounterInfo()
                {
                    Name = counter.CounterName,
                    Type = counter.CounterType,
                    Description = counter.CounterDescription,
                });
            }
            return categorySortedCollection;
        }

        private Dictionary<string, List<CounterInfo>> JoinCounters(Dictionary<string, List<CounterInfo>> counters1, Dictionary<string, List<CounterInfo>> counters2)
        {
            Dictionary<string, List<CounterInfo>> resultDictionary = new Dictionary<string, List<CounterInfo>>();
            if (counters1 != null && counters2 != null && counters1.Count > 0 && counters2.Count > 0)
            {
                foreach (var key in counters1.Keys)
                {
                    resultDictionary.Add(key, counters1[key]);
                }

                foreach (var key in resultDictionary.Keys)
                {
                    if (counters2.ContainsKey(key))
                    {
                        var resultList = counters1[key];
                        var componentList = counters2[key];
                        foreach (CounterInfo counterInfo in componentList)
                        {
                            if (resultList.Any(rl => rl.Name.Equals(counterInfo.Name)))
                            {
                                throw new InstallException(string.Format("Duplicate counter was found. Class counter '{0}' already defined in counters list.", counterInfo.Name));
                            }
                            else
                            {
                                resultList.Add(counterInfo);
                            }
                        }
                    }
                }
                foreach (var key in counters2.Keys)
                {
                    if (!resultDictionary.ContainsKey(key))
                    {
                        resultDictionary.Add(key, counters2[key]);
                    }
                }
            }
            else
            {
                if (counters1 != null && counters1.Count > 0)
                {
                    foreach (var key in counters1.Keys)
                    {
                        resultDictionary.Add(key, counters1[key]);
                    }
                }
                else if (counters2 != null && counters2.Count > 0)
                {
                    foreach (var key in counters2.Keys)
                    {
                        resultDictionary.Add(key, counters2[key]);
                    }
                }
            }
            return resultDictionary;
        }

        private Dictionary<string, List<CounterInfo>> GetCountersByCategories()
        {
            Dictionary<string, List<CounterInfo>> result = new Dictionary<string, List<CounterInfo>>();
            foreach (var componentType in _componentTypes)
            {
                var methodCounters = GetCategoriesForMethodDefinedCounters(componentType);
                var classCounters = GetCategoriesForClassDefinedCounters(componentType);
                //объединим кастомные счетчики и заданные в атрибутах, исключая дубли
                var joinedCounters = JoinCounters(methodCounters, classCounters);
                //объединим с результиурющим списком, чтобы исключить возможные дубли в различных компонентах
                result = JoinCounters(result, joinedCounters);
            }
            return result;
        }

        private void RegisterCounters()
        {
            var resultList = GetCountersByCategories();

            foreach (string category in resultList.Keys)
            {
                var catName = category;
                var catDesc = PerfCounterHelper.BuildCountersCategoryDescription(catName, logger);
                var categoryCounters = resultList[category];

                DeletePerformanceCategory(catName);

                var CCDC = new CounterCreationDataCollection();

                foreach (var counterInfo in categoryCounters)
                {
                    CreateCountersDataCollection(ref CCDC, counterInfo);
                }

                PerformanceCounterCategory.Create(catName, catDesc, CCDC);
                Context.LogMessage(string.Format("Category '{0}' created. The number of installed counters is {1}", catName, CCDC.Count));
            }
            Context.LogMessage(string.Format("The number of installed counter categories is: {0}", resultList.Count));
        }

        private void CreateCountersDataCollection(ref CounterCreationDataCollection CCDC, CounterInfo counterInfo)
        {
            var CounterData = new CounterCreationData
            {
                CounterName = counterInfo.Name,
                CounterHelp = counterInfo.Description,
                CounterType = counterInfo.Type.ToPerformanceCounterType(),
            };

            try
            {
                Context.LogMessage(string.Format("\tCounter '{0}'", counterInfo.Name));

                CCDC.Add(CounterData);

                CounterData = new CounterCreationData
                {
                    CounterName = counterInfo.Name + "Base",
                    CounterHelp = counterInfo.Description
                };

                switch (counterInfo.Type.ToPerformanceCounterType())
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
                Context.LogMessage(string.Format("Failed to add counter '{0}' to counters collection: '{1}'",
                    CounterData.CounterName, e.Message));
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

        private void UnregisterCounters()
        {
            var resultList = GetCountersByCategories();

            foreach (string category in resultList.Keys)
            {
                try
                {
                    DeletePerformanceCategory(category);
                }
                catch (Exception ex)
                {
                    Context.LogMessage(string.Format("Unable to delete counters category '{0}'. Exception: '{1}'", category, ex.Message));
                }
            }
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
