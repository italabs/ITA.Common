using System.Reflection;
using System.Runtime.CompilerServices;

namespace ITA.Common.Microservices.Metrics
{
    internal static class MethodExtensions
    {
        public static bool IsAsync(this MethodBase method)
        {
            var stateMachineAttr = method.GetCustomAttribute<AsyncStateMachineAttribute>();
            if (stateMachineAttr != null)
            {
                var stateMachineType = stateMachineAttr.StateMachineType;
                if (stateMachineType != null)
                {
                    return stateMachineType.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
                }
            }
            return false;
        }
    }
}