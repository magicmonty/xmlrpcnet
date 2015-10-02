// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayTest.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ntest;
using NUnit.Framework;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ArrayTest
    {
        private const string ExpectedJagged = @"<value>
  <array>
    <data>
      <value>
        <array>
          <data />
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>1</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>2</i4>
            </value>
            <value>
              <i4>3</i4>
            </value>
          </data>
        </array>
      </value>
    </data>
  </array>
</value>";

        private const string ExpectedMultiDim =
@"<value>
  <array>
    <data>
      <value>
        <array>
          <data>
            <value>
              <i4>1</i4>
            </value>
            <value>
              <i4>2</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>3</i4>
            </value>
            <value>
              <i4>4</i4>
            </value>
          </data>
        </array>
      </value>
      <value>
        <array>
          <data>
            <value>
              <i4>5</i4>
            </value>
            <value>
              <i4>6</i4>
            </value>
          </data>
        </array>
      </value>
    </data>
  </array>
</value>";

        private const string ExpectedEmptyArray = @"<value>
  <array>
    <data />
  </array>
</value>";

        [Test]
        public void SerializeJagged()
        {
            var jagged = new[] { new int[] { }, new[] { 1 }, new[] { 2, 3 } };
            var xml = Utils.SerializeValue(jagged, true);
            Assert.AreEqual(ExpectedJagged, xml);
        }

        [Test]
        public void DeserializeJagged()
        {
            var retVal = Utils.ParseValue(ExpectedJagged, typeof(int[][]));
            Assert.IsInstanceOf<int[][]>(retVal);
            var ret = (int[][])retVal;
            Assert.IsTrue(ret[0].Length == 0);
            Assert.IsTrue(ret[1].Length == 1);
            Assert.IsTrue(ret[2].Length == 2);
            Assert.AreEqual(1, ret[1][0]);
            Assert.AreEqual(2, ret[2][0]);
            Assert.AreEqual(3, ret[2][1]);
        }

        [Test]
        public void SerializeMultiDim()
        {
            var multiDim = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var xml = Utils.SerializeValue(multiDim, true);
            Assert.AreEqual(ExpectedMultiDim, xml);
        }

        [Test]
        public void DeserializeMultiDim()
        {
            var retVal = Utils.ParseValue(ExpectedMultiDim, typeof(int[,]));
            Assert.IsInstanceOf<int[,]>(retVal);
            var ret = (int[,])retVal;
            Assert.AreEqual(1, ret[0, 0]);
            Assert.AreEqual(2, ret[0, 1]);
            Assert.AreEqual(3, ret[1, 0]);
            Assert.AreEqual(4, ret[1, 1]);
            Assert.AreEqual(5, ret[2, 0]);
            Assert.AreEqual(6, ret[2, 1]);
        }

        [Test]
        public void SerializeEmpty()
        {
            var empty = new int[] { };
            var xml = Utils.SerializeValue(empty, true);
            Assert.AreEqual(ExpectedEmptyArray, xml);
        }

        [Test]
        public void DeserializeEmpty()
        {
            var retVal = Utils.ParseValue(ExpectedEmptyArray, typeof(int[]));
            Assert.IsInstanceOf<int[]>(retVal);
            var ret = (int[])retVal;
            Assert.IsTrue(ret.Length == 0);
        }

        // ---------------------- array -----------------------------------------// 
        [Test]
        public void MixedArray_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[], "result is array of object");
            var ret = obj as object[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual("Egypt", ret[1]);
            Assert.AreEqual(false, ret[2]);
        }

        [Test]
        public void MixedArray_ObjectArrayType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, typeof(object[]), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[], "result is array of object");
            var ret = obj as object[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual("Egypt", ret[1]);
            Assert.AreEqual(false, ret[2]);
        }

        [Test]
        public void MixedArray_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><string>Egypt</string></value>
      <value><boolean>0</boolean></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[], "result is array of object");
            var ret = obj as object[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual("Egypt", ret[1]);
            Assert.AreEqual(false, ret[2]);
        }

        [Test]
        public void HomogArray_NullType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, null, MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is int[], "result is array of int");
            var ret = obj as int[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual(13, ret[1]);
            Assert.AreEqual(14, ret[2]);
        }

        [Test]
        public void HomogArray_IntArrayType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, typeof(int[]), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is int[], "result is array of int");
            var ret = obj as int[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual(13, ret[1]);
            Assert.AreEqual(14, ret[2]);
        }

        [Test]
        public void HomogArray_ObjectArrayType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, typeof(object[]), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[], "result is array of object");
            var ret = obj as object[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual(13, ret[1]);
            Assert.AreEqual(14, ret[2]);
        }

        [Test]
        public void HomogArray_ObjectType()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
<value>
  <array>
    <data>
      <value><i4>12</i4></value>
      <value><i4>13</i4></value>
      <value><i4>14</i4></value>
    </data>
  </array>
</value>";
            var obj = Utils.Parse(Xml, typeof(object), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is int[], "result is array of int");
            var ret = obj as int[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual(13, ret[1]);
            Assert.AreEqual(14, ret[2]);
        }

        [Test]
        public void JaggedArray()
        {
            Type parsedType, parsedArrayType;
            const string Xml =
@"<?xml version=""1.0"" ?>
 <value>
   <array>
     <data>
       <value>
         <array>
           <data>
             <value>
               <i4>1213028</i4>
             </value>
             <value>
               <string>products</string>
             </value>
           </data>
         </array>
       </value>
       <value>
         <array>
           <data>
             <value>
               <i4>666</i4>
             </value>
           </data>
         </array>
       </value>
     </data>
   </array>
 </value>";
            var obj = Utils.Parse(Xml, typeof(object[][]), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[][]);
            var ret = (object[][])obj;
            Assert.AreEqual(1213028, ret[0][0]);
            Assert.AreEqual("products", ret[0][1]);
            Assert.AreEqual(666, ret[1][0]);
        }

        // ---------------------- array -----------------------------------------// 
        [Test]
        public void Array()
        {
            var testary = new object[] { 12, "Egypt", false };
            var xdoc = Utils.Serialize(
                "SerializeTest.testArray",
                testary,
                Encoding.UTF8,
                new MappingActions { NullMappingAction = NullMappingAction.Ignore });
            Type parsedType, parsedArrayType;
            var obj = Utils.Parse(xdoc, null, MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is object[], "result is array of object");
            var ret = obj as object[];
            Assert.AreEqual(12, ret[0]);
            Assert.AreEqual("Egypt", ret[1]);
            Assert.AreEqual(false, ret[2]);
        }

        // ---------------------- array -----------------------------------------// 
        [Test]
        public void MultiDimArray()
        {
            var myArray = new[,] { { 1, 2 }, { 3, 4 } };
            var xdoc = Utils.Serialize(
                "SerializeTest.testMultiDimArray",
                myArray,
                Encoding.UTF8,
                new MappingActions { NullMappingAction = NullMappingAction.Ignore });
            Type parsedType, parsedArrayType;
            var obj = Utils.Parse(xdoc, typeof(int[,]), MappingAction.Error, out parsedType, out parsedArrayType);
            Assert.IsTrue(obj is int[,], "result is 2 dim array of int");
            var ret = obj as int[,];
            Assert.AreEqual(1, ret[0, 0]);
            Assert.AreEqual(2, ret[0, 1]);
            Assert.AreEqual(3, ret[1, 0]);
            Assert.AreEqual(4, ret[1, 1]);
        }
    }
}