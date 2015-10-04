using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DeserializeResponseTests
    {
        // test return integer
        [Test]
        public void I4NullType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        public void I4WithType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(int));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        public void IntegerNullType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        public void IntegerWithType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(int));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        public void IntegerIncorrectType()
        {
            try
            {
                const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><int>12345</int></value>
    </param>
  </params>
</methodResponse>";

                var sr = new StringReader(Xml);
                var deserializer = new XmlRpcResponseDeserializer();
                deserializer.DeserializeResponse(sr, typeof(string));
                Assert.Fail("Should throw XmlRpcTypeMismatchException");
            }
            catch (XmlRpcTypeMismatchException)
            {
            }
        }

        [Test]
        public void StringNullType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is string, "retval is string");
            Assert.AreEqual((string)o, "test string", "retval is 'test string'");
        }

        [Test]
        public void String2NullType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is string, "retval is string");
            Assert.AreEqual((string)o, "test string", "retval is 'test string'");
        }

        [Test]
        public void String1WithType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><string>test string</string></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(string));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is string, "retval is string");
            Assert.AreEqual((string)o, "test string", "retval is 'test string'");
        }

        [Test]
        public void String2WithType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(string));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is string, "retval is string");
            Assert.AreEqual((string)o, "test string", "retval is 'test string'");
        }

        [Test]
        public void String1IncorrectType()
        {
            try
            {
                const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value>test string</value>
    </param>
  </params>
</methodResponse>";

                var sr = new StringReader(Xml);
                var deserializer = new XmlRpcResponseDeserializer();
                deserializer.DeserializeResponse(sr, typeof(int));
                Assert.Fail("Should throw XmlRpcTypeMismatchException");
            }
            catch (XmlRpcTypeMismatchException)
            {
            }
        }

        [Test]
        public void StringEmptyValue()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(string));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is string, "retval is string");
            Assert.AreEqual((string)o, string.Empty, "retval is empty string");
        }



        [Test]
        public void MinDateTime1NotStrict()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var response = deserializer.DeserializeResponse(sr, typeof(DateTime));
            var o = response.RetVal;
            Assert.IsTrue(o is DateTime, "retval is string");
            Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
        }

        [Test]
        public void MinDateTime2NotStrict()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00Z</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var response = deserializer.DeserializeResponse(sr, typeof(DateTime));

            var o = response.RetVal;
            Assert.IsTrue(o is DateTime, "retval is string");
            Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
        }

        [Test]
        public void MinDateTime3NotStrict()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>0000-00-00T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var response = deserializer.DeserializeResponse(sr, typeof(DateTime));

            var o = response.RetVal;
            Assert.IsTrue(o is DateTime, "retval is string");
            Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
        }

        [Test]
        public void MinDateTime4NotStrict()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>0000-00-00T00:00:00Z</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.MapZerosDateTimeToMinValue;
            var response = deserializer.DeserializeResponse(sr, typeof(DateTime));

            var o = response.RetVal;
            Assert.IsTrue(o is DateTime, "retval is string");
            Assert.AreEqual((DateTime)o, DateTime.MinValue, "DateTime.MinValue");
        }

        [Test]
        public void MinDateTimeStrict()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><dateTime.iso8601>00000000T00:00:00</dateTime.iso8601></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer
            {
                NonStandard = XmlRpcNonStandard.AllowNonStandardDateTime
            };
            try
            {
                deserializer.DeserializeResponse(sr, typeof(DateTime));
                Assert.Fail("dateTime 00000000T00:00:00 invalid when strict");
            }
            catch (XmlRpcInvalidXmlRpcException)
            {
            }
        }

        [Test]
        public void ReturnStructAsObject()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
          <member>
            <name>key4</name>
            <value>
              <string>this is a test 2</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(object));

            var o = response.RetVal;
            var ret = (string)((XmlRpcStruct)o)["key3"];
            Assert.AreEqual("this is a test", ret);
            var ret2 = (string)((XmlRpcStruct)o)["key4"];
            Assert.AreEqual("this is a test 2", ret2);
        }

        [Test]
        public void ReturnStructAsXmlRpcStruct()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(XmlRpcStruct));

            var o = response.RetVal;
            var ret = (string)((XmlRpcStruct)o)["key3"];
            Assert.AreEqual("this is a test", ret);
        }



        [Test]
        public void ArrayInStruct()
        {
            // reproduce problem reported by Alexander Agustsson
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>key3</name>
            <value>
              <array>
                <data>
                  <value>New Milk</value>
                  <value>Old Milk</value>
                </data>
              </array>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
            Assert.IsTrue(o is XmlRpcStruct, "retval is XmlRpcStruct");
            var xrs = (XmlRpcStruct)o;
            Assert.IsTrue(xrs.Count == 1, "retval contains one entry");
            var elem = xrs["key3"];
            Assert.IsTrue(elem != null, "element has correct key");
            Assert.IsTrue(elem is Array, "element is an array");
            var array = (object[])elem;
            Assert.IsTrue(array.Length == 2, "array has 2 members");
            var s = array[0] as string;
            Assert.IsTrue(
                s != null
                && s == "New Milk"
                && array[1] is string
                && (string)array[1] == "Old Milk",
              "values of array members");
        }


        [Test]
        public void StringAndStructInArray()
        {
            // reproduce problem reported by Eric Brittain
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <array>
          <data>
            <value>
              <string>test string</string>
            </value>
            <value>
              <struct>
                <member>
                  <name>fred</name>
                  <value><string>test string 2</string></value>
                </member>
              </struct>
            </value>
          </data>
        </array>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);
            var o = response.RetVal;
        }

        public struct InternalStruct
        {
            public string FirstName;
            public string LastName;
        }

        public struct MyStruct
        {
            public string Version;
            public InternalStruct Record;
        }

        [Test]
        public void ReturnNestedStruct()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>Version</name>
            <value><string>1.6</string></value>
          </member>
          <member>
            <name>Record</name>
            <value>
              <struct>
                <member>
                  <name>FirstName</name>
                  <value>Joe</value></member>
                <member>
                  <name>LastName</name>
                  <value>Test</value>
                </member>
              </struct>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(MyStruct));

            var o = response.RetVal;
            Assert.IsTrue(o is MyStruct, "retval is MyStruct");
            var mystr = (MyStruct)o;
            Assert.AreEqual(mystr.Version, "1.6", "version is 1.6");
            Assert.IsTrue(mystr.Record.FirstName == "Joe", "firstname is Joe");
            Assert.IsTrue(mystr.Record.LastName == "Test", "lastname is Test");
        }

        [Test]
        public void JoseProblem()
        {
            const string Xml =
@"<?xml version='1.0'?> 
<methodResponse> 
<params> 
<param> 
<value><int>12</int></value> 
</param> 
</params> 

</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(int));

            var o = response.RetVal;
            Assert.IsTrue(o is int, "retval is int");
            var myint = (int)o;
            Assert.AreEqual(myint, 12, "int is 12");
        }

        public struct BillStruct
        {
            public int x;
            public string s;
        }

        [Test]
        public void MissingStructMember()
        {
            const string Xml =
@"<?xml version='1.0'?> 
<methodResponse> 
  <params> 
    <param> 
      <value>
        <struct>
          <member>
            <name>x</name>
            <value>
              <i4>123</i4>
            </value>
          </member>
        </struct>
      </value> 
    </param> 
  </params> 
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                deserializer.DeserializeResponse(sr, typeof(BillStruct));
                Assert.Fail("Should detect missing struct member");
            }
            catch (AssertionException)
            {
                throw;
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void BillKeenanProblem()
        {
            const string Xml =
@"<?xml version='1.0'?> 
<methodResponse> 
  <params> 
    <param> 
      <value>
        <struct>
          <member>
            <name>x</name>
            <value>
              <i4>123</i4>
            </value>
          </member>
          <member>
            <name>s</name>
            <value>
              <string>ABD~~DEF</string>
            </value>
          </member>
          <member>
            <name>unexpected</name>
            <value>
              <string>this is unexpected</string>
            </value>
          </member>
        </struct>
      </value> 
    </param> 
  </params> 
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(BillStruct));

            var o = response.RetVal;
            Assert.IsTrue(o is BillStruct, "retval is BillStruct");
            var bs = (BillStruct)o;
            Assert.IsTrue(bs.x == 123 && bs.s == "ABD~~DEF", "struct members");
        }

        [Test]
        public void AdvogatoProblem()
        {
            const string Xml =
@"<?xml version='1.0'?> 
<methodResponse>
<params>
<param>
<array>
<data>
<value>
<dateTime.iso8601>20020707T11:25:37</dateTime.iso8601>
</value>
<value>
<dateTime.iso8601>20020707T11:37:12</dateTime.iso8601>
</value>
</data>
</array>
</param>
</params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                var response = deserializer.DeserializeResponse(sr, null);
                var o = response.RetVal;
                Assert.Fail("should have thrown XmlRpcInvalidXmlRpcException");
            }
            catch (XmlRpcInvalidXmlRpcException)
            {
            }
        }

        [Test]
        public void VoidReturnType()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value></value>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(void));
            Assert.IsTrue(response.RetVal == null, "retval is null");
        }

        [Test]
        public void EmptyValueReturn()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value/>
    </param>
  </params>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(string));
            var s = (string)response.RetVal;
            Assert.IsTrue(s == string.Empty, "retval is empty string");
        }

        [Test]
        public void ISO_8869_1()
        {
            using (var stm = new FileStream("testdocuments/iso-8859-1_response.xml", FileMode.Open, FileAccess.Read))
            {
                var deserializer = new XmlRpcResponseDeserializer();
                var response = deserializer.DeserializeResponse(stm, typeof(string));
                var ret = (string)response.RetVal;
                Assert.IsTrue(ret == "hæ hvað segirðu þá", "retVal is 'hæ hvað segirðu þá'");
            }
        }

        [Test]
        public void FaultResponse()
        {
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>FaultCode</name>
          <value><int>4</int></value>
        </member>
        <member>
          <name>FaultString</name>
          <value><string>Too many parameters.</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                deserializer.DeserializeResponse(sr, typeof(void));
            }
            catch (XmlRpcFaultException fex)
            {
                Assert.AreEqual(fex.FaultCode, 4);
                Assert.AreEqual(fex.FaultString, "Too many parameters.");
            }
        }

        [Test]
        public void FaultStringCode()
        {
            // Alex Hung reported that some servers, e.g. WordPress, return fault code
            // as a string
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>FaultCode</name>
          <value><string>4</string></value>
        </member>
        <member>
          <name>FaultString</name>
          <value><string>Too many parameters.</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                deserializer.DeserializeResponse(sr, typeof(void));
                Assert.Fail("Expected fault exception to be thrown");
            }
            catch (XmlRpcFaultException fex)
            {
                Assert.AreEqual(fex.FaultCode, 4);
                Assert.AreEqual(fex.FaultString, "Too many parameters.");
            }
        }

        [Test]
        public void FaultStringCodeWithAllowStringFaultCode()
        {
            // Alex Hung reported that some servers, e.g. WordPress, return fault code
            // as a string
            const string Xml =
@"<?xml version=""1.0"" ?> 
<methodResponse>
  <fault>
    <value>
      <struct>
        <member>
          <name>FaultCode</name>
          <value><string>4</string></value>
        </member>
        <member>
          <name>FaultString</name>
          <value><string>Too many parameters.</string></value>
        </member>
      </struct>
    </value>
  </fault>
</methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer { NonStandard = XmlRpcNonStandard.AllowStringFaultCode };
            try
            {
                deserializer.DeserializeResponse(sr, typeof(void));
                Assert.Fail("Expected fault exception to be thrown");
            }
            catch (XmlRpcFaultException fex)
            {
                Assert.AreEqual(fex.FaultCode, 4);
                Assert.AreEqual(fex.FaultString, "Too many parameters.");
            }
        }

        [Test]
        public void Yolanda()
        {
            const string Xml = @"<?xml version=""1.0"" encoding=""ISO-8859-1""?><methodResponse><params><param><value><array><data><value>addressbook</value><value>system</value></data></array></value></param></params></methodResponse>";
            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, null);

            var o = response.RetVal;
        }

        [Test]
        public void Gabe()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""UTF-8""?><methodResponse><params><param><value><struct><member><name>response</name><value><struct><member><name>result</name><value><array><data><value><struct><member><name>state</name><value><string>CO</string></value></member><member><name>latitude</name><value><double>39.74147878</double></value></member><member><name>add1</name><value><string>110 16th St.</string></value></member><member><name>add2</name><value><string /></value></member><member><name>image_map</name><value><array><data><value><string>rect</string></value><value><int>290</int></value><value><int>190</int></value><value><int>309</int></value><value><int>209</int></value></data></array></value></member><member><name>city</name><value><string>Denver</string></value></member><member><name>fax</name><value><string>303-623-1111</string></value></member><member><name>name</name><value><boolean>0"
