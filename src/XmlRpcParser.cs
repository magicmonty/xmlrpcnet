using System;
using System.Collections.Generic;
using System.Xml;

namespace CookComputing.XmlRpc
{
    public class XmlRpcParser
    {
        private static List<string> _xmlRpcMembers = new List<string>();

        static XmlRpcParser()
        {
            _xmlRpcMembers.AddRange(new [] {
                "name",
                "value",
            });
        }

        private int _depth;

        public IEnumerable<Node> ParseRequest(XmlReader rdr)
        {
            rdr.MoveToContent();
            if (!string.Equals(rdr.Name, "methodCall", StringComparison.Ordinal))
                throw new XmlRpcInvalidXmlRpcException("Request XML not valid XML-RPC - root element not methodCall.");
            
            var mcDepth = rdr.Depth;
            MoveToChild(rdr, "methodName", true);
            var mnDepth = rdr.Depth;
            var methodName = rdr.ReadElementContentAsString();
            if (string.IsNullOrEmpty(methodName))
                throw new XmlRpcInvalidXmlRpcException("Request XML not valid XML-RPC - empty methodName.");
            
            yield return CreateMethodName(methodName);

            if (MovetoSibling(rdr, "params", false))
            {
                yield return new ParamsNode(_depth);
                var psDepth = rdr.Depth;
                var gotP = MoveToChild(rdr, "param", false);
                while (gotP)
                {
                    foreach (var node in ParseParam(rdr))
                        yield return node;
                    gotP = MovetoSibling(rdr, "param");
                }

                MoveToEndElement(rdr, psDepth);
            }

            MoveToEndElement(rdr, mcDepth);
        }

        public IEnumerable<Node> ParseResponse(XmlReader rdr)
        {
            rdr.MoveToContent();
            if (!string.Equals(rdr.Name, "methodResponse", StringComparison.Ordinal))
                throw new XmlRpcInvalidXmlRpcException("Response XML not valid XML-RPC - root element not methodResponse.");
            
            var mrDepth = rdr.Depth;
            MoveToChild(rdr, "params", "fault");
            if (string.Equals(rdr.Name, "params", StringComparison.Ordinal))
            {
                yield return new ResponseNode(_depth);

                var psDepth = rdr.Depth;
                var gotP = MoveToChild(rdr, "param");
                if (gotP)
                {
                    foreach (var node in ParseParam(rdr))
                        yield return node;
                }

                MoveToEndElement(rdr, psDepth);
            }
            else
            {
                var fltDepth = rdr.Depth;
                foreach (var node in ParseFault(rdr))
                    yield return node;
                
                MoveToEndElement(rdr, fltDepth);
            }

            MoveToEndElement(rdr, mrDepth);
        }

        private IEnumerable<Node> ParseFault(XmlReader rdr)
        {
            var fDepth = rdr.Depth;
            yield return new FaultNode(_depth);

            MoveToChild(rdr, "value", true);

            foreach (var node in ParseValue(rdr))
                yield return node;
            
            MoveToEndElement(rdr, fDepth);
        }

        private IEnumerable<Node> ParseParam(XmlReader rdr)
        {
            var pDepth = rdr.Depth;

            MoveToChild(rdr, "value", true);
            foreach (var node in ParseValue(rdr))
                yield return node;
            
            MoveToEndElement(rdr, pDepth);
        }

