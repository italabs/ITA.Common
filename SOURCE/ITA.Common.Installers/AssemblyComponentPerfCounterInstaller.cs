using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ITA.Common.Host
{
    /// <summary>
    /// Обеспечивает де\инсталяцию всех найденных счетчиков в указанной сборке у наследников определенного типа
    /// </summary>
    [RunInstaller(true)]
    public class AssemblyComponentPerfCounterInstaller : BasePerfCounterInstaller
    {
        private readonly Assembly m_Assembly;

        private readonly Type m_BaseType;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="TargetAssembly">Сборка для поиска конструкторов</param>
        /// <param name="BaseType">Базовый тип по наследникам которого в указанной сборке осуществляется поиск счетчиков</param>
        public AssemblyComponentPerfCounterInstaller(Assembly TargetAssembly, Type BaseType)
        {
            m_Assembly = TargetAssembly;
            m_BaseType = BaseType;
        }

        /// <summary>
        /// Конструктор с установкой префикса для категорий устанавливаемых счетчиков.
        /// </summary>
        /// <param name="TargetAssembly">Сборка для поиска конструкторов</param>
        /// <param name="BaseType">Базовый тип по наследникам которого в указанной сборке осуществляется поиск счетчиков</param>
        public AssemblyComponentPerfCounterInstaller(Assembly TargetAssembly, Type BaseType, string appName)
            : this(TargetAssembly, BaseType)
        {
            categoryPrefix = appName;
        }

        private List<object> AllCounters
        {
            get
            {
                var Counters = new List<object>();

                foreach (Type ComponentType in m_Assembly.GetTypes())
                {
                    // Анализируем только наследников BaseType
                    if (ComponentType.IsSubclassOf(m_BaseType))
                    {
                        Context.LogMessage(string.Format("Component class type with name '{0}' was found", ComponentType.FullName));
                        Counters.AddRange(ComponentType.GetCustomAttributes(typeof(CounterAttribute), true));
                    }
                }

                return Counters;
            }
        }

        protected override void RegisterCategories()
        {
            base.RegisterCategories(AllCounters.ToArray());
        }

        protected override void UnregisterCategories()
        {
            base.UnregisterCategories(AllCounters.ToArray());
        }
    }
}