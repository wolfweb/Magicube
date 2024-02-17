using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Magicube.WebServer.Internal {
    public static class XmlDocumentationHelper {
        public static readonly Dictionary<Assembly, XmlDocument> XmlDocumentCache = new Dictionary<Assembly, XmlDocument>();

        public static XmlDocument GetXmlDocumentation(this Assembly assembly) {
            if (assembly == null)
                throw new ArgumentNullException("assembly", @"The parameter 'assembly' must not be null.");

            if (XmlDocumentCache.ContainsKey(assembly))
                return XmlDocumentCache[assembly];

            string assemblyFilename = assembly.Location;
            const string prefix = "file:///";

            if (string.IsNullOrWhiteSpace(assemblyFilename) == false && assemblyFilename.StartsWith(prefix)) {
                try {
                    string xmlDocumentationPath = Path.ChangeExtension(assemblyFilename.Substring(prefix.Length), ".xml");

                    if (File.Exists(xmlDocumentationPath)) {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(xmlDocumentationPath);
                        XmlDocumentCache.Add(assembly, xmlDocument);
                        return xmlDocument;
                    }
                } catch (Exception) {
                }
            }

            return null;
        }

        public static XmlElement GetXmlDocumentation(this Type type) {
            if (type == null)
                throw new ArgumentNullException("type", @"The parameter 'type' must not be null.");

            XmlDocument xmlDocument = type.Assembly.GetXmlDocumentation();

            if (xmlDocument == null)
                return null;

            string xmlMemberName = string.Format("T:{0}", GetFullTypeName(type));
            var memberElement = xmlDocument.GetMemberByName(xmlMemberName);
            return memberElement;
        }

        public static XmlElement GetXmlDocumentation(this MethodInfo methodInfo) {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo", @"The parameter 'methodInfo' must not be null.");

            Type declaryingType = methodInfo.DeclaringType;

            if (declaryingType == null)
                return null;

            XmlDocument xmlDocument = declaryingType.Assembly.GetXmlDocumentation();

            if (xmlDocument == null)
                return null;

            string parameterList = "";

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters().OrderBy(x => x.Position)) {
                if (parameterList.Length > 0)
                    parameterList += ",";

                parameterList += GetParameterTypeName(methodInfo, parameterInfo);
            }

            Type[] genericArguments = methodInfo.GetGenericArguments();
            string xmlMethodName = string.Format("M:{0}.{1}{2}{3}", GetFullTypeName(methodInfo.DeclaringType), methodInfo.Name, genericArguments.Length > 0 ? string.Format("``{0}", genericArguments.Length) : "", parameterList.Length > 0 ? string.Format("({0})", parameterList) : "");
            XmlElement memberElement = xmlDocument.GetMemberByName(xmlMethodName);
            return memberElement;
        }

        public static XmlElement GetXmlDocumentation(this FieldInfo fieldInfo) {
            if (fieldInfo == null)
                throw new ArgumentNullException("fieldInfo", @"The parameter 'fieldInfo' must not be null.");

            Type declaryingType = fieldInfo.DeclaringType;

            if (declaryingType == null)
                return null;

            XmlDocument xmlDocument = declaryingType.Assembly.GetXmlDocumentation();

            if (xmlDocument == null)
                return null;

            string xmlPropertyName = string.Format("F:{0}.{1}", GetFullTypeName(declaryingType), fieldInfo.Name);
            var memberElement = xmlDocument.GetMemberByName(xmlPropertyName);
            return memberElement;
        }

        public static XmlElement GetXmlDocumentation(this PropertyInfo propertyInfo) {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo", @"The parameter 'propertyInfo' must not be null.");

            Type declaryingType = propertyInfo.DeclaringType;

            if (declaryingType == null)
                return null;

            XmlDocument xmlDocument = declaryingType.Assembly.GetXmlDocumentation();

            if (xmlDocument == null)
                return null;

            string xmlPropertyName = string.Format("P:{0}.{1}", GetFullTypeName(declaryingType), propertyInfo.Name);
            var memberElement = xmlDocument.GetMemberByName(xmlPropertyName);
            return memberElement;
        }

        public static XmlElement GetXmlDocumentation(this MemberInfo memberInfo) {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo", @"The parameter 'memberInfo' must not be null.");

            Type declaryingType = memberInfo.DeclaringType;

            if (declaryingType == null)
                return null;

            XmlDocument xmlDocument = declaryingType.Assembly.GetXmlDocumentation();

            if (xmlDocument == null)
                return null;

            string xmlPropertyName = string.Format("{0}:{1}.{2}", memberInfo.MemberType.ToString()[0], GetFullTypeName(declaryingType), memberInfo.Name);
            var memberElement = xmlDocument.GetMemberByName(xmlPropertyName);
            return memberElement;
        }

        public static string GetParameterTypeName(this MethodInfo methodInfo, ParameterInfo parameterInfo) {
            // Handle nullable types
            Type underlyingType = Nullable.GetUnderlyingType(parameterInfo.ParameterType);
            if (underlyingType != null)
                return string.Format("System.Nullable{{{0}}}", GetFullTypeName(underlyingType));

            // Handle generic types
            if (parameterInfo.ParameterType.IsGenericType)
                return GetGenericTypeName(parameterInfo.ParameterType);

            string parameterTypeFullName = GetFullTypeName(parameterInfo.ParameterType);

            return parameterTypeFullName;
        }

        public static string GetGenericTypeName(Type type) {
            var genericArgumentsArray = type.GetGenericArguments();
            string genericTypeFullName = type.GetGenericTypeDefinition().FullName.Split('`')[0]; // disregard the stuff after the back-tick

            string genericArguments = "";
            for (var i = 0; i < genericArgumentsArray.Length; i++) {
                if (i > 0)
                    genericArguments += ",";
                string genericArgumentName = genericArgumentsArray[i].ToString();
                if (genericArgumentsArray[i].IsGenericType)
                    genericArgumentName = GetGenericTypeName(genericArgumentsArray[i]); // if generic argument is generic, recurse
                genericArguments += genericArgumentName;
            }

            return $"{genericTypeFullName}{{{genericArguments}}}";
        }

        public static string GetFullTypeName(Type type) {
            string parameterTypeFullName = type.FullName;

            if (string.IsNullOrWhiteSpace(parameterTypeFullName) == false)
                return parameterTypeFullName.Replace("+", ".");

            return null;
        }

        public static XmlElement GetMemberByName(this XmlDocument xmlDocument, string memberName) {
            if (xmlDocument == null)
                return null;

            if (string.IsNullOrWhiteSpace(memberName))
                throw new ArgumentNullException("memberName", @"The parameter 'memberName' must not be null.");

            XmlElement docElement = xmlDocument["doc"];

            if (docElement == null)
                return null;

            XmlElement membersElement = docElement["members"];

            if (membersElement == null)
                return null;

            foreach (XmlElement member in membersElement) {
                if (member == null)
                    continue;

                if (member.Attributes["name"].Value == memberName)
                    return member;
            }

            return null;
        }

        public static string GetSummary(this XmlElement xmlElement) {
            string summary = xmlElement.GetNodeText("summary");
            return summary;
        }

        public static string GetRemarks(this XmlElement xmlElement) {
            string summary = xmlElement.GetNodeText("remarks");
            return summary;
        }

        public static string GetParameterDescription(this XmlElement xmlElement, string parameterName) {
            string summary = xmlElement.GetNodeText(string.Format("param[@name='{0}']", parameterName));
            return summary;
        }

        public static string GetNodeText(this XmlElement xmlElement, string xpath) {
            if (xmlElement == null)
                return null;

            if (string.IsNullOrWhiteSpace(xpath))
                return null;

            XmlNode summaryNode = xmlElement.SelectSingleNode(xpath);

            if (summaryNode == null)
                return null;

            string summary = FormatXmlInnerText(summaryNode.InnerXml);
            return summary;
        }

        public static string FormatXmlInnerText(string xmlInnerText) {
            if (string.IsNullOrWhiteSpace(xmlInnerText))
                return xmlInnerText;

            string[] lines = xmlInnerText.Trim().Replace("\r", "").Split(new[] { "\n" }, StringSplitOptions.None);
            string formattedText = "";

            foreach (string line in lines) {
                if (formattedText.Length > 0)
                    formattedText += "\n";

                formattedText += line.Trim();
            }

            return formattedText;
        }
    }
}
