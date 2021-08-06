using System;
using System.Collections.Generic;
using ITA.Common.LINQ;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    [TestFixture]
    public class ArrayComparerTest : TestBase
    {
        #region test classes

        class C1 : IComparable
        {
            public C1(int value)
            {
                Value = value;
            }

            public int Value { get; set; }

            public int CompareTo(object obj)
            {
                if (obj == null) return -1;
                var obj2 = obj as C1;
                if (obj2 == null)
                {
                    throw new InvalidCastException("obj");
                }

                return Value.CompareTo(obj2.Value);
            }
        }

        class C2
        {
            public C2(int value)
            {
                Value = value;
            }

            public int Value { get; set; }

            public int CompareTo(object obj)
            {
                return Value.CompareTo(obj);
            }
        }
        #endregion

        [Test, Order(1)]
        public void CompareTest()
        {
            List<C1> array1 = new List<C1>();
            List<C1> array2 = new List<C1>();

            array1.Add(new C1(1));
            array1.Add(new C1(2));

            array2.Add(new C1(1));
            array2.Add(new C1(2));

            Assert.AreEqual(0, ArrayComparer.Compare(array1.ToArray(), array2.ToArray()));
            Assert.AreEqual(1, ArrayComparer.Compare(null, array2.ToArray()));
            Assert.AreEqual(-1, ArrayComparer.Compare(array1.ToArray(), null));
            Assert.AreEqual(0, ArrayComparer.Compare<object>(null, null));

            array1[0].Value = 5;
            Assert.AreEqual(1, ArrayComparer.Compare(array1.ToArray(), array2.ToArray()));

            array2[0].Value = 6;
            Assert.AreEqual(-1, ArrayComparer.Compare(array1.ToArray(), array2.ToArray()));

            array1[0].Value = 6;
            Assert.AreEqual(0, ArrayComparer.Compare(array1.ToArray(), array2.ToArray()));
            
            array1.Add(new C1(7));
            Assert.AreEqual(1, ArrayComparer.Compare(array1.ToArray(), array2.ToArray()));

            Assert.Throws<ArgumentException>(() =>
            {
                ArrayComparer.Compare(new[] { new C2(1) }, new[] { new C2(1) });
            });
        }
    }
}
