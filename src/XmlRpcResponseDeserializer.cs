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
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    struct Fault
    {
        public int FaultCode;
        public string FaultString;
    }

    public class XmlRpcResponseDeserializer : XmlRpcDeserializer
    {
        public XmlRpcResponse DeserializeResponse(Stream stm, Type svcType)
        {
            if (stm == null)
                throw new ArgumentNullException(
                    "stm",
                    "XmlRpcSerializer.DeserializeResponse");
            
            if (AllowInvalidHttpContent)
            {
                var newStm = new MemoryStream();
                Util.CopyStream(stm, newStm);
                stm = newStm;
                stm.Position = 0;
                while (true)
                {
                    // for now just strip off any leading CR-LF characters
                    var byt = stm.ReadByte();
                    if (byt == -1)
                        throw new XmlRpcIllFormedXmlException(
                            "Response from server does not contain valid XML.");
                    if (byt != 0x0d && byt != 0x0a && byt != ' ' && byt != '\t')
                    {
                        stm.Position = stm.Position - 1;
                        break;
                    }
                }
            }

            XmlReader xmlRdr = XmlRpcXmlReader.Create(stm);
            return DeserializeResponse(xmlRdr, svcType);
        }

        public XmlRpcResponse DeserializeResponse(TextReader txtrdr, Type svcType)
        {
            if (txtrdr == null)
                throw new ArgumentNullException(
                    "txtrdr",
                    "XmlRpcSerializer.DeserializeResponse");

            var xmlRdr = XmlRpcXmlReader.Create(txtrdr);
            return DeserializeResponse(xmlRdr, svcType);
        }

        public XmlRpcResponse DeserializeResponse(XmlReader rdr, Type returnType)
        {
            try
            {
                var iter = new XmlRpcParser().ParseResponse(rdr).GetEnumerator();
                iter.MoveNext();
                if (iter.Current is FaultNode)
                    throw DeserializeFault(iter);
                
                if (returnType == typeof(void) || !iter.MoveNext())
                    return new XmlRpcResponse { RetVal = null };

                var retObj = MapValueNode(
                    iter,
                    returnType,
                    new MappingStack("response"),
                    MappingAction.Error);
                
                return new XmlRpcResponse { RetVal = retObj };
            }
            catch (XmlException ex)
            {
                throw new XmlRpcIllFormedXmlException("Response contains invalid XML", ex);
            }
        }

        private XmlRpcException DeserializeFault(IEnumerator<Node> iter)
        {
            // TODO: use global action setting
            throw ParseFault(
                iter, 
                new MappingStack("fault response"), // TODO: fix
                MappingAction.Error);
        }

        XmlRpcFaultException ParseFault(
            IEnumerator<Node> iter,
            MappingStack parseStack,
            MappingAction mappingAction)
        {
            iter.MoveNext();  // move to StructValue
            Type parsedType;
            var faultStruct = MapHashtable(
                iter, 
                parseStack, 
                mappingAction,
                out parsedType) as XmlRpcStruct;
            
            var faultCode = faultStruct["FaultCode"];
            var faultString = faultStruct["FaultString"];
            var str = faultCode as string;
            if (str != null)
            {
                int value;
                if (!Int32.TryParse(str, out value))
                    throw new XmlRpcInvalidXmlRpcException("faultCode not int or string");
                faultCode = value;
            }

            return new XmlRpcFaultException((int)faultCode, (string)faultString);
        }

        struct FaultStruct
        {
            public int FaultCode;
            public string FaultString;
        }

        struct FaultStructStringCode
        {
            public string FaultCode;
            public string FaultString;
        }
    }
}


