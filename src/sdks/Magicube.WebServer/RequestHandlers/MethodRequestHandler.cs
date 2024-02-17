using Magicube.Core.Extensions;
using Magicube.WebServer.Internal;
using Magicube.WebServer.Metadata;
using Magicube.WebServer.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;
using TypeConverter = Magicube.WebServer.Internal.TypeConverter;

namespace Magicube.WebServer.RequestHandlers {
    public class MethodRequestHandler : RequestHandler {
        public MethodRequestHandler(string urlPath, EventHandler eventHandler, MethodInfo methodInfo)
            : base(urlPath, eventHandler) {
            if (methodInfo == null)
                throw new ArgumentNullException("methodInfo");

            Method = methodInfo;
            MethodParameters = GetMethodParameters(methodInfo);
            Description = GetDescription(Method);
        }

        public MethodInfo Method { get; private set; }

        public IList<MethodParameter> MethodParameters { get; private set; }

        public IApiMetaDataProvider MetadataProvider { get; set; }

        public string Description { get; set; }

        public string CaseSensitiveUrlPath { get; set; }

        public override async Task<MiniWebContext> HandleRequestAsync(MiniWebContext context) {
            context.Handled = true;
            object[] parameters = Bind(context, this);
            if (Method.ReturnType == typeof(Task)) {
                await (dynamic)Method.Invoke(null, parameters);
            } else if (Method.ReturnType.BaseType == typeof(Task) && Method.ReturnType.IsGenericType) {
                context.Response.ResponseObject = await (dynamic)Method.Invoke(null, parameters);
            } else {
                context.Response.ResponseObject = Method.Invoke(null, parameters);
            }

            if (context.Response.ResponseObject == null || context.Response.ResponseObject == context)
                return context;

            if (string.IsNullOrWhiteSpace(context.Response.ContentType))
                context.Response.ContentType = "application/json";

            return context;
        }

        public static string GetDescription(MethodInfo methodInfo) {
            object descriptionAttribute = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

            if (descriptionAttribute != null)
                return ((DescriptionAttribute)descriptionAttribute).Description;

            return methodInfo.GetXmlDocumentation().GetSummary();
        }

        public static IList<MethodParameter> GetMethodParameters(MethodInfo methodInfo) {
            var methodParameters = new List<MethodParameter>();

            XmlElement xmlMethodDocumentation = methodInfo.GetXmlDocumentation();

            foreach (ParameterInfo parameterInfo in methodInfo.GetParameters().OrderBy(x => x.Position)) {
                var methodParameter = new MethodParameter { Position = parameterInfo.Position, Name = parameterInfo.Name, Type = parameterInfo.ParameterType, IsOptional = parameterInfo.IsOptional, IsDynamic = IsDynamic(parameterInfo), Description = xmlMethodDocumentation.GetParameterDescription(parameterInfo.Name), HasDefaultValue = parameterInfo.HasDefaultValue, DefaultValue = parameterInfo.DefaultValue };
                methodParameters.Add(methodParameter);
            }

            return methodParameters;
        }

        public static bool IsDynamic(ParameterInfo parameterInfo) {
            return parameterInfo.GetCustomAttributes(typeof(DynamicAttribute), true).Length > 0;
        }

        public static object[] Bind(MiniWebContext context, MethodRequestHandler handler) {
            var methodInvokationParameters = new List<object>();

            if (handler == null)
                throw new Exception("Expected a MethodRoute but was a " + context.RequestHandler.GetType().Name);

            foreach (MethodParameter methodParameter in handler.MethodParameters.OrderBy(x => x.Position)) {
                if (methodParameter.Type == typeof(MiniWebContext) || methodParameter.Name.ToLower() == "context") {
                    methodInvokationParameters.Add(context);
                    continue;
                }

                object itemEntry;

                if (context.Items.TryGetValue(methodParameter.Name, out itemEntry)) {
                    methodInvokationParameters.Add(itemEntry);
                    continue;
                }

                string requestParameterValue = context.GetRequestParameterValue(methodParameter.Name);

                if (String.IsNullOrWhiteSpace(requestParameterValue)) {
                    if (context.Request.FormBodyParameters.Count == 0) {
                        try {
                            var sr = new StreamReader(context.Request.RequestBody);
                            requestParameterValue = sr.ReadToEnd();
                        } catch (Exception) {
                        }
                    }
                }

                if (String.IsNullOrWhiteSpace(requestParameterValue)) {
                    if (methodParameter.IsOptional) {
                        methodInvokationParameters.Add(Type.Missing);
                        continue;
                    }

                    if (Nullable.GetUnderlyingType(methodParameter.Type) != null || (!methodParameter.Type.IsValueType)) {
                        methodInvokationParameters.Add(null);
                        continue;
                    }

                    string errorMessage = String.Format("The query string, form body, and header parameters do not contain a parameter named '{0}' which is a required parameter for method '{1}'", methodParameter.Name, handler.Method.Name);
                    throw new Exception(errorMessage);
                }

                object methodInvokationParameterValue;

                if (requestParameterValue.IsJson() == false && methodParameter.IsDynamic == false) {
                    try {
                        methodInvokationParameterValue = TypeConverter.ConvertType(requestParameterValue, methodParameter.Type);
                        methodInvokationParameters.Add(methodInvokationParameterValue);
                        continue;
                    } catch (Exception) {
                    }
                }

                try {
                    Type underlyingType = Nullable.GetUnderlyingType(methodParameter.Type) ?? methodParameter.Type;

                    if (context.Configuration.SerializationService.TryParseJson(requestParameterValue, underlyingType, methodParameter.IsDynamic, out methodInvokationParameterValue))
                        methodInvokationParameters.Add(methodInvokationParameterValue);
                    else
                        throw new Exception("Type conversion error");
                } catch (Exception) {
                    string errorMessage = String.Format("An error occurred converting the parameter named '{0}' and value '{1}' to type {2} which is a required parameter for method '{3}'", methodParameter.Name, requestParameterValue, methodParameter.Type, handler.Method.Name);
                    throw new Exception(errorMessage);
                }
            }

            return methodInvokationParameters.ToArray();
        }
    }
}
