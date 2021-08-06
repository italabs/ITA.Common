using System;
using System.Runtime.Serialization;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Класс, хранящий информацию возможных полях сортировки
    /// </summary>
    [DataContract]
    public class SortPropertyInfo
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SortPropertyInfo"/>.
        /// </summary>
        /// <param name="propertyName">Имя свойства, после трансляции в БД (полное имя с точкой - TableName.PropertyName).</param>
        /// <param name="description">Имя поля сортировки для отображения в ГУИ.</param>
        /// <param name="propertyType">Тип свойства.</param>
        /// <remarks>
        /// При вызове этого конструктора значение свойства <see cref="VisualPropertyName"/> выставляется равным <c>propertyName</c>.
        /// Значение свойства <see cref="PropertyType"/> выставляется равным <c>null</c>.
        /// </remarks>
        public SortPropertyInfo(string propertyName, string description)
            : this(propertyName, propertyName, description)
        {
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="SortPropertyInfo"/>.
        /// </summary>
        /// <param name="visualPropertyName">Имя свойства, по которому выполняется сортировка - без точки.</param>
        /// <param name="propertyName">Имя свойства, после трансляции в БД (полное имя с точкой - TableName.PropertyName).</param>
        /// <param name="description">Имя поля сортировки для отображения в ГУИ.</param>
        /// <param name="propertyType">Тип свойства.</param>
        public SortPropertyInfo(string visualPropertyName, string propertyName, string description, Type propertyType = null)
        {
            this.VisualPropertyName = visualPropertyName;
            this.PropertyName = propertyName;
            this.Description = description;
            this.PropertyType = propertyType;
            this.VisibleIndex = -1;
            this.IsBound = true;
        }

        /// <summary>
        /// Тип свойства.
        /// </summary>
        /// <remarks>
        /// Тип свойства учитывается при поиске и фильтрации.
        /// Поиск подстроки осуществляется только по строковым свойствам, фильтрация осуществляется только по перечислениям, булевым типам и DateTime.
        /// </remarks>
        [DataMember]
        public Type PropertyType { get; set; }

        /// <summary>
        /// Имя свойства, после трансляции в БД (полное имя с точкой - TableName.PropertyName)
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        /// Имя свойства, по которому выполняется сортировка - без точки.
        /// </summary>
        [DataMember]
        public string VisualPropertyName { get; set; }

        /// <summary>
        /// Имя поля сортировки для отображения в ГУИ
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Номер столбца при отображении в гриде (в виде по умолчанию).
        /// </summary>
        /// <remarks>Если при отображении по умолчанию это поле не должно отображаться в гриде, следует установить значение <code>-1</code>.</remarks>
        [DataMember]
        public int VisibleIndex { get; set; }

        /// <summary>
        /// Флаг указания сортировки по умолчанию.
        /// </summary>
        /// <value>
        ///   <c>true</c> если по этому стобцу должна быть сортировка в гриде по умолчанию; иначе <c>false</c>.
        /// </value>
        [DataMember]
        public bool DefaultSort { get; set; }

        /// <summary>
        /// Значение, показывающее, что данное свойство привязано к полю в БД.
        /// </summary>
        /// <value>
        ///   <c>true</c> если данное свойство привязано к полю в БД ; <c>false</c> если данное свойство создано в UI.
        /// </value>
        [DataMember]
        public bool IsBound { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Description;
        }
    }
}
