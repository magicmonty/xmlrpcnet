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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    public static class XmlRpcProxyGen
    {
        static Dictionary<Type, Type> _types = new Dictionary<Type, Type>();

        public static T Create<T>()
        {
            return (T)Create(typeof(T));
        }

        public static object Create(Type itf)
        {
            // create transient assembly
            Type proxyType;
            lock (typeof(XmlRpcProxyGen))
            {
                if (!_types.ContainsKey(itf))
                {
                    var guid = Guid.NewGuid();
                    var assemblyName = "XmlRpcProxy" + guid;
                    var moduleName = string.Format("XmlRpcProxy{0}.dll", guid);
                    var typeName = string.Format("XmlRpcProxy{0}", guid);
                    var assBldr = BuildAssembly(itf, assemblyName, moduleName, typeName, AssemblyBuilderAccess.Run);
                    proxyType = assBldr.GetType(typeName);
                    _types.Add(itf, proxyType);
                }
                proxyType = _types[itf]; 
            }

            return Activator.CreateInstance(proxyType);
        }

        public static object CreateAssembly(
            Type itf,
            string typeName,
            string assemblyName)
        {
            // create persistable assembly
            if (assemblyName.IndexOf(".dll", StringComparison.Ordinal) == (assemblyName.Length - 4))
                assemblyName = assemblyName.Substring(0, assemblyName.Length - 4);
            
            var moduleName = assemblyName + ".dll";
            var assBldr = BuildAssembly(itf, assemblyName, moduleName, typeName, AssemblyBuilderAccess.RunAndSave);
            var proxyType = assBldr.GetType(typeName);
            var ret = Activator.CreateInstance(proxyType);
            assBldr.Save(moduleName);
            return ret;
        }

        private static AssemblyBuilder BuildAssembly(
            Type itf,
            string assemblyName,
            string moduleName,
            string typeName,
            AssemblyBuilderAccess access)
        {
            var urlString = GetXmlRpcUrl(itf);
            var methods = GetXmlRpcMethods(itf);
            var beginMethods = GetXmlRpcBeginMethods(itf);
            var endMethods = GetXmlRpcEndMethods(itf);
            var assName = new AssemblyName();
            assName.Name = assemblyName;
            if (access == AssemblyBuilderAccess.RunAndSave)
                assName.Version = itf.Assembly.GetName().Version;
            var assBldr = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, access);
            var modBldr = (access == AssemblyBuilderAccess.Run
                ? assBldr.DefineDynamicModule(assName.Name)
                : assBldr.DefineDynamicModule(assName.Name, moduleName));
            
            var typeBldr = modBldr.DefineType(
                typeName,
                TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Public,
                typeof(XmlRpcClientProtocol),
                new [] { itf });
            
            BuildConstructor(typeBldr, typeof(XmlRpcClientProtocol), urlString);
            BuildMethods(typeBldr, methods);
            BuildBeginMethods(typeBldr, beginMethods);
            BuildEndMethods(typeBldr, endMethods);
            typeBldr.CreateType();

            return assBldr;
        }

        private static void BuildMethods(TypeBuilder tb, List<MethodData> methods)
        {
            foreach (MethodData mthdData in methods)
            {
                var mi = mthdData.Mi;
                var argTypes = new Type[mi.GetParameters().Length];
                var paramNames = new string[mi.GetParameters().Length];
                for (var i = 0; i < mi.GetParameters().Length; i++)
                {
                    argTypes[i] = mi.GetParameters()[i].ParameterType;
                    paramNames[i] = mi.GetParameters()[i].Name;
                }

                var mattr = (XmlRpcMethodAttribute)Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute));
                var enattr = (XmlRpcEnumMappingAttribute)Attribute.GetCustomAttribute(mi, typeof(XmlRpcEnumMappingAttribute));

                BuildMethod(
                    tb, 
                    mi.Name, 
                    mthdData.XmlRpcName, 
                    paramNames, 
                    argTypes,
                    mthdData.ParamsMethod, 
                    mi.ReturnType, 
                    mattr.StructParams,
                    enattr == null ? EnumMapping.Number : EnumMapping.String);
            }
        }

        private static void BuildMethod(
            TypeBuilder tb,
            string methodName,
            string rpcMethodName,
            string[] paramNames,
            Type[] argTypes,
            bool paramsMethod,
            Type returnType,
            bool structParams,
            EnumMapping enumMapping)
        {
            var mthdBldr = tb.DefineMethod(
                methodName,
                MethodAttributes.Public | MethodAttributes.Virtual,
                returnType, 
                argTypes);
            
            // add attribute to method
            var oneString = new [] { typeof(string) };
            var methodAttr = typeof(XmlRpcMethodAttribute);
            var ci = methodAttr.GetConstructor(oneString);
            var pis  = new [] { methodAttr.GetProperty("StructParams") };
            var structParam = new object[] { structParams };
            var cab = new CustomAttributeBuilder(ci, new object[] { rpcMethodName }, pis, structParam);
            mthdBldr.SetCustomAttribute(cab);

            // add EnumMapingAttribute to method if not default
            if (enumMapping != EnumMapping.Number)
            {
                var oneEnumMapping = new [] { typeof(EnumMapping) };
                var enMapAttr = typeof(XmlRpcEnumMappingAttribute);
                var enMapCi = enMapAttr.GetConstructor(oneEnumMapping);
                var enMapCab = new CustomAttributeBuilder(enMapCi, new object[] { enumMapping });
                mthdBldr.SetCustomAttribute(enMapCab);
            }

            for (var i = 0; i < paramNames.Length; i++)
            {
                var paramBldr = mthdBldr.DefineParameter(i + 1, ParameterAttributes.In, paramNames[i]);
                // possibly add ParamArrayAttribute to final parameter
                if (i == paramNames.Length - 1 && paramsMethod)
                {
                    var ctorInfo = typeof(ParamArrayAttribute).GetConstructor(new Type[0]);
                    var attrBldr = new CustomAttributeBuilder(ctorInfo, new object[0]);
                    paramBldr.SetCustomAttribute(attrBldr);
                }
            }

            // generate IL
            var ilgen = mthdBldr.GetILGenerator();
            // if non-void return, declared locals for processing return value
            LocalBuilder retVal = null;
            LocalBuilder tempRetVal = null;
            if (typeof(void) != returnType)
            {
                tempRetVal = ilgen.DeclareLocal(typeof(object));
                retVal = ilgen.DeclareLocal(returnType);
            }

            // declare variable to store method args and emit code to populate ut
            var argValues = ilgen.DeclareLocal(typeof(object[]));
            ilgen.Emit(OpCodes.Ldc_I4, argTypes.Length);
            ilgen.Emit(OpCodes.Newarr, typeof(object));
            ilgen.Emit(OpCodes.Stloc, argValues);
            for (var argLoad = 0; argLoad < argTypes.Length; argLoad++)
            {
                ilgen.Emit(OpCodes.Ldloc, argValues);
                ilgen.Emit(OpCodes.Ldc_I4, argLoad);
                ilgen.Emit(OpCodes.Ldarg, argLoad + 1);
                if (argTypes[argLoad].IsValueType)
                    ilgen.Emit(OpCodes.Box, argTypes[argLoad]);
                ilgen.Emit(OpCodes.Stelem_Ref);
            }

            // call Invoke on base class
            var invokeTypes = new [] { typeof(MethodInfo), typeof(object[]) };
            var invokeMethod = typeof(XmlRpcClientProtocol).GetMethod("Invoke", invokeTypes);
            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
            ilgen.Emit(OpCodes.Castclass, typeof(MethodInfo));
            ilgen.Emit(OpCodes.Ldloc, argValues);
            ilgen.Emit(OpCodes.Call, invokeMethod);

            //  if non-void return prepare return value, otherwise pop to discard 
            if (typeof(void) != returnType)
            {
                // if return value is null, don't cast it to required type
                var retIsNull = ilgen.DefineLabel();
                ilgen.Emit(OpCodes.Stloc, tempRetVal);
                ilgen.Emit(OpCodes.Ldloc, tempRetVal);
                ilgen.Emit(OpCodes.Brfalse, retIsNull);
                ilgen.Emit(OpCodes.Ldloc, tempRetVal);
                if (returnType.IsValueType)
                {
                    ilgen.Emit(OpCodes.Unbox, returnType);
                    ilgen.Emit(OpCodes.Ldobj, returnType);
                }
                else
                    ilgen.Emit(OpCodes.Castclass, returnType);
                ilgen.Emit(OpCodes.Stloc, retVal);
                ilgen.MarkLabel(retIsNull);
                ilgen.Emit(OpCodes.Ldloc, retVal);
            }
            else
                ilgen.Emit(OpCodes.Pop);
            ilgen.Emit(OpCodes.Ret);
        }

        private static void BuildBeginMethods(TypeBuilder tb, List<MethodData> methods)
        {
            foreach (MethodData mthdData in methods)
            {
                var mi = mthdData.Mi;
                // assume method has already been validated for required signature   
                var paramCount = mi.GetParameters().Length;
                // argCount counts of params before optional AsyncCallback param
                var argCount = paramCount;
                var argTypes = new Type[paramCount];
                for (var i = 0; i < mi.GetParameters().Length; i++)
                {
                    argTypes[i] = mi.GetParameters()[i].ParameterType;
                    if (argTypes[i] == typeof(AsyncCallback))
                        argCount = i;
                }
                var mthdBldr = tb.DefineMethod(
                    mi.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    mi.ReturnType,
                    argTypes);
                
                // add attribute to method
                var oneString = new [] { typeof(string) };
                var methodAttr = typeof(XmlRpcBeginAttribute);
                var ci = methodAttr.GetConstructor(oneString);
                var cab = new CustomAttributeBuilder(ci, new object[] { mthdData.XmlRpcName });
                mthdBldr.SetCustomAttribute(cab);

                // start generating IL
                var ilgen = mthdBldr.GetILGenerator();

                // declare variable to store method args and emit code to populate it
                var argValues = ilgen.DeclareLocal(typeof(object[]));
                ilgen.Emit(OpCodes.Ldc_I4, argCount);
                ilgen.Emit(OpCodes.Newarr, typeof(object));
                ilgen.Emit(OpCodes.Stloc, argValues);
                for (var argLoad = 0; argLoad < argCount; argLoad++)
                {
                    ilgen.Emit(OpCodes.Ldloc, argValues);
                    ilgen.Emit(OpCodes.Ldc_I4, argLoad);
                    ilgen.Emit(OpCodes.Ldarg, argLoad + 1);
                    var pi = mi.GetParameters()[argLoad];
                    var paramTypeName = pi.ParameterType.AssemblyQualifiedName;
                    paramTypeName = paramTypeName.Replace("&", "");
                    var paramType = Type.GetType(paramTypeName);
                    if (paramType.IsValueType)
                        ilgen.Emit(OpCodes.Box, paramType);
                    ilgen.Emit(OpCodes.Stelem_Ref);
                }

                // emit code to store AsyncCallback parameter, defaulting to null 
                // if not in method signature
                var acbValue = ilgen.DeclareLocal(typeof(AsyncCallback));
                if (argCount < paramCount)
                {
                    ilgen.Emit(OpCodes.Ldarg, argCount + 1);
                    ilgen.Emit(OpCodes.Stloc, acbValue);
                }

                // emit code to store async state parameter, defaulting to null 
                // if not in method signature
                var objValue = ilgen.DeclareLocal(typeof(object));
                if (argCount < (paramCount - 1))
                {
                    ilgen.Emit(OpCodes.Ldarg, argCount + 2);
                    ilgen.Emit(OpCodes.Stloc, objValue);
                }

                // emit code to call BeginInvoke on base class
                var invokeTypes = new [] { 
                    typeof(MethodInfo), 
                    typeof(object[]), 
                    typeof(object),
                    typeof(AsyncCallback),
                    typeof(object)
                };

                var invokeMethod = typeof(XmlRpcClientProtocol).GetMethod("BeginInvoke", invokeTypes);

                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetCurrentMethod"));
                ilgen.Emit(OpCodes.Castclass, typeof(MethodInfo));
                ilgen.Emit(OpCodes.Ldloc, argValues);
                ilgen.Emit(OpCodes.Ldarg_0);
                ilgen.Emit(OpCodes.Ldloc, acbValue);
                ilgen.Emit(OpCodes.Ldloc, objValue);
                ilgen.Emit(OpCodes.Call, invokeMethod);

                // BeginInvoke will leave IAsyncResult on stack - leave it there
                // for return value from method being built
                ilgen.Emit(OpCodes.Ret);
            }
        }

        private static void BuildEndMethods(TypeBuilder tb, List<MethodData> methods)
        {
            LocalBuilder retVal = null;
            LocalBuilder tempRetVal = null;
            foreach (var mthdData in methods)
            {
                var mi = mthdData.Mi;
                var argTypes = new [] { typeof(IAsyncResult) };
                var mthdBldr = tb.DefineMethod(
                    mi.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    mi.ReturnType, 
                    argTypes);
                
                // start generating IL
                var ilgen = mthdBldr.GetILGenerator();

                // if non-void return, declared locals for processing return value
                if (typeof(void) != mi.ReturnType)
                {
                    tempRetVal = ilgen.DeclareLocal(typeof(object));
                    retVal = ilgen.DeclareLocal(mi.ReturnType);
                }

                // call EndInvoke on base class
                var invokeTypes = new [] { typeof(IAsyncResult), typeof(Type) };
                var invokeMethod = typeof(XmlRpcClientProtocol).GetMethod("EndInvoke", invokeTypes);
                var GetTypeTypes = new [] { typeof(string) };
                var GetTypeMethod = typeof(Type).GetMethod("GetType", GetTypeTypes);
                ilgen.Emit(OpCodes.Ldarg_0);  // "this"
                ilgen.Emit(OpCodes.Ldarg_1);  // IAsyncResult parameter
                ilgen.Emit(OpCodes.Ldstr, mi.ReturnType.AssemblyQualifiedName);
                ilgen.Emit(OpCodes.Call, GetTypeMethod);
                ilgen.Emit(OpCodes.Call, invokeMethod);

                //  if non-void return prepare return value otherwise pop to discard 
                if (typeof(void) != mi.ReturnType)
                {
                    // if return value is null, don't cast it to required type
                    var retIsNull = ilgen.DefineLabel();
                    ilgen.Emit(OpCodes.Stloc, tempRetVal);
                    ilgen.Emit(OpCodes.Ldloc, tempRetVal);
                    ilgen.Emit(OpCodes.Brfalse, retIsNull);
                    ilgen.Emit(OpCodes.Ldloc, tempRetVal);
                    if (mi.ReturnType.IsValueType)
                    {
                        ilgen.Emit(OpCodes.Unbox, mi.ReturnType);
                        ilgen.Emit(OpCodes.Ldobj, mi.ReturnType);
                    }
                    else
                        ilgen.Emit(OpCodes.Castclass, mi.ReturnType);
                    ilgen.Emit(OpCodes.Stloc, retVal);
                    ilgen.MarkLabel(retIsNull);
                    ilgen.Emit(OpCodes.Ldloc, retVal);
                }
                else
                {
                    // void method so throw away result from EndInvoke
                    ilgen.Emit(OpCodes.Pop);
                }

                ilgen.Emit(OpCodes.Ret);
            }
        }

        private static void BuildConstructor(
            TypeBuilder typeBldr,
            Type baseType,
            string urlStr)
        {
            var ctorBldr = typeBldr.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName | MethodAttributes.HideBySig,
                CallingConventions.Standard,
                Type.EmptyTypes);
            
            if (!string.IsNullOrEmpty(urlStr))
            {
                var urlAttr = typeof(XmlRpcUrlAttribute);
                var oneString = new [] { typeof(string) };
                var ci = urlAttr.GetConstructor(oneString);
                var cab = new CustomAttributeBuilder(ci, new object[] { urlStr });
                typeBldr.SetCustomAttribute(cab);
            }

            var ilgen = ctorBldr.GetILGenerator();
            //  Call the base constructor.
            ilgen.Emit(OpCodes.Ldarg_0);
            var ctorInfo = baseType.GetConstructor(Type.EmptyTypes);
            ilgen.Emit(OpCodes.Call, ctorInfo);
            ilgen.Emit(OpCodes.Ret);
        }

        private static string GetXmlRpcUrl(MemberInfo itf)
        {
            var attr = Attribute.GetCustomAttribute(itf, typeof(XmlRpcUrlAttribute)) as XmlRpcUrlAttribute;
            return attr == null ? null : attr.Uri;
        }

        /// <summary>
        /// Type.GetMethods() does not return methods that a derived interface
        /// inherits from its base interfaces; this method does.
        /// </summary>
        private static MethodInfo[] GetMethods(Type type)
        {
            var methods = type.GetMethods();
            if (!type.IsInterface)
                return methods;

            var interfaces = type.GetInterfaces();
            if (interfaces.Length == 0)
                return methods;

            var result = new List<MethodInfo>();
            result.AddRange(methods);
            foreach (var itf in type.GetInterfaces())
                result.AddRange(itf.GetMethods());
            
            return result.ToArray();
        }

        private static List<MethodData> GetXmlRpcMethods(Type itf)
        {
            var ret = new List<MethodData>();
            if (!itf.IsInterface)
                throw new Exception("type not interface");

            foreach (var mi in GetMethods(itf))
            {
                var xmlRpcName = GetXmlRpcMethodName(mi);
                if (xmlRpcName == null)
                    continue;
                var pis = mi.GetParameters();
                var paramsMethod = pis.Length > 0 
                    && Attribute.IsDefined(pis[pis.Length - 1], typeof(ParamArrayAttribute));

                ret.Add(new MethodData(mi, xmlRpcName, paramsMethod));
            }

            return ret;
        }

        private static string GetXmlRpcMethodName(MemberInfo mi)
        {
            var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcMethodAttribute)) as XmlRpcMethodAttribute;
            if (attr == null)
                return null;
            return string.IsNullOrEmpty(attr.Method) ? mi.Name : attr.Method;
        }

        private class MethodData
        {
            public MethodData(MethodInfo mi, string xmlRpcName, bool paramsMethod) : this (mi, xmlRpcName, paramsMethod, null)
            {
            }

            public MethodData(MethodInfo mi, string xmlRpcName, bool paramsMethod, Type returnType)
            {
                Mi = mi;
                XmlRpcName = xmlRpcName;
                ParamsMethod = paramsMethod;
                ReturnType = returnType;
            }

            public MethodInfo Mi;
            public string XmlRpcName;
            public Type ReturnType;
            public bool ParamsMethod;
        }

        private static List<MethodData> GetXmlRpcBeginMethods(Type itf)
        {
            var ret = new List<MethodData>();
            if (!itf.IsInterface)
                throw new Exception("type not interface");
            
            foreach (var mi in itf.GetMethods())
            {
                var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcBeginAttribute)) as XmlRpcBeginAttribute;
                if (attr == null)
                    continue;

                var rpcMethod = attr.Method;
                if (string.IsNullOrEmpty(rpcMethod))
                {
                    if (!mi.Name.StartsWith("Begin", StringComparison.Ordinal) || mi.Name.Length <= 5)
                        throw new Exception(
                            string.Format(
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
                                "method {0} has invalid signature for begin method", 
                                mi.Name));
                    
                    if (i == (paramCount - 2))
                    {
                        var paramType = mi.GetParameters()[i + 1].ParameterType;
                        if (paramType != typeof(object))
                            throw new Exception(
                                string.Format(
                                    "method {0} has invalid signature for begin method",
                                    mi.Name));
                    }
                }

                ret.Add(new MethodData(mi, rpcMethod, false, null));
            }

            return ret;
        }

        private static List<MethodData> GetXmlRpcEndMethods(Type itf)
        {
            var ret = new List<MethodData>();
            if (!itf.IsInterface)
                throw new Exception("type not interface");
            
            foreach (MethodInfo mi in itf.GetMethods())
            {
                var attr = Attribute.GetCustomAttribute(mi, typeof(XmlRpcEndAttribute)) as XmlRpcEndAttribute;
                if (attr == null)
                    continue;
                
                var pis = mi.GetParameters();
                if (pis.Length != 1)
                    throw new Exception(
                        string.Format(
                            "method {0} has invalid signature for end method", 
                            mi.Name));
                
                var paramType = pis[0].ParameterType;
                if (paramType != typeof(IAsyncResult))
                    throw new Exception(
                        string.Format(
                            "method {0} has invalid signature for end method", 
                            mi.Name));
                
                ret.Add(new MethodData(mi, string.Empty, false));
            }

            return ret;
        }
    }
}