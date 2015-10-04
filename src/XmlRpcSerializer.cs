// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlRpcSerializer.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.Contracts;
using System.Linq;

namespace CookComputing.XmlRpc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    public class XmlRpcSerializer
    {
        protected XmlRpcFormatSettings XmlRpcFormatSettings { get; private set; }

        public XmlRpcSerializer() : this(new XmlRpcFormatSettings())
        {
        }

        public XmlRpcSerializer(XmlRpcFormatSettings settings)
        {
            XmlRpcFormatSettings = settings;
        }

        public int Indentation {
            get { return XmlRpcFormatSettings.Indentation; }
            set { XmlRpcFormatSettings.Indentation = value; }
        }

        public bool UseEmptyElementTags {
            get { return XmlRpcFormatSettings.UseEmptyElementTags; }
            set { XmlRpcFormatSettings.UseEmptyElementTags = value; }
        }

        public bool UseEmptyParamsTag {
            get { return XmlRpcFormatSettings.UseEmptyParamsTag; }
            set { XmlRpcFormatSettings.UseEmptyParamsTag = value; }
        }

        public bool UseIndentation {
            get { return XmlRpcFormatSettings.UseIndentation; }
            set { XmlRpcFormatSettings.UseIndentation = value; }
        }

        public bool UseIntTag {
            get { return XmlRpcFormatSettings.UseIntTag; }
            set { XmlRpcFormatSettings.UseIntTag = value; }
        }

        public bool UseStringTag {
            get { return XmlRpcFormatSettings.UseStringTag; }
            set { XmlRpcFormatSettings.UseStringTag = value; }
        }

        public Encoding XmlEncoding {
            get { return XmlRpcFormatSettings.XmlEncoding; }
            set { XmlRpcFormatSettings.XmlEncoding = value; }
        }

        public void Serialize(XmlWriter xtw, object o, MappingActions mappingActions)
        {
            Serialize(xtw, o, mappingActions, new List<object>());
        }

        public void Serialize(XmlWriter xtw, object o, MappingActions mappingActions, List<object> nestedObjs)
        {
            if (nestedObjs.Contains(o))
            {
                throw new XmlRpcUnsupportedTypeException(
                    nestedObjs[0].GetType(),
                    "Cannot serialize recursive data structure");
            }

            nestedObjs.Add(o);
            try
            {
                xtw.WriteStartElement(string.Empty, "value", string.Empty);

                var xmlRpcType = XmlRpcTypeInfo.GetXmlRpcType(o);
                switch (xmlRpcType)
                {
                    case XmlRpcType.tArray:
                        SerializeArray(xtw, o, mappingActions, nestedObjs);
                        break;

                    case XmlRpcType.tMultiDimArray:
                        SerializeMultiDimensionalArray(xtw, o, mappingActions, nestedObjs);
                        break;

                    case XmlRpcType.tBase64:
                        SerializeBase64(xtw, o);
                        break;

                    case XmlRpcType.tBoolean:
                        SerializeBoolean(xtw, o);
                        break;

                    case XmlRpcType.tDateTime:
                        SerializeDateTime(xtw, o);
                        break;

                    case XmlRpcType.tDouble:
                        SerializeDouble(xtw, o);
                        break;

                    case XmlRpcType.tHashtable:
                        SerializeHashTable(xtw, o, mappingActions, nestedObjs);
                        break;

                    case XmlRpcType.tInt32:
                        SerializeInt32(xtw, o, mappingActions);
                        break;

                    case XmlRpcType.tInt64:
                        SerializeInt64(xtw, o, mappingActions);
                        break;

                    case XmlRpcType.tString:
                        SerializeString(xtw, o);
                        break;

                    case XmlRpcType.tStruct:
                        SerializeStruct(xtw, o, mappingActions, nestedObjs);
                        break;

                    case XmlRpcType.tVoid:
                        SerializeVoid(xtw);
                        break;

                    case XmlRpcType.tNil:
                        SerializeNil(xtw);
                        break;

                    default:
                        throw new XmlRpcUnsupportedTypeException(o.GetType());
                }

                WriteFullEndElement(xtw);
            }
            catch (NullReferenceException)
            {
                throw new XmlRpcNullReferenceException("Attempt to serialize data " + "containing null reference");
            }
            finally
            {
                nestedObjs.RemoveAt(nestedObjs.Count - 1);
            }
        }

        private void SerializeArray(XmlWriter xtw, object o, MappingActions mappingActions, List<object> nestedObjs)
        {
            xtw.WriteStartElement(string.Empty, "array", string.Empty);
            xtw.WriteStartElement(string.Empty, "data", string.Empty);
            var a = (Array)o;
            foreach (var aobj in a)
            {
                Serialize(xtw, aobj, mappingActions, nestedObjs);
            }

            WriteFullEndElement(xtw);
            WriteFullEndElement(xtw);
        }

        private void SerializeMultiDimensionalArray(
            XmlWriter xtw,
            object o,
            MappingActions mappingActions,
            List<object> nestedObjs)
        {
            var mda = (Array)o;
            var indices = new int[mda.Rank];
            BuildArrayXml(xtw, mda, 0, indices, mappingActions, nestedObjs);
        }

        private void SerializeBase64(XmlWriter xtw, object o)
        {
            var buf = (byte[])o;
            xtw.WriteStartElement(string.Empty, "base64", string.Empty);
            xtw.WriteBase64(buf, 0, buf.Length);
            WriteFullEndElement(xtw);
        }

        private void SerializeBoolean(XmlWriter xtw, object o)
        {
            var boolVal = (bool)o;
            WriteFullElementString(xtw, "boolean", boolVal ? "1" : "0");
        }

        private void SerializeDateTime(XmlWriter xtw, object o)
        {
            var dt = (DateTime)o;
            var sdt = dt.ToString("yyyyMMdd'T'HH':'mm':'ss", DateTimeFormatInfo.InvariantInfo);
            WriteFullElementString(xtw, "dateTime.iso8601", sdt);
        }

        private void SerializeDouble(XmlWriter xtw, object o)
        {
            var doubleVal = (double)o;
            WriteFullElementString(xtw, "double", doubleVal.ToString(null, CultureInfo.InvariantCulture));
        }

        private void SerializeHashTable(
            XmlWriter xtw,
            object o,
            MappingActions mappingActions,
            List<object> nestedObjs)
        {
            xtw.WriteStartElement(string.Empty, "struct", string.Empty);
            var xrs = o as XmlRpcStruct;
            if (xrs == null)
            {
                return;
            }

            foreach (var skey in from object obj in xrs.Keys select obj as string)
            {
                xtw.WriteStartElement(string.Empty, "member", string.Empty);
                WriteFullElementString(xtw, "name", skey);
                Serialize(xtw, xrs[skey], mappingActions, nestedObjs);
                WriteFullEndElement(xtw);
            }

            WriteFullEndElement(xtw);
        }

        private void SerializeInt32(XmlWriter xtw, object o, MappingActions mappingActions)
        {
            if (o.GetType().IsEnum)
            {
                if (mappingActions.EnumMapping == EnumMapping.String)
                {
                    SerializeString(xtw, o.ToString());
                    return;
                }

                o = Convert.ToInt32(o);
            }

            WriteFullElementString(xtw, UseIntTag ? "int" : "i4", o.ToString());
        }

        private void SerializeInt64(XmlWriter xtw, object o, MappingActions mappingActions)
        {
            if (o.GetType().IsEnum)
            {
                if (mappingActions.EnumMapping == EnumMapping.String)
                    SerializeString(xtw, o.ToString());
                else
                    o = Convert.ToInt64(o);
            }

            WriteFullElementString(xtw, "i8", o.ToString());
        }

        private void SerializeString(XmlWriter xtw, object o)
        {
            try
            {
                if (UseStringTag)
                    WriteFullElementString(xtw, "string", (string)o);
                else
                    xtw.WriteString((string)o);
            }
            catch (ArgumentException ex)
            {
                throw new XmlRpcException(
                    string.Format("Unable to serialize string to XML: {0}", ex.Message),
                    ex);
            }
        }

        private void SerializeStruct(XmlWriter xtw, object o, MappingActions mappingActions, List<object> nestedObjs)
        {
            var structActions = GetMappingActions(o.GetType(), mappingActions);
            xtw.WriteStartElement(string.Empty, "struct", string.Empty);
            var mis = o.GetType().GetMembers();
            foreach (var mi in mis.Where(mi => !Attribute.IsDefined(mi, typeof(NonSerializedAttribute))))
            {
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        SerializeField(xtw, o, nestedObjs, mi, structActions);
                        break;

                    case MemberTypes.Property:
                        SerializeProperty(xtw, o, nestedObjs, mi, structActions);
                        break;
                }
            }

            WriteFullEndElement(xtw);
        }

        private void SerializeField(
            XmlWriter xtw,
            object o,
            List<object> nestedObjs,
            MemberInfo mi,
            MappingActions structActions)
        {
            var fi = (FieldInfo)mi;
            var member = fi.Name;
            var attrchk = Attribute.GetCustomAttribute(fi, typeof(XmlRpcMemberAttribute));
            var attribute = attrchk as XmlRpcMemberAttribute;
            if (attribute != null)
            {
                var mmbr = attribute.Member;
                if (mmbr != string.Empty)
                    member = mmbr;
            }

            var memberActions = MemberMappingActions(o.GetType(), fi.Name, structActions);
            if (fi.GetValue(o) == null)
            {
                switch (memberActions.NullMappingAction)
                {
                    case NullMappingAction.Ignore:
                        return;
                    case NullMappingAction.Error:
                        throw new XmlRpcMappingSerializeException(
                            string.Format(
                                @"Member ""{0}"" of struct ""{1}"" cannot be null.",
                                member,
                                o.GetType().Name));
                }
            }

            xtw.WriteStartElement(string.Empty, "member", string.Empty);
            WriteFullElementString(xtw, "name", member);
            Serialize(xtw, fi.GetValue(o), memberActions, nestedObjs);
            WriteFullEndElement(xtw);
        }

        private void SerializeProperty(
            XmlWriter xtw,
            object o,
            List<object> nestedObjs,
            MemberInfo mi,
            MappingActions structActions)
        {
            var pi = (PropertyInfo)mi;
            var member = pi.Name;
            var attrchk = Attribute.GetCustomAttribute(pi, typeof(XmlRpcMemberAttribute));
            var attribute = attrchk as XmlRpcMemberAttribute;
            if (attribute != null)
            {
                var mmbr = attribute.Member;
                if (mmbr != string.Empty)
                    member = mmbr;
            }

            var memberActions = MemberMappingActions(o.GetType(), pi.Name, structActions);
            if (pi.GetValue(o, null) == null)
            {
                switch (memberActions.NullMappingAction)
                {
                    case NullMappingAction.Ignore:
                        return;
                    case NullMappingAction.Error:
                        throw new XmlRpcMappingSerializeException(
                            string.Format(
                                @"Member ""{0}"" of struct ""{1}"" cannot be null.",
                                member,
                                o.GetType().Name));
                }
            }

            xtw.WriteStartElement(string.Empty, "member", string.Empty);
            WriteFullElementString(xtw, "name", member);
            Serialize(xtw, pi.GetValue(o, null), memberActions, nestedObjs);
            WriteFullEndElement(xtw);
        }

        private void SerializeVoid(XmlWriter xtw)
        {
            WriteFullElementString(xtw, "string", string.Empty);
        }

        private void SerializeNil(XmlWriter xtw)
        {
            xtw.WriteStartElement("nil");
            WriteFullEndElement(xtw);
        }

        private void BuildArrayXml(
            XmlWriter xtw,
            Array ary,
            int curRank,
            int[] indices,
            MappingActions mappingActions,
            List<object> nestedObjs)
        {
            xtw.WriteStartElement(string.Empty, "array", string.Empty);
            xtw.WriteStartElement(string.Empty, "data", string.Empty);

            if (curRank < (ary.Rank - 1))
            {
                for (var i = 0; i < ary.GetLength(curRank); i++)
                {
                    indices[curRank] = i;
                    xtw.WriteStartElement(string.Empty, "value", string.Empty);
                    BuildArrayXml(xtw, ary, curRank + 1, indices, mappingActions, nestedObjs);
                    WriteFullEndElement(xtw);
                }
            }
            else
            {
                for (var i = 0; i < ary.GetLength(curRank); i++)
                {
                    indices[curRank] = i;
                    Serialize(xtw, ary.GetValue(indices), mappingActions, nestedObjs);
                }
            }

            WriteFullEndElement(xtw);
            WriteFullEndElement(xtw);
        }

        private struct FaultStruct
        {
            public int FaultCode;

            public string FaultString;
        }

        public void SerializeFaultResponse(Stream stm, XmlRpcFaultException faultEx)
        {
            FaultStruct fs;
            fs.FaultCode = faultEx.FaultCode;
            fs.FaultString = faultEx.FaultString;
            var xtw = XmlRpcXmlWriter.Create(stm, XmlRpcFormatSettings);
            xtw.WriteStartDocument();
            xtw.WriteStartElement(string.Empty, "methodResponse", string.Empty);
            xtw.WriteStartElement(string.Empty, "fault", string.Empty);
            Serialize(xtw, fs, new MappingActions { NullMappingAction = NullMappingAction.Error });
            WriteFullEndElement(xtw);
            WriteFullEndElement(xtw);
            xtw.Flush();
        }

        protected virtual XmlWriterSettings ConfigureXmlFormat()
        {
            if (UseIndentation)
            {
                return new XmlWriterSettings {
                    Indent = true,
                    IndentChars = new string(' ', Indentation),
                    Encoding = XmlEncoding,
                    NewLineHandling = NewLineHandling.None,
                };
            }

            return new XmlWriterSettings { Indent = false, Encoding = XmlEncoding };
        }

        protected void WriteFullEndElement(XmlWriter xtw)
        {
            if (UseEmptyElementTags)
                xtw.WriteEndElement();
            else
                xtw.WriteFullEndElement();
        }

        protected void WriteFullElementString(XmlWriter xtw, string name, string value)
        {
            if (UseEmptyElementTags)
                xtw.WriteElementString(name, value);
            else
            {
                xtw.WriteStartElement(name);
                xtw.WriteString(value);
                xtw.WriteFullEndElement();
            }
        }

        protected static bool IsStructParamsMethod(MethodInfo mi)
        {
            if (mi == null)
                return false;

            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
            if (attr == null)
                return false;

            var mattr = (XmlRpcMethodAttribute)attr;

            return mattr.StructParams;
        }

        private static MappingActions MemberMappingActions(Type type, string memberName, MappingActions currentActions)
        {
            Contract.Requires(type != null);
            Contract.Requires(memberName != null);
            Contract.Requires(currentActions != null);

            // if struct member has mapping action attribute, override the current
            // mapping action else just return the current action
            if (type == null)
                return currentActions;

            var mis = type.GetMember(memberName);
            return mis.Length == 0
                ? currentActions
                : GetMappingActions(mis[0], currentActions);
        }

        protected MappingActions GetTypeMappings(MethodInfo mi, MappingActions mappingActions)
        {
            if (mi == null)
                return mappingActions;

            var declaringType = mi.DeclaringType;
            if (declaringType == null)
                return mappingActions;

            mappingActions = declaringType
                .GetInterfaces()
                .Aggregate(mappingActions, (current, itf) => GetMappingActions(itf, current));

            return GetMappingActions(declaringType, mappingActions);
        }

        protected static MappingActions GetMappingActions(ICustomAttributeProvider cap, MappingActions mappingActions)
        {
            if (cap == null)
                return mappingActions;

            var ret = new MappingActions {
                EnumMapping = mappingActions.EnumMapping,
                NullMappingAction = mappingActions.NullMappingAction
            };

            var nullMappingAttr = GetAttribute<XmlRpcNullMappingAttribute>(cap);
            if (nullMappingAttr != null)
                ret.NullMappingAction = nullMappingAttr.Action;
            else
            {
                // check for missing mapping attribute for backwards compatibility
                var missingAttr = GetAttribute<XmlRpcMissingMappingAttribute>(cap);
                if (missingAttr != null)
                    ret.NullMappingAction = MapToNullMappingAction(missingAttr.Action);
            }

            var enumAttr = GetAttribute<XmlRpcEnumMappingAttribute>(cap);
            if (enumAttr != null)
                ret.EnumMapping = enumAttr.Mapping;

            return ret;
        }

        private static T GetAttribute<T>(ICustomAttributeProvider cap) where T : class
        {
            var attrs = cap.GetCustomAttributes(typeof(T), true);
            return attrs.Length == 0 ? null : attrs[0] as T;
        }

        private static NullMappingAction MapToNullMappingAction(MappingAction missingMappingAction)
        {
            switch (missingMappingAction)
            {
                case MappingAction.Error:
                    return NullMappingAction.Error;
                case MappingAction.Ignore:
                    return NullMappingAction.Ignore;
                default:
                    throw new XmlRpcException("Unexpected missingMappingAction in MapToNullMappingAction");
            }
        }
    }
}