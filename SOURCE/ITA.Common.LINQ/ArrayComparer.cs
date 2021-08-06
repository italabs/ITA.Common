using System;

namespace ITA.Common.LINQ
{
    /// <summary>
    /// Класс для сравнения массивов
    /// </summary>
    public static class ArrayComparer
    {
        /// <summary>
        /// Поэлементое Сравнение массивов
        /// </summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="x">Первый массив</param>
        /// <param name="y">Второй массив</param>
        /// <returns>0 - массивы равны; 1 - первый массив больше второго; -1 - второй массив больше первого</returns>
        public static int Compare<T>(T[] x, T[] y) 
            where T : class
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return 1;
            if (y == null) return -1;

            var res = x.Length.CompareTo(y.Length);
            if (res != 0)
            {
                return res;
            }

            for (int i = 0; i < x.Length; i++)
            {
                var xx = x[i];
                var yy = y[i];

                if (ReferenceEquals(xx, yy)) return 0;
                if (xx == null) return 1;
                if (yy == null) return -1;

                var xxx = xx as IComparable;
                if (xxx == null)
                {
                    throw new ArgumentException("Array element must implement IComparable");
                }

                res = xxx.CompareTo(yy);

                if (res != 0)
                {
                    return res;
                }
            }

            return 0;
        }
    }
}