+ "</boolean></value></member><member><name>longitude</name><value><double>-104.9874159</double></value></member><member><name>georesult</name><value><string>10 W2GIADDRESS</string></value></member><member><name>zip</name><value><string>80202</string></value></member><member><name>hours</name><value><string>Mon-Sun 10am-6pm</string></value></member><member><name>dealerid</name><value><string>545</string></value></member><member><name>phone</name><value><string>303-623-5050</string></value></member></struct></value></data></array></value></member><member><name>map_id</name><value><string>a5955239d080dfbb7002fd063aa7b47e0d</string></value></member><member><name>map</name><value><struct><member><name>zoom_level</name><value><int>3</int></value></member><member><name>image_type</name><value><string>image/png</string></value></member><member><name>miles</name><value><double>1.75181004463519</double></value></member><member><name>kilometers</name><value><double>2.81926498447338"
+ "</double></value></member><member><name>scalebar</name><value><int>1</int></value></member><member><name>content</name><value><string>http://mapserv.where2getit.net/maptools/mapserv.cgi/a5955239d080dfbb7002fd063aa7b47e0d.png</string></value></member><member><name>scale</name><value><int>26000</int></value></member><member><name>map_style</name><value><string>default</string></value></member><member><name>size</name><value><array><data><value><int>600</int></value><value><int>400</int></value></data></array></value></member><member><name>content_type</name><value><string>text/uri-list</string></value></member><member><name>buffer</name><value><double>0.01</double></value></member><member><name>center</name><value><struct><member><name>georesult</name><value><string>AUTOBBOX</string></value></member><member><name>latitude</name><value><double>39.74147878</double></value></member><member><name>longitude</name><value><double>-104.9874159</double></value></member></struct></value></member></struct></value></member><member><name>result_count</name><value><int>1</int></value></member><member><name>image_map</name><value><boolean>1</boolean></value></member><member><name>result_total_count</name><value><int>1</int></value></member></struct></value></member><member><name>times</name><value><struct><member><name>csys</name><value><int>0</int></value></member><member><name>cusr</name><value><int>0</int></value></member><member><name>sys</name><value><int>0</int></value></member><member><name>usr</name><value><double>0.0200000000000005"
+ "</double></value></member><member><name>wallclock</name><value><double>2.547471</double></value></member></struct></value></member><member><name>request</name><value><struct><member><name>state</name><value><string>CO</string></value></member><member><name>%sort</name><value><array><data /></array></value></member><member><name>%id</name><value><string>4669b341d87be7f450b4bf0dc4cd0a1e</string></value></member><member><name>city</name><value><string>denver</string></value></member><member><name>%limit</name><value><int>10</int></value></member><member><name>%offset</name><value><int>0</int></value></member></struct></value></member></struct></value></param></params></methodResponse>";

            var sr = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr, typeof(XmlRpcStruct));

            var response_struct = (XmlRpcStruct)response.RetVal;
            var _response = (XmlRpcStruct)response_struct["response"];
            var results = (Array)_response["result"];
            Assert.AreEqual(results.Length, 1);
        }

        public struct DupMem
        {
            public string foo;
        }

        [Test]
        public void StructDuplicateMember()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>foo</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
          <member>
            <name>foo</name>
            <value>
              <string>duplicate this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                deserializer.DeserializeResponse(sr1, typeof(DupMem));
                Assert.Fail("Ignored duplicate member");
            }
            catch (XmlRpcInvalidXmlRpcException)
            {
            }

            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var sr2 = new StringReader(Xml);
            var response2 = deserializer.DeserializeResponse(sr2, typeof(DupMem));
            var dupMem = (DupMem)response2.RetVal;
            Assert.AreEqual("this is a test", dupMem.foo);
        }

        [Test]
        public void XmlRpcStructDuplicateMember()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""ISO-8859-1""?> 
