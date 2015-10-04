using System.IO;
using System.Reflection;
using NUnit.Framework;
using Shouldly;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class EnumTestServer
    {
        [return: XmlRpcEnumMapping(EnumMapping.String)]
        public IntEnum MappingReturnOnMethod()
        {
            return IntEnum.One;
        }

        [Test]
        public void SerializeResponseOnMethod()
        {
            var deserializer = new XmlRpcResponseSerializer();
            var response = new XmlRpcResponse(IntEnum.One, GetType().GetMethod("MappingReturnOnMethod"));
            var stm = new MemoryStream();
            deserializer.SerializeResponse(stm, response);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            var reqstr = tr.ReadToEnd();
            reqstr.ShouldBe(
@"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>One</string>
      </value>
    </param>
  </params>
</methodResponse>");
        }

        [Test]
        public void SerializeResponseOnType()
        {
            var deserializer = new XmlRpcResponseSerializer();
            var proxy = XmlRpcProxyGen.Create<TestMethods2>();
            var mi = proxy.GetType().GetMethod("Bar");
            var response = new XmlRpcResponse(IntEnum.Three, mi);
            var stm = new MemoryStream();
            deserializer.SerializeResponse(stm, response);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            var reqstr = tr.ReadToEnd();
            reqstr.ShouldBe(
@"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value>
        <string>Three</string>
      </value>
    </param>
  </params>
</methodResponse>", reqstr);
        }
    }
}