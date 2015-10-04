using System.IO;
using NUnit.Framework;
using Shouldly;
using System;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    internal class NilTestServer
    {
        [Test]
        public void DeserializeResponseNilMethod()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><nil /></value>
    </param>
  </params>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, GetType());

            response.RetVal.ShouldBeNull();
        }

        [Test]
        public void DeserializeResponseStructWithNil()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>lowerBound</name>
            <value><nil/></value>
          </member>
          <member>
            <name>upperBound</name>
            <value><nil/></value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";
            
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(ServerBounds));
            response.RetVal.ShouldBeOfType<ServerBounds>();
            var bounds = response.RetVal as ServerBounds;
            bounds.lowerBound.ShouldBeNull();
            bounds.upperBound.ShouldBeNull();
        }

        [Test]
        public void DeserializeRequestStructWithNil()
        {
            const string Xml = 
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>StructWithArrayMethod</methodName>
    <params>
        <param>
            <value>
                <struct>
                    <member>
                        <name>ints</name>
                        <value>
                            <array>
                                <data>
                                    <value>
                                        <i4>1</i4>
                                    </value>
                                    <value>
                                        <nil />
                                    </value>
                                    <value>
                                        <i4>3</i4>
                                    </value>
                                </data>
                            </array>
                        </value>
                    </member>
                </struct>
            </value>
        </param>
    </params>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcRequestDeserializer();
            var request = deserializer.DeserializeRequest(sr, GetType());

            request.Method.ShouldBe("StructWithArrayMethod");
            request.Args.Length.ShouldBe(1);
            request.Args[0].ShouldBeOfType<StructWithArray>();
            ((StructWithArray)request.Args[0]).ints
                .ShouldBe(new int?[] { 1, null, 3});
        }

        [Test]
        public void DeserializeRequestNilMethod()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>NilMethod</methodName> 
  <params>
    <param>
      <value><nil /></value>
    </param>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcRequestDeserializer();
            var request = deserializer.DeserializeRequest(sr, GetType());

            request.Method.ShouldBe("NilMethod");
            request.Args[0].ShouldBeNull();
            request.Args[1].ShouldBe(12345);
        }

        [XmlRpcNullMapping(NullMappingAction.Nil)]
        public struct StructWithArray
        {
            public int?[] ints;
        }

        [XmlRpcMethod]
        public void StructWithArrayMethod(StructWithArray x)
        {
        }

        [XmlRpcMethod]
        public int? NilMethod(int? x, int? y)
        {
            return null;
        }
    }

    [XmlRpcNullMapping(NullMappingAction.Nil)]
    class ServerBounds
    {
        public int? lowerBound;

        public int? upperBound;
    }
}