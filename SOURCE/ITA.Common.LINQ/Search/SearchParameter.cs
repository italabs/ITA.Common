using System.Runtime.Serialization;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Класс, содержащий параметры поиска подстроки в заданном списке свойств
    /// </summary>
    [DataContract]
    public class SearchParameter : IVisibleColumnsSearchParam
    {
        /// <summary>
        /// Подстрока поиска
        /// </summary>
        [DataMember]
        public string SearchString
        {
            get; set;
        }

        /// <summary>   
        /// Список отображаемых полей (без точки), по которым должен производиться поиск. 
        /// </summary>
        [DataMember]
        public string[] VisibleColumns
        {
            get; set;
        }

        public SearchParameter(string searchString, string[] visibleColumns)
        {
            SearchString = searchString;
            VisibleColumns = visibleColumns;
        }
    }
}
