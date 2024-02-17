using System;
using Magicube.Core.Extensions;
using Magicube.WebServer.Serialization;
using TypeConverter = Magicube.WebServer.Internal.TypeConverter;

namespace Magicube.WebServer {

    public static class ModelBinder {
        public static T Bind<T>(this MiniWebContext context, string parameterName) {
            var type = typeof(T);
            return (T)Bind(context, type, parameterName);
        }

        public static object Bind(this MiniWebContext context, Type type, string parameterName) {
            string requestParameterValue = context.GetRequestParameterValue(parameterName);

            if (requestParameterValue.IsJson() == false) {
                try {
                    return TypeConverter.ConvertType(requestParameterValue, type);
                } catch (Exception) {
                }
            }

            try {
                Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
                object convertedValue;

                if (context.Configuration.SerializationService.TryParseJson(requestParameterValue, underlyingType, false, out convertedValue))
                    return convertedValue;

                throw new Exception("Type conversion error");
            } catch (Exception) {
                string errorMessage = String.Format("An error occurred converting the parameter named '{0}' and value '{1}' to type {2}.", parameterName, requestParameterValue, type);
                throw new Exception(errorMessage);
            }
        }

        public static object GetRequestParameterValue<T>(this MiniWebContext context, string parameterName) {
            return Bind<T>(context, parameterName);
        }

        public static object GetRequestParameterValue(this MiniWebContext context, string parameterName, Type type) {
            return Bind(context, type, parameterName);
        }

        public static string GetRequestParameterValue(this MiniWebContext context, string parameterName) {
            return context.Request.GetRequestParameterValue(parameterName);
        }

        public static string GetRequestParameterValue(this MiniWebRequest request, string parameterName) {
            string requestParameterValue = request.QueryStringParameters.Get(parameterName) ??
                                           request.FormBodyParameters.Get(parameterName) ??
                                           request.HeaderParameters.Get(parameterName);

            return requestParameterValue;
        }
    }

}
