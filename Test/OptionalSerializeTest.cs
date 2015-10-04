using System;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Shouldly;

namespace CookComputing.XmlRpc
{
    [TestFixture]
    public class OptionalSerializeTest
    {
        public struct ChildStruct
        {
            public int x;

            public ChildStruct(int num)
            {
                x = num;
            }
        }

        public struct Struct0
        {
            public int? xi;

            public bool? xb;

            public double? xd;

            public DateTime? xdt;

            public int? nxi;

            public bool? nxb;

            public double? nxd;

            public DateTime? nxdt;

            public ChildStruct? nxstr;
        }

        public struct Struct1
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

            public int? nxi;

            public bool? nxb;

            public double? nxd;

            public DateTime? nxdt;

            public ChildStruct? nxstr;
        }

        [XmlRpcMissingMapping(MappingAction.Error)]
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

            public int? nxi;

            public bool? nxb;

            public double? nxd;

            public DateTime? nxdt;

            public ChildStruct? nxstr;
        }

        public struct Struct3
        {
            [XmlRpcMissingMapping(MappingAction.Error)]
            public int mi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public string ms;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool mb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double md;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime mdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public byte[] mb64;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int[] ma;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int? xi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool? xb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double? xd;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime? xdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public XmlRpcStruct xstr;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int? nxi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool? nxb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double? nxd;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime? nxdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public ChildStruct? nxstr;
        }

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct Struct4
        {
            [XmlRpcMissingMapping(MappingAction.Error)]
            public int mi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public string ms;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool mb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double md;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime mdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public byte[] mb64;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int[] ma;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int? xi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool? xb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double? xd;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime? xdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public XmlRpcStruct xstr;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public int? nxi;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public bool? nxb;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public double? nxd;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public DateTime? nxdt;

            [XmlRpcMissingMapping(MappingAction.Error)]
            public ChildStruct? nxstr;
        }

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public struct Struct5
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

            public int? nxi;

            public bool? nxb;

            public double? nxd;

            public DateTime? nxdt;

            public ChildStruct? nxstr;
        }

        public struct Struct6
        {
            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int mi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public string ms;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool mb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double md;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime mdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public byte[] mb64;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int[] ma;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int? xi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool? xb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double? xd;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime? xdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public XmlRpcStruct xstr;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int? nxi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool? nxb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double? nxd;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime? nxdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public ChildStruct? nxstr;
        }

        [XmlRpcMissingMapping(MappingAction.Error)]
        public struct Struct7
        {
            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int mi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public string ms;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool mb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double md;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime mdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public byte[] mb64;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int[] ma;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int? xi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool? xb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double? xd;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime? xdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public XmlRpcStruct xstr;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public int? nxi;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public bool? nxb;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public double? nxd;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public DateTime? nxdt;

            [XmlRpcMissingMapping(MappingAction.Ignore)]
            public ChildStruct? nxstr;
        }

        // -------------------------------------------------------------------------/
        [Test]
        public void Struct0_AllExist()
        {
            var strout = new Struct0();
            strout.xi = 1234;
            strout.xb = true;
            strout.xd = 1234.567;
            strout.xdt = new DateTime(2006, 8, 9, 10, 11, 13);
            strout.nxi = 5678;
            strout.nxb = true;
            strout.nxd = 2345.678;
            strout.nxdt = new DateTime(2007, 9, 10, 11, 12, 14);
            strout.nxstr = new ChildStruct(567);

            var xdoc = Utils.Serialize(
                "Struct0_AllExist",
                strout,
                Encoding.UTF8,
                new MappingActions { NullMappingAction = NullMappingAction.Error });
            
            Type parsedType, parsedArrayType;
            var obj = Utils.Parse(xdoc, typeof(Struct0), MappingAction.Error, out parsedType, out parsedArrayType);
            obj.ShouldBeOfType<Struct0>();
            var strin = (Struct0)obj;
            strout.xi.ShouldBe(strin.xi);
            strout.xb.ShouldBe(strin.xb);
            strout.xd.ShouldBe(strin.xd);
            strout.xdt.ShouldBe(strin.xdt);
            strout.nxi.ShouldBe(strin.nxi);
            strout.nxb.ShouldBe(strin.nxb);
            strout.nxd.ShouldBe(strin.nxd);
            strout.nxdt.ShouldBe(strin.nxdt);
            ((ChildStruct)strout.nxstr).x.ShouldBe(((ChildStruct)strin.nxstr).x);
        }

        [Test]
        public void Struct1_AllMissing_ErrorDefault()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct1_AllMissing_ErrorDefault",
                    new Struct1(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct1_AllMissing_IgnoreDefault()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct1_AllMissing_IgnoreDefault",
                    new Struct1(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct2_AllMissing_ErrorError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct2_AllMissing_ErrorError",
                    new Struct2(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct2_AllMissing_IgnoreError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct2_AllMissing_IgnoreError",
                    new Struct2(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct3_AllMissing_ErrorDefaultError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct3_AllMissing_ErrorDefaultError",
                    new Struct3(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct3_AllMissing_IgnoreDefaultError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct3_AllMissing_IgnoreDefaultError",
                    new Struct3(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct4_AllMissing_ErrorIgnoreError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct4_AllMissing_ErrorIgnoreError",
                    new Struct4(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct4_AllMissing_IgnoreIgnoreError()
        {
            Should.Throw<XmlRpcMappingSerializeException>(() => 
                Utils.Serialize(
                    "Struct4_AllMissing_IgnoreIgnoreError",
                    new Struct4(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct5_AllMissing_ErrorIgnoreDefault()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct5_AllMissing_ErrorIgnoreDefault",
                    new Struct5(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct5_AllMissing_IgnoreIgnoreDefault()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct5_AllMissing_IgnoreIgnoreDefault",
                    new Struct5(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct6_AllMissing_ErrorDefaultIgnore()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct6_AllMissing_ErrorDefaultIgnore",
                    new Struct6(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct6_AllMissing_IgnoreDefaultIgnore()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct6_AllMissing_IgnoreDefaultIgnore",
                    new Struct6(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Ignore }));
        }

        [Test]
        public void Struct7_AllMissing_ErrorErrorIgnore()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct7_AllMissing_ErrorErrorIgnore",
                    new Struct7(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }

        [Test]
        public void Struct7_AllMissing_IgnoreErrorIgnore()
        {
            Should.NotThrow(() => 
                Utils.Serialize(
                    "Struct7_AllMissing_IgnoreErrorIgnore",
                    new Struct7(),
                    Encoding.UTF8,
                    new MappingActions { NullMappingAction = NullMappingAction.Error }));
        }
    }
}