<methodResponse>
  <params>
    <param>
      <value>
        <struct>
          <member>
            <name>foo</name>
            <value>
              <string>this is a test</string>
            </value>
          </member>
          <member>
            <name>foo</name>
            <value>
              <string>duplicate this is a test</string>
            </value>
          </member>
        </struct>
      </value>
    </param>
  </params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            try
            {
                deserializer.DeserializeResponse(sr1, typeof(XmlRpcStruct));
                Assert.Fail("Ignored duplicate member");
            }
            catch (XmlRpcInvalidXmlRpcException)
            {

            }

            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var sr2 = new StringReader(Xml);
            var response2 = deserializer.DeserializeResponse(sr2, typeof(XmlRpcStruct));
            var dupMem = (XmlRpcStruct)response2.RetVal;
            Assert.IsTrue((string)dupMem["foo"] == "this is a test");
        }

        [Test]
        [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
        public void InvalidHTTPContentLeadingWhiteSpace()
        {
            const string Xml = @"
 
   
<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.DeserializeResponse(stm, typeof(int));
        }

        [Test]
        public void AllowInvalidHTTPContentLeadingWhiteSpace()
        {
            const string Xml =
@"
 
   
<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
            var response = deserializer.DeserializeResponse(stm, typeof(int));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        public void AllowInvalidHTTPContentTrailingWhiteSpace()
        {
            const string Xml =
@"


<?xml version=""1.0"" ?> 
<methodResponse>
  <params>
    <param>
      <value><i4>12345</i4></value>
    </param>
  </params>
</methodResponse>";

            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;

            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
            var response = deserializer.DeserializeResponse(stm, typeof(int));

            var o = response.RetVal;
            Assert.IsTrue(o != null, "retval not null");
            Assert.IsTrue(o is int, "retval is int");
            Assert.AreEqual((int)o, 12345, "retval is 12345");
        }

        [Test]
        [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
        public void InvalidXML()
        {
            const string Xml = @"response>";
            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.DeserializeResponse(stm, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
        public void InvalidXMLWithAllowInvalidHTTPContent()
        {
            const string Xml = @"response>";
            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
            deserializer.DeserializeResponse(stm, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
        public void OneByteContentAllowInvalidHTTPContent()
        {
            const string Xml = @"<";
            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
            deserializer.DeserializeResponse(stm, typeof(int));
        }

        [Test]
        [ExpectedException(typeof(XmlRpcIllFormedXmlException))]
        public void ZeroByteContentAllowInvalidHTTPContent()
        {
            const string Xml = @"";
            Stream stm = new MemoryStream();
            var wrtr = new StreamWriter(stm, Encoding.ASCII);
            wrtr.Write(Xml);
            wrtr.Flush();
            stm.Position = 0;
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.AllowInvalidHTTPContent;
            deserializer.DeserializeResponse(stm, typeof(int));
        }

        [Test]
        public void Donhrobjartz_XmlRpcStructNonMemberStructChild()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<foo>
This should be ignored.
</foo>
<member>
<name>period</name>
<value><string>1w</string></value>
</member>
<bar>
This should be ignored.
</bar>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(XmlRpcStruct));
            var ret = (XmlRpcStruct)response.RetVal;
            Assert.AreEqual(ret.Count, 1);
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
        public void Donhrobjartz_XmlRpcStructMemberDupName()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<name>price</name>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(XmlRpcStruct));
            var ret = (XmlRpcStruct)response.RetVal;
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
        public void Donhrobjartz_XmlRpcStructMemberDupValue()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<value><string>284</string></value>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(XmlRpcStruct));
            var ret = (XmlRpcStruct)response.RetVal;
        }

        public struct Donhrobjartz
        {
            public string period;
        }

        [Test]
        public void Donhrobjartz_StructNonMemberStructChild()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<foo>
This should be ignored.
</foo>
<member>
<name>period</name>
<value><string>1w</string></value>
</member>
<bar>
This should be ignored.
</bar>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(Donhrobjartz));
            var ret = (Donhrobjartz)response.RetVal;
            Assert.AreEqual("1w", ret.period);
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
        public void Donhrobjartz_StructMemberDupName()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<name>price</name>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(Donhrobjartz));
            var ret = (Donhrobjartz)response.RetVal;
        }

        [Test]
        [ExpectedException(typeof(XmlRpcInvalidXmlRpcException))]
        public void Donhrobjartz_StructMemberDupValue()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
