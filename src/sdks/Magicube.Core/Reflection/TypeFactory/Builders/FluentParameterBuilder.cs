using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentParameterBuilder : IParameterBuilder {
        private Type _parameterType;

        public FluentParameterBuilder() {
        }

        public FluentParameterBuilder(Type parameterType, string parameterName, ParameterAttributes attrs) {
            _parameterType = parameterType;
            ParameterName = parameterName;
            Attributes = attrs;
        }

        public FluentParameterBuilder(IGenericParameterBuilder parameterType, string parameterName, ParameterAttributes attrs) {
            GenericParameterType = parameterType;
            ParameterName = parameterName;
            Attributes = attrs;
        }

        internal IGenericParameterBuilder GenericParameterType { get; }

        internal Type ParameterType {
            get {
                return _parameterType ?? GenericParameterType.AsType();
            }
        }

        internal string ParameterName { get; private set; }

        internal ParameterAttributes Attributes { get; private set; } = ParameterAttributes.None;

        internal List<CustomAttributeBuilder> CustomAttributes { get; private set; }

        public IParameterBuilder Type<T>() {
            return Type(typeof(T));
        }

        public IParameterBuilder Type(Type parameterType) {
            _parameterType = parameterType;
            return this;
        }

        public IParameterBuilder Name(string parameterName) {
            ParameterName = parameterName;
            return this;
        }

        public IParameterBuilder HasDefault() {
            Attributes |= ParameterAttributes.HasDefault;
            return this;
        }

        public IParameterBuilder HasFieldMarshal() {
            Attributes |= ParameterAttributes.HasFieldMarshal;
            return this;
        }

        public IParameterBuilder In() {
            Attributes |= ParameterAttributes.In;
            return this;
        }

        public IParameterBuilder Lcid() {
            Attributes |= ParameterAttributes.Lcid;
            return this;
        }

        public IParameterBuilder Optional() {
            Attributes |= ParameterAttributes.Optional;
            return this;
        }

        public IParameterBuilder Out() {
            Attributes |= ParameterAttributes.Out;
            return this;
        }

        public IParameterBuilder Retval() {
            Attributes |= ParameterAttributes.Retval;
            return this;
        }

        public IParameterBuilder SetCustomAttribute(CustomAttributeBuilder attributeBuilder) {
            CustomAttributes = CustomAttributes ?? new List<CustomAttributeBuilder>();
            CustomAttributes.Add(attributeBuilder);
            return this;
        }
    }
}