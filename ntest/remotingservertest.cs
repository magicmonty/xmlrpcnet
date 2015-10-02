using ntest;
using NUnit.Framework;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class RemotingServerTest
    {
        [Test]
        public void Method1()
        {
            var proxy = (ITest)XmlRpcProxyGen.Create(typeof(ITest));
            XmlRpcClientProtocol cp = (XmlRpcClientProtocol)proxy;
        }
    }
}