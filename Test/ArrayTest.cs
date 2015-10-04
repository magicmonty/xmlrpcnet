using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ArrayTest
    {
        private const string ExpectedJagged = 
@"<value>
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

        private const string ExpectedEmptyArray = 
@"<value>
  <array>
    <data />
  </array>
</value>";

        [Test]
        public void SerializeJagged()
        {
            var jagged = new[] { new int[] { }, new[] { 1 }, new[] { 2, 3 } };

            Utils
                .SerializeValue(jagged, true)
                .ShouldBe(ExpectedJagged);
        }

        [Test]
        public void DeserializeJagged()
        {
            var retVal = Utils.ParseValue(ExpectedJagged, typeof(int[][]));
            retVal.ShouldBeOfType<int[][]>();

            ((int[][])retVal).ShouldBe(new[] { new int[] { }, new[] { 1 }, new[] { 2, 3 } });
        }

        [Test]
        public void SerializeMultiDim()
        {
            var multiDim = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            Utils.SerializeValue(multiDim, true).ShouldBe(ExpectedMultiDim);
        }

        [Test]
        public void DeserializeMultiDim()
        {
            var retVal = Utils.ParseValue(ExpectedMultiDim, typeof(int[,]));

            retVal.ShouldBeOfType<int[,]>();
            ((int[,])retVal).ShouldBe(new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } });
        }

        [Test]
        public void SerializeEmpty()
        {
            var empty = new int[] { };
            Utils.SerializeValue(empty, true).ShouldBe(ExpectedEmptyArray);
        }

        [Test]
        public void DeserializeEmpty()
        {
            var retVal = Utils.ParseValue(ExpectedEmptyArray, typeof(int[]));
            retVal.ShouldBeOfType<int[]>();

            ((int[])retVal).ShouldBeEmpty();
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
            obj.ShouldBeOfType<object[]>();
            obj.ShouldBe(new object[] { 12, "Egypt", false });
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
            obj.ShouldBeOfType<object[]>();
            obj.ShouldBe(new object[] { 12, "Egypt", false });
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
            obj.ShouldBeOfType<object[]>();
            obj.ShouldBe(new object[] { 12, "Egypt", false });
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
            obj.ShouldBeOfType<int[]>();
            obj.ShouldBe(new [] { 12, 13, 14 });
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
            obj.ShouldBeOfType<int[]>();
            obj.ShouldBe(new [] { 12, 13, 14 });
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
            obj.ShouldBeOfType<object[]>();
            obj.ShouldBe(new object[] { 12, 13, 14 });
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
            obj.ShouldBeOfType<int[]>();
            obj.ShouldBe(new [] { 12, 13, 14 });
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
            obj.ShouldBeOfType<object[][]>();

            var ret = (object[][])obj;
            ret.ShouldSatisfyAllConditions(
                () => ret[0][0].ShouldBe(1213028),
                () => ret[0][1].ShouldBe("products"),
                () => ret[1][0].ShouldBe(666));
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
            obj.ShouldBeOfType<object[]>();
            obj.ShouldBe(new object[] { 12, "Egypt", false });
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
            obj.ShouldBeOfType<int[,]>();
            obj.ShouldBe(new[,] { { 1, 2 }, { 3, 4 } });
        }
    }
}