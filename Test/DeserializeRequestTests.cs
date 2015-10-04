using System;
using System.Globalization;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Shouldly;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class DeserializeRequestTests
    {
        private static XmlRpcRequest DeserializeRequest(string xml)
        {
            var sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            return deserializer.DeserializeRequest(sr, null);
        }

        [Test]
        public void StringElement()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestString", "method is TestString");
            request.Args[0].ShouldBeOfType<string>("argument is string");
            ((string)request.Args[0]).ShouldBe("test string", "argument is 'test string'");
        }

        [Test]
        public void StringNoStringElement()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestString", "method is TestString");
            request.Args[0].ShouldBeOfType<string>("argument is string");
            ((string)request.Args[0]).ShouldBe("test string", "argument is 'test string'");
        }

        [Test]
        public void StringEmptyValue1()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestString", "method is TestString");
            request.Args[0].ShouldBeOfType<string>("argument is string");
            ((string)request.Args[0]).ShouldBeEmpty("argument is empty string");
        }

        [Test]
        public void StringEmptyValue2()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestString</methodName> 
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestString", "method is TestString");
            request.Args[0].ShouldBeOfType<string>("argument is string");
            ((string)request.Args[0]).ShouldBeEmpty("argument is empty string");
        }

        [Test]
        public void FlatXml()
        {
            const string Xml = @"<?xml version=""1.0"" ?><methodCall><methodName>TestString</methodName><params><param><value>test string</value></param></params></methodCall>";
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestString", "method is TestString");
            request.Args[0].ShouldBeOfType<string>("argument is string");
            ((string)request.Args[0]).ShouldBe("test string", "argument is 'test string'");
        }

        [Test]
        public void NullRequestStream()
        {
            Should.Throw<ArgumentNullException>(() =>
                new XmlRpcRequestDeserializer().DeserializeRequest((Stream)null, null));
        }

        [Test]
        public void EmptyRequestStream()
        {
            Should.Throw<XmlRpcIllFormedXmlException>(() => DeserializeRequest(string.Empty));
        }

        [Test]
        public void InvalidXml()
        {
            Should.Throw<XmlRpcIllFormedXmlException>(() => DeserializeRequest(@"<?xml version=""1.0"" ?><methodCall> </duffMmethodCall>"));
        }

        // test handling of methodCall element
        [Test]
        public void MissingMethodCall()
        {
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(@"<?xml version=""1.0"" ?> <elem/>"));
        }

        // test handling of methodName element
        [Test]
        public void MissingMethodName()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void EmptyMethodName()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName/> 
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void ZeroLengthMethodName()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName></methodName> 
<params>
  <param>
    <value>test string</value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        // test handling of params element
        [Test]
        public void MissingParams()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
</methodCall>";
            
            Should.NotThrow(() => DeserializeRequest(Xml));
        }

        [XmlRpcMethod]
        public string MethodNoArgs()
        {
            return string.Empty;
        }

        // test handling of params element
        [Test]
        public void NoParam1()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>MethodNoArgs</methodName> 
<params/>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var serializer = new XmlRpcRequestDeserializer();
            Should.NotThrow(() =>  serializer.DeserializeRequest(sr, GetType()));
        }

        // test handling of param element
        [Test]
        public void NoParam2()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>MethodNoArgs</methodName> 
  <params>
  </params>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcRequestDeserializer();
            Should.NotThrow(() =>  deserializer.DeserializeRequest(sr, GetType()));
        }

        [Test]
        public void EmptyParam1()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
<params>
  <param/>
</params>
</methodCall>";
            
            var sr = new StringReader(Xml);
            var serializer = new XmlRpcRequestDeserializer();
            Should.NotThrow(() =>  serializer.DeserializeRequest(sr, null));
        }

        [Test]
        public void EmptyParam2()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestString</methodName> 
<params>
  <param>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        // test handling integer values
        [Test]
        public void Integer()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value>
        <int>666</int>
      </value>
    </param>
  </params>
</methodCall>";

            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestInt");
            request.Args[0].ShouldBeOfType<int>();
            ((int)request.Args[0]).ShouldBe(666);
        }

        [Test]
        public void I4Integer()
        {
            const string Xml = @"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>666</i4></value>
    </param>
  </params>
</methodCall>";

            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestInt");
            request.Args[0].ShouldBeOfType<int>();
            ((int)request.Args[0]).ShouldBe(666);
        }

        [Test]
        public void IntegerWithPlus()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>+666</i4></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestInt");
            request.Args[0].ShouldBeOfType<int>();
            ((int)request.Args[0]).ShouldBe(666);
        }

        [Test]
        public void NegativeInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>-666</i4></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestInt");
            request.Args[0].ShouldBeOfType<int>();
            ((int)request.Args[0]).ShouldBe(-666);
        }

        [Test]
        public void EmptyInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4></i4></value>
  </param>
