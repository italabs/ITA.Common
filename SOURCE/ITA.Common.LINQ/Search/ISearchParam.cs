namespace ITA.Common.LINQ
{
    /// <summary>
    /// Интерфейс пустых параметров поиска.
    /// </summary>
    public interface IEmptySearchParam
    {
    }

    /// <summary>
    /// Интерфейс параметров поиска подстроки.
    /// </summary>
    public interface ISearchTextParam : IEmptySearchParam
    {
        /// <summary>
        /// Подстрока поиска.
        /// </summary>
        string SearchString { get; set; }
    }

    /// <summary>
    /// Интерфейс параметров поиска с использованием фильтра.
    /// </summary>
    public interface IFilterSearchParam : IEmptySearchParam
    {
        /// <summary>
        /// Набор параметров фильтрации.
        /// </summary>
        FilterParameter[] FilterParameter { get; set; }
    }

    /// <summary>
    /// Интерфейс параметров поиска подстроки в указанном списке свойств.
    /// </summary>
    public interface IVisibleColumnsSearchParam : ISearchTextParam
    {
        /// <summary>
        /// Перечень свойств (колонок), в которых будет осуществляться поиск подстроки.
        /// </summary>
        string[] VisibleColumns { get; set; }
    }
}