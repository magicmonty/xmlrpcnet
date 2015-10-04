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
using System.IO;
using System.Web.UI;

namespace CookComputing.XmlRpc
{
    public class XmlRpcHttpServerProtocol : XmlRpcServerProtocol,
    IHttpRequestHandler
    {
        public void HandleHttpRequest(
            IHttpRequest httpReq, 
            IHttpResponse httpResp)
        {
            // GET has its own handler because it can be used to return a 
            // HTML description of the service
            if (httpReq.HttpMethod == "GET")
            {
                var svcAttr = Attribute.GetCustomAttribute(GetType(), typeof(XmlRpcServiceAttribute)) as XmlRpcServiceAttribute;
                if (svcAttr != null && !svcAttr.AutoDocumentation)
                    HandleUnsupportedMethod(httpResp);
                else
                {
                    var autoDocVersion = true;
                    if (svcAttr != null)
                        autoDocVersion = svcAttr.AutoDocVersion;
                    HandleGET(httpResp, autoDocVersion);
                }

                return;
            }

            // calls on service methods are via POST
            if (httpReq.HttpMethod != "POST")
            {
                HandleUnsupportedMethod(httpResp);
                return;
            }

            // process the request
            var responseStream = Invoke(httpReq.InputStream);
            httpResp.ContentType = "text/xml";
            if (!httpResp.SendChunked)
                httpResp.ContentLength = responseStream.Length;
            
            var respStm = httpResp.OutputStream;
            Util.CopyStream(responseStream, respStm);

            respStm.Flush();
        }

        protected void HandleGET(
            IHttpResponse httpResp,
            bool autoDocVersion)
        {
            using (var stm = new MemoryStream())
            using (var wrtr = new HtmlTextWriter(new StreamWriter(stm)))
            {
                XmlRpcDocWriter.WriteDoc(wrtr, GetType(), autoDocVersion);
                wrtr.Flush();

                httpResp.ContentType = "text/html";
                httpResp.StatusCode = 200;
                if (!httpResp.SendChunked)
                    httpResp.ContentLength = stm.Length;
                    
                stm.Position = 0;
                Stream respStm = httpResp.OutputStream;
                Util.CopyStream(stm, respStm);
                respStm.Flush();
            }
        }

        protected void HandleUnsupportedMethod(
            IHttpResponse httpResp)
        {
            // RFC 2068 error 405: "The method specified in the Request-Line   
            // is not allowed for the resource identified by the Request-URI. 
            // The response MUST include an Allow header containing a list 
            // of valid methods for the requested resource."
            //!! add Allow header
            httpResp.StatusCode = 405;
            httpResp.StatusDescription = "Unsupported HTTP verb";
        }
    }
}