</params>
</methodCall>";

            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void InvalidInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>12kiol</i4></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void OverflowInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>99999999999999999999</i4></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void ZeroInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i4>0</i4></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);

            request.Method.ShouldBe("TestInt");
            request.Args[0].ShouldBeOfType<int>();
            ((int)request.Args[0]).ShouldBe(0);
        }

        [Test]
        public void NegativeOverflowInteger()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i4>-99999999999999999999</i4></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        // test handling i8 values
        [Test]
        public void I8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value>
        <i8>123456789012</i8>
      </value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);
            request.Args[0].ShouldBeOfType<long>();
            ((long)request.Args[0]).ShouldBe(123456789012);
        }

        [Test]
        public void I8WithPlus()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>+123456789012</i8></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);
            request.Args[0].ShouldBeOfType<long>();
            ((long)request.Args[0]).ShouldBe(123456789012);
        }

        [Test]
        public void NegativeI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>-123456789012</i8></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);
            request.Args[0].ShouldBeOfType<long>();
            ((long)request.Args[0]).ShouldBe(-123456789012);
        }

        [Test]
        public void EmptyI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8></i8></value>
  </param>
</params>
</methodCall>";

            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void InvalidI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>12kiol</i8></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void OverflowI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>9999999999999999999999999999999999999999999</i8></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void ZeroI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
  <methodName>TestInt</methodName> 
  <params>
    <param>
      <value><i8>0</i8></value>
    </param>
  </params>
</methodCall>";
            
            var request = DeserializeRequest(Xml);
            request.Args[0].ShouldBeOfType<long>();
            ((long)request.Args[0]).ShouldBe(0);
        }

        [Test]
        public void NegativeOverflowI8()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestInt</methodName> 
<params>
  <param>
    <value><i8>-9999999999999999999999999999999999999999999</i8></value>
  </param>
