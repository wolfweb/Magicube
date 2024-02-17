using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Emitters;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentDynamicMethodBuilder : IDynamicMethodBuilder {
        private string _methodName;

        private Type _methodOwner;

        private Type _returnType;

        private List<FluentParameterBuilder> _parms = new List<FluentParameterBuilder>();

        private DynamicMethod _dynamicMethod;

        internal FluentDynamicMethodBuilder(string methodName, Type methodOwner) {
            _methodName = methodName;
            _methodOwner = methodOwner;
            _returnType = typeof(void);
        }

        private void ThrowIfDefined() {
            if (_dynamicMethod != null) {
                throw new InvalidOperationException("Method already defined");
            }
        }

        public IEmitter Body() {
            Define();

            var il = _dynamicMethod.GetILGenerator();
            var emitter = new ILGeneratorEmitter(il);
            return emitter;
        }

        public IDynamicMethodBuilder Body(Action<IEmitter> action) {
            action(Body());
            return this;
        }

        public IDynamicMethodBuilder Returns<TReturn>() {
            return Returns(typeof(TReturn));
        }

        public IDynamicMethodBuilder Returns(Type returnType) {
            ThrowIfDefined();
            _returnType = returnType;
            return this;
        }

        public IDynamicMethodBuilder Param<TParam>(string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            return Param(typeof(TParam), parameterName, attrs);
        }

        public IDynamicMethodBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            ThrowIfDefined();
            _parms.Add(new FluentParameterBuilder(
                    parameterType,
                    parameterName,
                    attrs));

            return this;
        }

        public IDynamicMethodBuilder Param(Action<IParameterBuilder> action) {
            ThrowIfDefined();
            var builder = new FluentParameterBuilder();
            _parms.Add(builder);
            action(builder);
            return this;
        }

        public IDynamicMethodBuilder Param(IParameterBuilder parameter) {
            ThrowIfDefined();
            _parms.Add((FluentParameterBuilder)parameter);
            return this;
        }

        public IDynamicMethodBuilder Params(params Type[] parameterTypes) {
            ThrowIfDefined();
            _parms = parameterTypes.Select(t => new FluentParameterBuilder(t, null, ParameterAttributes.None)).ToList();
            return this;
        }

        public IDynamicMethodBuilder Params(params IParameterBuilder[] parameters) {
            ThrowIfDefined();
            _parms = parameters.Cast<FluentParameterBuilder>().ToList();
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

        public DynamicMethod Define() {
            if (_dynamicMethod == null) {
                int parmCount = _parms.Count;
                Type[] parameterTypes = new Type[parmCount];
                for (int i = 0; i < parmCount; i++) {
                    parameterTypes[i] = _parms[i].ParameterType;
                }

                _dynamicMethod = new DynamicMethod(_methodName, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, _returnType, parameterTypes, _methodOwner, false);
            }

            return _dynamicMethod;
        }
    }
}