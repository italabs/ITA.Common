using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using ITA.Common.Host.Windows.Extensions;
using log4net;

namespace ITA.Common.Host
{
    /// <summary>
    /// Installs performance counters for specified type.
    /// </summary>
    [RunInstaller(true)]
    public class ComponentPerfCounterInstaller : BasePerfCounterInstaller
    {
        private static ILog logger = Log4NetItaHelper.GetLogger(typeof(ComponentPerfCounterInstaller).Name);

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;

        public ComponentPerfCounterInstaller(Type componentType)
        {
            // This call is required by the Designer.
            InitializeComponent();

            logger.DebugFormat("Got ComponentType value:'{0}'", componentType != null ? componentType.FullName : "NULL");
            this.ComponentType = componentType;
        }

        public ComponentPerfCounterInstaller(Type componentType, string appName)
            : this(componentType)
        {
            categoryPrefix = appName;
        }

        public Type ComponentType { get; set; }

        protected override void RegisterCategories()
        {
            logger.DebugFormat("RegisterCategories () >>>");
            try
            {
                logger.DebugFormat("ComponentType == {0}", ComponentType != null ? ComponentType.FullName : "NULL");
                base.RegisterCategories(ComponentType.GetCustomAttributes(typeof(CounterAttribute), true));
            }
            catch (Exception ex)
            {
                logger.Error("Exception in RegisterCategories():", ex);
                throw;
            }
            finally
            {
                logger.DebugFormat("RegisterCategories () <<<");
            }
        }

        protected override void UnregisterCategories()
        {
            logger.DebugFormat("UnregisterCategories () >>>");
            try
            {
                logger.DebugFormat("ComponentType == {0}", ComponentType != null ? ComponentType.FullName : "NULL");
                base.UnregisterCategories(ComponentType.GetCustomAttributes(typeof(CounterAttribute), true));
            }
            catch (Exception ex)
            {
                logger.Error("Exception in UnregisterCategories():", ex);
                throw;
            }
            finally
            {
                logger.DebugFormat("UnregisterCategories () <<<");
            }
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }

    /// <summary>
    /// Базовый класс реализующий де\инсталяцию категорий счетчиков производительности
    /// </summary>
    [RunInstaller(false)]
    public abstract class BasePerfCounterInstaller : Installer
    {
        /// <summary>
        /// Prefix which, if defined, is added to the performance counter category name.
        /// </summary>
        protected string categoryPrefix;

        /// <summary>
        /// Регистрация категорий
        /// </summary>
        /// <param name="CounterObjects">Массив объектов-счетчиков</param>
        protected virtual void RegisterCategories(object[] CounterObjects)
        {
            var CategorySortedCollection = new HybridDictionary();

            foreach (CounterAttribute Counter in CounterObjects)
            {
                ArrayList CounterList = null;
                string composedCategory = ComposeCategory(Counter);
                if (CategorySortedCollection.Contains(composedCategory))
                {
                    CounterList = CategorySortedCollection[composedCategory] as ArrayList;
                }
                else
                {
                    CounterList = new ArrayList();
                    CategorySortedCollection.Add(composedCategory, CounterList);
                }

                if (CounterList != null) CounterList.Add(Counter);
            }

            int InstalledCount = 0;

            foreach (string CategoryName in CategorySortedCollection.Keys)
            {
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                    Context.LogMessage(string.Format("Category '{0}' already exists", CategoryName));
                    PerformanceCounterCategory.Delete(CategoryName);
                    Context.LogMessage(string.Format("Category '{0}' has been deleted", CategoryName));
                }

                string CategoryDescription = null;
                var CCDC = new CounterCreationDataCollection();

                var Counters = CategorySortedCollection[CategoryName] as ArrayList;

                if (Counters != null)
                    foreach (CounterAttribute Counter in Counters)
                    {
                        var CounterData = new CounterCreationData
                        {
                            CounterName = Counter.CounterName,
                            CounterType = Counter.CounterType.ToPerformanceCounterType(),
                            CounterHelp = Counter.CounterDescription
                        };

                        CCDC.Add(CounterData);

                        CategoryDescription = Counter.CategoryDescription;

                        CounterData = new CounterCreationData
                        {
                            CounterName = Counter.CounterName + "Base",
                            CounterHelp = Counter.CounterDescription
                        };

                        switch (Counter.CounterType.ToPerformanceCounterType())
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

                PerformanceCounterCategory.Create(CategoryName, CategoryDescription, CCDC);

                Context.LogMessage(string.Format("Category '{0}' created. The number of installed counters is {1}",
                                                 CategoryName, CCDC.Count));
                InstalledCount++;
            }

            Context.LogMessage(string.Format("The number of installed counter categories is: {0}", InstalledCount));
        }

        /// <summary>
        /// Отмена регистрации категорий
        /// </summary>
        /// <param name="CounterObjects">Массив объектов-счетчиков</param>
        protected virtual void UnregisterCategories(object[] CounterObjects)
        {
            var CategorySortedCollection = new HybridDictionary();

            int UnInstalledCount = 0;

            foreach (CounterAttribute Counter in CounterObjects)
            {
                ArrayList CounterList = null;

                string composedCategory = ComposeCategory(Counter);
                if (CategorySortedCollection.Contains(composedCategory))
                {
                    CounterList = CategorySortedCollection[composedCategory] as ArrayList;
                }
                else
                {
                    CounterList = new ArrayList();
                    CategorySortedCollection.Add(composedCategory, CounterList);
                }

                if (CounterList != null) CounterList.Add(Counter);
            }

            foreach (string CategoryName in CategorySortedCollection.Keys)
            {
                if (PerformanceCounterCategory.Exists(CategoryName))
                {
                    PerformanceCounterCategory.Delete(CategoryName);
                    UnInstalledCount++;
                }
            }

            Context.LogMessage(string.Format("The number of uninstalled counter categories is: {0}", UnInstalledCount));
        }


        protected abstract void RegisterCategories();

        protected abstract void UnregisterCategories();


        public override void Commit(IDictionary savedState)
        {
            RegisterCategories();
            base.Commit(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            UnregisterCategories();
            base.Uninstall(savedState);
        }

        /// <summary>
        /// Category name builds.
        /// </summary>
        /// <param name="counter">Instance of <see cref="CounterAttribute"/> object, describing performance counter.</param>
        /// <returns>
        /// Counter category with or without application prefix. 
        /// If prefix is defined, resulting pattern is this: {prefix}: {origCategoryValue}.
        /// </returns>
        private string ComposeCategory(CounterAttribute counter)
        {
            string result = counter.CategoryName;
            if (!String.IsNullOrEmpty(categoryPrefix))
            {
                result = categoryPrefix + ": " + result;
            }

            return result;
        }
    }
}