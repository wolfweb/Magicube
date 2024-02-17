using Magicube.WebServer.Internal;
using Magicube.WebServer.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public class MetadataRequestHandler : RequestHandler {
        public MetadataRequestHandler(string urlPath, EventHandler eventHandler)
            : base(urlPath, eventHandler) {
        }

        public override async Task<MiniWebContext> HandleRequestAsync(MiniWebContext context) {
            context.Handled = true;
            var apiMetadata = new ApiMetadata();
            apiMetadata.ApplicationName = context.Configuration.ApplicationName;

            foreach (MethodRequestHandler methodRequestHandler in context.Configuration.RequestHandlers.OfType<MethodRequestHandler>()) {
                IApiMetaDataProvider metadataProvider = methodRequestHandler.MetadataProvider;

                if (metadataProvider == null)
                    continue;

                var metadata = new OperationMetaData { UrlPath = methodRequestHandler.CaseSensitiveUrlPath };

                metadata.Name = metadataProvider.GetOperationName(context, methodRequestHandler);
                metadata.Description = metadataProvider.GetOperationDescription(context, methodRequestHandler);
                IList<MethodParameter> parameters = metadataProvider.GetOperationParameters(context, methodRequestHandler);

                foreach (MethodParameter methodParameter in parameters) {
                    if (methodParameter.Name.ToLower() == "context" || methodParameter.Type == typeof(MiniWebContext))
                        continue;

                    var inputParameter = new OperationParameter { Name = methodParameter.Name, Description = methodParameter.Description, Type = GetTypeName(methodParameter.Type), IsOptional = methodParameter.IsOptional, HasDefaultValue = methodParameter.HasDefaultValue, DefaultValue = methodParameter.DefaultValue };

                    metadata.InputParameters.Add(inputParameter);

                    AddModels(apiMetadata, methodParameter.Type);
                }

                var returnParameterType = metadataProvider.GetOperationReturnParameterType(context, methodRequestHandler);
                metadata.ReturnParameterType = GetTypeName(returnParameterType);

                AddModels(apiMetadata, returnParameterType);
                apiMetadata.Operations.Add(metadata);
            }

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(t => context.Configuration.MetadataNamespaces.Contains(t.Namespace) && t.IsPublic);
            foreach (var x in types) {
                AddModels(apiMetadata, x);
            }

            context.Response.ResponseObject = apiMetadata;
            context.Response.ContentType = "application/json";
            return context;
        }

        public static void AddModels(ApiMetadata apiMetadata, Type type) {
            if (type == null || type.FullName == null)
                return;

            var nestedUserTypes = new List<Type>();

            if (type.IsGenericType) {
                var types = type.GetGenericArguments();
                nestedUserTypes.AddRange(types.Where(t => t.FullName != null));

                type = type.GetGenericTypeDefinition();

                types = type.GetGenericArguments();
                foreach (var t in types) {
                    if (t.FullName == null || nestedUserTypes.Contains(t))
                        continue;

                    nestedUserTypes.Add(t);
                }

                foreach (Type nestedUserType in nestedUserTypes)
                    AddModels(apiMetadata, nestedUserType);
            }

            if (type != null && IsUserType(type) && apiMetadata.Models.Any(x => x.Type == type.Name) == false) {
                var modelMetadata = new ModelMetadata { Type = GetTypeName(type), Description = GetDescription(type) };
                FieldInfo[] fields = type.GetFields();

                foreach (FieldInfo field in fields) {
                    if (IsUserType(type) && apiMetadata.Models.Any(x => x.Type == type.Name) == false)
                        nestedUserTypes.Add(field.FieldType);

                    var modelProperty = new ModelProperty { Name = field.Name, Type = GetTypeName(field.FieldType), Description = GetDescription(field) };
                    modelMetadata.Properties.Add(modelProperty);
                }

                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties) {
                    if (IsUserType(type) && apiMetadata.Models.Any(x => x.Type == type.Name) == false)
                        nestedUserTypes.Add(property.PropertyType);

                    var modelProperty = new ModelProperty { Name = property.Name, Type = GetTypeName(property.PropertyType), Description = GetDescription(property) };
                    modelMetadata.Properties.Add(modelProperty);
                }

                apiMetadata.Models.Add(modelMetadata);

                foreach (Type nestedUserType in nestedUserTypes)
                    AddModels(apiMetadata, nestedUserType);
            }
        }

        public static string GetDescription(Type type) {
            object descriptionAttribute = type.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

            if (descriptionAttribute != null)
                return ((DescriptionAttribute)descriptionAttribute).Description;

            return type.GetXmlDocumentation().GetSummary();
        }

        public static string GetDescription(FieldInfo fieldInfo) {
            object descriptionAttribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

            if (descriptionAttribute != null)
                return ((DescriptionAttribute)descriptionAttribute).Description;

            return fieldInfo.GetXmlDocumentation().GetSummary();
        }

        public static string GetDescription(PropertyInfo propertyInfo) {
            object descriptionAttribute = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();

            if (descriptionAttribute != null)
                return ((DescriptionAttribute)descriptionAttribute).Description;

            return propertyInfo.GetXmlDocumentation().GetSummary();
        }

        public static bool IsUserType(Type type) {
            return type != null && (type.Namespace != null &&
                                     type.Namespace.StartsWith("System") == false &&
                                     type.Namespace.StartsWith("Microsoft") == false);
        }

        public static string GetTypeName(Type type) {
            if (type == null)
                return "";

            if (type.FullName == null)
                return type.Name;

            string name = type.Name.Replace('+', '.');

            if (type.IsGenericType) {
                int backtickIndex = name.IndexOf('`');

                if (backtickIndex > 0) {
                    name = name.Remove(backtickIndex);
                }

                name += "<";

                var typeParameters = type.GetGenericArguments();

                for (var i = 0; i < typeParameters.Length; ++i) {
                    string typeParameterName = GetTypeName(typeParameters[i]);
                    name += (i == 0 ? typeParameterName : "," + typeParameterName);
                }

                name += ">";
            }

            return name;
        }
    }
}