        public IEnumerable<Node> ParseValue(XmlReader rdr)
        {
            var vDepth = rdr.Depth;
            if (rdr.IsEmptyElement)
            {
                yield return CreateStringValue(string.Empty, true);
            }
            else
            {
                rdr.Read(); // TODO: check all return values from rdr.Read()
                if (rdr.NodeType == XmlNodeType.Text)
                {
                    yield return CreateStringValue(rdr.Value, true);
                }
                else
                {
                    var strValue = string.Empty;
                    if (rdr.NodeType == XmlNodeType.Whitespace)
                    {
                        strValue = rdr.Value;
                        rdr.Read();
                    }
                    if (rdr.NodeType == XmlNodeType.EndElement)
                    {
                        yield return CreateStringValue(strValue, true);
                    }
                    else if (rdr.NodeType == XmlNodeType.Element)
                    {
                        switch (rdr.Name)
                        {
                            case "string":
                                yield return CreateStringValue(rdr.ReadElementContentAsString(), false);
                                break;
                            case "int":
                            case "i4":
                                yield return CreateIntValue(rdr.ReadElementContentAsString());
                                break;
                            case "i8":
                                yield return CreateLongValue(rdr.ReadElementContentAsString());
                                break;
                            case "double":
                                yield return CreateDoubleValue(rdr.ReadElementContentAsString());
                                break;
                            case "dateTime.iso8601":
                                yield return CreateDateTimeValue(rdr.ReadElementContentAsString());
                                break;
                            case "boolean":
                                yield return CreateBooleanValue(rdr.ReadElementContentAsString());
                                break;
                            case "base64":
                                yield return CreateBase64Value(rdr.ReadElementContentAsString());
                                break;
                            case "struct":
                                foreach (var node in ParseStruct(rdr))
                                    yield return node;
                                break;
                            case "array":
                                foreach (var node in ParseArray(rdr))
                                    yield return node;
                                break;
                            case "nil":
                                yield return CreateNilValue();
                                break;
                        }
                    }
                }
            }

            MoveToEndElement(rdr, vDepth);
        }

        private IEnumerable<Node> ParseArray(XmlReader rdr)
        {
            yield return CreateArrayValue();

            var aDepth = rdr.Depth;

            MoveToChild(rdr, "data");

            var gotV = MoveToChild(rdr, "value");
            var vDepth = rdr.Depth;

            while (gotV)
            {
                foreach (var node in ParseValue(rdr))
                    yield return node;
                
                gotV = MovetoSibling(rdr, "value");
            }

            yield return CreateEndArrayValue();
        }

        private IEnumerable<Node> ParseStruct(XmlReader rdr)
        {
            yield return CreateStructValue();

            var sDepth = rdr.Depth;
            var gotM = MoveToChild(rdr, "member");
            var mDepth = rdr.Depth;

            while (gotM)
            {
                MoveToChild(rdr, "name", true);
                var name = rdr.ReadElementContentAsString();
                if (string.IsNullOrEmpty(name))
                    throw new XmlRpcInvalidXmlRpcException("Struct contains member with empty name element.");

                yield return CreateStructMember(name);

                MoveOverWhiteSpace(rdr);
                if (!(rdr.NodeType == XmlNodeType.Element && string.Equals(rdr.Name, "value", StringComparison.Ordinal)))
                    throw new Exception();

                foreach (var node in ParseValue(rdr))
                    yield return node;
                
                MoveToEndElement(rdr, mDepth);

                gotM = MovetoSibling(rdr, "member");
            }

            MoveToEndElement(rdr, sDepth);

            yield return CreateEndStructValue();
        }

        private bool MovetoSibling(XmlReader rdr, string p)
        {
            return MovetoSibling(rdr, p, false);
        }

        private static bool MovetoSibling(XmlReader rdr, string elementName, bool required)
        {
            if (!rdr.IsEmptyElement 
                && rdr.NodeType == XmlNodeType.Element 
                && string.Equals(rdr.Name, elementName, StringComparison.Ordinal))
                return true;
            
            var depth = rdr.Depth;
            rdr.Read();
            while (rdr.Depth >= depth)
            {
                if (rdr.Depth == (depth) 
                    && rdr.NodeType == XmlNodeType.Element 
                    && string.Equals(rdr.Name, elementName, StringComparison.Ordinal))
                    return true;
                
                if (!rdr.Read())
                    break;
            }

            if (required)
                throw new XmlRpcInvalidXmlRpcException(string.Format("Missing element {0}", elementName));

            return false;
        }

