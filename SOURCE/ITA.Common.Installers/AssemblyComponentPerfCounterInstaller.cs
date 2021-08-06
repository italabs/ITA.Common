using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ITA.Common.Host
{
    /// <summary>
    /// ������������ ��\���������� ���� ��������� ��������� � ��������� ������ � ����������� ������������� ����
    /// </summary>
    [RunInstaller(true)]
    public class AssemblyComponentPerfCounterInstaller : BasePerfCounterInstaller
    {
        private readonly Assembly m_Assembly;

        private readonly Type m_BaseType;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="TargetAssembly">������ ��� ������ �������������</param>
        /// <param name="BaseType">������� ��� �� ����������� �������� � ��������� ������ �������������� ����� ���������</param>
        public AssemblyComponentPerfCounterInstaller(Assembly TargetAssembly, Type BaseType)
        {
            m_Assembly = TargetAssembly;
            m_BaseType = BaseType;
        }

        /// <summary>
        /// ����������� � ���������� �������� ��� ��������� ��������������� ���������.
        /// </summary>
        /// <param name="TargetAssembly">������ ��� ������ �������������</param>
        /// <param name="BaseType">������� ��� �� ����������� �������� � ��������� ������ �������������� ����� ���������</param>
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
                    // ����������� ������ ����������� BaseType
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