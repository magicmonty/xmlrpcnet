// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlRpcDeserializer.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace CookComputing.XmlRpc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    public class XmlRpcDeserializer
    {
        public XmlRpcNonStandard NonStandard
        {
            get { return _nonStandard; }
            set { _nonStandard = value; }
        }

        private XmlRpcNonStandard _nonStandard = XmlRpcNonStandard.None;

        // private properties
        protected bool AllowInvalidHttpContent
        {
            get { return (_nonStandard & XmlRpcNonStandard.AllowInvalidHTTPContent) != 0; }
        }

        protected bool AllowNonStandardDateTime
        {
            get { return (_nonStandard & XmlRpcNonStandard.AllowNonStandardDateTime) != 0; }
        }

        protected bool AllowStringFaultCode
        {
            get { return (_nonStandard & XmlRpcNonStandard.AllowStringFaultCode) != 0; }
        }

        protected virtual bool IgnoreDuplicateMembers
        {
            get
            {
                return (_nonStandard & XmlRpcNonStandard.IgnoreDuplicateMembers) != 0;
            }
        }

        protected virtual bool MapEmptyDateTimeToMinValue
        {
            get { return (_nonStandard & XmlRpcNonStandard.MapEmptyDateTimeToMinValue) != 0; }
        }

        protected virtual bool MapZerosDateTimeToMinValue
        {
            get { return (_nonStandard & XmlRpcNonStandard.MapZerosDateTimeToMinValue) != 0; }
        }

        public object MapValueNode(
            IEnumerator<Node> iter,
            Type valType,
            MappingStack mappingStack,
            MappingAction mappingAction)
        {
            var valueNode = iter.Current as ValueNode;

            // if suppplied type is System.Object then ignore it because
            // if doesn't provide any useful information (parsing methods
            // expect null in this case)
            if (valType != null && valType.BaseType == null)
                valType = null;

            if (valueNode is StringValue && valueNode.ImplicitValue)
                CheckImplictString(valType, mappingStack);

            Type mappedType;

            if (iter.Current is ArrayValue)
                return MapArray(iter, valType, mappingStack, mappingAction, out mappedType);

            if (iter.Current is StructValue)
            {
                // if we don't know the expected struct type then we must
                // map the XML-RPC struct as an instance of XmlRpcStruct
                if (valType != null && valType != typeof(XmlRpcStruct) && !valType.IsSubclassOf(typeof(XmlRpcStruct)))
                    return MapStruct(iter, valType, mappingStack, mappingAction, out mappedType);

                return MapHashtable(iter, mappingStack, mappingAction, out mappedType);
            }

            if (iter.Current is Base64Value)
                return MapBase64(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is IntValue)
                return MapInt(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is LongValue)
                return MapLong(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is StringValue)
                return MapString(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is BooleanValue)
                return MapBoolean(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is DoubleValue)
                return MapDouble(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is DateTimeValue)
                return MapDateTime(valueNode.Value, valType, mappingStack, out mappedType);

            if (iter.Current is NilValue)
                return MapNilValue(valType, mappingStack, out mappedType);

            return null;
        }

        private object MapDateTime(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(DateTime), mappingStack);
            mappedType = typeof(DateTime);
            return OnStack(
                "dateTime",
                mappingStack,
                () => ParseDateTime(value, mappingStack));
        }

        private DateTime ParseDateTime(string value, MappingStack mappingStack)
        {
            if (value == string.Empty && MapEmptyDateTimeToMinValue)
                return DateTime.MinValue;

            DateTime retVal;
            if (DateTime8601.TryParseDateTime8601(value, out retVal))
                return retVal;

            if (MapZerosDateTimeToMinValue
                && value.StartsWith("0000")
                && (value == "00000000T00:00:00"
                    || value == "0000-00-00T00:00:00Z"
                    || value == "00000000T00:00:00Z"
                    || value == "0000-00-00T00:00:00"))
                return DateTime.MinValue;

            throw new XmlRpcInvalidXmlRpcException(
                string.Format(
                    "{0} contains invalid dateTime value {1}",
                    mappingStack.MappingType,
                    StackDump(mappingStack)));
        }

        private object MapDouble(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(double), mappingStack);
            mappedType = typeof(double);
            return OnStack(
                "double",
                mappingStack,
                () => ParseDouble(value, mappingStack));
        }

        private double ParseDouble(string value, MappingStack mappingStack)
        {
            try
            {
                return double.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
            }
            catch (Exception)
            {
                throw new XmlRpcInvalidXmlRpcException(
                    string.Format(
                    "{0} contains invalid double value {1}",
                    mappingStack.MappingType,
                    StackDump(mappingStack)));
            }
        }

        private object MapBoolean(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(bool), mappingStack);
            mappedType = typeof(bool);
            return OnStack(
                "boolean",
                mappingStack,
                () => ParseBoolean(value, mappingStack));
        }

        private bool ParseBoolean(string value, MappingStack mappingStack)
        {
            switch (value)
            {
                case "1":
                    return true;
                case "0":
                    return false;
                default:
                    throw new XmlRpcInvalidXmlRpcException(
                        string.Format(
                            "{0} contains invalid boolean value {1}",
                            mappingStack.MappingType,
                            StackDump(mappingStack)));
            }
        }

        private object MapString(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(string), mappingStack);
            if (valType != null && valType.IsEnum)
                return MapStringToEnum(value, valType, "i8", mappingStack, out mappedType);

            mappedType = typeof(string);
            return OnStack("string", mappingStack, () => value);
        }

        private object MapStringToEnum(
            string value,
            Type enumType,
            string xmlRpcType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            mappedType = enumType;
            return OnStack(
                xmlRpcType,
                mappingStack,
                () => ParseEnum(value, enumType, xmlRpcType, mappingStack));
        }

        private object ParseEnum(string value, Type enumType, string xmlRpcType, MappingStack mappingStack)
        {
            try
            {
                return Enum.Parse(enumType, value, true);
            }
            catch (XmlRpcInvalidEnumValue)
            {
                throw;
            }
            catch
            {
                throw new XmlRpcInvalidEnumValue(
                    string.Format(
                        "{0} contains invalid or out of range {1} value mapped to enum {2}",
                        mappingStack.MappingType,
                        xmlRpcType,
                        StackDump(mappingStack)));
            }
        }

        private object MapLong(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(long), mappingStack);
            if (valType != null && valType.IsEnum)
                return MapNumberToEnum(value, valType, "i8", mappingStack, out mappedType);

            mappedType = typeof(long);
            return OnStack(
                "i8",
                mappingStack,
                () => ParseLong(value, mappingStack));
        }

        private static long ParseLong(string value, MappingStack mappingStack)
        {
            long ret;
            if (long.TryParse(value, out ret))
                return ret;

            throw new XmlRpcInvalidXmlRpcException(
                string.Format(
                "{0} contains invalid i8 value {1}",
                mappingStack.MappingType,
                StackDump(mappingStack)));
        }

        private object MapInt(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(int), mappingStack);
            if (valType != null && valType.IsEnum)
                return MapNumberToEnum(value, valType, "int", mappingStack, out mappedType);

            mappedType = typeof(int);
            return OnStack(
                "integer",
                mappingStack,
                () => ParseInt(value, mappingStack));
        }

        private static int ParseInt(string value, MappingStack mappingStack)
        {
            int ret;
            if (int.TryParse(value, out ret))
                return ret;

            throw new XmlRpcInvalidXmlRpcException(
                string.Format(
                    "{0} contains invalid int value {1}",
                    mappingStack.MappingType,
                    StackDump(mappingStack)));
        }

        private object MapNumberToEnum(
            string value,
            Type enumType,
            string xmlRpcType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            mappedType = enumType;
            return OnStack(
                xmlRpcType,
                mappingStack,
                () => ParseEnumFromNumber(value, enumType, xmlRpcType, mappingStack));
        }

        private static object ParseEnumFromNumber(string value, Type enumType, string xmlRpcType, MappingStack mappingStack)
        {
            try
            {
                var lnum = long.Parse(value);
                var underlyingType = Enum.GetUnderlyingType(enumType);
                var enumNumberValue = Convert.ChangeType(lnum, underlyingType, null);
                if (Enum.IsDefined(enumType, enumNumberValue))
                    return Enum.ToObject(enumType, enumNumberValue);

                throw new XmlRpcInvalidEnumValue(
                    string.Format(
                        "{0} contains {1}mapped to undefined enum value {2}",
                        mappingStack.MappingType,
                        xmlRpcType,
                        StackDump(mappingStack)));
            }
            catch (XmlRpcInvalidEnumValue)
            {
                throw;
            }
            catch (Exception)
            {
                throw new XmlRpcInvalidEnumValue(
                    string.Format(
                        "{0} contains invalid or out of range {1} value mapped to enum {2}",
                        mappingStack.MappingType,
                        xmlRpcType,
                        StackDump(mappingStack)));
            }
        }

        private object MapBase64(
            string value,
            Type valType,
            MappingStack mappingStack,
            out Type mappedType)
        {
            CheckExpectedType(valType, typeof(byte[]), mappingStack);
            mappedType = typeof(int);
            return OnStack(
                "base64",
                mappingStack,
                () => ParseBase64(value, mappingStack));
        }

        private static byte[] ParseBase64(string value, MappingStack mappingStack)
        {
            if (value == string.Empty)
                return new byte[0];

            try
            {
                return Convert.FromBase64String(value);
            }
            catch (Exception)
            {
                throw new XmlRpcInvalidXmlRpcException(
                    string.Format(
                        "{0} contains invalid base64 value {1}",
                        mappingStack.MappingType,
                        StackDump(mappingStack)));
            }
        }

        private static object MapNilValue(
            Type type,
            MappingStack mappingStack,
            out Type mappedType)
        {
            if (type == null
                || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                || (!type.IsPrimitive || !type.IsValueType) || type == typeof(object))
            {
                mappedType = type;
                return null;
            }

            throw new XmlRpcInvalidXmlRpcException(
                string.Format(
                    "{0} contains <nil> value which cannot be mapped to type {1} {2}",
                    mappingStack.MappingType,
                    type != typeof(object) ? type.Name : "object",
                    StackDump(mappingStack)));
        }

        protected object MapHashtable(
            IEnumerator<Node> iter,
            MappingStack mappingStack,
            MappingAction mappingAction,
            out Type mappedType)
        {
            mappedType = null;
            var retObj = new XmlRpcStruct();
            mappingStack.Push("struct mapped to XmlRpcStruct");
            try
            {
                while (iter.MoveNext() && iter.Current is StructMember)
                {
                    var rpcName = ((StructMember)iter.Current).Value;
                    if (retObj.ContainsKey(rpcName) && !IgnoreDuplicateMembers)
                    {
                        throw new XmlRpcInvalidXmlRpcException(
                            mappingStack.MappingType + " contains struct value with duplicate member " + rpcName + " "
                            + StackDump(mappingStack));
                    }

                    iter.MoveNext();

                    var value = OnStack(
                        string.Format("member {0}", rpcName),
                        mappingStack,
                        () => MapValueNode(iter, null, mappingStack, mappingAction));

                    if (!retObj.ContainsKey(rpcName))
                        retObj[rpcName] = value;
                }
            }
            finally
            {
                mappingStack.Pop();
            }

            return retObj;
        }

        private object MapStruct(
            IEnumerator<Node> iter,
            Type valueType,
            MappingStack mappingStack,
            MappingAction mappingAction,
            out Type mappedType)
        {
            mappedType = null;

            if (valueType.IsPrimitive)
            {
                throw new XmlRpcTypeMismatchException(
                    string.Format(
                        "{0} contains struct value where {1} expected {2}",
                        mappingStack.MappingType,
                        XmlRpcTypeInfo.GetXmlRpcTypeString(valueType),
                        StackDump(mappingStack)));
            }

            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
                valueType = valueType.GetGenericArguments()[0];

            object retObj;
            try
            {
                retObj = Activator.CreateInstance(valueType);
            }
            catch (Exception)
            {
                throw new XmlRpcTypeMismatchException(
                    string.Format(
                        "{0} contains struct value where {1} expected (as type {2}) {3}",
                        mappingStack.MappingType,
                        XmlRpcTypeInfo.GetXmlRpcTypeString(valueType),
                        valueType.Name,
                        StackDump(mappingStack)));
            }

            // Note: mapping action on a struct is only applied locally - it 
            // does not override the global mapping action when members of the 
            // struct are mapped
            var localAction = mappingAction;
            if (valueType != null)
            {
                mappingStack.Push("struct mapped to type " + valueType.Name);
                localAction = StructMappingAction(valueType, mappingAction);
            }
            else
                mappingStack.Push("struct");

            // create map of field names and remove each name from it as 
            // processed so we can determine which fields are missing
            var names = new List<string>();
            CreateFieldNamesMap(valueType, names);
            var rpcNames = new List<string>();
            try
            {
                while (iter.MoveNext())
                {
                    if (!(iter.Current is StructMember))
                        break;

                    var rpcName = ((StructMember)iter.Current).Value;
                    if (rpcNames.Contains(rpcName))
                    {
                        if (!IgnoreDuplicateMembers)
                            throw new XmlRpcInvalidXmlRpcException(
                                string.Format(
                                    "{0} contains struct value with duplicate member {1} {2}",
                                    mappingStack.MappingType,
                                    rpcName,
                                    StackDump(mappingStack)));
                        else
                            continue;
                    }
                    else
                        rpcNames.Add(rpcName);

                    var name = GetStructName(valueType, rpcName) ?? rpcName;
                    if (valueType == null)
                        continue;

                    var mi = valueType.GetField(name) ?? (MemberInfo)valueType.GetProperty(name);
                    if (mi == null)
                    {
                        iter.MoveNext(); // move to value
                        if (iter.Current is ComplexValueNode)
                        {
                            var depth = iter.Current.Depth;
                            while (!(iter.Current is EndComplexValueNode && iter.Current.Depth == depth))
                                iter.MoveNext();
                        }

                        continue;
                    }

                    if (names.Contains(name))
                        names.Remove(name);
                    else
                    {
                        if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
                        {
                            mappingStack.Push(string.Format("member {0}", name));
                            throw new XmlRpcNonSerializedMember(
                                string.Format(
                                    "Cannot map XML-RPC struct member onto member marked as [NonSerialized]: {0}",
                                    StackDump(mappingStack)));
                        }
                    }

                    var memberType = mi.MemberType == MemberTypes.Field
                                         ? ((mi as FieldInfo) == null ? null : (mi as FieldInfo).FieldType)
                                         : ((mi as PropertyInfo) == null ? null : (mi as PropertyInfo).PropertyType);

                    string mappingMsg = memberType == null
                                            ? string.Format("member {0}", name)
                                            : string.Format("member {0} mapped to type {1}", name, memberType.Name);

                    iter.MoveNext();
                    var valObj = OnStack(
                        mappingMsg,
                        mappingStack,
                        () => MapValueNode(iter, memberType, mappingStack, mappingAction));

                    if (mi.MemberType == MemberTypes.Field && (mi is FieldInfo))
                        (mi as FieldInfo).SetValue(retObj, valObj);
                    else if (mi is PropertyInfo)
                        (mi as PropertyInfo).SetValue(retObj, valObj, null);
                }

                if (localAction == MappingAction.Error && names.Count > 0)
                    ReportMissingMembers(valueType, names, mappingStack);

                return retObj;
            }
            finally
            {
                mappingStack.Pop();
            }
        }

        private object MapArray(
            IEnumerator<Node> iter,
            Type valType,
            MappingStack mappingStack,
            MappingAction mappingAction,
            out Type mappedType)
        {
            mappedType = null;

            // required type must be an array
            if (valType != null && !(valType.IsArray || valType == typeof(Array) || valType == typeof(object)))
            {
                throw new XmlRpcTypeMismatchException(
                    string.Format(
                        "{0} contains array value where {1} expected {2}",
                        mappingStack.MappingType,
                        XmlRpcTypeInfo.GetXmlRpcTypeString(valType),
                        StackDump(mappingStack)));
            }

            if (valType != null)
            {
                var xmlRpcType = XmlRpcTypeInfo.GetXmlRpcType(valType);
                if (xmlRpcType == XmlRpcType.tMultiDimArray)
                {
                    mappingStack.Push("array mapped to type " + valType.Name);
                    return MapMultiDimArray(iter, valType, mappingStack, mappingAction);
                }

                mappingStack.Push("array mapped to type " + valType.Name);
            }
            else
                mappingStack.Push("array");

            var values = new List<object>();
            var elemType = DetermineArrayItemType(valType);

            var gotType = false;
            Type useType = null;

            while (iter.MoveNext() && iter.Current is ValueNode)
            {
                mappingStack.Push(string.Format("element {0}", values.Count));
                var value = MapValueNode(iter, elemType, mappingStack, mappingAction);
                values.Add(value);
                mappingStack.Pop();
            }

            foreach (var value in values.Where(value => value != null))
            {
                if (!gotType)
                {
                    useType = value.GetType();
                    gotType = true;
                }
                else
                {
                    if (useType != value.GetType())
                        useType = null;
                }
            }

            var args = new object[1];
            args[0] = values.Count;
            object retObj;
            if (valType != null && valType != typeof(Array) && valType != typeof(object))
                retObj = CreateArrayInstance(valType, args);
            else
            {
                retObj = useType == null
                    ? CreateArrayInstance(typeof(object[]), args)
                    : Array.CreateInstance(useType, (int)args[0]);
            }

            for (var j = 0; j < values.Count; j++)
                ((Array)retObj).SetValue(values[j], j);

            mappingStack.Pop();

            return retObj;
        }

        private static Type DetermineArrayItemType(Type valType)
        {
            return valType != null && valType != typeof(Array) && valType != typeof(object)
                ? valType.GetElementType()
                : typeof(object);
        }

        private void CheckImplictString(Type valType, MappingStack mappingStack)
        {
            if (valType != null && valType != typeof(string) && !valType.IsEnum)
            {
                throw new XmlRpcTypeMismatchException(
                    string.Format(
                        "{0} contains implicit string value where {1} expected {2}",
                        mappingStack.MappingType,
                        XmlRpcTypeInfo.GetXmlRpcTypeString(valType),
                        StackDump(mappingStack)));
            }
        }

        private object MapMultiDimArray(
            IEnumerator<Node> iter,
            Type valueType,
            MappingStack mappingStack,
            MappingAction mappingAction)
        {
            // parse the type name to get element type and array rank
            var elemType = valueType.GetElementType();
            var rank = valueType.GetArrayRank();

            // elements will be stored sequentially as nested arrays are mapped
            var elements = new List<object>();

            // create array to store length of each dimension - initialize to 
            // all zeroes so that when parsing we can determine if an array for 
            // that dimension has been mapped already
            var dimLengths = new int[rank];
            dimLengths.Initialize();
            MapMultiDimElements(iter, rank, 0, elemType, elements, dimLengths, mappingStack, mappingAction);

            // build arguments to define array dimensions and create the array
            var args = new object[dimLengths.Length];
            for (var argi = 0; argi < dimLengths.Length; argi++)
                args[argi] = dimLengths[argi];

            var ret = (Array)CreateArrayInstance(valueType, args);

            // copy elements into new multi-dim array
            // !! make more efficient
            var length = ret.Length;
            for (var e = 0; e < length; e++)
            {
                var indices = new int[dimLengths.Length];
                var div = 1;
                for (var f = indices.Length - 1; f >= 0; f--)
                {
                    indices[f] = (e / div) % dimLengths[f];
                    div *= dimLengths[f];
                }

                ret.SetValue(elements[e], indices);
            }

            return ret;
        }

        private void MapMultiDimElements(
            IEnumerator<Node> iter,
            int rank,
            int curRank,
            Type elemType,
            ICollection<object> elements,
            IList<int> dimLengths,
            MappingStack mappingStack,
            MappingAction mappingAction)
        {
            var nodeCount = 0;
            if (curRank < (rank - 1))
            {
                while (iter.MoveNext() && iter.Current is ArrayValue)
                {
                    nodeCount++;
                    MapMultiDimElements(
                        iter,
                        rank,
                        curRank + 1,
                        elemType,
                        elements,
                        dimLengths,
                        mappingStack,
                        mappingAction);
                }
            }
            else
            {
                while (iter.MoveNext() && iter.Current is ValueNode)
                {
                    nodeCount++;
                    var value = MapValueNode(iter, elemType, mappingStack, mappingAction);
                    elements.Add(value);
                }
            }

            dimLengths[curRank] = nodeCount;
        }

        public object ParseValueElement(
            XmlReader rdr,
            Type valType,
            MappingStack mappingStack,
            MappingAction mappingAction)
        {
            var iter = new XmlRpcParser().ParseValue(rdr).GetEnumerator();
            iter.MoveNext();

            return MapValueNode(iter, valType, mappingStack, mappingAction);
        }

        private static void CreateFieldNamesMap(Type valueType, List<string> names)
        {
            names.AddRange(from fi in valueType.GetFields() where !Attribute.IsDefined(fi, typeof(NonSerializedAttribute)) select fi.Name);
            names.AddRange(from pi in valueType.GetProperties() where !Attribute.IsDefined(pi, typeof(NonSerializedAttribute)) select pi.Name);
        }

        private void CheckExpectedType(Type expectedType, Type actualType, MappingStack mappingStack)
        {
            if (expectedType != null && expectedType.IsEnum)
            {
                var fourBitTypes = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int) };
                var eightBitTypes = new[] { typeof(uint), typeof(long) };
                var underlyingType = Enum.GetUnderlyingType(expectedType);
                if (Array.IndexOf(fourBitTypes, underlyingType) >= 0)
                    expectedType = typeof(int);
                else if (Array.IndexOf(eightBitTypes, underlyingType) >= 0)
                    expectedType = typeof(long);
                else
                {
                    throw new XmlRpcInvalidEnumValue(
                        string.Format(
                            "{0} contains {1} which cannot be mapped to  {2} {3}",
                            mappingStack.MappingType,
                            XmlRpcTypeInfo.GetXmlRpcTypeString(actualType),
                            XmlRpcTypeInfo.GetXmlRpcTypeString(expectedType),
                            StackDump(mappingStack)));
                }
            }

            // TODO: throw exception for invalid enum type
            if (expectedType != null
                && expectedType != typeof(object)
                && expectedType != actualType
                && (actualType.IsValueType && expectedType != typeof(Nullable<>).MakeGenericType(actualType)))
            {
                throw new XmlRpcTypeMismatchException(
                    string.Format(
                        "{0} contains {1} value where {2} expected {3}",
                        mappingStack.MappingType,
                        XmlRpcTypeInfo.GetXmlRpcTypeString(actualType),
                        XmlRpcTypeInfo.GetXmlRpcTypeString(expectedType),
                        StackDump(mappingStack)));
            }
        }

        private static T OnStack<T>(string p, MappingStack mappingStack, Func<T> func)
        {
            mappingStack.Push(p);
            try
            {
                return func();
            }
            finally
            {
                mappingStack.Pop();
            }
        }

        private void ReportMissingMembers(Type valueType, IEnumerable<string> names, MappingStack mappingStack)
        {
            var sb = new StringBuilder();
            var errorCount = 0;
            var sep = string.Empty;
            foreach (var s in from s in names let memberAction = MemberMappingAction(valueType, s, MappingAction.Error) where memberAction == MappingAction.Error select s)
            {
                sb.Append(sep);
                sb.Append(s);
                sep = " ";
                errorCount++;
            }

            if (errorCount <= 0)
                return;

            var plural = string.Empty;
            if (errorCount > 1)
                plural = "s";

            throw new XmlRpcTypeMismatchException(
                string.Format(
                    "{0} contains struct value with missing non-optional member{1}: {2} {3}",
                    mappingStack.MappingType,
                    plural,
                    sb,
                    StackDump(mappingStack)));
        }

        private string GetStructName(Type valueType, string xmlRpcName)
        {
            // given a member name in an XML-RPC struct, check to see whether
            // a field has been associated with this XML-RPC member name, return
            // the field name if it has else return null
            if (valueType == null)
                return null;

            foreach (var ret in from fi in valueType.GetFields()
                                let attr = Attribute.GetCustomAttribute(fi, typeof(XmlRpcMemberAttribute))
                                where attr is XmlRpcMemberAttribute && ((XmlRpcMemberAttribute)attr).Member == xmlRpcName
                                select fi.Name)
                return ret;

            return (from pi in valueType.GetProperties()
                    let attr = Attribute.GetCustomAttribute(pi, typeof(XmlRpcMemberAttribute))
                    where attr is XmlRpcMemberAttribute && ((XmlRpcMemberAttribute)attr).Member == xmlRpcName
                    select pi.Name)
                    .FirstOrDefault();
        }

        private static MappingAction StructMappingAction(Type type, MappingAction currentAction)
        {
            // if struct member has mapping action attribute, override the current
            // mapping action else just return the current action
            if (type == null)
                return currentAction;

            var attr = Attribute.GetCustomAttribute(type, typeof(XmlRpcMissingMappingAttribute)) as XmlRpcMissingMappingAttribute;
            return attr == null
                ? currentAction
                : attr.Action;
        }

        private static MappingAction MemberMappingAction(Type type, string memberName, MappingAction currentAction)
        {
            // if struct member has mapping action attribute, override the current
            // mapping action else just return the current action
            if (type == null)
                return currentAction;

            XmlRpcMissingMappingAttribute attr;
            var fi = type.GetField(memberName);
            if (fi != null)
                attr = Attribute.GetCustomAttribute(fi, typeof(XmlRpcMissingMappingAttribute)) as XmlRpcMissingMappingAttribute;
            else
            {
                var pi = type.GetProperty(memberName);
                attr = Attribute.GetCustomAttribute(pi, typeof(XmlRpcMissingMappingAttribute)) as XmlRpcMissingMappingAttribute;
            }

            return attr == null
                ? currentAction
                : attr.Action;
        }

        private static string StackDump(MappingStack mappingStack)
        {
            var sb = new StringBuilder();
            foreach (var elem in mappingStack)
            {
                sb.Insert(0, elem);
                sb.Insert(0, " : ");
            }

            sb.Insert(0, mappingStack.MappingType);
            sb.Insert(0, "[");
            sb.Append("]");

            return sb.ToString();
        }

        // TODO: following to return Array?
        private static object CreateArrayInstance(Type type, object[] args)
        {
            return Activator.CreateInstance(type, args);
        }
    }
}