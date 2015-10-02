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

using System.Security;

namespace CookComputing.XmlRpc
{
    using System;
    using System.Runtime.Serialization;

    // used to return server-side errors to client code - also can be 
    // thrown by Service implmentation code to return custom Fault Responses
    [Serializable]
    public class XmlRpcFaultException : ApplicationException
    {
        public XmlRpcFaultException(int theCode, string theString)
            : base(string.Format("Server returned a fault exception: [{0}] {1}", theCode.ToString(), theString))
        {
            _faultCode = theCode;
            _faultString = theString;
        }

        // deserialization constructor
        protected XmlRpcFaultException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
            _faultCode = info.GetInt32("_faultCode");
            _faultString = info.GetString("_faultString");
        }

        public int FaultCode { get { return _faultCode; } }

        private readonly int _faultCode;

        public string FaultString { get { return _faultString; } }

        private readonly string _faultString;

        [SecurityCritical]
        public override void GetObjectData(
          SerializationInfo info,
          StreamingContext context)
        {
            info.AddValue("_faultCode", _faultCode);
            info.AddValue("_faultString", _faultString);
            base.GetObjectData(info, context);
        }
    }
}
