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

namespace CookComputing.XmlRpc
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public class XmlRpcServerProtocol : SystemMethodsBase
    {
        public Stream Invoke(Stream requestStream)
        {
            try
            {
                var serializer = new XmlRpcResponseSerializer();
                var deserializer = new XmlRpcRequestDeserializer();
                var serviceAttr = Attribute.GetCustomAttribute(GetType(), typeof(XmlRpcServiceAttribute)) as XmlRpcServiceAttribute;
                if (serviceAttr != null)
                {
                    if (serviceAttr.XmlEncoding != null)
                        serializer.XmlEncoding = Encoding.GetEncoding(serviceAttr.XmlEncoding);
                    serializer.UseEmptyParamsTag = serviceAttr.UseEmptyElementTags;
                    serializer.UseIntTag = serviceAttr.UseIntTag;
                    serializer.UseStringTag = serviceAttr.UseStringTag;
                    serializer.UseIndentation = serviceAttr.UseIndentation;
                    serializer.Indentation = serviceAttr.Indentation;
                }

                var xmlRpcReq = deserializer.DeserializeRequest(requestStream, GetType());
                var xmlRpcResp = Invoke(xmlRpcReq);
                var responseStream = new MemoryStream();
                serializer.SerializeResponse(responseStream, xmlRpcResp);
                responseStream.Seek(0, SeekOrigin.Begin);
                return responseStream;
            }
            catch (Exception ex)
            {
                XmlRpcFaultException fex;
                var xmlRpcException = ex as XmlRpcException;
                if (xmlRpcException != null)
                    fex = new XmlRpcFaultException(0, xmlRpcException.Message);
                else
                {
                    fex = (ex as XmlRpcFaultException) 
                        ?? new XmlRpcFaultException(0, ex.Message);
                }

                var serializer = new XmlRpcSerializer();
                var responseStream = new MemoryStream();
                serializer.SerializeFaultResponse(responseStream, fex);
                responseStream.Seek(0, SeekOrigin.Begin);
                return responseStream;      
            }
        }

        public XmlRpcResponse Invoke(XmlRpcRequest request)
        {
            var mi = request.Mi ?? GetType().GetMethod(request.Method);

            // exceptions thrown during an MethodInfo.Invoke call are
            // package as inner of 
            object reto;
            try
            {
                reto = mi.Invoke(this, request.Args);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;

                throw ex;
            }

            // methods which have void return type always return integer 0
            // because XML-RPC doesn't support no return type (could use nil
            // but want to maintain backwards compatibility in this area)
            if (mi != null && mi.ReturnType == typeof(void))
                reto = 0;

            return new XmlRpcResponse(reto);
        }

        private bool IsVisibleXmlRpcMethod(MemberInfo mi)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            return attr != null && !attr.Hidden;
        }
    }
}