        private static bool MoveToEndElement(XmlReader rdr, int mcDepth)
        {
            // TODO: better error reporting required, i.e. include end element node type expected
            if (rdr.Depth == mcDepth && rdr.IsEmptyElement)
                return true;
            
            if (rdr.Depth == mcDepth && rdr.NodeType == XmlNodeType.EndElement)
                return true;

            while (rdr.Depth >= mcDepth)
            {
                rdr.Read();
                if (rdr.NodeType == XmlNodeType.Element && IsXmlRpcElement(rdr.Name))
                    throw new XmlRpcInvalidXmlRpcException(string.Format("Unexpected element {0}",  rdr.Name));
                
                if (rdr.Depth == mcDepth && rdr.NodeType == XmlNodeType.EndElement)
                    return true;
            }

            return false;
        }

        private static bool IsXmlRpcElement(string elementName)
        {
            return _xmlRpcMembers.Contains(elementName);
        }

        private bool MoveToChild(XmlReader rdr, string nodeName)
        {
            return MoveToChild(rdr, nodeName, false);
        }

        private bool MoveToChild(XmlReader rdr, string nodeName1, string nodeName2)
        {
            return MoveToChild(rdr, nodeName1, nodeName2, false);
        }

        private bool MoveToChild(XmlReader rdr, string nodeName, bool required)
        {
            return MoveToChild(rdr, nodeName, null, required);
        }

        private static bool MoveToChild(XmlReader rdr, string nodeName1, string nodeName2, bool required)
        {
            int depth = rdr.Depth;
            if (rdr.IsEmptyElement)
            {
                if (required)
                    throw new XmlRpcInvalidXmlRpcException(MakeMissingChildMessage(nodeName1, nodeName2));
                
                return false;
            }

            rdr.Read();
            while (rdr.Depth > depth)
            {
                if (rdr.Depth == (depth + 1) 
                    && rdr.NodeType == XmlNodeType.Element
                    && (string.Equals(rdr.Name, nodeName1, StringComparison.Ordinal) 
                        || (nodeName2 != null && string.Equals(rdr.Name, nodeName2, StringComparison.Ordinal))))
                    return true;
                
                rdr.Read();
            }

            if (required)
                throw new XmlRpcInvalidXmlRpcException(MakeMissingChildMessage(nodeName1, nodeName2));

            return false;
        }

        private static string MakeMissingChildMessage(string nodeName1, string nodeName2)
        {
            return nodeName2 == null
                ? string.Format("Missing element:  {0}", nodeName1)
                : string.Format("Missing element: {0} or {1}", nodeName1, nodeName2);
        }

        private static void MoveOverWhiteSpace(XmlReader rdr)
        {
            while (rdr.NodeType == XmlNodeType.Whitespace
                   || rdr.NodeType == XmlNodeType.SignificantWhitespace)
                rdr.Read();
        }


        private MethodName CreateMethodName(string name)
        {
            return new MethodName(_depth, name);
        }

        private StringValue CreateStringValue(string value, bool implicitValue)
        {
            return new StringValue(_depth, value, implicitValue);
        }

        private IntValue CreateIntValue(string value)
        {
            return new IntValue(_depth, value);
        }

        private LongValue CreateLongValue(string value)
        {
            return new LongValue(_depth, value);
        }

        private DoubleValue CreateDoubleValue(string value)
        {
            return new DoubleValue(_depth, value);
        }

        private BooleanValue CreateBooleanValue(string value)
        {
            return new BooleanValue(_depth, value);
        }

        private DateTimeValue CreateDateTimeValue(string value)
        {
            return new DateTimeValue(_depth, value);
        }

        private Base64Value CreateBase64Value(string value)
        {
            return new Base64Value(_depth, value);
        }

        private NilValue CreateNilValue()
        {
            return new NilValue(_depth);
        }

