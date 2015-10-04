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

    public class XmlRpcServiceInfo
    {
        private XmlRpcMethodInfo[] _methodInfos;

        public static XmlRpcServiceInfo CreateServiceInfo(Type type)
        {
            var svcInfo = new XmlRpcServiceInfo();
            // extract service info
            var svcAttr = Attribute.GetCustomAttribute(type, typeof(XmlRpcServiceAttribute)) as XmlRpcServiceAttribute;
            if (svcAttr != null && !string.IsNullOrEmpty(svcAttr.Description))
                svcInfo.Doc = svcAttr.Description;
            
            if (svcAttr != null && !string.IsNullOrEmpty(svcAttr.Name))
                svcInfo.Name = svcAttr.Name;
            else
                svcInfo.Name = type.Name;
            
            // extract method info
            var methods = new Dictionary<string, XmlRpcMethodInfo>();

            foreach (Type itf in type.GetInterfaces())
            {
                var itfAttr = Attribute.GetCustomAttribute(itf, typeof(XmlRpcServiceAttribute)) as XmlRpcServiceAttribute;
                if (itfAttr != null)
                    svcInfo.Doc = itfAttr.Description;
                
                var imap = type.GetInterfaceMap(itf);
                foreach (var mi in imap.InterfaceMethods)
                    ExtractMethodInfo(methods, mi, itf);
            }

            foreach (var mi in type.GetMethods())
            {
                var mthds = new List<MethodInfo>();
                mthds.Add(mi);

                var curMi = mi;
                while (true)
                {
                    var baseMi = curMi.GetBaseDefinition();
                    if (baseMi.DeclaringType == curMi.DeclaringType)
                        break;

                    mthds.Insert(0, baseMi);
                    curMi = baseMi;
                }

                foreach (var mthd in mthds)
                    ExtractMethodInfo(methods, mthd, type);
            }

            svcInfo._methodInfos = new XmlRpcMethodInfo[methods.Count];
            methods.Values.CopyTo(svcInfo._methodInfos, 0);
            Array.Sort(svcInfo._methodInfos);
            return svcInfo;
        }

        public MethodInfo GetMethodInfo(string xmlRpcMethodName)
        {
            foreach (var xmi in _methodInfos)
            {
                if (xmlRpcMethodName == xmi.XmlRpcName)
                    return xmi.MethodInfo;
            }

            return null;
        }

        private static void ExtractMethodInfo(
            IDictionary<string, XmlRpcMethodInfo> methods, 
            MethodInfo mi, 
            MemberInfo type)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            if (attr == null)
                return;
            
            var mthdInfo = new XmlRpcMethodInfo();
            mthdInfo.MethodInfo = mi;
            mthdInfo.XmlRpcName = XmlRpcTypeInfo.GetXmlRpcMethodName(mi);
            mthdInfo.MiName = mi.Name;
            mthdInfo.Doc = attr.Description;
            mthdInfo.IsHidden = attr.IntrospectionMethod | attr.Hidden;

            // extract parameters information
            var parmList = new List<XmlRpcParameterInfo>();
            var parms = mi.GetParameters();
            foreach (var parm in parms)
            {
                var parmInfo = new XmlRpcParameterInfo();
                parmInfo.Name = parm.Name;
                parmInfo.Type = parm.ParameterType;
                parmInfo.XmlRpcType = XmlRpcTypeInfo.GetXmlRpcTypeString(parm.ParameterType);

                // retrieve optional attributed info
                parmInfo.Doc = string.Empty;
                var pattr = Attribute.GetCustomAttribute(parm, typeof(XmlRpcParameterAttribute)) as XmlRpcParameterAttribute;

                if (pattr != null)
                {
                    parmInfo.Doc = pattr.Description;
                    parmInfo.XmlRpcName = pattr.Name;
                }

                parmInfo.IsParams = Attribute.IsDefined(parm, typeof(ParamArrayAttribute));
                parmList.Add(parmInfo);
            }

            mthdInfo.Parameters = parmList.ToArray();

            // extract return type information
            mthdInfo.ReturnType = mi.ReturnType;
            mthdInfo.ReturnXmlRpcType = XmlRpcTypeInfo.GetXmlRpcTypeString(mi.ReturnType);
            var orattrs = mi.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(XmlRpcReturnValueAttribute), false);
            if (orattrs.Length > 0)
                mthdInfo.ReturnDoc = ((XmlRpcReturnValueAttribute)orattrs[0]).Description;

            if (methods.ContainsKey(mthdInfo.XmlRpcName))
            {
                throw new XmlRpcDupXmlRpcMethodNames(
                    string.Format(
                        "Method {0} in type {1} has duplicate XmlRpc method name {2}",
                        mi.Name, 
                        type.Name, 
                        mthdInfo.XmlRpcName));
            }
            else
                methods.Add(mthdInfo.XmlRpcName, mthdInfo);
        }

        public string GetMethodName(string XmlRpcMethodName)
        {
            foreach (var methodInfo in _methodInfos)
            {
                if (string.Equals(methodInfo.XmlRpcName, XmlRpcMethodName, StringComparison.Ordinal))
                    return methodInfo.MiName;
            }

            return null;
        }

        public String Doc { get; set; }

        public String Name { get; set; }

        public XmlRpcMethodInfo[] Methods { get { return _methodInfos; } }

        public XmlRpcMethodInfo GetMethod(string methodName)
        {
            foreach (var mthdInfo in _methodInfos)
            {
                if (mthdInfo.XmlRpcName == methodName)
                    return mthdInfo;
            }

            return null;
        }

        private XmlRpcServiceInfo() { }
    }
}