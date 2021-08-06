using System.Runtime.Serialization;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Класс, хранящий информацию о поле и порядке сортировки
    /// </summary>
    [DataContract]
    public class SortParameter
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SortParameter"/>.
        /// </summary>
        /// <param name="propertyName">Имя свойства для сортировки.</param>
        /// <param name="asc">Порядок сортировки: <c>true</c> - прямой, <c>false</c> - обратный.</param>
        public SortParameter(string propertyName, bool asc)
        {
            this.PropertyName = propertyName;
            this.Asc = asc;
            this.PropertyId = null;
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SortParameter"/>.
        /// </summary>
        /// <param name="propertyId">Идентификатор свойства для сортировки.</param>
        /// <param name="asc">Порядок сортировки: <c>true</c> - прямой, <c>false</c> - обратный.</param>
        public SortParameter(int propertyId, bool asc)
        {
            this.PropertyName = null;
            this.Asc = asc;
            this.PropertyId = propertyId;
        }
       
        /// <summary>
        /// Имя свойства, по которому выполняется сортировка
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }


        /// <summary>
        /// Идентификатор параметра сортировки (если такой имеет место быть) - например может быть идентификатор кастомного параметра UserAccountPropertyMetadataId
        /// </summary>
        [DataMember]
        public int? PropertyId { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        /// <value>
        ///   <c>true</c> для ASC, <c>false</c> для DESC.
        /// </value>
        [DataMember]
        public bool Asc { get; set; }

        /// <summary>
        /// Фомирует правило сортировки для LINQ/SQL.
        /// </summary>
        /// <returns>Строка, которая может подставляться в скрипт сортировки для LINQ/SQL.</returns>
        public override string ToString()
        {
            if (this.PropertyId.HasValue)
                return this.PropertyId + (this.Asc ? " asc" : " desc");
            else
                return this.PropertyName + (this.Asc ? " asc" : " desc");
        }
    }
}
