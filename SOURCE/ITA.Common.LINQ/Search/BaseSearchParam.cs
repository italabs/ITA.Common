using System.Linq;
using System.Runtime.Serialization;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Базовый класс, содержащий параметры поиска подстроки и фильтрации по подстроке
    /// </summary>
    [DataContract]
    public class BaseSearchParam : IFilterSearchParam, IVisibleColumnsSearchParam
    {
        public BaseSearchParam()
        {
            VisibleColumns = new string[0];
            FilterParameter = new FilterParameter[0];
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        [DataMember]
        public string SearchString { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        [DataMember]
        public string[] VisibleColumns { get; set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        [DataMember]
        public FilterParameter[] FilterParameter { get; set; }

        #region Equality members

        public static bool Equals(BaseSearchParam first, BaseSearchParam second)
        {
            if (first == null && second == null)
                return true;
            if (ReferenceEquals(null, first) || ReferenceEquals(null, second))
                return false;
            return first.Equals(second);
        }

        protected bool Equals(BaseSearchParam other)
        {
            return string.Equals(SearchString, other.SearchString) &&
                   VisibleColumns.OrderBy(x => x).SequenceEqual(other.VisibleColumns.OrderBy(x => x)) &&
                   FilterParameter.OrderBy(x => x).SequenceEqual(other.FilterParameter.OrderBy(x => x));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BaseSearchParam)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = SearchString != null ? SearchString.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (VisibleColumns != null ? VisibleColumns.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FilterParameter != null ? FilterParameter.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion
    }
}
