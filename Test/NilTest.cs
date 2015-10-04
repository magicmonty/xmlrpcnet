using System;
using System.IO;
using NUnit.Framework;
using Shouldly;

namespace CookComputing.XmlRpc

// TODO: test any culture dependencies
{
    [TestFixture]
    public class NilTest
    {
        [Test]
        public void SerializeRequestNil()
        {
            var stm = new MemoryStream();
            var req = new XmlRpcRequest();
            req.Args = new Object[] { null, 1234567 };
            req.Method = "NilMethod";
            req.Mi = GetType().GetMethod("NilMethod");
            var ser = new XmlRpcRequestSerializer();
            ser.Indentation = 4;
            ser.SerializeRequest(stm, req);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            tr.ReadToEnd().ShouldBe(
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>NilMethod</methodName>
    <params>
        <param>
            <value>
                <nil />
            </value>
        </param>
        <param>
            <value>
                <i4>1234567</i4>
            </value>
        </param>
    </params>
</methodCall>");
        }

        [XmlRpcMethod]
        public int? NilMethod(int? x, int? y)
        {
            return null;
        }

        [Test]
        public void SerializeRequestNilParams()
        {
            var stm = new MemoryStream();
            var req = new XmlRpcRequest();
            req.Args = new Object[] { new object[] { 1, null, 2 } };
            req.Method = "NilParamsMethod";
            req.Mi = GetType().GetMethod("NilParamsMethod");
            var ser = new XmlRpcRequestSerializer();
            ser.Indentation = 4;
            ser.SerializeRequest(stm, req);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            tr.ReadToEnd().ShouldBe(
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>NilParamsMethod</methodName>
    <params>
        <param>
            <value>
                <i4>1</i4>
            </value>
        </param>
        <param>
            <value>
                <nil />
            </value>
        </param>
        <param>
            <value>
                <i4>2</i4>
            </value>
        </param>
    </params>
</methodCall>");
        }

        [Test]
        public void SerializeRequestArrayWithNull()
        {
            var stm = new MemoryStream();
            var req = new XmlRpcRequest();
            var array = new [] { "AAA", null, "CCC" };
            req.Args = new Object[] { array };
            req.Method = "ArrayMethod";
            req.Mi = GetType().GetMethod("ArrayMethod");
            var ser = new XmlRpcRequestSerializer();
            ser.Indentation = 4;
            ser.SerializeRequest(stm, req);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            tr.ReadToEnd().ShouldBe(
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>ArrayMethod</methodName>
    <params>
        <param>
            <value>
                <array>
                    <data>
                        <value>
                            <string>AAA</string>
                        </value>
                        <value>
                            <nil />
                        </value>
                        <value>
                            <string>CCC</string>
                        </value>
                    </data>
                </array>
            </value>
        </param>
    </params>
</methodCall>");
        }

        public void ArrayMethod(string[] strings)
        {
        }

        [Test]
        public void SerializeRequestStructWithNil()
        {
            var stm = new MemoryStream();
            var req = new XmlRpcRequest();
            req.Args = new Object[] { new Bounds() };
            req.Method = "NilMethod";
            req.Mi = GetType().GetMethod("NilMethod");
            var ser = new XmlRpcRequestSerializer();
            ser.Indentation = 4;
            ser.SerializeRequest(stm, req);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            tr.ReadToEnd().ShouldBe(
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>NilMethod</methodName>
    <params>
        <param>
            <value>
                <struct>
                    <member>
                        <name>lowerBound</name>
                        <value>
                            <nil />
                        </value>
                    </member>
                    <member>
                        <name>upperBound</name>
                        <value>
                            <nil />
                        </value>
                    </member>
                </struct>
            </value>
        </param>
    </params>
</methodCall>");
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

        [Test]
        public void SerializeRequestStructArrayWithNil()
        {
            var stm = new MemoryStream();
            var req = new XmlRpcRequest();
            req.Args = new Object[] { new StructWithArray { ints = new int?[] { 1, null, 3 } } };
            req.Method = "NilMethod";
            req.Mi = GetType().GetMethod("NilMethod");
            var ser = new XmlRpcRequestSerializer();
            ser.Indentation = 4;
            ser.SerializeRequest(stm, req);
            stm.Position = 0;
            var tr = new StreamReader(stm);
            tr.ReadToEnd().ShouldBe(
@"<?xml version=""1.0""?>
<methodCall>
    <methodName>NilMethod</methodName>
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
</methodCall>");
        }

        public void NilParamsMethod(params int?[] numbers)
        {
        }

        [Test]
        public void DeserializeNilObject()
        {
            const string Xml = "<value><nil /></value>";
            Utils.ParseValue(Xml, typeof(object)).ShouldBeNull();
        }
    }

    [XmlRpcNullMapping(NullMappingAction.Nil)]
    public class Bounds
    {
        public int? lowerBound;
        public int? upperBound;
    }

}


