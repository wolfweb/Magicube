using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Magicube.Core.Reflection.Emitters;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentConstructorBuilder : IConstructorBuilder {
        private readonly Func<MethodAttributes, CallingConventions, Type[], Type[][], Type[][], ConstructorBuilder> _define;

        private readonly Func<MethodAttributes, ConstructorBuilder> _defineDefault;

        private CallingConventions _callingConvention;

        private List<FluentParameterBuilder> _parameters = new List<FluentParameterBuilder>();

        private ConstructorBuilder _ctor;

        private MethodImplAttributes _methodImplAttributes;

        private EmitterBase _body;

        public FluentConstructorBuilder(Func<MethodAttributes, CallingConventions, Type[], Type[][], Type[][], ConstructorBuilder> define) {
            _define = define;
            _callingConvention = CallingConventions.HasThis;
            _body = new DeferredILGeneratorEmitter();
        }

        public FluentConstructorBuilder(Func<MethodAttributes, ConstructorBuilder> define) {
            _defineDefault = define;
        }

        public MethodAttributes MethodAttributes { get; set; }

        public IEmitter Body() {
            return _body;
        }

        public IConstructorBuilder CallingConvention(CallingConventions callingConvention) {
            _callingConvention = callingConvention;
            return this;
        }

        public IConstructorBuilder Param(Type parameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            _parameters.Add(new FluentParameterBuilder(parameterType, parameterName, attrs));
            return this;
        }

        public IConstructorBuilder Param(IGenericParameterBuilder genericParameterType, string parameterName, ParameterAttributes attrs = ParameterAttributes.None) {
            _parameters.Add(new FluentParameterBuilder(genericParameterType, parameterName, attrs));
            return this;
        }

        public IConstructorBuilder Param(Action<IParameterBuilder> action) {
            var builder = new FluentParameterBuilder();
            _parameters.Add(builder);
            action(builder);
            return this;
        }

        public IConstructorBuilder Params(params Type[] parameterTypes) {
            _parameters = parameterTypes.Select(t => new FluentParameterBuilder(t, null, ParameterAttributes.None)).ToList();
            return this;
        }

        public IConstructorBuilder SetMethodAttributes(MethodAttributes attributes) {
            MethodAttributes = attributes;
            return this;
        }

        public IConstructorBuilder SetImplementationFlags(MethodImplAttributes attributes) {
            _methodImplAttributes = attributes;
            return this;
        }

        public ConstructorBuilder Define() {
            if (_ctor != null) {
                return _ctor;
            }

            if (_define != null) {
                var parms = _parameters.Select(p => p.ParameterType).ToArray();

                _ctor = _define(
                    MethodAttributes,
                    _callingConvention,
                    parms,
                    null,
                    null);

                int i = 0;
                foreach (var parm in _parameters) {
                    _ctor.DefineParameter(++i, parm.Attributes, parm.ParameterName);
                }

                _ctor.SetImplementationFlags(_methodImplAttributes);

                _body.EmitIL(_ctor.GetILGenerator());
            } else if (_defineDefault != null) {
                _ctor = _defineDefault(MethodAttributes);
            }

            return _ctor;
        }
    }
}