<methodResponse>
<params>
<param>
<value>
<struct>
<member>
<name>period</name>
<value><string>1w</string></value>
<value><string>284</string></value>
</member>
</struct>
</value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            var response = deserializer.DeserializeResponse(sr1, typeof(Donhrobjartz));
            var ret = (Donhrobjartz)response.RetVal;
        }

        public struct Category
        {
            public string Title;
            public int id;
        }

        [Test]
        [ExpectedException(typeof(XmlRpcTypeMismatchException))]
        public void StructContainingArrayError()
        {
            const string Xml =
@"<?xml version=""1.0"" encoding=""iso-8859-1""?>
 <methodResponse>
 <params>
 <param>
 <value>
 <struct>
 <member>
 <name>Categories</name>
 <value>
 <array>
 <data>
 <value>
 <struct>
 <member>
 <name>id</name>
 <value>
 <int>0</int>
 </value>
 </member>
 <member>
 <name>Title</name>
 <value>
 <string>Other</string>
 </value>
 </member>
 </struct>
 </value>
 <value>
 <struct>
 <member>
 <name>id</name>
 <value>
 <int>41</int>
 </value>
 </member>
 <member>
 <name>Title</name>
 <value>
 <string>Airplanes</string>
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
 </param>
 </params>
 </methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            deserializer.NonStandard = XmlRpcNonStandard.IgnoreDuplicateMembers;
            deserializer.DeserializeResponse(sr1, typeof(Category[]));
        }

        [Test]
        public void EmptyParamsVoidReturn()
        {
            const string Xml =
@"<?xml version=""1.0""?>
<methodResponse>
  <params/>
</methodResponse>
";
            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr1, typeof(void));
            Assert.IsNull(response.RetVal);
        }

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct upcLookupValues
        {
            public string upc;
            public int pendingUpdates;
            public bool isCoupon;
            public string ean;
            public string issuerCountryCode;
            public string mfr_comment;
            public string mfr;
            public string description;
            public bool found;
            public string size;
            public string message;
            public string issuerCountry;
            public DateTime lastModifiedUTC;
        }

        [Test]
        public void UpcResponse()
        {
            const string Xml =
@"<?xml version=""1.0""?>
<methodResponse>
<params>
<param><value><struct>
<member><name>upc</name><value><string>639382000393</string></value>
</member>
<member><name>pendingUpdates</name><value><int>0</int></value>
</member>
<member><name>status</name><value><string>success</string></value>
</member>
<member><name>ean</name><value><string>0639382000393</string></value>
</member>
<member><name>issuerCountryCode</name><value><string>us</string></value>
</member>
<member><name>found</name><value><boolean>1</boolean></value>
</member>
<member><name>description</name><value><string>The Teenager's Guide to the Real World by BYG Publishing</string></value>
</member>
<member><name>message</name><value><string>Database entry found</string></value>
</member>
<member><name>size</name><value><string>book</string></value>
</member>
<member><name>issuerCountry</name><value><string>United States</string></value>
</member>
<member><name>noCacheAfterUTC</name><value><dateTime.iso8601>2011-03-18T13:26:28</dateTime.iso8601></value>
</member>
<member><name>lastModifiedUTC</name><value><dateTime.iso8601>2002-08-23T23:07:36</dateTime.iso8601></value>
</member>
</struct></value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr1, typeof(upcLookupValues));
            Assert.IsTrue(response.RetVal is upcLookupValues);
            var ret = (upcLookupValues)response.RetVal;
            Assert.AreEqual("639382000393", ret.upc);
            Assert.AreEqual("0639382000393", ret.ean);
            Assert.AreEqual("us", ret.issuerCountryCode);
            Assert.AreEqual(null, ret.mfr_comment);
            Assert.AreEqual(null, ret.mfr);
            Assert.AreEqual("The Teenager's Guide to the Real World by BYG Publishing", ret.description);
            Assert.AreEqual(true, ret.found);
            Assert.AreEqual("book", ret.size);
            Assert.AreEqual("Database entry found", ret.message);
            Assert.AreEqual("United States", ret.issuerCountry);
            Assert.AreEqual(new DateTime(2002, 08, 23, 23, 07, 36), ret.lastModifiedUTC);

        }

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct upcsearchresults
        {
            public int result_count;
            public string search;
            public DateTime noCacheAfterUTC;
            public string status;
            //public XmlRpcStruct[] results;
            public int? max_results;
            public string message;
        }

        [Test]
        public void UpcSearchResponse()
        {
            const string Xml =
@"<?xml version=""1.0""?>
<methodResponse>
<params>
<param><value><struct>
<member><name>results</name><value><array><data>
<value><struct>
<member><name>ean</name><value><string>9338671003008</string></value>
</member>
<member><name>description</name><value><string>Driving Licence Success Theory Test Practical Test</string></value>
</member>
<member><name>size</name><value><string></string></value>
</member>
</struct></value>
<value><struct>
<member><name>ean</name><value><string>0084942384008</string></value>
</member>
<member><name>description</name><value><string>Mardel Master Test Kit</string></value>
</member>
<member><name>size</name><value><string>50 Test Srips</string></value>
</member>
</struct></value>
<value><struct>
<member><name>ean</name><value><string>0978087447523</string></value>
</member>
<member><name>description</name><value><string>LOOK INSIDE THE SAT I TEST PREP FROM THE TEST MAKERS VHS</string></value>
</member>
<member><name>size</name><value><string></string></value>
</member>
</struct></value>
<value><struct>
<member><name>ean</name><value><string>4719869700261</string></value>
</member>
<member><name>description</name><value><string>AIDS TEST QUICKPAC ONE STEP</string></value>
</member>
<member><name>size</name><value><string>HIV 1+2 Test</string></value>
</member>
</struct></value>
<value><struct>
<member><name>ean</name><value><string>5050582453249</string></value>
</member>
<member><name>description</name><value><string>DVD &quot;The big entertainment test, test the nation&quot;</string></value>
</member>
<member><name>size</name><value><string>Regular DVD box</string></value>
</member>
</struct></value>
</data></array></value>
</member>
<member><name>max_results</name><value><int>5</int></value>
</member>
<member><name>message</name><value><string>Search successful</string></value>
</member>
</struct></value>
</param>
</params>
</methodResponse>";

            var sr1 = new StringReader(Xml);
            var deserializer = new XmlRpcResponseDeserializer();
            var response = deserializer.DeserializeResponse(sr1, typeof(upcsearchresults));
            Assert.IsTrue(response.RetVal is upcsearchresults);
            var ret = (upcsearchresults)response.RetVal;
            Assert.AreEqual(5, ret.max_results);
            Assert.AreEqual("Search successful", ret.message);
        }
    }
}