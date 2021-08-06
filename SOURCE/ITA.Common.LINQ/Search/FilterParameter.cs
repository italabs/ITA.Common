using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Абстрактный класс параметров фильтра
    /// </summary>
    [DataContract]
    [KnownType(typeof(EqualFilterParameter))]
    [KnownType(typeof(ContainFilterParameter))]
    [KnownType(typeof(RangeFilterParameter))]
    public abstract class FilterParameter: IComparable
    {
        protected FilterParameter(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Свойство
        /// </summary>
        [DataMember]
        public string PropertyName
        {
            get;
            set;
        }

        [DataMember]
        public string Predicate
        {
            get; set;
        }

        /// <summary>
        /// Список значений
        /// </summary>
        [DataMember]
        public object[] Values
        {
            get;
            set;
        }

        #region Equality members

        protected bool Equals(FilterParameter other)
        {
            return string.Equals(PropertyName, other.PropertyName) && string.Equals(Predicate, other.Predicate) &&
                   Enumerable.SequenceEqual(Values, other.Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FilterParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (PropertyName != null ? PropertyName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Predicate != null ? Predicate.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Values != null ? Values.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">Obj.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var fp = obj as FilterParameter;

            if (fp == null)
            {
                throw new ArgumentException("Argument must be FilterParameter");
            }

            var res = string.Compare(this.PropertyName, fp.PropertyName);
            if (res != 0)
                return res;

            res = string.Compare(this.Predicate, fp.Predicate);

            if (res != 0)
                return res;

            return ArrayComparer.Compare(this.Values, fp.Values);
        }

        #endregion

        /// <summary>
        /// Преобразует параметры фильтра в условный оператор SQL
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <param name="argIndex">Индекс аргумента</param>
        /// <returns></returns>
        internal abstract string GetPredicateRawSql(string prefix, ref int argIndex);

    }

    /// <summary>
    /// Параметры фильтра на сравнение
    /// </summary>
    [DataContract]
    public class EqualFilterParameter : FilterParameter
    {
        public EqualFilterParameter(string propertyName, object value)
            : base(propertyName)
        {

            Predicate = string.Format("({0} = @0)", PropertyName);
            Values = new[] { value };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        internal override string GetPredicateRawSql(string prefix, ref int argIndex)
        {
            var p = String.Format("{0}[{1}]", prefix, PropertyName);

            string sql = string.Format("({0} = @q{1})", p, argIndex);
            argIndex++;
          
            return sql;
        }
    }

    /// <summary>
    /// Параметры фильтра на вхождение подстроки
    /// </summary>
    [DataContract]
    public class ContainFilterParameter : FilterParameter
    {
        public ContainFilterParameter(string propertyName, string value)
            : base(propertyName)
        {
            Predicate = string.Format("(({0} != null) && {0}.ToUpper().Contains(@0.ToUpper()))", PropertyName);
            Values = new object[] { value };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        internal override string GetPredicateRawSql(string prefix, ref int argIndex)
        {
            var p = String.Format("{0}[{1}]", prefix, PropertyName);

            string sql = string.Format("({0} LIKE '%' + @q{1} + '%')", p, argIndex );
            argIndex++;
           
            return sql;
        }
    }

    /// <summary>
    /// Параметры фильтра на нахождение значения в заданном диапазоне значений
    /// </summary>
    [DataContract]
    public class RangeFilterParameter : FilterParameter
    {
        [DataMember]
        protected object _greaterOrEqual;

        [DataMember]
        protected object _less;

        public RangeFilterParameter(string propertyName, object greaterOrEqual, object less)
            : base(propertyName)
        {
            if ((greaterOrEqual == null) && (less == null))
                throw new ArgumentException("greaterOrEqual and less both are null");

            this._greaterOrEqual = greaterOrEqual;
            this._less = less;

            FillPredicateAndValues();
        }

        public override string ToString()
        {
            return string.Format("RangeFilterParameter ({0}): _greaterOrEqual={1} _less={2}", PropertyName, _greaterOrEqual ?? "null", _less ?? "null");
        }

        protected void FillPredicateAndValues()
        {
            var p = PropertyName;

            if (_greaterOrEqual == null)
            {
                Predicate = string.Format("({0} < @0)", p);
                Values = new object[] {_less};
                return;
            }

            if (_less == null)
            {
                Predicate = string.Format("(@0 <= {0})", p);
                Values = new object[] { _greaterOrEqual };
                return;
            }

            Predicate = string.Format("((@0 <= {0}) && ({0} < @1))", p);
            Values = new object[] { _greaterOrEqual, _less };
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        internal override string GetPredicateRawSql(string prefix, ref int argIndex)
        {
            var p = String.Format("{0}[{1}]", prefix, PropertyName);

            string sql = null;

            if (_greaterOrEqual == null)
            {
                sql = string.Format("({0} < @q{1})", p, argIndex);
                argIndex++;
             
                return sql;
            }

            if (_less == null)
            {
                sql = string.Format("(@q{1} <= {0})", p, argIndex);
                argIndex++;

                return sql;
            }

            sql = string.Format("(@q{1} <= {0}) AND ({0} < @q{2})", p, argIndex, argIndex + 1);
            argIndex++;
            argIndex++;

            return sql;
        }
    }

    /// <summary>
    /// Преобразует массив фильтров в SQL-запрос
    /// </summary>
    public class PredicateRawSqlBuilder
    {
        private List<object> _values = new List<object>();

        public String _rawSql = string.Empty;

        /// <summary>
        /// FilterParameter[] -> RAW SQL (not LINQ!)
        /// </summary>
        /// <param name="prefix">Префикс</param>
        /// <param name="filterParameters">Массив фильтров</param>
        /// <param name="startArgIndex">Начальный индекс - для передачи параметров</param>
        public PredicateRawSqlBuilder(string prefix, FilterParameter[] filterParameters, int startArgIndex)
        {
            if (filterParameters != null && filterParameters.Length > 0)
            {
                string prevColumn = null;

                foreach (var filterParameter in filterParameters.OrderBy( f => f.PropertyName))
                {
                    if (prevColumn != null && filterParameter.PropertyName != prevColumn)
                    {
                        _rawSql = _rawSql + ") ";
                    }

                    if (filterParameter.PropertyName != prevColumn)
                    {
                        _rawSql = _rawSql + " AND (" + filterParameter.GetPredicateRawSql(prefix, ref startArgIndex);                     
                    }

                    if (filterParameter.PropertyName == prevColumn)
                    {
                        _rawSql = _rawSql + " OR " + filterParameter.GetPredicateRawSql(prefix, ref startArgIndex); 
                    }

                    prevColumn = filterParameter.PropertyName;                    

                    _values.AddRange(filterParameter.Values);
                }

                _rawSql = _rawSql + ") ";
            }
        }

        public List<object> Values
        {
            get { return this._values; }
        }

        public string RawSql
        {
            get { return this._rawSql; }
        }
    }
}
