namespace Magicube.Core.Reflection.Builders {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class FluentGenericParameterBuilder : IGenericParameterBuilder {
        private readonly Func<string, GenericTypeParameterBuilder> _define;

        private Type _baseType;

        private List<Type> _interfaceTypes;

        private GenericTypeParameterBuilder _builder;

        public FluentGenericParameterBuilder(
            string parameterName,
            Func<string, GenericTypeParameterBuilder> defineFunc = null) {
            ParameterName = parameterName;
            _define = defineFunc;
        }

        public string ParameterName { get; }

        internal GenericTypeParameterBuilder ParameterBuilder => _builder;

        public GenericParameterAttributes Attributes { get; set; }

        public IGenericParameterBuilder BaseType<T>() {
            return BaseType(typeof(T));
        }

        public IGenericParameterBuilder BaseType(Type baseType) {
            _baseType = baseType;
            return this;
        }

        public IGenericParameterBuilder InterfaceType<T>() {
            return InterfaceType(typeof(T));
        }

        public IGenericParameterBuilder InterfaceType(Type interfaceType) {
            if (interfaceType.IsInterface == false)
            {
                throw new InvalidOperationException("Type must be an interface");
            }

            _interfaceTypes = _interfaceTypes ?? new List<Type>();
            _interfaceTypes.Add(interfaceType);
            return this;
        }

        public IGenericParameterBuilder Contravariant() {
            Attributes |= GenericParameterAttributes.Contravariant;
            return this;
        }

        public IGenericParameterBuilder Covariant() {
            Attributes |= GenericParameterAttributes.Covariant;
            return this;
        }

        public IGenericParameterBuilder DefaultConstructor() {
            Attributes |= GenericParameterAttributes.DefaultConstructorConstraint;
            return this;
        }

        public IGenericParameterBuilder NotNullableValueType() {
            Attributes |= GenericParameterAttributes.NotNullableValueTypeConstraint;
            return this;
        }

        public IGenericParameterBuilder ReferenceType() {
            Attributes |= GenericParameterAttributes.ReferenceTypeConstraint;
            return this;
        }

        public void Build(GenericTypeParameterBuilder builder) {
            _builder = builder;

            if (_baseType != null) {
                _builder.SetBaseTypeConstraint(_baseType);
            }

            if (_interfaceTypes != null) {
                _builder.SetInterfaceConstraints(_interfaceTypes.ToArray());
            }

            _builder.SetGenericParameterAttributes(Attributes);
        }

        public Type AsType() {
            return _builder;
        }
    }
}