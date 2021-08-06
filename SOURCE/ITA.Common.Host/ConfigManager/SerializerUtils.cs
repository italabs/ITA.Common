using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//using System.Runtime.Serialization.Formatters.Soap;

namespace ITA.Common.Host.ConfigManager
{
    public static class SerializerUtils
    {
        /// <summary>
        /// Проверка - есть ли атрибут BinarySerializableAttribute у типа объекта.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsBinarySerializable(object value)
        {
            object[] CustomAttr = value.GetType().GetCustomAttributes(true);
            foreach (Attribute attr in CustomAttr)
            {
                if (attr is BinarySerializableAttribute)
                {
                    return true;                    
                }
            }

            return false;     
        }

        /// <summary>
        /// Десериализация объекта.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] value)
        {
            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream m = new MemoryStream(value))
            {
                object o = f.Deserialize(m);
                return o;
            }
        }

        /// <summary>
        /// Сериализация объекта.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Serialize(object value)
        {
            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream m = new MemoryStream())
            {
                f.Serialize(m, value);
                return m.ToArray();
            }
        }
    }
}
