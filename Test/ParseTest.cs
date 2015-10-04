using System;
using System.Collections;
using System.Xml;
using NUnit.Framework;

// TODO: parse array
// TODO: parse struct
// TODO: parse XmlRpcStruct
// TODO: parse XmlRpcStruct derived
// TODO: array of base64
using Shouldly;


namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class ParseTest
    {
        public struct Struct2
        {
            public int mi;
            public string ms;
            public bool mb;
            public double md;
            public DateTime mdt;
            public byte[] mb64;
            public int[] ma;
            public int? xi;
            public bool? xb;
            public double? xd;
            public DateTime? xdt;
            public XmlRpcStruct xstr;
        }

        [Test]
        public void Int_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>12345</int></value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(12345);
        }

        [Test]
        public void Int_IntType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>12345</int></value>";
            Utils
                .Parse(Xml, typeof(int), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(12345);
        }

        [Test]
        public void Int_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>12345</int></value>";
            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(12345);
        }

        [Test]
        public void Int_TooLarge()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>123456789012</int></value>";
            Should.Throw<XmlRpcInvalidXmlRpcException>(
                () => Utils.Parse(Xml, typeof(int), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        [Test]
        public void Int64_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><i8>123456789012</i8></value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(123456789012);
        }

        [Test]
        public void Int64_IntType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><i8>123456789012</i8></value>";
            Utils
                .Parse(Xml, typeof(long), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(123456789012);
        }

        [Test]
        public void Int64_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><i8>123456789012</i8></value>";
            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(123456789012);
        }

        [Test]
        public void String_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><string>astring</string></value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe("astring");
        }

        [Test]
        public void DefaultString_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =@"<?xml version=""1.0"" ?><value>astring</value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe("astring");
        }

        [Test]
        public void String_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><string>astring</string></value>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe("astring");
        }

        [Test]
        public void String_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><string>astring</string></value>";
            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe("astring");
        }

        [Test]
        public void DefaultString_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value>astring</value>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe("astring");
        }

        [Test]
        public void Empty1String_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><string></string></value>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(string.Empty);
        }

        [Test]
        public void Empty2String_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><string/></value>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(string.Empty);
        }

        [Test]
        public void Default1EmptyString_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value></value>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(string.Empty);
        }

        [Test]
        public void Default2EmptyString_StringType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value/>";
            Utils
                .Parse(Xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(string.Empty);
        }

        [Test]
        public void IllegalChars_String()
        {
            Type parsedType, parsedArrayType;
            var str = new string('\a', 1);
            var xml = @"<?xml version=""1.0"" ?><value><string>" + str + "</string></value>";

            Should.Throw<XmlException>(() =>
                Utils.Parse(xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        [Test]
        public void IllegalChars_ValueString()
        {
            Type parsedType, parsedArrayType;
            var str = new string('\a', 1);
            var xml = @"<?xml version=""1.0"" ?><value>" + str + "</value>";
            Should.Throw<XmlException>(() =>
                Utils.Parse(xml, typeof(string), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        [Test]
        public void Boolean_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><boolean>1</boolean></value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(true);
        }

        [Test]
        public void Boolean_BooleanType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><boolean>1</boolean></value>";

            Utils
                .Parse(Xml, typeof(bool), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(true);
        }

        [Test]
        public void Boolean_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><boolean>1</boolean></value>";

            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(true);
        }

        [Test]
        public void Double_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><double>543.21</double></value>";

            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(543.21);
        }

        [Test]
        public void Double_DoubleType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><double>543.21</double></value>";

            Utils
                .Parse(Xml, typeof(double), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(543.21);
            
        }

        [Test]
        public void Double_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><double>543.21</double></value>";

            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(543.21);
        }

        [Test]
        public void DateTime_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37</dateTime.iso8601></value>";
            Utils
                .Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(new DateTime(2002, 7, 6, 11 ,25 , 37));
        }

        [Test]
        public void DateTime_DateTimeType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37</dateTime.iso8601></value>";
            Utils
                .Parse(Xml, typeof(DateTime), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(new DateTime(2002, 7, 6, 11 ,25 , 37));
        }

        [Test]
        public void DateTime_ObjectTimeType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37</dateTime.iso8601></value>";
            Utils
                .Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType)
                .ShouldBe(new DateTime(2002, 7, 6, 11 ,25 , 37));
        }

        [Test]
        public void DateTime_ROCA()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>2002-07-06T11:25:37</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);

            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_WordPress()
        {
            // yyyyMMddThh:mm:ssZ
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37Z</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TypePad()
        {
            // yyyy-MM-ddThh:mm:ssZ
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>2002-07-06T11:25:37Z</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZPlus01()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T12:25:37+01</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZPlus0130()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T12:55:37+0130</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZPlus01Colon30()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T12:55:37+01:30</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZPlus00()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37+00</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZPlus0000()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37+0000</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZMinus01()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T10:25:37-01</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZMinus0130()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T09:55:37-0130</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZMinus01Colon30()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T09:55:37-01:30</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZMinus00()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37-00</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_TZMinus0000()
        {
            // yyyyMMddThh:mm:ssZ+00
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37-0000</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        [Test]
        public void DateTime_allZeros1()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>0000-00-00T00:00:00</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(DateTime.MinValue);
        }

        [Test]
        public void DateTime_allZeros2()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>0000-00-00T00:00:00Z</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(DateTime.MinValue);
        }

        [Test]
        public void DateTime_allZeros3()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>00000000T00:00:00Z</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(DateTime.MinValue);
        }

        [Test]
        public void DateTime_allZeros4()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>0000-00-00T00:00:00</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(DateTime.MinValue);
        }

        [Test]
        public void DateTime_Empty_Standard()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601></dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.MapEmptyDateTimeToMinValue;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(DateTime.MinValue);
        }

        [Test]
        public void DateTime_Empty_NonStandard()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601></dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => 
                Utils.Parse(
                    Xml,
                    typeof(DateTime),
                    MappingAction.Error,
                    serializer,
                    out parsedType,
                    out parsedArrayType));
        }

        [Test]
        public void Issue72()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20090209T22:20:01+01:00</dateTime.iso8601></value>";
            var serializer = new XmlRpcDeserializer();
            serializer.NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime;
            var obj = Utils.Parse(
                Xml,
                typeof(DateTime),
                MappingAction.Error,
                serializer,
                out parsedType,
                out parsedArrayType);
            obj.ShouldBe(new DateTime(2009, 2, 9, 21, 20, 01));
        }

        private byte[] testb = 
        {
            121, 111, 117, 32, 99, 97, 110, 39, 
            116, 32, 114, 101, 97, 100, 32, 116, 
            104, 105, 115, 33
        };

        [Test]
        public void Base64_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64></value>";
            var obj = Utils.Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(testb);
        }

        [Test]
        public void Base64_Base64Type()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64></value>";
            var obj = Utils.Parse(Xml, typeof(byte[]), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(testb);
        }

        [Test]
        public void Base64_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><base64>eW91IGNhbid0IHJlYWQgdGhpcyE=</base64></value>";
            var obj = Utils.Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(testb);
        }

        [Test]
        public void Base64_ZeroLength()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><base64></base64></value>";
            var obj = Utils.Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(new byte[0]);
        }

        [Test]
        public void Base64_ZeroLengthEmptyTag()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><base64 /></value>";
            var obj = Utils.Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(new byte[0]);
        }

        // ---------------------- XmlRpcInt -------------------------------------// 
        [Test]
        public void XmlRpcInt_XmlRpcIntType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>12345</int></value>";
            var obj = Utils.Parse(Xml, typeof(int?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(12345);
        }

        // ---------------------- XmlRpcBoolean ---------------------------------// 
        [Test]
        public void XmlRpcBoolean_XmlRpcBooleanType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><boolean>1</boolean></value>";
            var obj = Utils.Parse(Xml, typeof(bool?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(true);
        }

        // ---------------------- double? ----------------------------------// 
        [Test]
        public void XmlRpcDouble_XmlRpcDoubleType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><double>543.21</double></value>";
            var obj = Utils.Parse(Xml, typeof(double?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(543.21);
        }

        // ---------------------- DateTime? --------------------------------// 
        [Test]
        public void XmlRpcDateTime_XmlRpcDateTimeType()
        {
            Type parsedType, parsedArrayType;
            const string xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37</dateTime.iso8601></value>";
            var obj = Utils.Parse(xml, typeof(DateTime?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        // ---------------------- int? -------------------------------------// 
        [Test]
        public void nullableIntType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><int>12345</int></value>";
            var obj = Utils.Parse(Xml, typeof(int?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(12345);
        }

        // ---------------------- bool? ---------------------------------// 
        [Test]
        public void nullableBoolType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><boolean>1</boolean></value>";
            var obj = Utils.Parse(Xml, typeof(bool?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(true);
        }

        // ---------------------- double? ----------------------------------// 
        [Test]
        public void nullableDoubleType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><double>543.21</double></value>";
            var obj = Utils.Parse(Xml, typeof(double?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(543.21);
        }

        // ---------------------- DateTime? --------------------------------// 
        [Test]
        public void nullableDateTimeType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?><value><dateTime.iso8601>20020706T11:25:37</dateTime.iso8601></value>";
            var obj = Utils.Parse(Xml, typeof(DateTime?), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBe(new DateTime(2002, 7, 6, 11, 25, 37));
        }

        // ---------------------- XmlRpcStruct array ----------------------------// 
        [Test]
        public void XmlRpcStructArray()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value>
        <struct>
          <member>
            <name>mi</name>
            <value><i4>18</i4></value>
          </member>
        </struct>
      </value>
      <value>
        <struct>
          <member>
            <name>mi</name>
            <value><i4>28</i4></value>
          </member>
        </struct>
      </value>
    </data>
  </array>
</value>";

            var obj = Utils.Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType);

            obj.ShouldBeOfType<XmlRpcStruct[]>();
            var objarray = (object[])obj;
            objarray[0].ShouldBeOfType<XmlRpcStruct>();
            objarray[1].ShouldBeOfType<XmlRpcStruct>();
            var xstruct1 = objarray[0] as XmlRpcStruct;
            var xstruct2 = objarray[1] as XmlRpcStruct;

            xstruct1["mi"].ShouldBe(18);
            xstruct2["mi"].ShouldBe(28);
        }

        // ---------------------- struct ------------------------------------------// 
        [Test]
        public void NameEmptyString()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name/>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";
            Should.Throw<XmlRpcInvalidXmlRpcException>(() => 
                Utils.Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType));
        }

        // ------------------------------------------------------------------------// 
        public struct Struct3
        {
            [XmlRpcMember("IntField")]
            public int intOne;

            [XmlRpcMember("IntProperty")]
            public int intTwo
            {
                get { return _intTwo; }
                set { _intTwo = value; }
            }

            private int _intTwo;
        }

        [Test]
        public void PropertyXmlRpcName()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>IntField</name>
      <value><i4>18</i4></value>
    </member>
    <member>
      <name>IntProperty</name>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";
            
            Should.NotThrow(() =>
                Utils.Parse(Xml, typeof(Struct3), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        private struct Struct4
        {
            [NonSerialized]
            public int x;

            public int y;
        }

        [Test]
        public void IgnoreNonSerialized()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>y</name>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";
            Should.NotThrow(() => 
                Utils.Parse(Xml, typeof(Struct4), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        [Test]
        public void NonSerializedInStruct()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>x</name>
      <value><i4>12</i4></value>
    </member>
    <member>
      <name>y</name>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";

            Should.Throw<XmlRpcNonSerializedMember>(() =>
                Utils.Parse(Xml, typeof(Struct4), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        public struct Struct5
        {
            public int x;
        }

        [Test]
        public void UnexpectedMember()
        {
            Type parsedType, parsedArrayType;
            const string Xml = @"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>x</name>
      <value><i4>12</i4></value>
    </member>
    <member>
      <name>y</name>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";
            Should.NotThrow(() =>
                Utils.Parse(Xml, typeof(Struct5), MappingAction.Error, out parsedType, out parsedArrayType));
        }

        [Test]
        public void NonSerializedNonXmlRpcType()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>y</name>
      <value><i4>18</i4></value>
    </member>
  </struct>
</value>";
            
            var obj = Utils.Parse(Xml, typeof(Struct4), MappingAction.Error, out parsedType, out parsedArrayType);
            var ret = (Struct4)obj;
            ret.x.ShouldBe(0);
            ret.y.ShouldBe(18);
        }

        [Test]
        public void XmlRpcStructOrder()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>a</name>
      <value><i4>1</i4></value>
    </member>
    <member>
      <name>c</name>
      <value><i4>3</i4></value>
    </member>
    <member>
      <name>b</name>
      <value><i4>2</i4></value>
    </member>
  </struct>
</value>";
            
            var obj = Utils.Parse(
                Xml,
                typeof(XmlRpcStruct),
                MappingAction.Error,
                out parsedType,
                out parsedArrayType);

            obj.ShouldBeOfType<XmlRpcStruct>();
            var strct = obj as XmlRpcStruct;
            var denumerator = strct.GetEnumerator();
            denumerator.MoveNext();
            denumerator.Key.ShouldBe("a");
            denumerator.Value.ShouldBe(1);
            denumerator.MoveNext();
            denumerator.Key.ShouldBe("c");
            denumerator.Value.ShouldBe(3);
            denumerator.MoveNext();
            denumerator.Key.ShouldBe("b");
            denumerator.Value.ShouldBe(2);
        }

        public class RecursiveMember
        {
            public string Level;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public RecursiveMember childExample;
        }

        [Test]
        public void RecursiveMemberTest()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>Level</name>
      <value>1</value>
    </member>
    <member>
      <name>childExample</name>
      <value>
        <struct>
          <member>
            <name>Level</name>
            <value>2</value>
          </member>
          <member>
            <name>childExample</name>
            <value>
              <struct>
                <member>
                  <name>Level</name>
                  <value>3</value>
                </member>
              </struct>
            </value>
          </member>
        </struct>
      </value>
    </member>
  </struct>
</value>";
            var obj = Utils.Parse(
                Xml,
                typeof(RecursiveMember),
                MappingAction.Error,
                out parsedType,
                out parsedArrayType);

            obj.ShouldNotBeNull();
            obj.ShouldBeOfType<RecursiveMember>();
        }

        public class RecursiveArrayMember
        {
            public string Level;

            public RecursiveArrayMember[] childExamples;
        }

        [Test]
        public void RecursiveArrayMemberTest()
        {
            Type parsedType, parsedArrayType;
            const string Xml = 
@"<?xml version=""1.0"" ?>
<value>
  <struct>
    <member>
      <name>Level</name>
      <value>1</value>
    </member>
    <member>
      <name>childExamples</name>
      <value>
       <array>
          <data>
            <value>
              <struct>
                <member>
                  <name>Level</name>
                  <value>1-1</value>
                </member>
                <member>
                  <name>childExamples</name>
                  <value>
                   <array>
                      <data>
                        <value>
                          <struct>
                            <member>
                              <name>Level</name>
                              <value>1-1-1</value>
                            </member>
                            <member>
                              <name>childExamples</name>
                              <value>
                               <array>
                                  <data>
                                  </data>
                                </array>
                              </value>
                            </member>
                          </struct>
                        </value>
                        <value>
                          <struct>
                            <member>
                              <name>Level</name>
                              <value>1-1-2</value>
                            </member>
                            <member>
                              <name>childExamples</name>
                              <value>
                               <array>
                                  <data>
                                  </data>
                                </array>
                              </value>
                            </member>
                          </struct>
                        </value>
                      </data>
                    </array>
                  </value>
                </member>
              </struct>
            </value>
            <value>
              <struct>
                <member>
                  <name>Level</name>
                  <value>1-2</value>
                </member>
                <member>
                  <name>childExamples</name>
                  <value>
                   <array>
                      <data>
                        <value>
                          <struct>
                            <member>
                              <name>Level</name>
                              <value>1-2-1</value>
                            </member>
                            <member>
                              <name>childExamples</name>
                              <value>
                               <array>
                                  <data>
                                  </data>
                                </array>
                              </value>
                            </member>
                          </struct>
                        </value>
                        <value>
                          <struct>
                            <member>
                              <name>Level</name>
                              <value>1-2-2</value>
                            </member>
                            <member>
                              <name>childExamples</name>
                              <value>
                               <array>
                                  <data>
                                  </data>
                                </array>
                              </value>
                            </member>
                          </struct>
                        </value>
                      </data>
                    </array>
                  </value>
                </member>
              </struct>
            </value>
          </data>
        </array>
      </value>
    </member>
  </struct>
</value>";
            var obj = Utils.Parse(
                Xml,
                typeof(RecursiveArrayMember),
                MappingAction.Error,
                out parsedType,
                out parsedArrayType);

            obj.ShouldBeOfType<RecursiveMember>();
            obj.ShouldNotBeNull();
        }
    }
}