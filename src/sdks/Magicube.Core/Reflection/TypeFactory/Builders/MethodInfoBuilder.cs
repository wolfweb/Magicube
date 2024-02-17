using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magicube.Core.Reflection.Builders {
    public class MethodInfoBuilder {
        private IEnumerable<MethodInfo> _methods;

        public MethodInfoBuilder(Type type, string methodName) {
            _methods = type.GetMethods().Where(m => m.Name == methodName);
        }

        public MethodInfoBuilder IsGenericDefinition() {
            _methods = _methods.Where(m => m.IsGenericMethodDefinition);
            return this;
        }

        public MethodInfoBuilder HasParameterTypes(params Type[] types) {
            _methods = _methods.Select(m => new {
                Method = m,
                Parms = m.GetParameters()
            }).Where(m => m.Parms.Length == types.Length && ParameterTypesMatch(m.Parms, types))
            .Select(m => m.Method);

            return this;
        }

        public MethodInfoBuilder HasMetadataToken(int metadataToken) {
            _methods = _methods.Where(m => m.MetadataToken == metadataToken);

            return this;
        }

        public MethodInfo FirstOrDefault() {
            return _methods.FirstOrDefault();
        }

        public IEnumerable<MethodInfo> All() {
            return _methods;
        }

        private bool ParameterTypesMatch(ParameterInfo[] parms, Type[] types) {
            if (parms.Length < types.Length) {
                return false;
            }

            for (int i = 0; i < types.Length; i++) {
                if (types[i] != null) {
                    Type parmType = parms[i].ParameterType;
                    if (types[i].IsGenericTypeDefinition == true) {
                        parmType = parmType.GetGenericTypeDefinition();
                    }

                    if (parmType != types[i]) {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}