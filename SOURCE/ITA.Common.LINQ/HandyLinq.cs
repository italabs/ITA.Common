using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Класс-расширение для фильтрации и поиска данных в запросе IQueryable
    /// </summary>
    public static class HandyLinq
    {
        /// <summary>
        /// Проверка типа на Nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type type)
        {
            if (!type.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            return false;
        }

        /// <summary>
        /// Фильтрация данных запроса в соответствии с заданным списком фильтров
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Запрос IQueryable над которым необходимо произвести фильтрацию</param>
        /// <param name="filters">Список фильтров</param>
        /// <returns></returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, FilterParameter[] filters)
        {
            if (filters == null)
                return source;
            if (filters.Length == 0)
                return source;

            var result = source;

            var groupped = filters.GroupBy(parameter => parameter.PropertyName);
            foreach (var g in groupped)
            {
                var values = new List<object>();
                string predicate = null;
                foreach (var p in g)
                {
                    string pr = FixParamIndex(p.Predicate, values.Count, p.Values.Length);
                    if (string.IsNullOrEmpty(predicate))
                    {
                        predicate = pr;
                    }
                    else
                    {
                        predicate = predicate + " || " + pr;
                    }
                    values.AddRange(p.Values);
                }

                result = result.Where(predicate, values.ToArray());
            }

            return result;
        }

        private static string FixParamIndex(string original, int baseIndex, int maxCount)
        {
            string result = original;
            for (int i = maxCount -1 ; i >= 0; i--)
            {
                result = result.Replace(string.Format("@{0}", i), string.Format("@{0}", baseIndex + i));
            }
            return result;
        }

        /// <summary>
        /// Поиск данных в запросе в соответствии с заданными параметрами поиска
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Запрос IQueryable над которым необходимо произвести поиск</param>
        /// <param name="searchParameter">Параметры поиска</param>
        /// <param name="propertyInfos">Параметры сортировки</param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> source, IVisibleColumnsSearchParam searchParameter, SortPropertyInfo[] propertyInfos)
        {
            if (searchParameter == null)
                return source;

            if (string.IsNullOrEmpty(searchParameter.SearchString))
                return source;

            var result = source;

            var propertyNames = new List<string>();

            if ((searchParameter.VisibleColumns == null) || (searchParameter.VisibleColumns.Length == 0))
            {
                propertyNames.AddRange(
                    propertyInfos
                    .Where(info => info.PropertyType == typeof(string))
                    .Select(info => info.PropertyName)
                        );
            }
            else
            {
                propertyNames.AddRange(
                    propertyInfos
                    .Where(info => info.PropertyType == typeof(string))
                    .Where(info => searchParameter.VisibleColumns.Contains(info.VisualPropertyName))
                    .Select(info => info.PropertyName)
                        );
            }

            var predicate = string.Join(" || ", propertyNames.Select(s => string.Format("(({0} != null) && {0}.ToUpper().Contains(@0.ToUpper()))", s)));

            if (!string.IsNullOrEmpty(predicate))
            {
                result = result.Where(predicate, searchParameter.SearchString);
            }

            return result;
        }

    }
}