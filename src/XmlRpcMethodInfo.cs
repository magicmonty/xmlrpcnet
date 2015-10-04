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
    using System.Reflection;

    public class XmlRpcMethodInfo : IComparable
    {
        public XmlRpcMethodInfo()
        {
            IsHidden = false;
            Doc = string.Empty;
            MethodInfo = null;
            Parameters = null;
            ReturnType = null;
            ReturnXmlRpcType = null;
            ReturnDoc = string.Empty;
            XmlRpcName = string.Empty;
            MiName = string.Empty;
        }

        public bool IsHidden { get; set; }

        public String Doc { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public String MiName { get; set; }

        public XmlRpcParameterInfo[] Parameters { get; set; }

        public Type ReturnType { get; set; }

        public string ReturnXmlRpcType { get; set; }

        public String ReturnDoc { get; set; }

        public String XmlRpcName { get; set; }

        public int CompareTo(object obj)
        {
            var xmi = (XmlRpcMethodInfo)obj;
            return string.Compare(XmlRpcName, xmi.XmlRpcName, StringComparison.Ordinal);
        }

    }
}