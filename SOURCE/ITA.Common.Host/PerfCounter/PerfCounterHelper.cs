using log4net;

namespace ITA.Common.Host
{
    public class PerfCounterHelper
    {
        public static string BuildCountersCategoryName(string category, string appPrefix, ILog logger)
        {
            var str = string.Format("{0} - ", appPrefix);
            if (category.StartsWith(str))
                return category;

            var name = string.Format("{0}{1}", str, category);
            name = name.Length > 80 ? name.Substring(0, 80) : name;
            if (logger != null)
            {
                logger.DebugFormat("Builded category name: {0}", name);
            }

            return name;
        }

        public static string BuildCountersCategoryDescription(string className, ILog logger)
        {
            var name = string.Format("Contains counters of {0}", className);
            if (logger != null)
            {
                logger.DebugFormat("Built category desc: {0}", name);
            }
            return name;
        }
    }
}
