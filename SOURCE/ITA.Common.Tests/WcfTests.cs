using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using ITA.Common.WCF;
using NUnit.Framework;

namespace ITA.Common.Tests
{
    [TestFixture]
    public class WcfTests : TestBase
    {
        #region Test Environment

        [DataContract]
        internal class TestDetailedException
        {
            [DataMember]
            public int Code { get; set; }

            public string Message { get; set; }

            public object[] Args { get; set; }
        }

        [ServiceContract]
        internal interface ITestWcfService
        {
            [OperationContract]
            [FaultContract(typeof(TestDetailedException))]
            bool TestDetailedException();
        }

        internal class TestWcfService : ITestWcfService
        {
            public bool TestDetailedException()
            {
                throw new FaultException<TestDetailedException>(new TestDetailedException()
                {
                    Args = null,
                    Message = "Exception from TestService.",
                    Code = 0
                }, "Exception from TestService.");
            }
        }

        internal class TestWcfServiceClient : RemoteClientBase<ITestWcfService>, ITestWcfService
        {

            public TestWcfServiceClient(string uri, SecurityType securityType)
                : base(uri, new BindingOptions { SecurityType = securityType })
            {
            }

            public TestWcfServiceClient(string uri, IEndpointBehavior endpointBehavior)
                : base(uri)
            {
                m_factoryWrapper.FactoryEndpoint.Behaviors.Add(endpointBehavior);
            }

            public bool TestDetailedException()
            {
                return DoWithValidation<TestDetailedException, bool>(proxy => proxy.TestDetailedException());
            }
        }

        #endregion

        private const string ServiceUri = "http://localhost:11111/TestWcfService/";

        [Test, Order(1)]
        public void TestThatWcfExceptionThrowing()
        {
            var options = BindingOptions.Default;
            options.SecurityType = SecurityType.Basic;

            Assert.Throws<WCFException>(() => BindingHelper.CreateNetTcpBinding(options), "Wcf exception not generated.");
        }

        [Test, Order(2)]
        public void TestDetailedExceptionHandling()
        {
            var serviceHost = InitServiceHost();
            try
            {
                var client = new TestWcfServiceClient(ServiceUri, SecurityType.Windows);

                Assert.Throws<FaultException<TestDetailedException>>(() => client.TestDetailedException(),
                    "Detailed exception not catched by FaultException<TestDetailedException>.");
            }
            finally
            {
                serviceHost.Close();
            }
        }

        [Test, Order(3)]
        public void TestDetailedExceptionCatchByFaultException()
        {
            var serviceHost = InitServiceHost();
            try
            {
                var client = new TestWcfServiceClient(ServiceUri, SecurityType.Windows);

                Assert.Throws<FaultException<TestDetailedException>>(() => client.TestDetailedException(),
                    "Detailed exception not catched by FaultException.");
            }
            finally
            {
                serviceHost.Close();
            }
        }

        [Test, Order(4)]
        public void TestServiceHostCloseAndDisposeMethod()
        {
            var serviceHost = InitServiceHost();
            try
            {
                var client = new TestWcfServiceClient(ServiceUri, SecurityType.Windows);

                Assert.Throws<FaultException<TestDetailedException>>(() => client.TestDetailedException(),
                    "Detailed exception not catched by FaultException<TestDetailedException>.");
            }
            finally
            {
                Assert.AreNotEqual(CommunicationState.Closed, serviceHost.State);
                serviceHost.CloseAndDispose(null);
                Assert.AreEqual(CommunicationState.Closed, serviceHost.State);
            }
        }

        [Test, Order(5)]
        public void TestNullParametersWhileBindingCreatingThrowException()
        {
            Assert.Throws<ArgumentException>(()=> BindingHelper.CreateMexBindingByUri(""), "Mex Binding was created by empty uri parameter.");
            Uri uri = null;
            Assert.Throws<ArgumentNullException>(() => BindingHelper.CreateMexBindingByUri(uri), "Mex Binding was created by empty uri parameter.");

            Assert.Throws<ArgumentException>(() => BindingHelper.CreateBindingByUri("", null), "Binding was created by empty uri parameter.");
            Assert.Throws<ArgumentNullException>(() => BindingHelper.CreateBindingByUri(ServiceUri, null), "Binding was created by empty options parameter.");
            Assert.Throws<ArgumentNullException>(() => BindingHelper.CreateBindingByUri(uri, null), "Binding was created by empty uri parameter.");
            Assert.Throws<ArgumentNullException>(() => BindingHelper.CreateBindingByUri(new Uri(ServiceUri), null), "Binding was created by empty options parameter.");
        }

        [Test, Order(6)]
        public void TestBindingCreatingWithUnsupportedSchemeThrowException()
        {
            Assert.Throws<WCFException>(() => BindingHelper.CreateMexBindingByUri(ServiceUri, "https"), "Mex Binding was created with not supported scheme.");

            Assert.Throws<WCFException>(() => BindingHelper.CreateBindingByUri(ServiceUri, BindingOptions.Default, "https"), "Binding was created with not supported scheme.");
        }

        [Test, Order(7)]
        public void TestBindingCreating()
        {
            Assert.NotNull(BindingHelper.CreateMexBindingByUri(ServiceUri, "http"), "Mex Binding was not created.");

            Assert.NotNull(BindingHelper.CreateBindingByUri(ServiceUri, BindingOptions.Default, "http"), "Mex Binding was not created.");
        }

        private ServiceHost InitServiceHost()
        {
            Uri baseAddress = new Uri(ServiceUri);
            var _serviceHost = new ServiceHost(typeof(TestWcfService), baseAddress);

            BindingOptions options = BindingOptions.Default;
            string[] supportedUriSchemes =
            {
                Uri.UriSchemeNetTcp, Uri.UriSchemeNetPipe, Uri.UriSchemeHttp,
                Uri.UriSchemeHttps
            };

            Binding baseBinding = BindingHelper.CreateBindingByUri(baseAddress, options, supportedUriSchemes);

            _serviceHost.AddServiceEndpoint(typeof (ITestWcfService), baseBinding, "");

            _serviceHost.Open();

            return _serviceHost;
        }
    }
}
