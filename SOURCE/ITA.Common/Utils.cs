using System.Collections.Generic;

namespace ITA.Common
{
    public delegate bool Func<T>(T arg);

    /// <summary>
    /// Linq utils
    /// </summary>
    public static class Utils
    {
        public static IEnumerable<T> Union<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var list = new List<T>();
            foreach (var val in first)
            {
                if (!list.Contains(val))
                {
                    list.Add(val);
                }
            }
            foreach (var val in second)
            {
                if(!list.Contains(val))
                {
                    list.Add(val);
                }
            }

            return list;
        }

        public static IEnumerable<T> Where<T>(IEnumerable<T> source, Func<T> predicate)
        {
            var res = new List<T>();
            foreach (var val in source)
            {
                if(predicate(val))
                {
                    res.Add(val);
                }
            }
            return res;
        }

        public static T FirstOrDefault<T>(IEnumerable<T> source)
        {
            T res = default(T);
            foreach (var val in source)
            {
                res = val;
                break;
            }
            return res;
        }

        public static int Sum(IEnumerable<int> source)
        {
            int res = 0;
            foreach (var val in source)
            {
                res += val;
            }
            return res;
        }
    }
}
