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
    using System.Collections;
    using System.Reflection;
    using System.Web.UI;

    public static class XmlRpcDocWriter
    {
        public static void WriteDoc(
            HtmlTextWriter wrtr, 
            Type type,
            bool autoDocVersion)
        {
            var svcInfo = XmlRpcServiceInfo.CreateServiceInfo(type);

            wrtr.WriteFullBeginTag("html");
            wrtr.WriteLine();
            WriteHead(wrtr, svcInfo.Name);
            wrtr.WriteLine();
            WriteBody(wrtr, type, autoDocVersion);
            wrtr.WriteEndTag("html");
        }

        public static void WriteHead(HtmlTextWriter wrtr, string title)
        {
            wrtr.WriteFullBeginTag("head");
            wrtr.WriteLine();
            WriteStyle(wrtr);
            WriteTitle(wrtr, title);
            wrtr.WriteEndTag("head");
        }

        public static void WriteFooter(
            HtmlTextWriter wrtr, 
            Type type,
            bool autoDocVersion)
        {
            wrtr.WriteBeginTag("div");
            wrtr.WriteAttribute("id", "content");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.WriteLine();

            wrtr.WriteFullBeginTag("h2");
            wrtr.Write("&nbsp;");
            wrtr.WriteEndTag("h2");
            wrtr.WriteLine();

            if (autoDocVersion)
            {
                var name1 = type.Assembly.GetName();
                wrtr.Write(
                    "{0} {1}.{2}.{3}&nbsp;&nbsp;&nbsp;", 
                    name1.Name, 
                    name1.Version.Major, 
                    name1.Version.Minor, 
                    name1.Version.Build);

                var name2 = typeof(XmlRpcServerProtocol).Assembly.GetName();
                wrtr.Write(
                    "{0} {1}.{2}.{3}&nbsp;&nbsp;&nbsp;", 
                    name2.Name, 
                    name2.Version.Major, 
                    name2.Version.Minor, 
                    name2.Version.Build);

                wrtr.Write(
                    ".NET CLR {0}.{1}.{2}&nbsp;&nbsp;&nbsp;", 
                    Environment.Version.Major, 
                    Environment.Version.Minor, 
                    Environment.Version.Build);
            }
            wrtr.WriteEndTag("div");
            wrtr.WriteLine();
        }

        private static void WriteStyle(HtmlTextWriter wrtr)
        {
            wrtr.WriteBeginTag("style");
            wrtr.WriteAttribute("type", "text/css");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.WriteLine();

            wrtr.WriteLine("BODY { color: #000000; background-color: white; font-family: Verdana; margin-left: 0px; margin-top: 0px; }");
            wrtr.WriteLine("#content { margin-left: 30px; font-size: .70em; padding-bottom: 2em; }");
            wrtr.WriteLine("A:link { color: #336699; font-weight: bold; text-decoration: underline; }");
            wrtr.WriteLine("A:visited { color: #6699cc; font-weight: bold; text-decoration: underline; }");
            wrtr.WriteLine("A:active { color: #336699; font-weight: bold; text-decoration: underline; }");
            wrtr.WriteLine("A:hover { color: cc3300; font-weight: bold; text-decoration: underline; }");
            wrtr.WriteLine("P { color: #000000; margin-top: 0px; margin-bottom: 12px; font-family: Verdana; }");
            wrtr.WriteLine("pre { background-color: #e5e5cc; padding: 5px; font-family: Courier New; font-size: x-small; margin-top: -5px; border: 1px #f0f0e0 solid; }");
            wrtr.WriteLine("td { color: #000000; font-family: Verdana; font-size: .7em; border: solid 1px;  }");
            wrtr.WriteLine("h2 { font-size: 1.5em; font-weight: bold; margin-top: 25px; margin-bottom: 10px; border-top: 1px solid #003366; margin-left: -15px; color: #003366; }");
            wrtr.WriteLine("h3 { font-size: 1.1em; color: #000000; margin-left: -15px; margin-top: 10px; margin-bottom: 10px; }");
            wrtr.WriteLine("ul, ol { margin-top: 10px; margin-left: 20px; }");
            wrtr.WriteLine("li { margin-top: 10px; color: #000000; }");
            wrtr.WriteLine("font.value { color: darkblue; font: bold; }");
            wrtr.WriteLine("font.key { color: darkgreen; font: bold; }");
            wrtr.WriteLine(".heading1 { color: #ffffff; font-family: Tahoma; font-size: 26px; font-weight: normal; background-color: #003366; margin-top: 0px; margin-bottom: 0px; margin-left: -30px; padding-top: 10px; padding-bottom: 3px; padding-left: 15px; width: 105%; }");
            wrtr.WriteLine(".intro { margin-left: -15px; }");
            wrtr.WriteLine("table { border: solid 1px; }");

            wrtr.WriteEndTag("style");
            wrtr.WriteLine();
        }

        private static void WriteTitle(
            HtmlTextWriter wrtr, 
            string title)
        {
            wrtr.WriteFullBeginTag("title");
            wrtr.Write(title);
            wrtr.WriteEndTag("title");
            wrtr.WriteLine();
        }

        public static void WriteBody(
            HtmlTextWriter wrtr, 
            Type type,
            bool autoDocVersion)
        {
            wrtr.WriteFullBeginTag("body");
            wrtr.WriteLine();

            WriteType(wrtr, type);         
            wrtr.WriteLine();
      
            WriteFooter(wrtr, type, autoDocVersion);

            wrtr.WriteEndTag("div");
            wrtr.WriteLine();
            wrtr.WriteEndTag("body");
            wrtr.WriteLine();
        }

        public static void WriteType(
            HtmlTextWriter wrtr, 
            Type type)
        {
            var structs = new ArrayList();
      
            wrtr.WriteBeginTag("div");
            wrtr.WriteAttribute("id", "content");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.WriteLine();

            var svcInfo = XmlRpcServiceInfo.CreateServiceInfo(type);

            wrtr.WriteBeginTag("p");
            wrtr.WriteAttribute("class", "heading1");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.Write(svcInfo.Name);
            wrtr.WriteEndTag("p");
            wrtr.WriteFullBeginTag("br");
            wrtr.WriteEndTag("br");
            wrtr.WriteLine();

            if (!string.IsNullOrEmpty(svcInfo.Doc))
            {
                wrtr.WriteBeginTag("p");
                wrtr.WriteAttribute("class", "intro");
                wrtr.Write(HtmlTextWriter.TagRightChar);
                wrtr.Write(svcInfo.Doc);
                wrtr.WriteEndTag("p");
                wrtr.WriteLine();
            }

            wrtr.WriteBeginTag("p");
            wrtr.WriteAttribute("class", "intro");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.Write("The following methods are supported:");
            wrtr.WriteEndTag("p");
            wrtr.WriteLine();

            wrtr.WriteFullBeginTag("ul");
            wrtr.WriteLine();

            foreach (XmlRpcMethodInfo mthdInfo in svcInfo.Methods)
            {
                if (mthdInfo.IsHidden)
                    continue;
                
                wrtr.WriteFullBeginTag("li");
                wrtr.WriteBeginTag("a");
                wrtr.WriteAttribute("href", "#" + mthdInfo.XmlRpcName);
                wrtr.Write(HtmlTextWriter.TagRightChar);
                wrtr.Write(mthdInfo.XmlRpcName);
                wrtr.WriteEndTag("a");
                wrtr.WriteEndTag("li");
                wrtr.WriteLine();
            }
     
            wrtr.WriteEndTag("ul");
            wrtr.WriteLine();

            foreach (var mthdInfo in svcInfo.Methods)
            {
                if (!mthdInfo.IsHidden)
                    WriteMethod(wrtr, mthdInfo, structs);
            }

            for (var j = 0; j < structs.Count; j++)
                WriteStruct(wrtr, structs[j] as Type, structs);

            wrtr.WriteEndTag("div");
            wrtr.WriteLine();
        }

        private static void WriteMethod(
            HtmlTextWriter wrtr, 
            XmlRpcMethodInfo mthdInfo,
            IList structs)
        {
            wrtr.WriteFullBeginTag("span");
            wrtr.WriteLine();
            wrtr.WriteFullBeginTag("h2");
            wrtr.WriteBeginTag("a");
            wrtr.WriteAttribute("name", "#" + mthdInfo.XmlRpcName);
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.Write("method " + mthdInfo.XmlRpcName);
            wrtr.WriteEndTag("a");
            wrtr.WriteEndTag("h2");
            wrtr.WriteLine();

            if (!string.IsNullOrEmpty(mthdInfo.Doc))
            {
                wrtr.WriteBeginTag("p");
                wrtr.WriteAttribute("class", "intro");
                wrtr.Write(HtmlTextWriter.TagRightChar);
                wrtr.Write(mthdInfo.Doc);
                wrtr.WriteEndTag("p");
                wrtr.WriteLine();
            }
      
            wrtr.WriteFullBeginTag("h3");
            wrtr.Write("Parameters");
            wrtr.WriteEndTag("h3");
            wrtr.WriteLine();

            wrtr.WriteBeginTag("table");
            wrtr.WriteAttribute("cellspacing", "0");
            wrtr.WriteAttribute("cellpadding", "5");
            wrtr.WriteAttribute("width", "90%");
            wrtr.Write(HtmlTextWriter.TagRightChar);

            if (mthdInfo.Parameters.Length > 0)
            {
                foreach (var parInfo in mthdInfo.Parameters)
                {
                    wrtr.WriteFullBeginTag("tr");
                    wrtr.WriteBeginTag("td");
                    wrtr.WriteAttribute("width", "33%");
                    wrtr.Write(HtmlTextWriter.TagRightChar);
                    WriteType(wrtr, parInfo.Type, parInfo.IsParams, structs);
                    wrtr.WriteEndTag("td");

                    wrtr.WriteFullBeginTag("td");
                    if (string.IsNullOrEmpty(parInfo.Doc))
                        wrtr.Write(parInfo.Name);
                    else
                    {
                        wrtr.Write(parInfo.Name);
                        wrtr.Write(" - ");
                        wrtr.Write(parInfo.Doc);
                    }

                    wrtr.WriteEndTag("td");
                    wrtr.WriteEndTag("tr");
                }
            }
            else
            {
                wrtr.WriteFullBeginTag("tr");
                wrtr.WriteBeginTag("td");
                wrtr.WriteAttribute("width", "33%");
                wrtr.Write(HtmlTextWriter.TagRightChar);
                wrtr.Write("none");
                wrtr.WriteEndTag("td");
                wrtr.WriteFullBeginTag("td");
                wrtr.Write("&nbsp;");
                wrtr.WriteEndTag("td");
                wrtr.WriteEndTag("tr");
            }

            wrtr.WriteEndTag("table");
            wrtr.WriteLine();
            wrtr.WriteFullBeginTag("h3");
            wrtr.Write("Return Value");
            wrtr.WriteEndTag("h3");
            wrtr.WriteLine();

            wrtr.WriteBeginTag("table");
            wrtr.WriteAttribute("cellspacing", "0");
            wrtr.WriteAttribute("cellpadding", "5");
            wrtr.WriteAttribute("width", "90%");
            wrtr.Write(HtmlTextWriter.TagRightChar);

            wrtr.WriteFullBeginTag("tr");

            wrtr.WriteBeginTag("td");
            wrtr.WriteAttribute("width", "33%");
            wrtr.Write(HtmlTextWriter.TagRightChar);
            WriteType(wrtr, mthdInfo.ReturnType, false, structs);
            wrtr.WriteEndTag("td");

            wrtr.WriteFullBeginTag("td");

            if (!string.IsNullOrEmpty(mthdInfo.ReturnDoc))
                wrtr.Write(mthdInfo.ReturnDoc);
            else
                wrtr.Write("&nbsp;");
            
            wrtr.WriteEndTag("td");
        
            wrtr.WriteEndTag("tr");

            wrtr.WriteEndTag("table");
            wrtr.WriteLine();

            wrtr.WriteEndTag("span");
            wrtr.WriteLine();
        }

        private static void WriteStruct(
            HtmlTextWriter wrtr, 
            Type structType,
            IList structs)
        {
            wrtr.WriteFullBeginTag("span");
            wrtr.WriteLine();
            wrtr.WriteFullBeginTag("h2");
            wrtr.WriteBeginTag("a");
            wrtr.WriteAttribute("name", "#" + structType.Name);
            wrtr.Write(HtmlTextWriter.TagRightChar);
            wrtr.Write("struct " + structType.Name);
            wrtr.WriteEndTag("a");
            wrtr.WriteEndTag("h2");
            wrtr.WriteLine();
    
            wrtr.WriteFullBeginTag("h3");
            wrtr.Write("Members");
            wrtr.WriteEndTag("h3");
            wrtr.WriteLine();    
            wrtr.WriteEndTag("span");
            wrtr.WriteLine();  
   
            wrtr.WriteBeginTag("table");
            wrtr.WriteAttribute("cellspacing", "0");
            wrtr.WriteAttribute("cellpadding", "5");
            wrtr.WriteAttribute("width", "90%");
            wrtr.Write(HtmlTextWriter.TagRightChar);

            var structAction = MappingAction.Error;
            var structAttr = Attribute.GetCustomAttribute(structType,  typeof(XmlRpcMissingMappingAttribute)) as XmlRpcMissingMappingAttribute;
            if (structAttr != null)
                structAction = structAttr.Action;

            var mis = structType.GetMembers();
            foreach (MemberInfo mi in mis)
            {
                if (mi.MemberType == MemberTypes.Field)
                {
                    var fi = (FieldInfo)mi;
        
                    wrtr.WriteFullBeginTag("tr");

                    wrtr.WriteBeginTag("td");
                    wrtr.WriteAttribute("width", "33%");
                    wrtr.Write(HtmlTextWriter.TagRightChar);
                    WriteType(wrtr, fi.FieldType, false, structs);
                    wrtr.WriteEndTag("td");

                    wrtr.WriteFullBeginTag("td");
                    var memberAction = structAction;
                    var attr = Attribute.GetCustomAttribute(fi, typeof(XmlRpcMissingMappingAttribute)) as XmlRpcMissingMappingAttribute;
                    if (attr != null)
                        memberAction = attr.Action;
                    
                    var memberName = fi.Name + " ";
                    var desc = string.Empty;
                    var mmbrAttr = Attribute.GetCustomAttribute(fi, typeof(XmlRpcMemberAttribute)) as XmlRpcMemberAttribute;
                    if (mmbrAttr != null)
                    {
                        if (!string.IsNullOrEmpty(mmbrAttr.Member))
                            memberName = mmbrAttr.Member + " ";
                        desc = mmbrAttr.Description;
                    }

                    if (memberAction == MappingAction.Ignore)
                        memberName += " (optional) ";
                    
                    if (!string.IsNullOrWhiteSpace(desc))
                        memberName = memberName + "- " + desc;
                    
                    wrtr.Write(memberName);
                    wrtr.WriteEndTag("td");
                 
                    wrtr.WriteEndTag("tr");
                }
            }

            wrtr.WriteEndTag("table");
            wrtr.WriteLine();
        }

        private static void WriteType(
            HtmlTextWriter wrtr, 
            Type type,
            bool isparams,
            IList structs)
        {
            // TODO: following is hack for case when type is Object
            string xmlRpcType;
            if (!isparams)
            {
                xmlRpcType = type != typeof(object) 
                    ? XmlRpcTypeInfo.GetXmlRpcTypeString(type) 
                    : "any";
            }
            else
                xmlRpcType = "varargs";
            
            wrtr.Write(xmlRpcType);
            if (string.Equals(xmlRpcType, "struct", StringComparison.Ordinal) && type != typeof(XmlRpcStruct))
            {
                if (!structs.Contains(type))
                    structs.Add(type);

                wrtr.Write(" ");
                wrtr.WriteBeginTag("a");
                wrtr.WriteAttribute("href", "#" + type.Name);
                wrtr.Write(HtmlTextWriter.TagRightChar);
                wrtr.Write(type.Name);
                wrtr.WriteEndTag("a");
            }
            else if (string.Equals(xmlRpcType, "array", StringComparison.Ordinal) 
                || string.Equals(xmlRpcType, "varargs", StringComparison.Ordinal))
            {
                if (type.GetArrayRank() == 1)  // single dim array
                {
                    wrtr.Write(" of ");
                    var elemType = type.GetElementType();
                    string elemXmlRpcType;
                    elemXmlRpcType = elemType != typeof(object) 
                        ? XmlRpcTypeInfo.GetXmlRpcTypeString(elemType) 
                        : "any";
                    
                    wrtr.Write(elemXmlRpcType);
                    if (string.Equals(elemXmlRpcType, "struct", StringComparison.Ordinal) && elemType != typeof(XmlRpcStruct))
                    {
                        if (!structs.Contains(elemType))
                            structs.Add(elemType);
                        
                        wrtr.Write(" ");
                        wrtr.WriteBeginTag("a");
                        wrtr.WriteAttribute("href", "#" + elemType.Name);
                        wrtr.Write(HtmlTextWriter.TagRightChar);
                        wrtr.Write(elemType.Name);
                        wrtr.WriteEndTag("a");
                    }
                }
            }
        }
    }
}