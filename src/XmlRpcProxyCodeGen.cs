/* 
XML-RPC.NET  proxy class code generator
Copyright (c) 2003, Joe Bork <joe@headblender.com>
Portions Copyright (c) 2001-2003, Charles Cook <ccook@cookcomputing.com>

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

    using System.Globalization;
    using System.IO;
    using System.CodeDom;
    using System.CodeDom.Compiler;

    using CookComputing.XmlRpc;

    public sealed class XmlRpcProxyCodeGen
    {
        private const string DEFAULT_RET = "xrtReturn";
        private const string DEFAULT_TEMP = "xrtTemp";
        private const string DEFAULT_ARR = "xrtArray";
        private const string DEFAULT_CALLBACK = "xrtCallback";
        private const string DEFAULT_STATUS = "xrtStatus";
        private const string DEFAULT_RESULT = "xrtResult";

        private const string DEFAULT_SUFFIX = "RpcProxy";
        private const string DEFAULT_END = "End";
        private const string DEFAULT_BEGIN = "Begin";

        private XmlRpcProxyCodeGen()
        {
            // no public constructor where all public methods are static
        }

        private delegate void BuildMethodDelegate(
			CodeTypeDeclaration declaration, 
			string methodName,
			string rpcMethodName,
			Type[] argTypes,
			string[] argNames,
			Type returnType,
			Type implementationType);

        public static string CreateCode(
            Type proxyType, 
            ICodeGenerator generator)
        {
            return CreateCode(proxyType, generator, new XmlRpcProxyCodeGenOptions());
        }

        public static string CreateCode(
            Type proxyType, 
            ICodeGenerator generator,
            XmlRpcProxyCodeGenOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(
                    "options",
                    "The options parameter cannot be null");
            }

            CodeCompileUnit ccu = CreateCodeCompileUnit(proxyType, options);

            var cgo = new CodeGeneratorOptions();
            cgo.BlankLinesBetweenMembers = true;
            cgo.BracingStyle = "C";

            using (var sw = new StringWriter(CultureInfo.InvariantCulture))
            {
                generator.GenerateCodeFromCompileUnit(ccu, sw, cgo);
                return sw.ToString();
            }
        }

        public static CodeCompileUnit CreateCodeCompileUnit(Type proxyType)
        {
            return CreateCodeCompileUnit(proxyType, new XmlRpcProxyCodeGenOptions());
        }

        public static CodeCompileUnit CreateCodeCompileUnit(
            Type proxyType, 
            XmlRpcProxyCodeGenOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(
                    "options",
                    "The options parameter cannot be null");
            }

            // create unique names
            string baseName = proxyType.Name;

            // string leading "I"
            if (baseName.StartsWith("I", StringComparison.Ordinal))
                baseName = baseName.Remove(0, 1);

            string assemblyName = options.Namespace.Length > 0 
                ? options.Namespace 
                : string.Format(
                                      CultureInfo.InvariantCulture, 
                                      "{0}{1}", 
                                      baseName, 
                                      DEFAULT_SUFFIX);

            string typeName = options.TypeName.Length > 0 
                ? options.TypeName 
                : assemblyName;

            var implicitAsync = options.ImplicitAsync;
            var flattenInterfaces = options.FlattenInterfaces;

            return BuildCompileUnit(
                proxyType, 
                assemblyName, 
                typeName, 
                implicitAsync, 
                flattenInterfaces);
        }

        private static CodeCompileUnit BuildCompileUnit(
            Type proxyType, 
            string assemblyName,
            string typeName,
            bool implicitAsync,
            bool flattenInterfaces)
        {
            var urlString = GetXmlRpcUrl(proxyType);
            var methods = GetXmlRpcMethods(proxyType, flattenInterfaces);
            var beginMethods = GetXmlRpcBeginMethods(proxyType, flattenInterfaces);
            var endMethods = GetXmlRpcEndMethods(proxyType, flattenInterfaces);

            // if there are no Begin and End methods,
            // we can implicitly generate them
            if ((beginMethods.Count == 0) && (endMethods.Count == 0) && implicitAsync)
            {
                beginMethods = GetXmlRpcMethods(proxyType, flattenInterfaces);
                endMethods = GetXmlRpcMethods(proxyType, flattenInterfaces);
            }

            var ccu = new CodeCompileUnit();
            var cn = new CodeNamespace(assemblyName);

            cn.Imports.Add(new CodeNamespaceImport("System"));
            cn.Imports.Add(new CodeNamespaceImport(proxyType.Namespace));
            cn.Imports.Add(new CodeNamespaceImport("CookComputing.XmlRpc"));

            var ctd = new CodeTypeDeclaration(typeName);

            // its a class
            ctd.IsClass = true;

            // class is public and sealed
            ctd.TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed;

            // class derives from XmlRpcClientProtocol
            ctd.BaseTypes.Add(typeof(XmlRpcClientProtocol));

            // and implements I(itf)
            ctd.BaseTypes.Add(proxyType);

            BuildConstructor(ctd, urlString);
            BuildMethods(ctd, methods, new BuildMethodDelegate(BuildStandardMethod));
            BuildMethods(ctd, beginMethods, new BuildMethodDelegate(BuildBeginMethod));
            BuildMethods(ctd, endMethods, new BuildMethodDelegate(BuildEndMethod));

            cn.Types.Add(ctd);
            ccu.Namespaces.Add(cn);

            return ccu;
        }

        private static void BuildMethods(
            CodeTypeDeclaration declaration, 
            Hashtable methods,
            BuildMethodDelegate buildDelegate)
        {
            foreach (DictionaryEntry de in methods)
            {
                var mthdData = (MethodData)de.Value;
                var mi = mthdData.mi;
                var argTypes = new Type[mi.GetParameters().Length];
                var argNames = new string[mi.GetParameters().Length];
                for (int i = 0; i < mi.GetParameters().Length; i++)
                {
                    argTypes[i] = mi.GetParameters()[i].ParameterType;
                    argNames[i] = mi.GetParameters()[i].Name;
                }

                var n = (string)de.Key;
                buildDelegate(
                    declaration, 
                    n, 
                    mthdData.xmlRpcName, 
                    argTypes, 
                    argNames, 
                    mi.ReturnType, 
                    mthdData.implementationType);
            }
        }

        private static void BuildStandardMethod(
            CodeTypeDeclaration declaration, 
            string methodName,
            string rpcMethodName,
            Type[] argTypes,
            string[] argNames,
            Type returnType,
            Type implementationType)
        {
            var cmm = new CodeMemberMethod();

            // normal, unqualified type names are public
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            cmm.ImplementationTypes.Add(implementationType);
            cmm.Name = methodName;
			
            // set the return type
            var ctrReturn = new CodeTypeReference(returnType);
            cmm.ReturnType = ctrReturn;

            MakeParameterList(cmm, argTypes, argNames);

            // add an XmlRpcMethod attribute to the type
            var cad = new CodeAttributeDeclaration();
            cad.Name = typeof(XmlRpcMethodAttribute).FullName;

            var caa = new CodeAttributeArgument();
            var cpe = new CodePrimitiveExpression(rpcMethodName);
            caa.Value = cpe;

            cad.Arguments.Add(caa);

            cmm.CustomAttributes.Add(cad);

            // generate the method body:

            // if non-void return, declared locals for processing return value
            if (returnType != typeof(void))
            {
                // add some local variables
                MakeTempVariable(cmm, typeof(object));
                MakeReturnVariable(cmm, returnType);
            }

            MakeTempParameterArray(cmm, argTypes, argNames);

            // construct a call to the base Invoke method
            var ctre = new CodeThisReferenceExpression();

            var cmre = new CodeMethodReferenceExpression(ctre, "Invoke");
			
            var cmie = new CodeMethodInvokeExpression();
            cmie.Method = cmre;
            cmie.Parameters.Add(new CodePrimitiveExpression(methodName));
            cmie.Parameters.Add(new CodeVariableReferenceExpression(DEFAULT_ARR));

            if (returnType != typeof(void))
            {
                // assign the result to tempRetVal
                var casTemp = new CodeAssignStatement();
                casTemp.Left = new CodeVariableReferenceExpression(DEFAULT_TEMP);
                casTemp.Right = cmie;

                cmm.Statements.Add(casTemp);
            }
            else
            {
                // discard return type
                cmm.Statements.Add(cmie);
            }

            MakeReturnStatement(cmm, returnType);

            // add the finished method to the type
            declaration.Members.Add(cmm);
        }

        private static void BuildBeginMethod(
            CodeTypeDeclaration declaration, 
            string methodName,
            string rpcMethodName,
            Type[] argTypes,
            string[] argNames,
            Type returnType,
            Type implementationType)
        {
            string beginMethodName;

            var cmm = new CodeMemberMethod();

            // set the attributes and name
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            if (methodName.StartsWith(DEFAULT_BEGIN, StringComparison.Ordinal))
            {
                // strip method name prefix
                cmm.Name = methodName.Substring(DEFAULT_BEGIN.Length, methodName.Length - DEFAULT_BEGIN.Length);
            }
            beginMethodName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", DEFAULT_BEGIN, methodName);

            cmm.Name = beginMethodName;

            //!cmm.ImplementationTypes.Add(implementationType);
			
            // set the return type (always IAsyncResult)
            cmm.ReturnType = new CodeTypeReference(typeof(IAsyncResult));

            MakeParameterList(cmm, argTypes, argNames);

            // add callback and state params
            cmm.Parameters.Add(new CodeParameterDeclarationExpression(
                typeof(AsyncCallback), 
                DEFAULT_CALLBACK)
            );

            cmm.Parameters.Add(new CodeParameterDeclarationExpression(
                typeof(object),
                DEFAULT_STATUS)
            );

            MakeReturnVariable(cmm, typeof(IAsyncResult));

            MakeTempParameterArray(cmm, argTypes, argNames);

            // construct a call to the base beginInvoke method

            var ctre = new CodeThisReferenceExpression();
            var cmre = new CodeMethodReferenceExpression(ctre, "BeginInvoke");
            var cmie = new CodeMethodInvokeExpression();
            cmie.Method = cmre;
            cmie.Parameters.Add(new CodePrimitiveExpression(methodName));
            cmie.Parameters.Add(new CodeVariableReferenceExpression(DEFAULT_ARR));
            cmie.Parameters.Add(new CodeVariableReferenceExpression(DEFAULT_CALLBACK));
            cmie.Parameters.Add(new CodeVariableReferenceExpression(DEFAULT_STATUS));

            // assign the result to RetVal
            var casTemp = new CodeAssignStatement();
            casTemp.Left = new CodeVariableReferenceExpression(DEFAULT_RET);
            casTemp.Right = cmie;

            cmm.Statements.Add(casTemp);

            // return retVal
            var cmrsCast = new CodeMethodReturnStatement();
            cmrsCast.Expression = new CodeVariableReferenceExpression(DEFAULT_RET);

            cmm.Statements.Add(cmrsCast);

            // add the finished method to the type
            declaration.Members.Add(cmm);
        }

        private static void BuildEndMethod(
            CodeTypeDeclaration declaration, 
            string methodName,
            string rpcMethodName,
            Type[] argTypes,
            string[] argNames,
            Type returnType,
            Type implementationType)
        {
            string endMethodName;

            var cmm = new CodeMemberMethod();

            // set the attributes and name
            cmm.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            if (methodName.StartsWith(DEFAULT_END, StringComparison.Ordinal))
            {
                // strip method name prefix
                cmm.Name = methodName.Substring(DEFAULT_END.Length, methodName.Length - DEFAULT_END.Length);
            }
            endMethodName = string.Format(CultureInfo.InvariantCulture, "{0}{1}", DEFAULT_END, methodName);

            cmm.Name = endMethodName;

            //!cmm.ImplementationTypes.Add(implementationType);
			
            // set the return type
            var ctrReturn = new CodeTypeReference(returnType);
            cmm.ReturnType = ctrReturn;

            // set the parameter list (always a single IAsyncResult)
            var cpde = new CodeParameterDeclarationExpression();

            cpde.Name = DEFAULT_RESULT;
            cpde.Type = new CodeTypeReference(typeof(IAsyncResult));

            cmm.Parameters.Add(cpde);

            // generate the method body:

            // if non-void return, declared locals for processing return value
            if (returnType != typeof(void))
            {
                // add some local variables:
                MakeTempVariable(cmm, typeof(object));
                MakeReturnVariable(cmm, returnType);
            }

            // construct a call to the base EndInvoke method

            var ctre = new CodeThisReferenceExpression();

            var cmre = new CodeMethodReferenceExpression(ctre, "EndInvoke");
			
            var cmie = new CodeMethodInvokeExpression();
            cmie.Method = cmre;
            cmie.Parameters.Add(new CodeVariableReferenceExpression(DEFAULT_RESULT));

            var cie = new CodeIndexerExpression();
            cie.TargetObject = cmie;
            cie.Indices.Add(new CodePrimitiveExpression(0));

            if (returnType != typeof(void))
            {
                // assign the result to tempRetVal
                var casTemp = new CodeAssignStatement();
                casTemp.Left = new CodeVariableReferenceExpression(DEFAULT_TEMP);
                //!casTemp.Right = cie;
                casTemp.Right = cmie;

                cmm.Statements.Add(casTemp);
            }
            else
            {
                // discard return type
                //!cmm.Statements.Add(cie);
                cmm.Statements.Add(cmie);
            }

            MakeReturnStatement(cmm, returnType);

            // add the finished method to the type
            declaration.Members.Add(cmm);
        }

        private static void BuildConstructor(
            CodeTypeDeclaration declaration, 
            string urlStr)
        {
            var cc = new CodeConstructor();

            if (!string.IsNullOrEmpty(urlStr))
            {
                // add an XmlRpcUrl attribute to the type
                var cad = new CodeAttributeDeclaration();
                cad.Name = typeof(XmlRpcUrlAttribute).FullName;

                var caa = new CodeAttributeArgument();
                var cpe = new CodePrimitiveExpression(urlStr);
                caa.Value = cpe;

                cad.Arguments.Add(caa);
                declaration.CustomAttributes.Add(cad);
            }

            // call the base constructor:
            cc.Attributes = MemberAttributes.Public;

            // add the constructor to the type
            declaration.Members.Add(cc);
        }

        private static string GetXmlRpcUrl(MemberInfo proxyType)
        {
            var attr = Attribute.GetCustomAttribute(proxyType, typeof(XmlRpcUrlAttribute)) as XmlRpcUrlAttribute;
            return attr == null ? null : attr.Uri;
        }

        private static Hashtable GetXmlRpcMethods(Type proxyType, bool flatten)
        {
            var ret = new Hashtable();

            RecurseGetXmlRpcMethods(proxyType, ref ret, flatten);

            return ret;
        }

        private static void RecurseGetXmlRpcMethods(Type proxyType, ref Hashtable h, bool flatten)
        {
            if (!proxyType.IsInterface)
                throw new Exception("type not interface");

            foreach (MethodInfo mi in proxyType.GetMethods())
            {
                var xmlRpcName = GetXmlRpcMethodName(mi);
                if (xmlRpcName == null)
                    continue;

                var n = mi.Name;

                if (h.Contains(n))
                    throw new Exception("duplicate method name encountered in type hierarchy");

                // add new method
                h.Add(n, new MethodData(mi, xmlRpcName, mi.ReturnType, proxyType));
            }

            if (flatten)
            {
                var ifs = proxyType.GetInterfaces();
                for (int i = 0; i < ifs.Length; ++i)
                    RecurseGetXmlRpcMethods(ifs[i], ref h, flatten);
            }
        }

        private static Hashtable GetXmlRpcBeginMethods(Type proxyType, bool flatten)
        {
            var ret = new Hashtable();

            RecurseGetXmlRpcBeginMethods(proxyType, ref ret, flatten);

            return ret;
        }

        private static void RecurseGetXmlRpcBeginMethods(Type proxyType, ref Hashtable h, bool flatten)
        {
            if (!proxyType.IsInterface)
                throw new Exception("type not interface");
            
            foreach (var mi in proxyType.GetMethods())
            {
                var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcBeginAttribute)) as XmlRpcBeginAttribute;
                if (attr == null)
                    continue;
                
                var rpcMethod = attr.Method;
                if (rpcMethod.Length == 0)
                {
                    if (!mi.Name.StartsWith("Begin", StringComparison.Ordinal) || mi.Name.Length <= 5)
                        throw new Exception(
                            string.Format(
                                CultureInfo.InvariantCulture, 
                                "method {0} has invalid signature for begin method", 
                                mi.Name));

                    rpcMethod = mi.Name.Substring(5);
                }

                var paramCount = mi.GetParameters().Length;
                int i;
                for (i = 0; i < paramCount; i++)
                {
                    var paramType = mi.GetParameters()[0].ParameterType;
                    if (paramType == typeof(AsyncCallback))
                        break;
                }

                if (paramCount > 1)
                {
                    if (i < paramCount - 2)
                        throw new Exception(
                            string.Format(
                                CultureInfo.InvariantCulture, 
                                "method {0} has invalid signature for begin method", 
                                mi.Name));
                    
                    if (i == (paramCount - 2))
                    {
                        var paramType = mi.GetParameters()[i + 1].ParameterType;
                        if (paramType != typeof(object))
                            throw new Exception(
                                string.Format(
                                    CultureInfo.InvariantCulture, 
                                    "method {0} has invalid signature for begin method", 
                                    mi.Name));
                    }
                }
                var n = mi.Name;

                if (h.Contains(n))
                    throw new Exception("duplicate begin method name encountered in type hierarchy");

                h.Add(n, new MethodData(mi, rpcMethod, null, null));
            }

            if (flatten)
            {
                var ifs = proxyType.GetInterfaces();
                for (int i = 0; i < ifs.Length; ++i)
                    RecurseGetXmlRpcBeginMethods(ifs[i], ref h, flatten);
            }
        }

        private static Hashtable GetXmlRpcEndMethods(Type proxyType, bool flatten)
        {
            var ret = new Hashtable();

            RecurseGetXmlRpcEndMethods(proxyType, ref ret, flatten);

            return ret;
        }

        private static void RecurseGetXmlRpcEndMethods(Type proxyType, ref Hashtable h, bool flatten)
        {
            if (!proxyType.IsInterface)
                throw new Exception("type not interface");
            
            foreach (var mi in proxyType.GetMethods())
            {
                var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcEndAttribute)) as XmlRpcEndAttribute;
                if (attr == null)
                    continue;
                
                if (mi.GetParameters().Length != 1)
                    throw new Exception(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            "method {0} has invalid signature for end method", 
                            mi.Name));
                
                var paramType = mi.GetParameters()[0].ParameterType;
                if (paramType != typeof(IAsyncResult))
                    throw new Exception(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            "method {0} has invalid signature for end method", 
                            mi.Name));

                var n = mi.Name;

                if (h.Contains(n))
                    throw new Exception("duplicate end method name encountered in type hierarchy");

                h.Add(h, new MethodData(mi, "", null, null));
            }

            if (flatten)
            {
                var ifs = proxyType.GetInterfaces();
                for (var i = 0; i < ifs.Length; ++i)
                {
                    RecurseGetXmlRpcEndMethods(ifs[i], ref h, flatten);
                }
            }
        }

		
        private static string GetXmlRpcMethodName(MemberInfo mi)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            if (attr == null)
                return null;

            var rpcMethod = attr.Method;
            return rpcMethod.Length == 0 
                ? mi.Name 
                : rpcMethod;
        }

        private class MethodData
        {
            public MethodData(MethodInfo Mi, string XmlRpcName, Type ReturnType, Type ImplementationType)
            {
                mi = Mi;
                xmlRpcName = XmlRpcName;
                returnType = ReturnType;
                implementationType = ImplementationType;
            }

            public MethodInfo mi { get; set; }

            public string xmlRpcName { get; set; }

            public Type returnType { get; set; }

            public Type implementationType { get; set; }
        }

        private static void MakeParameterList(CodeMemberMethod method, Type[] types, System.Collections.Generic.IList<string> names)
        {
            // set the parameter list
            for (var i = 0; i < types.Length; ++i)
            {
                var cpde = new CodeParameterDeclarationExpression();

                cpde.Name = names[i];

                var ctr = new CodeTypeReference(types[i]);
                cpde.Type = ctr;

                method.Parameters.Add(cpde);
            }
        }

        private static void MakeReturnVariable(CodeMemberMethod method, Type returnType)
        {
            // return variable
            var cvdsRet = new CodeVariableDeclarationStatement();
            cvdsRet.Name = DEFAULT_RET;
            cvdsRet.Type = new CodeTypeReference(returnType);

            if (!returnType.IsValueType)
                cvdsRet.InitExpression = new CodePrimitiveExpression(null);

            method.Statements.Add(cvdsRet);
        }

        private static void MakeTempVariable(CodeMemberMethod method, Type tempType)
        {
            // temp object variable
            var cvdsTemp = new CodeVariableDeclarationStatement();
            cvdsTemp.Name = DEFAULT_TEMP;
            cvdsTemp.Type = new CodeTypeReference(tempType);

            if (!tempType.IsValueType)
                cvdsTemp.InitExpression = new CodePrimitiveExpression(null);

            method.Statements.Add(cvdsTemp);
        }

        private static void MakeTempParameterArray(CodeMemberMethod method, Array types, System.Collections.Generic.IList<string> names)
        {
            // declare array variable to store method args
            var cvdsArr = new CodeVariableDeclarationStatement();
            cvdsArr.Name = DEFAULT_ARR;

            var ctrArrType = new CodeTypeReference(typeof(object));
            var ctrArr = new CodeTypeReference(ctrArrType, 1);

            cvdsArr.Type = ctrArr;

            // gen code to initialize the array
            var cace = new CodeArrayCreateExpression(typeof(object), 1);

            // create array initializers
            for (int i = 0; i < types.Length; ++i)
            {
                var care = new CodeArgumentReferenceExpression();
                care.ParameterName = names[i];
                cace.Initializers.Add(care);
            }

            cvdsArr.InitExpression = cace;

            method.Statements.Add(cvdsArr);
        }

        private static void MakeReturnStatement(CodeMemberMethod method, Type returnType)
        {
            if (returnType != typeof(void))
            {
                // create a cast statement
                var cce = new CodeCastExpression(
                    returnType,
                    new CodeVariableReferenceExpression(DEFAULT_TEMP));

                var casCast = new CodeAssignStatement();
                casCast.Left = new CodeVariableReferenceExpression(DEFAULT_RET);
                casCast.Right = cce;

                method.Statements.Add(casCast);

                // return retVal
                var cmrsCast = new CodeMethodReturnStatement();
                cmrsCast.Expression = new CodeVariableReferenceExpression(DEFAULT_RET);

                method.Statements.Add(cmrsCast);
            }
            else
            {
                // construct an undecorated return statement
                method.Statements.Add(new CodeMethodReturnStatement());
            }
        }
    }
}
