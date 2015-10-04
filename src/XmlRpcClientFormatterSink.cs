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

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

using CookComputing.XmlRpc;

namespace CookComputing.XmlRpc
{
    public class XmlRpcClientFormatterSink : IClientChannelSink, IMessageSink
    {
        public XmlRpcClientFormatterSink(IClientChannelSink NextSink)
        {
            _next = NextSink;
        }

        public IClientChannelSink NextChannelSink { get { return _next; } }
        IClientChannelSink _next;

        public IMessageSink NextSink {
            get { throw new NotSupportedException(); }
        }

        public IDictionary Properties { get { return null; } }

        public IMessageCtrl AsyncProcessMessage(
            IMessage msg,
            IMessageSink replySink)
        {
            throw new NotSupportedException();
        }

        public void AsyncProcessRequest(
            IClientChannelSinkStack sinkStack,
            IMessage msg,
            ITransportHeaders headers,
            Stream stream)
        {
            throw new NotImplementedException();
        }

        public void AsyncProcessResponse(
            IClientResponseChannelSinkStack sinkStack,
            object state,
            ITransportHeaders headers,
            Stream stream)
        {
            throw new NotImplementedException();
        }

        public Stream GetRequestStream(
            IMessage msg,
            ITransportHeaders headers)
        {
            return null; // TODO: ??? 
        }

        public void ProcessMessage(
            IMessage msg,
            ITransportHeaders requestHeaders,
            Stream requestStream,
            out ITransportHeaders responseHeaders,
            out Stream responseStream)
        {
            responseHeaders = null;
            responseStream = null;
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            var mcm = msg as IMethodCallMessage;
            try {
                Stream reqStm = null;
                ITransportHeaders reqHeaders = null;
                SerializeMessage(mcm, ref reqHeaders, ref reqStm);
        
                Stream respStm;
                ITransportHeaders respHeaders;
                _next.ProcessMessage(
                    msg, 
                    reqHeaders, 
                    reqStm, 
                    out respHeaders, 
                    out respStm);

                return DeserializeMessage(mcm, respStm);
            }
            catch (Exception ex) {
                return new ReturnMessage(ex, mcm);
            }
        }

        private static void SerializeMessage(
            IMethodCallMessage mcm, 
            ref ITransportHeaders headers, 
            ref Stream stream)
        {
            var reqHeaders = new TransportHeaders();
            reqHeaders["__Uri"] = mcm.Uri;
            reqHeaders["Content-Type"] = "text/xml; charset=\"utf-8\"";
            reqHeaders["__RequestVerb"] = "POST";

            var mi = (MethodInfo)mcm.MethodBase;
            var methodName = GetRpcMethodName(mi);
            var xmlRpcReq = new XmlRpcRequest(methodName, mcm.InArgs);

            // TODO: possibly call GetRequestStream from next sink in chain?
            // TODO: SoapClientFormatter sink uses ChunkedStream - check why?
            var stm = new MemoryStream();
            var serializer = new XmlRpcRequestSerializer();
            serializer.SerializeRequest(stm, xmlRpcReq);
            stm.Position = 0;

            headers = reqHeaders;
            stream = stm;
        }

        private static IMessage DeserializeMessage(
            IMethodCallMessage mcm,
            Stream stream)
        {
            var deserializer = new XmlRpcResponseDeserializer();
            var tp = mcm.MethodBase;
            var mi = (MethodInfo)tp;
            var t = mi.ReturnType;
            var xmlRpcResp = deserializer.DeserializeResponse(stream, t);
            return new ReturnMessage(xmlRpcResp.RetVal, null, 0, null, mcm);
        }

        private static string GetRpcMethodName(MemberInfo mi)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            var rpcMethod = string.Empty;
            if (attr != null)
                rpcMethod = attr.Method;
            
            if (string.IsNullOrEmpty(rpcMethod))
                rpcMethod = mi.Name;

            return rpcMethod;
        }

    }
}