        private StructValue CreateStructValue()
        {
            return new StructValue(_depth++);
        }

        private StructMember CreateStructMember(string name)
        {
            return new StructMember(_depth, name);
        }

        private EndStructValue CreateEndStructValue()
        {
            return new EndStructValue(--_depth);
        }

        private ArrayValue CreateArrayValue()
        {
            return new ArrayValue(_depth++);
        }

        private EndArrayValue CreateEndArrayValue()
        {
            return new EndArrayValue(--_depth);
        }
    }

    public class Node
    {
        public int Depth { get; private set; }

        public Node(int depth)
        {
            Depth = depth;
        }
    }

    public class ValueNode : Node
    {
        public ValueNode(int depth) : base(depth)
        {
        }

        public ValueNode(int depth, string value)
            : base(depth)
        {
            Value = value;
        }

        public ValueNode(int depth, string value, bool implicitValue)
            : base(depth)
        {
            Value = value;
            ImplicitValue = implicitValue;
        }

        public string Value { get; set; }

        public bool ImplicitValue { get; private set; }
    }

    public class SimpleValueNode : ValueNode
    {
        public SimpleValueNode(int depth) : base(depth)
        {
        }

        public SimpleValueNode(int depth, string value)
            : base(depth, value)
        {
        }

        public SimpleValueNode(int depth, string value, bool implicitValue)
            : base(depth, value, implicitValue)
        {
        }
    }

    public class ComplexValueNode : ValueNode
    {
        public ComplexValueNode(int depth) : base(depth)
        {
        }
    }

    public class EndComplexValueNode : Node
    {
        public EndComplexValueNode(int depth) : base(depth)
        {
        }
    }

    public class StringValue : SimpleValueNode
    {
        public StringValue(int depth, string value, bool implicitValue)
            : base(depth, value, implicitValue)
        {
        }
    }

    public class IntValue : SimpleValueNode
    {
        public IntValue(int depth, string value) : base(depth, value)
        {
        }
    }

    public class LongValue : SimpleValueNode
    {
        public LongValue(int depth, string value) : base(depth, value)
        {
        }
    }

    public class DoubleValue : SimpleValueNode
    {
        public DoubleValue(int depth, string value) : base(depth, value)
        {
        }
    }

    public class BooleanValue : SimpleValueNode
    {
        public BooleanValue(int depth, string value) : base(depth, value)
        {
        }
    }

    public class DateTimeValue : SimpleValueNode
    {
        public DateTimeValue(int depth, string value) : base(depth, value)
        {
        }
    }

    public class Base64Value : SimpleValueNode
    {
        public Base64Value(int depth, string value) : base(depth, value)
        {
        }
    }

    public class NilValue : SimpleValueNode
    {
        public NilValue(int depth) : base(depth)
        {
        }
    }

    public class StructMember : ValueNode
    {
        public StructMember(int depth, string name) : base(depth, name)
        {
        }
    }

    public class EndStructValue : EndComplexValueNode
    {
        public EndStructValue(int depth) : base(depth)
        {
        }
    }

    public class ArrayValue : ComplexValueNode
    {
        public ArrayValue(int depth) : base(depth)
        {
        }
    }

    public class EndArrayValue : EndComplexValueNode
    {
        public EndArrayValue(int depth) : base(depth)
        {
        }
    }

    public class MethodName : Node
    {
        public MethodName(int depth, string name) : base(depth)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class FaultNode : Node
    {
        public FaultNode(int depth) : base(depth)
        {
        }
    }

    public class ResponseNode : Node
    {
        public ResponseNode(int depth) : base(depth)
        {
        }
    }

    public class ParamsNode : Node
    {
        public ParamsNode(int depth) : base(depth)
        {
        }
    }

    public class ParamNode : Node
    {
        public ParamNode(int depth) : base(depth)
        {
        }
    }

    public class StructValue : ComplexValueNode
    {
        public StructValue(int depth) : base(depth)
        {
        }
    }
}
