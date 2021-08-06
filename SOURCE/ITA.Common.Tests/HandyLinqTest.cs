using System;
using System.Collections.Generic;
using System.Linq;
using ITA.Common.LINQ;
using NUnit.Framework;
using System.Linq.Dynamic;

namespace ITA.Common.Tests
{
    /// <summary>
    /// Тесты динамического поиска.
    /// </summary>
    [TestFixture]
    public class HandyLinqTest : TestBase
    {
        #region test class

        class TestEntity
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public bool Enabled { get; set; }
            public DateTime DateOfBirth { get; set; }
            public int Salary { get; set; }

            public TestEntity() { }

            public TestEntity(string name, string phone, bool enabled, DateTime dob, int salary)
            {
                Name = name;
                Phone = phone;
                Enabled = enabled;
                DateOfBirth = dob;
                Salary = salary;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (!(obj is TestEntity)) return false;

                TestEntity entity = obj as TestEntity;
                
                return Name.Equals(entity.Name) && 
                    Phone.Equals(entity.Phone) &&
                    Enabled == entity.Enabled &&
                    DateOfBirth == entity.DateOfBirth &&
                    Salary == entity.Salary;
            }

            public class Params
            {
                public const string Name = "Name";
                public const string Phone = "Phone";
                public const string Enabled = "Enabled";
                public const string DateOfBirth = "DateOfBirth";
                public const string Salary = "Salary";

                public static readonly SortPropertyInfo[] SortPropertyInfos =
                {
                    new SortPropertyInfo(Name, Name, Name, typeof (string)) {VisibleIndex = 0},
                    new SortPropertyInfo(Phone, Phone, Phone, typeof (string)) {VisibleIndex = 1},
                    new SortPropertyInfo(Enabled, Enabled, Enabled, typeof (bool)) {VisibleIndex = 2},
                    new SortPropertyInfo(DateOfBirth, DateOfBirth, DateOfBirth, typeof (DateTime)) {VisibleIndex = 3},
                    new SortPropertyInfo(Salary, Salary, Salary, typeof (int)) {VisibleIndex = 4}
                };
            }
        }

        #endregion

        List<TestEntity> _entities = new List<TestEntity>();

        [OneTimeSetUp]
        public void Initialize()
        {
            _entities.Add(new TestEntity("Vasya", "1234567", true, new DateTime(1980,10,10), 10000));//0
            _entities.Add(new TestEntity("Petya", "3484395", true, new DateTime(1970, 01, 01), 20000));//1
            _entities.Add(new TestEntity("Sasha", "4567823", true, new DateTime(1975, 02, 02), 30000));//2
            _entities.Add(new TestEntity("Ivan",  "3633463", false, new DateTime(1985, 12, 12), 15000));//3

            _entities.Add(new TestEntity("Vasilisa", "1234567", true, new DateTime(1965, 11, 11), 9000));//4
            _entities.Add(new TestEntity("Gena", "9837570", true, new DateTime(1967, 5, 5), 23000));//5
            _entities.Add(new TestEntity("Alex", "8036212", false, new DateTime(1977,7, 7), 50000));//6
            _entities.Add(new TestEntity("Victoriya 12", "7232559", true, new DateTime(1987, 9, 9), 45000));//7
        }

        [Test, Order(1)]
        public void FilterTest()
        {
            var sourceQuery = _entities.AsQueryable();

            var res = sourceQuery
                .Filter(new FilterParameter[] {new ContainFilterParameter(TestEntity.Params.Name, "Vas")})
                .ToList();
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[4], res[1]);

            res = sourceQuery
                .Filter(new FilterParameter[] { new EqualFilterParameter(TestEntity.Params.Enabled, true) })
                .ToList();
            Assert.AreEqual(6, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[1], res[1]);
            Assert.AreEqual(_entities[2], res[2]);
            Assert.AreEqual(_entities[4], res[3]);
            Assert.AreEqual(_entities[5], res[4]);
            Assert.AreEqual(_entities[7], res[5]);

