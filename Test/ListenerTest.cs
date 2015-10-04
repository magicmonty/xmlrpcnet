using System.IO;
using System.Net;
using NUnit.Framework;
using CookComputing.XmlRpc.TestHelpers;
using Shouldly;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class ListenerTest
    {
        private readonly Listener _listener = new Listener(
            new StateNameListnerService(),
            "http://127.0.0.1:11000/");

        private readonly Listener _listenerDerived = new Listener(
            new StateNameListnerDerivedService(),
            "http://127.0.0.1:11001/");

        [TestFixtureSetUp]
        public void Setup()
        {
            _listener.Start();
            _listenerDerived.Start();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _listener.Stop();
            _listenerDerived.Stop();
        }


        [Test]
        public void MakeCall()
        {
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy.AllowAutoRedirect = false;
            proxy.GetStateName(1).ShouldBe("Alabama");
        }

        [Test]
        public void MakeSystemListMethodsCall()
        {
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy
                .SystemListMethods()
                .ShouldBe(new [] { "examples.getStateName" });
        }

        [Test]
        public void GetCookie()
        {
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy.GetStateName(1);
            var cookies = proxy.ResponseCookies;
            cookies["FooCookie"].Value.ShouldBe("FooValue");
        }

        [Test]
        public void GetHeader()
        {
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy.GetStateName(1);
            var headers = proxy.ResponseHeaders;
            headers["BarHeader"].ShouldBe("BarValue");
        }

        [Test]
        public void GetAutoDocumentation()
        {
            var req = WebRequest.Create("http://127.0.0.1:11000/");
            var stm = req.GetResponse().GetResponseStream();
            var doc = new StreamReader(stm).ReadToEnd();
            doc.ShouldNotBeNull();
            doc.ShouldStartWith("<html>");
        }

        [Test]
        public void GetAutoDocumentationDerived()
        {
            var req = WebRequest.Create("http://127.0.0.1:11001/");
            var stm = req.GetResponse().GetResponseStream();
            var doc = new StreamReader(stm).ReadToEnd();
            doc.ShouldNotBeNull();
            doc.ShouldStartWith("<html>");
        }

        [Test]
        public void ResponseEvent()
        {
            string response = null;
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy.AllowAutoRedirect = false;
            proxy.ResponseEvent += (sender, args) =>
            {
                response = args.ProxyID.ToString();
            };
            proxy.GetStateName(1);
            response.ShouldNotBeNull();
        }

        [Test]
        public void RequestEvent()
        {
            string response = null;
            var proxy = XmlRpcProxyGen.Create<IStateName>();
            proxy.Url = "http://127.0.0.1:11000/";
            proxy.AllowAutoRedirect = false;
            proxy.RequestEvent += (sender, args) =>
            {
                response = args.ProxyID.ToString();
            };
            proxy.GetStateName(1);
            response.ShouldNotBeNull();
        }
    }
}