using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Emitters;

namespace Magicube.Core.Reflection.Builders {
    public class FluentMethodBuilder : IMethodBuilder {
        private readonly Func<string, MethodAttributes, CallingConventions, Type, Type[], Type[], Type[], Type[][], Type[][], MethodBuilder> _defineMethod;

        private readonly EmitterBase _body;

        private string _methodName;

        private CallingConventions _callingConvention;

        private Type _returnType;

        private IGenericParameterBuilder _genericReturnType;

        private IGenericParameterBuilder[] _genericReturnTypes;

        private List<FluentParameterBuilder> _parms = new List<FluentParameterBuilder>();

        private List<FluentGenericParameterBuilder> _genericParameterBuilders;

        private GenericTypeParameterBuilder[] _genericParameters;

        private MethodBuilder _methodBuilder;

        private List<CustomAttributeBuilder> _customAttributes;

        private MethodImplAttributes _methodImplAttributes;

        internal FluentMethodBuilder(
            string methodName,
            Func<string, MethodAttributes, CallingConventions, Type, Type[], Type[], Type[], Type[][], Type[][], MethodBuilder> defineMethod,
            Func<MethodInfo> postAction = null) {
            _methodName   = methodName;
            _returnType   = typeof(void);
            _defineMethod = defineMethod;
            _body         = new DeferredILGeneratorEmitter();
        }

        internal Type ReturnType {
            get {
                if (_genericReturnType != null) {
                    return _genericReturnType.AsType();
                } else if (_genericReturnTypes != null) {
                }

                return _returnType;
            }
        }

        public MethodAttributes Attributes { get; set; } = System.Reflection.MethodAttributes.Public;

        public IEmitter Body() {
            return _body;
        }

        public IMethodBuilder Body(Action<IEmitter> action) {
            action(_body);
            return this;
        }

        public IMethodBuilder Param<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            return Param(typeof(TParam), parameterName, attrs);
        }

        public IMethodBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            ThrowIfDefined();
            _parms.Add(new FluentParameterBuilder(
                    parameterType,
                    parameterName,
                    attrs));

