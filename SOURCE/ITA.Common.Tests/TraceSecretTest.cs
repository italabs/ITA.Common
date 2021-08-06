using System;
using System.Collections.Generic;
using System.Linq;
using ITA.Common.Tracing;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    [TestFixture]
    public class TraceSecretTest
    {
        private const string LOGGER_NAME = "Test";
        private Test _test = new Test();
        private StringAppender _stringAppender;

        #region Test class

        [Trace(LOGGER_NAME)]
        public class Test
        {
            public Test()
            {
                this.Prop3 = "TestSecretPrivateSetter";
            }

            [TraceSecret]
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            [TraceSecret]
            public string Prop3 { get; private set; }

            public string Test1(string p1, string p2)
            {
                return p1 + p2;
            }

            public string Test2(string p1, [TraceSecret]string p2, int p3)
            {
                return string.Format("{0}-{1}-{2}", p1, p2, p3);
            }

            public string Test3(string p1, [TraceSecret] ref string p2, int p3)
            {
                return string.Format("{0}-{1}-{2}", p1, p2, p3);
            }

            public string Test4(string p1, [TraceSecret] out string p2, int p3)
            {
                p2 = "Test4 secret parameter";
                return string.Format("{0}-{1}-{2}", p1, p2, p3);
            }

            [return: TraceSecret]
            public string Test5([TraceSecret]string p1, string p2, [TraceSecret]int p3)
            {
                return string.Format("{0}-{1}-{2}", p1, p2, p3);
            }
        }

        #endregion

        [OneTimeSetUp]
        public void Initialize()
        {
            StringAppender.Configure();
            _stringAppender = StringAppender.GetStringAppender(LOGGER_NAME);
        }
        
        [Test]
        public void NoSecretParamsTest()
        {
            _stringAppender.ClearTrace();
            var res1 = _test.Test1(p1: "1", p2: "2");
            AssertSecret(_stringAppender.TraceStrings, "p1", false);
            AssertSecret(_stringAppender.TraceStrings, "p2", false);
        }

        [Test]
        public void OneInputSecretParamsTest()
        {
            _stringAppender.ClearTrace();
            var res2 = _test.Test2(p1: "1", p2: "2", p3: 3);
            AssertSecret(_stringAppender.TraceStrings, "p1", false);
            AssertSecret(_stringAppender.TraceStrings, "p2", true);
            AssertSecret(_stringAppender.TraceStrings, "p3", false);
        }

        [Test]
        public void OneInputRefSecretParamsTest()
        {
            _stringAppender.ClearTrace();
            var str = "2";
            var res3 = _test.Test3(p1: "1", p2: ref str, p3: 3);
            AssertSecret(_stringAppender.TraceStrings, "p1", false);
            AssertSecret(_stringAppender.TraceStrings, "p2", true);
            AssertSecret(_stringAppender.TraceStrings, "p3", false);
        }

        [Test]
        public void OneInputOutSecretParamsTest()
        {
            _stringAppender.ClearTrace();
            var str = "2";
            var res4 = _test.Test4(p1: "1", p2: out str, p3: 3);
            AssertSecret(_stringAppender.TraceStrings, "p1", false);
            AssertSecret(_stringAppender.TraceStrings, "p2", true);
            AssertSecret(_stringAppender.TraceStrings, "p3", false);
        }

        [Test]
        public void ReturnAndTwoInputSecretParamsTest()
        {
            _stringAppender.ClearTrace();
            var res5 = _test.Test5(p1: "1", p2: "2", p3: 3);
            AssertSecret(_stringAppender.TraceStrings, "p1", true);
            AssertSecret(_stringAppender.TraceStrings, "p2", false);
            AssertSecret(_stringAppender.TraceStrings, "p3", true);
            AssertSecret(_stringAppender.TraceStrings, string.Empty, true, false);
        }

        [Test]
        public void SecretPropertySetterTest()
        {
            _stringAppender.ClearTrace();
            _test.Prop1 = "SecretPropertySetterTest";
            AssertSecret(_stringAppender.TraceStrings, "value", true);
        }

        [Test]
        public void SecretPropertyGetterTest()
        {
            _stringAppender.ClearTrace();
            var res = _test.Prop1;
            AssertSecret(_stringAppender.TraceStrings, "get_Prop1", true, false);
        }

        [Test]
        public void NotSecretPropertySetterTest()
        {
            _stringAppender.ClearTrace();
            _test.Prop2 = "NotSecretPropertySetterTest";
            AssertSecret(_stringAppender.TraceStrings, "value", false);
        }

        [Test]
        public void NotSecretPropertyGetterTest()
        {
            _stringAppender.ClearTrace();
            var res = _test.Prop2;
            AssertSecret(_stringAppender.TraceStrings, "get_Prop2", false, false);
        }


        private void AssertSecret(List<string> trace, string paramName, bool isSecret, bool isInput = true)
        {
            Assert.AreEqual(2, trace.Count, "In tracing expected 2 records: for the input parameters and output");
            var inputs = trace[0];
            var output = trace[1];

            if (isInput)
            {
                var strs =
                    inputs.Split(new[] { "\n", "\r", "\t" }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(s => s.Contains("="));

                var paramsValue = strs
                    .Select(s => s.Split(new[] { " = " }, StringSplitOptions.RemoveEmptyEntries))
                    .ToDictionary(s => s[0], s => s[1]);

                var paramExists = paramsValue.ContainsKey(paramName);
                Assert.True(paramExists, "In tracing missing parameter'{0}'", paramName);

                var paramValue = paramsValue[paramName];
                var paramSecret = string.Equals(paramValue, TraceAttribute.SECRET_PATTERN);
                Assert.True(isSecret == paramSecret,
                    isSecret
                        ? "It was expected availability a secret in the trace for the input parameter '{0}'"
                        : "Expected no secret in the trace for the input parameter '{0}'", paramName);
            }
            else
            {
                var str = output.Split(new[] { "\n", "\r", "\t" }, StringSplitOptions.RemoveEmptyEntries).First(s => s.Contains("<<<"));
                var strs = str.Split(new[] { " : " }, StringSplitOptions.RemoveEmptyEntries);
                var outputValue = strs[1];
                var paramSecret = string.Equals(outputValue, TraceAttribute.SECRET_PATTERN);
                Assert.True(isSecret == paramSecret, isSecret 
                    ? "It was expected availability a secret in the trace for the function result" 
                    : "Expected no secret in the trace for the function result");
            }
        }
    }
}
