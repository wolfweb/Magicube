using System;
using System.Linq;
using System.Reflection;

namespace Magicube.Core.Reflection {
    public static class ParameterInfoExtensions {
        public static bool HasAttribute(this ParameterInfo parameterInfo, Type attributeType) {
            return parameterInfo.GetCustomAttributes(attributeType).FirstOrDefault() != null;
        }
    }
}