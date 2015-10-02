// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlRpcParserTest.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



using System;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace CookComputing.XmlRpc
{
    internal struct Struct1
    {
        private int mi;

        private string ms;

        private bool mb;

        private double md;

        private DateTime mdt;

        private byte[] mb64;

        private int[] ma;

        private int? xi;

        private bool? xb;

        private double? xd;

        private DateTime? xdt;

        // XmlRpcStruct xstr;
        // #if !FX1_0
        // int? nxi;
        // bool? nxb;
        // double? nxd;
        // DateTime? nxdt;
        // ChildStruct? nxstr;
        // #endif
    }

    [TestFixture]
    public class XmlRpcParserTest
    {
        [Test]
        [TestCaseSource("Requests")]
        public void ParseRequest(string xml)
        {
            var rdr = XmlReader.Create(new StringReader(xml));
            var iter = new XmlRpcParser().ParseRequest(rdr).GetEnumerator();
            Assert.IsTrue(iter.MoveNext());
            Assert.IsInstanceOf<MethodName>(iter.Current);
            Assert.AreEqual("examples.getValues", (iter.Current as MethodName).Name);

            Assert.IsTrue(iter.MoveNext() && iter.Current is ParamsNode);

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "41");

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "-12");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is LongValue && (iter.Current as LongValue).Value == "123456789012");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == "Hello World");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == "Hello World");

            Assert.IsTrue(iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == " ");

            Assert.IsTrue(iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == string.Empty);

            Assert.IsTrue(iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == string.Empty);

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is DoubleValue && (iter.Current as DoubleValue).Value == "-12.214");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is DateTimeValue
                && (iter.Current as DateTimeValue).Value == "19980717T14:08:55");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is Base64Value
                && (iter.Current as Base64Value).Value == "eW91IGNhbid0IHJlYWQgdGhpcyE=");

            Assert.IsTrue(iter.MoveNext() && iter.Current is StructValue);

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StructMember && (iter.Current as StructMember).Value == "lowerBound");

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "18");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StructMember && (iter.Current as StructMember).Value == "upperBound");

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "139");

            Assert.IsTrue(iter.MoveNext());
            Assert.IsTrue(iter.Current is EndStructValue);

            Assert.IsTrue(iter.MoveNext());
            Assert.IsTrue(iter.Current is ArrayValue);

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "12");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == "Egypt");

            Assert.IsTrue(
                iter.MoveNext() && iter.Current is BooleanValue && (iter.Current as BooleanValue).Value == "0");

            Assert.IsTrue(iter.MoveNext() && iter.Current is IntValue && (iter.Current as IntValue).Value == "-31");

            Assert.IsTrue(iter.MoveNext());
            Assert.IsTrue(iter.Current is EndArrayValue);

            Assert.IsFalse(iter.MoveNext());
        }

        [Test]
        [TestCaseSource("Responses")]
        public void ParseResponse(string xml)
        {
            var rdr = XmlReader.Create(new StringReader(xml));
            var iter = new XmlRpcParser().ParseResponse(rdr).GetEnumerator();
            Assert.IsTrue(iter.MoveNext() && iter.Current is ResponseNode);
            Assert.IsTrue(
                iter.MoveNext() && iter.Current is StringValue && (iter.Current as StringValue).Value == "South Dakota");

            Assert.IsFalse(iter.MoveNext());
        }

        private static string[] Requests = new[]
                                               {
                                                   @"<?xml version=""1.0""?>
<methodCall>
  <methodName>examples.getValues</methodName>
  <params>
    <param>
      <value><int>41</int></value>
    </param>
    <param>
      <value><i4>-12</i4></value>
    </param>
    <param>
      <value><i8>123456789012</i8></value>
    </param>
    <param>
      <value><string>Hello World</string></value>
    </param>
    <param>
      <value>Hello World</value>
    </param>
    <param>
      <value> </value>
    </param>
    <param>
      <value></value>
    </param>
    <param>
      <value />
    </param>
    <param>
      <value><double>-12.214</double></value>
    </param>
    <param>
      <value><dateTime.iso8601>19980717T14:08:55</dateTime.iso8601></value>
    </param>
    <param>
      <value><base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64></value>
    </param>
    <param>
      <value>
        <struct>
          <member>
            <name>lowerBound</name>
            <value><i4>18</i4></value>
          </member>
          <member>
            <name>upperBound</name>
            <value><i4>139</i4></value>
          </member>
        </struct>
      </value>
    </param>
    <param>
      <value>
        <array>
          <data>
            <value><i4>12</i4></value>
            <value><string>Egypt</string></value>
            <value><boolean>0</boolean></value>
            <value><i4>-31</i4></value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodCall>", @"<?xml version=""1.0""?>
<methodCall>
  <methodName>examples.getValues</methodName>
  <params>
    <param>
      <value>
        <int>41</int>
      </value>
    </param>
    <param>
      <value>
        <i4>-12</i4>
      </value>
    </param>
    <param>
      <value>
        <i8>123456789012</i8>
      </value>
    </param>
    <param>
      <value>
        <string>Hello World</string>
      </value>
    </param>
    <param>
      <value>Hello World</value>
    </param>
    <param>
      <value> </value>
    </param>
    <param>
      <value></value>
    </param>
    <param>
      <value />
    </param>
    <param>
      <value>
        <double>-12.214</double>
      </value>
    </param>
    <param>
      <value>
        <dateTime.iso8601>19980717T14:08:55</dateTime.iso8601>
      </value>
    </param>
    <param>
      <value>
        <base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64>
      </value>
    </param>
    <param>
      <value>
        <struct>
          <member>
            <name>lowerBound</name>
            <value>
              <i4>18</i4>
            </value>
          </member>
          <member>
            <name>upperBound</name>
            <value>
              <i4>139</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
    <param>
      <value>
        <array>
          <data>
            <value>
              <i4>12</i4>
            </value>
            <value>
              <string>Egypt</string>
            </value>
            <value>
              <boolean>0</boolean>
            </value>
            <value>
              <i4>-31</i4>
            </value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodCall>", 
                                                   @"<?xml version=""1.0""?><methodCall><methodName>examples.getValues</methodName><params><param><value><int>41</int></value></param><param><value><i4>-12</i4></value></param><param><value><i8>123456789012</i8></value></param><param><value><string>Hello World</string></value></param><param><value>Hello World</value></param><param><value> </value></param><param><value></value></param><param><value /></param><param><value><double>-12.214</double></value></param><param><value><dateTime.iso8601>19980717T14:08:55</dateTime.iso8601></value></param><param><value><base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64></value></param><param><value><struct><member><name>lowerBound</name><value><i4>18</i4></value></member><member>          <name>upperBound</name><value><i4>139</i4></value></member></struct></value></param><param><value><array><data><value><i4>12</i4></value><value><string>Egypt</string></value><value><boolean>0</boolean></value><value><i4>-31</i4></value></data></array></value></param></params></methodCall>", 
                                               };

        private static string[] Responses = new[]
                                                {
                                                    @"<?xml version=""1.0""?>
<methodResponse>
  <params>
    <param>
      <value><string>South Dakota</string></value>
    </param>
  </params>
</methodResponse>", 
                                                    @"<?xml version=""1.0""?><methodResponse><params><param><value><string>South Dakota</string></value></param></params></methodResponse>", 
                                                };
    }
}