</params>
</methodCall>";
            
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => DeserializeRequest(Xml));
        }

        [Test]
        public void ISO_8859_1()
        {
            using (var stm = new FileStream("testdocuments/iso-8859-1_request.xml", FileMode.Open, FileAccess.Read))
            {
                var deserializer = new XmlRpcRequestDeserializer();
                var request = deserializer.DeserializeRequest(stm, null);
                request.Args[0].ShouldBeOfType<string>();
                request.Args[0].ShouldBe("hæ hvað segirðu þá");
            }
        }

        struct Struct3
        {
            int _member1;
            public int member1 { get { return _member1; } set { _member1 = value; } }

            int _member2;
            public int member2 { get { return _member2; } }

            int _member3;
            [XmlRpcMember("member-3")]
            public int member3 { get { return _member3; } set { _member3 = value; } }

            int _member4;
            [XmlRpcMember("member-4")]
            public int member4 { get { return _member4; } }
        }

        [Test]
        public void StructProperties()
        {
            const string Xml = 
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>Foo</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>member1</name>
            <value>
              <i4>1</i4>
            </value>
          </member>
          <member>
            <name>member2</name>
            <value>
              <i4>2</i4>
            </value>
          </member>
          <member>
            <name>member-3</name>
            <value>
              <i4>3</i4>
            </value>
          </member>
          <member>
            <name>member-4</name>
            <value>
              <i4>4</i4>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>";

            var request = DeserializeRequest(Xml);

            request.Args[0].ShouldBeOfType<XmlRpcStruct>();

            var xrs = (XmlRpcStruct)request.Args[0];
            xrs.ShouldSatisfyAllConditions(
                () => xrs.Count.ShouldBe(4),
                () => xrs.ContainsKey("member1").ShouldBeTrue(),
                () => xrs["member1"].ShouldBe(1),
                () => xrs.ContainsKey("member2").ShouldBeTrue(),
                () => xrs["member2"].ShouldBe(2),
                () => xrs.ContainsKey("member-3").ShouldBeTrue(),
                () => xrs["member-3"].ShouldBe(4),
                () => xrs.ContainsKey("member-4").ShouldBeTrue(),
                () => xrs["member-4"].ShouldBe(4));
        }

        // test handling dateTime values
        [Test]
        public void DateTimeFormats()
        {
            const string Xml = 
@"<?xml version=""1.0"" ?> 
<methodCall>
<methodName>TestDateTime</methodName> 
<params>
  <param>
    <value><dateTime.iso8601>20020707T11:25:37Z</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>20020707T11:25:37</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>2002-07-07T11:25:37Z</dateTime.iso8601></value>
  </param>
  <param>
    <value><dateTime.iso8601>2002-07-07T11:25:37</dateTime.iso8601></value>
  </param>
</params>
</methodCall>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcRequestDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, null);

            request.Args[0].ShouldBeOfType<DateTime>();
            request.Args[1].ShouldBeOfType<DateTime>();
            request.Args[2].ShouldBeOfType<DateTime>();
            request.Args[3].ShouldBeOfType<DateTime>();

            var dt0 = (DateTime)request.Args[0];
            var dt1 = (DateTime)request.Args[1];
            var dt2 = (DateTime)request.Args[2];
            var dt3 = (DateTime)request.Args[3];

            var dt = new DateTime(2002, 7, 7, 11, 25, 37);
            dt0.ShouldBe(dt, "DateTime WordPress");
            dt1.ShouldBe(dt, "DateTime XML-RPC spec");
            dt2.ShouldBe(dt, "DateTime TypePad");
            dt3.ShouldBe(dt, "DateTime other");
        }

        [Test]
        public void DateTimeLocales()
        {
            var oldci = Thread.CurrentThread.CurrentCulture;
            try
            {
                Should.NotThrow(() =>
                {
                    foreach (var locale in Utils.GetLocales())
                    {
                        var ci = new CultureInfo(locale);
                        Thread.CurrentThread.CurrentCulture = ci;
                        if (ci.LCID == 0x401    // ar-SA  (Arabic - Saudi Arabia)
                          || ci.LCID == 0x465   // div-MV (Dhivehi - Maldives)
                          || ci.LCID == 0x41e)  // th-TH  (Thai - Thailand)
                            break;

                        var dt = new DateTime(1900, 01, 02, 03, 04, 05);
                        while (dt < DateTime.Now)
                        {
                            var stm = new MemoryStream();
                            var req = new XmlRpcRequest();
                            req.Args = new object[] { dt };
                            req.Method = "Foo";

                            var ser = new XmlRpcRequestSerializer();
                            ser.SerializeRequest(stm, req);
                            stm.Position = 0;

                            var deserializer = new XmlRpcRequestDeserializer();
                            var request = deserializer.DeserializeRequest(stm, null);

                            request.Args[0].ShouldBeOfType<DateTime>();
                            var dt0 = (DateTime)request.Args[0];
                            dt0.ShouldBe(dt, "DateTime argument 0");
                            dt += new TimeSpan(100, 1, 1, 1);
                        }
                    }
                });
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldci;
            }
        }

        [Test]
        public void Base64Empty()
        {
            const string Xml = 
@"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64></base64>
      </value>
    </param>
  </params>
</methodCall>";
            StringReader sr = new StringReader(Xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, null);

            Assert.AreEqual(request.Args[0].GetType(), typeof(byte[]),
              "argument is byte[]");
            Assert.AreEqual(request.Args[0], new byte[0],
              "argument is zero length byte[]");
        }

        [Test]
        public void Base64()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64>AQIDBAUGBwg=</base64>
      </value>
    </param>
  </params>
</methodCall>";
            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, null);

            Assert.AreEqual(request.Args[0].GetType(), typeof(byte[]),
              "argument is byte[]");
            byte[] ret = (byte[])request.Args[0];
            Assert.AreEqual(8, ret.Length, "argument is byte[8]");
            for (int i = 0; i < ret.Length; i++)
                Assert.AreEqual(i + 1, ret[i], "members are 1 to 8");
        }

        [Test]
        public void Base64MultiLine()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestHex</methodName>
  <params>
    <param>
      <value>
        <base64>AQIDBAUGBwgJ
AQIDBAUGBwg=</base64>
      </value>
    </param>
  </params>
