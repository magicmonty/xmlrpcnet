// --------------------------------------------------------------------------------------------------------------------
// <copyright file="utils.cs" company="">
//   
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace CookComputing.XmlRpc
{
    public static class Utils
    {
        public static XmlReader Serialize(string testName, object obj, Encoding encoding, MappingActions actions)
        {
            Stream stm = new MemoryStream();

            var xtw = XmlRpcXmlWriter.Create(stm, new XmlRpcFormatSettings());
            xtw.WriteStartDocument();
            var ser = new XmlRpcSerializer();
            ser.Serialize(xtw, obj, actions);
            xtw.Flush();
            stm.Position = 0;

            return XmlRpcXmlReader.Create(stm);
        }

        // ----------------------------------------------------------------------// 
        public static object Parse(
            string xml,
            Type valueType,
            MappingAction action,
            out Type parsedType,
            out Type parsedArrayType)
        {
            var sr = new StringReader(xml);

            return Parse(
                XmlRpcXmlReader.Create(sr),
                valueType,
                action,
                out parsedType,
                out parsedArrayType);
        }

        public static object Parse(
            XmlReader rdr,
            Type valueType,
            MappingAction action,
            out Type parsedType,
            out Type parsedArrayType)
        {
            parsedType = parsedArrayType = null;
            rdr.ReadToDescendant("value");
            var mappingStack = new MappingStack("request");
            return new XmlRpcDeserializer().ParseValueElement(rdr, valueType, mappingStack, action);
        }

        public static object Parse(
            string xml,
            Type valueType,
            MappingAction action,
            XmlRpcDeserializer serializer,
            out Type parsedType,
            out Type parsedArrayType)
        {
            var sr = new StringReader(xml);
            return Parse(
                XmlRpcXmlReader.Create(sr),
                valueType,
                action,
                serializer,
                out parsedType,
                out parsedArrayType);
        }

        public static object Parse(
            XmlReader rdr,
            Type valueType,
            MappingAction action,
            XmlRpcDeserializer deserializer,
            out Type parsedType,
            out Type parsedArrayType)
        {
            parsedType = parsedArrayType = null;
            rdr.ReadToDescendant("value");
            var parseStack = new MappingStack("request");

            return deserializer.ParseValueElement(
                rdr,
                valueType,
                parseStack,
                action);
        }

        public static object ParseValue(string xml, Type valueType)
        {
            var sr = new StringReader(xml);
            var reader = XmlRpcXmlReader.Create(sr);
            reader.MoveToContent();
            return new XmlRpcDeserializer()
                .ParseValueElement(
                    reader,
                    valueType,
                    new MappingStack("value"),
                    MappingAction.Error);
        }

        public static string SerializeValue(object value, bool indent)
        {
            var memStm = new MemoryStream();
            var writer = XmlRpcXmlWriter.Create(
                memStm,
                new XmlRpcFormatSettings { OmitXmlDeclaration = true, UseIndentation = indent });
            var serializer = new XmlRpcSerializer();
            serializer.Serialize(writer, value, new MappingActions { NullMappingAction = NullMappingAction.Error });
            writer.Flush();
            memStm.Position = 0;

            return new StreamReader(memStm).ReadToEnd();
        }

        public static IEnumerable<string> GetLocales()
        {
            return new[]
                       {
                           "af-ZA", "sq-AL", "ar-DZ", "ar-BH", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", 
                           "ar-MA", "ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-AE", "ar-YE", "hy-AM", "az-Cyrl-AZ", 
                           "az-Latn-AZ", "eu-ES", "be-BY", "bg-BG", "ca-ES", "zh-HK", "zh-MO", "zh-CN", "zh-SG", "zh-TW", 
                           "hr-HR", "cs-CZ", "da-DK", "dv-MV", "nl-BE", "nl-NL", "en-AU", "en-BZ", "en-CA", "en-029", 
                           "en-IE", "en-JM", "en-NZ", "en-PH", "en-ZA", "en-TT", "en-GB", "en-US", "en-ZW", "et-EE", 
                           "fo-FO", "fa-IR", "fi-FI", "fr-BE", "fr-CA", "fr-FR", "fr-LU", "fr-MC", "fr-CH", "gl-ES", 
                           "ka-GE", "de-AT", "de-DE", "de-LI", "de-LU", "de-CH", "el-GR", "gu-IN", "he-IL", "hi-IN", 
                           "hu-HU", "is-IS", "id-ID", "it-IT", "it-CH", "ja-JP", "kn-IN", "kk-KZ", "kok-IN", "ko-KR", 
                           "lv-LV", "lt-LT", "mk-MK", "ms-BN", "ms-MY", "mr-IN", "mn-MN", "nb-NO", "nn-NO", "pl-PL", 
                           "pt-BR", "pt-PT", "pa-IN", "ro-RO", "ru-RU", "sa-IN", "sr-Cyrl-CS", "sr-Latn-CS", "sk-SK", 
                           "sl-SI", "es-AR", "es-BO", "es-CL", "es-CO", "es-CR", "es-DO", "es-EC", "es-SV", "es-GT", 
                           "es-HN", "es-MX", "es-NI", "es-PA", "es-PY", "es-PE", "es-PR", "es-ES", "es-UY", "es-VE", 
                           "sw-KE", "sv-FI", "sv-SE", "syr-SY", "ta-IN", "tt-RU", "te-IN", "th-TH", "tr-TR", "uk-UA", 
                           "ur-PK", "uz-Cyrl-UZ", "uz-Latn-UZ", "vi-VN"
                       };
        }
    }
}