            res = sourceQuery
                .Filter(new FilterParameter[] { new RangeFilterParameter(TestEntity.Params.DateOfBirth, new DateTime(1970, 1, 1), new DateTime(1987, 1, 1)) })
                .ToList();
            Assert.AreEqual(5, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[1], res[1]);
            Assert.AreEqual(_entities[2], res[2]);
            Assert.AreEqual(_entities[3], res[3]);
            Assert.AreEqual(_entities[6], res[4]);

            res = sourceQuery
                .Filter(new FilterParameter[]
                {
                    new ContainFilterParameter(TestEntity.Params.Name, "ya"),
                    new RangeFilterParameter(TestEntity.Params.DateOfBirth, new DateTime(1970, 1, 2), new DateTime(1987, 11, 11))
                })
                .ToList();
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[7], res[1]);

            res = sourceQuery
                .Filter(new FilterParameter[]{})
                .ToList();
            Assert.AreEqual(8, res.Count);
        }

        [Test, Order(2)]
        public void SearchTest()
        {
            var sourceQuery = _entities.AsQueryable();

            var res = sourceQuery.Search(null, null).ToList();
            Assert.AreEqual(8, res.Count);

            res = sourceQuery.Search(new BaseSearchParam(), null).ToList();
            Assert.AreEqual(8, res.Count);

            res = sourceQuery.Search(new BaseSearchParam(), new SortPropertyInfo[]{}).ToList();
            Assert.AreEqual(8, res.Count);

            res = sourceQuery
                .Search(new BaseSearchParam()
                {
                    SearchString = "12"
                }, TestEntity.Params.SortPropertyInfos)
                .ToList();
            Assert.AreEqual(4, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[4], res[1]);
            Assert.AreEqual(_entities[6], res[2]);
            Assert.AreEqual(_entities[7], res[3]);

            res = sourceQuery
                .Search(new BaseSearchParam()
                {
                    SearchString = "12",
                    VisibleColumns = new []{TestEntity.Params.Phone}
                }, TestEntity.Params.SortPropertyInfos)
                .ToList();
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[4], res[1]);
            Assert.AreEqual(_entities[6], res[2]);
        }

        [Test, Order(3)]
        public void SortTest()
        {
            var sourceQuery = _entities.AsQueryable();

            var res = sourceQuery.OrderBy((new SortParameter(TestEntity.Params.Name, true)).ToString()).ToList();
            Assert.AreEqual(_entities[6], res[0]);
            Assert.AreEqual(_entities[5], res[1]);
            Assert.AreEqual(_entities[3], res[2]);
            Assert.AreEqual(_entities[1], res[3]);
            Assert.AreEqual(_entities[2], res[4]);
            Assert.AreEqual(_entities[4], res[5]);
            Assert.AreEqual(_entities[0], res[6]);
            Assert.AreEqual(_entities[7], res[7]);

            res = sourceQuery.OrderBy((new SortParameter(TestEntity.Params.Salary, true)).ToString()).ToList();
            Assert.AreEqual(_entities[4], res[0]);
            Assert.AreEqual(_entities[0], res[1]);
            Assert.AreEqual(_entities[3], res[2]);
            Assert.AreEqual(_entities[1], res[3]);
            Assert.AreEqual(_entities[5], res[4]);
            Assert.AreEqual(_entities[2], res[5]);
            Assert.AreEqual(_entities[7], res[6]);
            Assert.AreEqual(_entities[6], res[7]);
        }

        [Test, Order(4)]
        public void MixedLinqFeaturesTest()
        {
            var sourceQuery = _entities.AsQueryable();

            var searchParam = new BaseSearchParam
            {
                SearchString = "12",
                FilterParameter = new FilterParameter[]
                {
                    new RangeFilterParameter(TestEntity.Params.Salary, 10000, 100000), 
                }
            };

            var res = sourceQuery
                .OrderBy(new SortParameter(TestEntity.Params.Enabled, false).ToString())
                .Filter(searchParam.FilterParameter)
                .Search(searchParam, TestEntity.Params.SortPropertyInfos)
                .ToList();
            Assert.AreEqual(3, res.Count);
            Assert.AreEqual(_entities[0], res[0]);
            Assert.AreEqual(_entities[7], res[1]);
            Assert.AreEqual(_entities[6], res[2]);
        }

        [Test, Order(5)]
        public void PredicateRawSqlBuilderTest()
        {
            string testRawSql1 = @"AND ((TestPrefix[Name] = @q0))  AND ((TestPrefix[Phone] LIKE '%' + @q1 + '%'))  AND ((@q2 <= TestPrefix[Salary]) AND (TestPrefix[Salary] < @q3))";
            string testRawSql2 = @"AND (([Name] = @q5))  AND (([Phone] LIKE '%' + @q6 + '%'))  AND ((@q7 <= [Salary]) AND ([Salary] < @q8))";
            string testRawSql3 = @"AND (([Name] = @q5) OR ([Name] = @q6))  AND (([Phone] LIKE '%' + @q7 + '%'))  AND ((@q8 <= [Salary]) AND ([Salary] < @q9))";

            PredicateRawSqlBuilder rawSqlBuilder = new PredicateRawSqlBuilder("TestPrefix", 
                new FilterParameter[]
                {
                    new EqualFilterParameter(TestEntity.Params.Name, "Vasya"), 
                    new ContainFilterParameter(TestEntity.Params.Phone, "1234"), 
                    new RangeFilterParameter(TestEntity.Params.Salary, 1000, 30000), 
                }, 
                0);
            Assert.AreEqual(testRawSql1, rawSqlBuilder.RawSql.Trim());

            rawSqlBuilder = new PredicateRawSqlBuilder(null,
                new FilterParameter[]
                {
                    new EqualFilterParameter(TestEntity.Params.Name, "Vasya"), 
                    new ContainFilterParameter(TestEntity.Params.Phone, "1234"), 
                    new RangeFilterParameter(TestEntity.Params.Salary, 1000, 30000), 
                },
                5);
            Assert.AreEqual(testRawSql2, rawSqlBuilder.RawSql.Trim());

            rawSqlBuilder = new PredicateRawSqlBuilder(null,
                new FilterParameter[]
                {
                    new EqualFilterParameter(TestEntity.Params.Name, "Vasya"), 
                    new EqualFilterParameter(TestEntity.Params.Name, "Sasha"), 
                    new ContainFilterParameter(TestEntity.Params.Phone, "1234"), 
                    new RangeFilterParameter(TestEntity.Params.Salary, 1000, 30000), 
                },
                5);
            Assert.AreEqual(testRawSql3, rawSqlBuilder.RawSql.Trim());
        }

        [Test, Order(6)]
        public void TestDateTimeRangeFilterParameter()
        {
            var sourceEntities = new List<TestEntity>();
            sourceEntities.Add(new TestEntity("Vasya", "1234567", true, new DateTime(1980, 10, 10, 10, 1, 1), 10000));//0
            sourceEntities.Add(new TestEntity("Petya", "3484395", true, new DateTime(1970, 01, 01, 11, 11, 11), 20000));//1
            sourceEntities.Add(new TestEntity("Sasha", "4567823", true, new DateTime(1980, 10, 10, 10, 1, 5), 30000));//2
            sourceEntities.Add(new TestEntity("Ivan", "3633463", false, new DateTime(1985, 12, 12, 12, 12, 12), 15000));//3
            
            var sourceQuery = sourceEntities.AsQueryable();

            var res = sourceQuery
                .Filter(new FilterParameter[]
                {
                    new RangeFilterParameter(
                        TestEntity.Params.DateOfBirth, 
                        new DateTime(1980, 10, 10, 10, 1, 1), 
                        new DateTime(1980, 10, 10, 10, 2, 1))
                })
                .ToList();
            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(sourceEntities[0], res[0]);
            Assert.AreEqual(sourceEntities[2], res[1]);

            res = sourceQuery
                .Filter(new FilterParameter[]
                {
                    new RangeFilterParameter(
                        TestEntity.Params.DateOfBirth, 
                        new DateTime(1980, 10, 10, 9, 1, 1), 
                        new DateTime(1980, 10, 10, 9, 2, 1))
                })
                .ToList();
            Assert.AreEqual(0, res.Count);
        }
    }
}
