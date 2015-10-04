/* 
XML-RPC.NET library
Copyright (c) 2001-2011, Charles Cook <charlescook@cookcomputing.com>

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

// TODO: overriding default mapping action in a struct should not affect nested structs

namespace CookComputing.XmlRpc
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Collections.Generic;

    public class XmlRpcRequestDeserializer : XmlRpcDeserializer
    {
        public XmlRpcRequest DeserializeRequest(Stream stm, Type svcType)
        {
            if (stm == null)
                throw new ArgumentNullException("stm", "XmlRpcSerializer.DeserializeRequest");
            
            var xmlRdr = XmlRpcXmlReader.Create(stm);
            return DeserializeRequest(xmlRdr, svcType);
        }

        public XmlRpcRequest DeserializeRequest(TextReader txtrdr, Type svcType)
        {
            if (txtrdr == null)
                throw new ArgumentNullException("txtrdr", "XmlRpcSerializer.DeserializeRequest");
            
            var xmlRdr = XmlRpcXmlReader.Create(txtrdr);
            return DeserializeRequest(xmlRdr, svcType);
        }

        public XmlRpcRequest DeserializeRequest(XmlReader rdr, Type svcType)
        {
            try
            {
                var request = new XmlRpcRequest();
                var iter = new XmlRpcParser().ParseRequest(rdr).GetEnumerator();

                iter.MoveNext();
                var methodName = (iter.Current as MethodName).Name;
                request.Method = methodName;

                request.Mi = null;
                ParameterInfo[] pis = null;
                if (svcType != null)
                {
                    // retrieve info for the method which handles this XML-RPC method
                    var svcInfo = XmlRpcServiceInfo.CreateServiceInfo(svcType);
                    request.Mi = svcInfo.GetMethodInfo(request.Method);

                    // if a service type has been specified and we cannot find the requested
                    // method then we must throw an exception
                    if (request.Mi == null)
                    {
                        var msg = string.Format("unsupported method called: {0}", request.Method);
                        throw new XmlRpcUnsupportedMethodException(msg);
                    }

                    // method must be marked with XmlRpcMethod attribute
                    var attr = Attribute.GetCustomAttribute(request.Mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
                    if (attr == null)
                        throw new XmlRpcMethodAttributeException("Method must be marked with the XmlRpcMethod attribute.");

                    pis = request.Mi.GetParameters();
                }

                var gotParams = iter.MoveNext();
                if (!gotParams)
                {
                    if (svcType != null)
                    {
                        if (pis.Length == 0)
                        {
                            request.Args = new object[0];
                            return request;
                        }
                        else
                        {
                            throw new XmlRpcInvalidParametersException(
                                "Method takes parameters and params element is missing.");
                        }
                    }
                    else
                    {
                        request.Args = new object[0];
                        return request;
                    }
                }

                var paramsPos = pis != null ? GetParamsPos(pis) : -1;
                Type paramsType = null;
                if (paramsPos != -1)
                    paramsType = pis[paramsPos].ParameterType.GetElementType();

                var minParamCount = pis == null 
                    ? int.MaxValue
                    : (paramsPos == -1 ? pis.Length : paramsPos);
                
                var mappingStack = new MappingStack("request");
                var mappingAction = MappingAction.Error;
                var objs = new List<object>();
                var paramsObjs = new List<object>();
                var paramCount = 0;

                while (iter.MoveNext())
                {
                    paramCount++;
                    if (svcType != null && paramCount > minParamCount && paramsPos == -1)
                        throw new XmlRpcInvalidParametersException(
                            "Request contains too many param elements based on method signature.");

                    if (paramCount <= minParamCount)
                    {
                        if (svcType != null)
                        {
                            mappingStack.Push(String.Format("parameter {0}", paramCount));

                            // TODO: why following commented out?
                            //          parseStack.Push(String.Format("parameter {0} mapped to type {1}", 
                            //            i, pis[i].ParameterType.Name));
                            var obj = MapValueNode(iter,
                                pis[paramCount - 1].ParameterType, mappingStack, mappingAction);

                            objs.Add(obj);
                        }
                        else
                        {
                            mappingStack.Push(String.Format("parameter {0}", paramCount));
                            var obj = MapValueNode(iter, null, mappingStack, mappingAction);
                            objs.Add(obj);
                        }

                        mappingStack.Pop();
                    }
                    else
                    {
                        mappingStack.Push(String.Format("parameter {0}", paramCount + 1));
                        var paramsObj = MapValueNode(iter, paramsType, mappingStack, mappingAction);
                        paramsObjs.Add(paramsObj);
                        mappingStack.Pop();
                    }
                }

                if (svcType != null && paramCount < minParamCount)
                    throw new XmlRpcInvalidParametersException(
                        "Request contains too few param elements based on method signature.");

                if (paramsPos != -1)
                {
                    var args = new object[1];
                    args[0] = paramCount - minParamCount;
                    var varargs = (Array)Activator.CreateInstance(pis[paramsPos].ParameterType, args);
                    for (var i = 0; i < paramsObjs.Count; i++)
                        varargs.SetValue(paramsObjs[i], i);

                    objs.Add(varargs);
                }

                request.Args = objs.ToArray();
                return request;
            }
            catch (XmlException ex)
            {
                throw new XmlRpcIllFormedXmlException("Request contains invalid XML", ex);
            }
        }

        private static int GetParamsPos(ParameterInfo[] pis)
        {
            if (pis.Length == 0)
                return -1;
            
            return Attribute.IsDefined(pis[pis.Length - 1], typeof(ParamArrayAttribute)) 
                ? pis.Length - 1 
                : -1;
        }
    }
}


