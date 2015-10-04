/* 
XML-RPC.NET library
Copyright (c) 2001-2006, Charles Cook <charlescook@cookcomputing.com>

Permission is hereby granted, free of charge, to any person 
obtaining a copy of this software and associated documentation 
files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, 
publish, distribute, sublicense, and/or sell copies of the Software, 
and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be 
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
DEALINGS IN THE SOFTWARE.
*/

namespace CookComputing.XmlRpc
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class XmlRpcTypeInfo
    {
        private static bool IsVisibleXmlRpcMethod(MemberInfo mi)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;

            return attr != null 
                && !(attr.Hidden || attr.IntrospectionMethod);
        }

        public static string GetXmlRpcMethodName(MethodInfo mi)
        {
            var attr = (XmlRpcMethodAttribute)Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
            return attr != null && !string.IsNullOrEmpty(attr.Method) 
                ? attr.Method 
                : mi.Name;
        }

        public static XmlRpcType GetXmlRpcType(object o)
        {
            return o == null 
                ? XmlRpcType.tNil 
                : GetXmlRpcType(o.GetType());
        }

        public static XmlRpcType GetXmlRpcType(Type t)
        {
            return GetXmlRpcType(t, new Stack<Type>());
        }

        private static XmlRpcType GetXmlRpcType(Type t, Stack<Type> typeStack)
        {
            if (t == typeof(Int32))
                return XmlRpcType.tInt32;

            if (t == typeof(Int64))
                return XmlRpcType.tInt64;

            if (t == typeof(Boolean))
                return XmlRpcType.tBoolean;

            if (t == typeof(String))
                return XmlRpcType.tString;

            if (t == typeof(Double))
                return XmlRpcType.tDouble;

            if (t == typeof(DateTime))
                return XmlRpcType.tDateTime;

            if (t == typeof(byte[]))
                return XmlRpcType.tBase64;

            if (t == typeof(XmlRpcStruct))
                return XmlRpcType.tHashtable;

            if (t == typeof(Array))
                return XmlRpcType.tArray;

            if (t.IsArray)
            {
                var elemType = t.GetElementType();
                if (elemType != typeof(object) && GetXmlRpcType(elemType, typeStack) == XmlRpcType.tInvalid)
                    return XmlRpcType.tInvalid;
                
                return t.GetArrayRank() == 1
                    ? XmlRpcType.tArray 
                    : XmlRpcType.tMultiDimArray;
            }

            if (t == typeof(int?))
                return XmlRpcType.tInt32;

            if (t == typeof(long?))
                return XmlRpcType.tInt64;

            if (t == typeof(bool?))
                return XmlRpcType.tBoolean;

            if (t == typeof(double?))
                return XmlRpcType.tDouble;

            if (t == typeof(DateTime?))
                return XmlRpcType.tDateTime;

            if (t == typeof(void))
                return XmlRpcType.tVoid;

            if ((t.IsValueType && !t.IsPrimitive && !t.IsEnum) || t.IsClass)
            {
                // if type is struct or class its only valid for XML-RPC mapping if all 
                // its members have a valid mapping or are of type object which
                // maps to any XML-RPC type
                var mis = t.GetMembers();
                foreach (var mi in mis)
                {
                    if (Attribute.IsDefined(mi, typeof(NonSerializedAttribute)))
                        continue;

                    if (mi.MemberType == MemberTypes.Field)
                    {
                        var fi = (FieldInfo)mi;
                        if (typeStack.Contains(fi.FieldType))
                            continue;

                        try
                        {
                            typeStack.Push(fi.FieldType);
                            if ((fi.FieldType != typeof(object) && GetXmlRpcType(fi.FieldType, typeStack) == XmlRpcType.tInvalid))
                                return XmlRpcType.tInvalid;
                        }
                        finally
                        {
                            typeStack.Pop();
                        }
                    }
                    else if (mi.MemberType == MemberTypes.Property)
                    {
                        var pi = (PropertyInfo)mi;
                        if (pi.GetIndexParameters().Length > 0)
                            return XmlRpcType.tInvalid;

                        if (typeStack.Contains(pi.PropertyType))
                            continue;

                        try
                        {
                            typeStack.Push(pi.PropertyType);

                            if ((pi.PropertyType != typeof(object) && GetXmlRpcType(pi.PropertyType, typeStack) == XmlRpcType.tInvalid))
                                return XmlRpcType.tInvalid;
                        }
                        finally
                        {
                            typeStack.Pop();
                        }
                    }
                }

                return XmlRpcType.tStruct;
            }

            if (t.IsEnum)
            {
                var enumBaseType = Enum.GetUnderlyingType(t);
                if (enumBaseType == typeof(int) || enumBaseType == typeof(byte)
                    || enumBaseType == typeof(sbyte) || enumBaseType == typeof(short)
                    || enumBaseType == typeof(ushort))
                    return XmlRpcType.tInt32;

                if (enumBaseType == typeof(long) || enumBaseType == typeof(UInt32))
                    return XmlRpcType.tInt64;
                
                return XmlRpcType.tInvalid;
            }

            return XmlRpcType.tInvalid;
        }

        public static string GetXmlRpcTypeString(Type t)
        {
            XmlRpcType rpcType = GetXmlRpcType(t);
            return GetXmlRpcTypeString(rpcType);
        }

        public static string GetXmlRpcTypeString(XmlRpcType t)
        {
            switch (t)
            {
                case XmlRpcType.tInt32:
                    return "integer";
                case XmlRpcType.tInt64:
                    return "i8";
                case XmlRpcType.tBoolean:
                    return "boolean";
                case XmlRpcType.tString:
                    return "string";
                case XmlRpcType.tDouble:
                    return "double";
                case XmlRpcType.tDateTime:
                    return "dateTime";
                case XmlRpcType.tBase64:
                    return "base64";
                case XmlRpcType.tStruct:
                    return "struct";
                case XmlRpcType.tHashtable:
                    return "struct";
                case XmlRpcType.tArray:
                    return "array";
                case XmlRpcType.tMultiDimArray:
                    return "array";
                case XmlRpcType.tVoid:
                    return "void";
                default:
                    return null;
            }
        }

        public static string GetUrlFromAttribute(Type type)
        {
            var urlAttr = Attribute.GetCustomAttribute(type, typeof(XmlRpcUrlAttribute)) as XmlRpcUrlAttribute;
            return urlAttr != null ? urlAttr.Uri : null;
        }

        public static string GetRpcMethodName(MethodInfo mi)
        {
            string methodName = mi.Name;
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcBeginAttribute)) as XmlRpcBeginAttribute;
            if (attr != null)
            {
                if (string.IsNullOrEmpty(attr.Method))
                {
                    if (!methodName.StartsWith("Begin", StringComparison.Ordinal) || methodName.Length <= 5)
                        throw new Exception(
                            string.Format(
                                "method {0} has invalid signature for begin method",
                                methodName));
                    
                    return methodName.Substring(5);
                }

                return attr.Method;
            }

            // if no XmlRpcBegin attribute, must have XmlRpcMethod attribute   
            var mattr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            if (mattr == null)
                throw new Exception("missing method attribute");

            return string.IsNullOrEmpty(mattr.Method) 
                ? mi.Name 
                : mattr.Method;
        }
    }
}