</methodCall>";
            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, null);

            Assert.AreEqual(request.Args[0].GetType(), typeof(byte[]),
              "argument is byte[]");
            byte[] ret = (byte[])request.Args[0];
            Assert.AreEqual(17, ret.Length, "argument is byte[17]");
            for (int i = 0; i < 9; i++)
                Assert.AreEqual(i + 1, ret[i], "first 9 members are 1 to 9");
            for (int i = 0; i < 8; i++)
                Assert.AreEqual(i + 1, ret[i + 9], "last 8 members are 1 to 9");
        }


        //    // test array handling
        //
        //
        //
        //    // tests of handling of structs
        //    public void testMissingMemberStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //
        //    public void testAdditonalMemberStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //
        //    public void testReversedMembersStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //    
        //    public void testWrongTypeMembersStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //
        //    public void testDuplicateMembersStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //
        //    public void testNonAsciiMemberNameStruct()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }
        //
        //    // test various invalid requests
        //    public void testIncorrectParamType()
        //    {
        //      string xml = @"<?xml version=""1.0"" ?> 
        //<methodCall>
        //  <methodName>TestStruct</methodName> 
        //  <params>
        //    <param>
        //    </param>
        //  </params>
        //</methodCall>";
        //      StringReader sr = new StringReader(xml);
        //      XmlRpcDeserializer serializer = new XmlRpcDeserializer();
        //      XmlRpcRequest request = serializer.DeserializeRequest(sr, null);
        //    }



        public class TestClass
        {
            public int _int;
            public string _string;
        }

        [XmlRpcMethod]
        public void TestClassMethod(TestClass testClass)
        {
        }

        [Test]
        public void Class()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall>
  <methodName>TestClassMethod</methodName>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>_int</name>
            <value>
              <i4>456</i4>
            </value>
          </member>
          <member>
            <name>_string</name>
            <value>
              <string>Test Class</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(TestClass),
              "argument is TestClass");
            //      XmlRpcStruct xrs = (XmlRpcStruct)request.args[0];
            //      Assert.IsTrue(xrs.Count == 4, "XmlRpcStruct has 4 members");
            //      Assert.IsTrue(xrs.ContainsKey("member1") && (int)xrs["member1"] == 1, 
            //        "member1");
            //      Assert.IsTrue(xrs.ContainsKey("member2") && (int)xrs["member2"] == 2, 
            //        "member2");
            //      Assert.IsTrue(xrs.ContainsKey("member-3") && (int)xrs["member-3"] == 3,
            //        "member-3");
            //      Assert.IsTrue(xrs.ContainsKey("member-4") && (int)xrs["member-4"] == 4,
            //        "member-4");

        }


        public struct simple
        {
            public int number;
            public string detail;
        }

        [XmlRpcMethod("rtx.useArrayOfStruct")]
        public string UseArrayOfStruct(simple[] myarr)
        {
            return "";
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
        public void Blakemore()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.useArrayOfStruct</methodName>
<params>
<param><value><array>
<data><value>
<struct><member><name>detail</name><value><string>elephant</string></value></member><member><name>number</name><value><int>76</int></value></member></struct>
</value></data>
<data><value>
<struct><member><name>detail</name><value><string>rhino</string></value></member><member><name>number</name><value><int>33</int></value></member></struct>
</value></data>
<data><value>
<struct><member><name>detail</name><value><string>porcupine</string></value></member><member><name>number</name><value><int>106</int></value></member></struct>
</value></data>
</array></value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());
        }

        [XmlRpcMethod("rtx.EchoString")]
        public string EchoString(string str)
        {
            return str;
        }

        [Test]
        public void SingleSpaceString()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value> </value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(string),
              "argument is string");
            Assert.AreEqual(" ", request.Args[0],
              "argument is string containing single space");
        }

        [Test]
        public void TwoSpaceString()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>  </value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(string),
              "argument is string");
            Assert.AreEqual("  ", request.Args[0],
              "argument is string containing two spaces");
        }

        [Test]
        public void LeadingSpace()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value> ddd</value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(string),
              "argument is string");
            Assert.AreEqual(" ddd", request.Args[0]);
        }

        [Test]
        public void TwoLeadingSpace()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>  ddd</value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(string),
              "argument is string");
            Assert.AreEqual("  ddd", request.Args[0]);
        }

        [Test]
        public void EmptyLines()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall><methodName>rtx.EchoString</methodName>
<params>
<param><value>
</value></param>
</params></methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());

            Assert.AreEqual(request.Args[0].GetType(), typeof(string),
              "argument is string");
            Assert.AreEqual("\r\n", request.Args[0]);
        }

        [XmlRpcMethod("blogger.getUsersBlogs")]
        public void GetUsersBlogs(string username, string password)
        {

        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidParametersException))]
        public void TooManyParameters()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall>
<methodName>blogger.getUsersBlogs</methodName>
<params>
<param>
<value>
<string>ffffffabffffffce6dffffff93ffffffac29ffffffc9fffffff826ffffffdefffff
fc9ff\
ffffe43c0b763036ffffffa0fffffff3ffffffa963377716</string>
</value>
</param>
<param>
<value>
<string>myusername</string>
</value>
</param>
<param>
<value>
<string>mypassword</string>
</value>
</param>
</params>
</methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidParametersException))]
        public void TooFewParameters()
        {
            string xml = @"<?xml version=""1.0""?>
<methodCall>
<methodName>blogger.getUsersBlogs</methodName>
<params>
<param>
<value>
<string>myusername</string>
</value>
</param>
</params>
</methodCall>";

            StringReader sr = new StringReader(xml);
            var deserializer = new XmlRpcRequestDeserializer();
            XmlRpcRequest request = deserializer.DeserializeRequest(sr, GetType());
        }
    }
}