            return this;
        }

        public IMethodBuilder Param(Action<IParameterBuilder> action) {
            ThrowIfDefined();
            var builder = new FluentParameterBuilder();
            _parms.Add(builder);
            action(builder);
            return this;
        }

        public IMethodBuilder Param(IParameterBuilder parameter) {
            ThrowIfDefined();
            _parms.Add((FluentParameterBuilder)parameter);
            return this;
        }

        public IMethodBuilder Params(params Type[] parameterTypes) {
            ThrowIfDefined();
            _parms = parameterTypes.Select(
                t => new FluentParameterBuilder(t, null, ParameterAttributes.None))
                .ToList();
            return this;
        }

        public IMethodBuilder Params(params IParameterBuilder[] parameters) {
            ThrowIfDefined();
            _parms = parameters
                .Cast<FluentParameterBuilder>()
                .ToList();
            return this;
        }

        public IParameterBuilder CreateParam<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            return CreateParam(typeof(TParam), parameterName, attrs);
        }

        public IParameterBuilder CreateParam(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            ThrowIfDefined();
            return new FluentParameterBuilder(
                    parameterType,
                    parameterName,
                    attrs);
        }

        public bool HasParameter(string parameterName) {
            return _parms.Any(p => p.ParameterName == parameterName);
        }

        public IParameterBuilder GetParameter(string parameterName) {
            return _parms.FirstOrDefault(p => p.ParameterName == parameterName);
        }

        public IMethodBuilder MethodAttributes(MethodAttributes attributes) {
            ThrowIfDefined();
            Attributes = attributes;
            return this;
        }

        public IMethodBuilder CallingConvention(CallingConventions callingConventions) {
            ThrowIfDefined();
            _callingConvention = callingConventions;
            return this;
        }

        public IMethodBuilder Returns<TReturn>() {
            return Returns(typeof(TReturn));
        }

        public IMethodBuilder Returns(Type returnType) {
            ThrowIfDefined();
            _returnType = returnType;
            return this;
        }

        public IMethodBuilder Returns(IGenericParameterBuilder genericType) {
            ThrowIfDefined();
            _genericReturnType = genericType;
            return this;
        }

        public IMethodBuilder Returns(Type genericTypeDefintion, params IGenericParameterBuilder[] genericTypes) {
            ThrowIfDefined();
            _returnType = genericTypeDefintion;
            _genericReturnTypes = genericTypes;
            return this;
        }

        public IGenericParameterBuilder NewGenericParameter(string parameterName) {
            _genericParameterBuilders = _genericParameterBuilders ?? new List<FluentGenericParameterBuilder>();
            var builder = new FluentGenericParameterBuilder(
                parameterName,
                (name) => {
                    Define();
                    return GetGenericParameter(name);
                });

            _genericParameterBuilders.Add(builder);
            return builder;
        }

        public IMethodBuilder NewGenericParameter(string parameterName, Action<IGenericParameterBuilder> action) {
            _genericParameterBuilders = _genericParameterBuilders ?? new List<FluentGenericParameterBuilder>();
            var builder = new FluentGenericParameterBuilder(
                parameterName,
                (name) => {
                    Define();
                    return GetGenericParameter(name);
                });

            action(builder);
            _genericParameterBuilders.Add(builder);
            return this;
        }

        public IMethodBuilder NewGenericParameters(params string[] parameterNames) {
            return NewGenericParameters(parameterNames, (Action<IGenericParameterBuilder[]>)null);
        }

        public IMethodBuilder NewGenericParameters(string[] parameterNames, Action<IGenericParameterBuilder[]> action) {
            _genericParameterBuilders = _genericParameterBuilders ?? new List<FluentGenericParameterBuilder>();
            foreach (var parameterName in parameterNames) {
                var builder = new FluentGenericParameterBuilder(
                    parameterName,
                    (name) => {
                        Define();
                        return GetGenericParameter(parameterName);
                    });

                _genericParameterBuilders.Add(builder);
            }

            action?.Invoke(_genericParameterBuilders.ToArray());

            return this;
        }

        public GenericTypeParameterBuilder GetGenericParameter(string parameterName) {
            return _genericParameters
                .FirstOrDefault(g => g.Name == parameterName);
        }

        public IMethodBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute) {
            _customAttributes = _customAttributes ?? new List<CustomAttributeBuilder>();
            _customAttributes.Add(customAttribute);
            return this;
        }

        public IMethodBuilder SetImplementationFlags(MethodImplAttributes attributes) {
            _methodImplAttributes = attributes;
            return this;
        }

        public MethodBuilder Define() {
            if (_methodBuilder != null) {
                return _methodBuilder;
            }

            int parmCount = _parms.Count;
            Type[] parameterTypes = new Type[parmCount];
            for (int i = 0; i < parmCount; i++) {
                parameterTypes[i] = _parms[i].ParameterType;
            }

            _methodBuilder = _defineMethod(
                    _methodName,
                    Attributes,
                    _callingConvention,
                    ReturnType,
                    null,
                    null,
                    parameterTypes,
                    null,
                    null);

            if (_genericParameterBuilders != null) {
                _genericParameters = _methodBuilder.DefineGenericParameters(
                _genericParameterBuilders.Select(g => g.ParameterName).ToArray());

                for (int i = 0; i < _genericParameterBuilders.Count; i++) {
                    _genericParameterBuilders[i].Build(_genericParameters[i]);
                }
            }

            int parmIndex = 0;
            foreach (var parm in _parms) {
                var paramBuilder = _methodBuilder
                    .DefineParameter(++parmIndex, parm.Attributes, parm.ParameterName);

                parm.CustomAttributes.SetCustomAttributes(a => paramBuilder.SetCustomAttribute(a));
            }

            _customAttributes.SetCustomAttributes(a => _methodBuilder.SetCustomAttribute(a));

            _methodBuilder.SetImplementationFlags(_methodImplAttributes);

            _body.EmitIL(_methodBuilder.GetILGenerator());

            return _methodBuilder;
        }

        private void ThrowIfDefined() {
            if (_methodBuilder != null) {
                throw new InvalidOperationException("Method already defined");
            }
        }
    }
}