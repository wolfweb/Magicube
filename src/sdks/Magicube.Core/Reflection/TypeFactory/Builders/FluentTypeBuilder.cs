using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentTypeBuilder : ITypeBuilder {
        private readonly Func<string, TypeAttributes, Type, Type[], TypeBuilder> _define;

        private readonly ModuleBuilder _moduleBuilder;

        private readonly string _typeName;

        private Type _baseType;

        private TypeBuilder _typeBuilder;

        private List<Type> _interfaces = new List<Type>();

        private List<Action> _actions = new List<Action>();

        private List<FluentGenericParameterBuilder> _genericParameters;

        private List<CustomAttributeBuilder> _customAttributes;

        private TypeInfo _typeInfo;

        private bool _genericParemetersBuilt;

        internal FluentTypeBuilder(ModuleBuilder moduleBuilder, string typeName) {
            _moduleBuilder = moduleBuilder;
            _typeName = typeName;
            TypeAttributes = TypeAttributes.Class;
        }

        internal FluentTypeBuilder(string typeName, Func<string, TypeAttributes, Type, Type[], TypeBuilder> define) {
            _typeName = typeName;
            _define = define;
        }

        public string TypeName => _typeName;

        public TypeAttributes TypeAttributes { get; set; }

        public IEnumerable<Type> Interfaces {
            get {
                return _interfaces;
            }
        }

        public ITypeBuilder Attributes(TypeAttributes attributes) {
            ThrowIfAlreadyBuilt();
            TypeAttributes = attributes;
            return this;
        }

        public ITypeBuilder InheritsFrom<T>() {
            return InheritsFrom(typeof(T));
        }

        public ITypeBuilder InheritsFrom(Type baseType) {
            ThrowIfAlreadyBuilt();

            if (baseType.IsInterface == true) {
                throw new InvalidOperationException("Type cannot be an interface.");
            }

            if (baseType.IsSealed == true) {
                throw new InvalidOperationException("Type cannot inherit from a sealed type.");
            }

            _baseType = baseType;
            return this;
        }

        public ITypeBuilder Implements<T>() {
            return Implements(typeof(T));
        }

        public ITypeBuilder Implements(Type interfaceType) {
            ThrowIfAlreadyBuilt();

            if (interfaceType.IsInterface == false) {
                throw new InvalidOperationException("Type must be an interface.");
            }

            _interfaces.Add(interfaceType);
            return this;
        }

        public IConstructorBuilder NewConstructor() {
            var ctorBuilder = new FluentConstructorBuilder(
                (attrs,
                calling,
                parms,
                required,
                optional) => {
                    var type = Define();

                    return type
                        .DefineConstructor(
                            attrs,
                            calling,
                            parms,
                            required,
                            optional);
                });

            _actions.Add(() => ctorBuilder.Define());
            return ctorBuilder;
        }

        public ITypeBuilder NewConstructor(Action<IConstructorBuilder> constructorBuilder) {
            constructorBuilder(NewConstructor());
            return this;
        }

        public ITypeBuilder NewDefaultConstructor(MethodAttributes constructorAttributes) {
            var ctorBuilder = new FluentConstructorBuilder(
                (attrs) => {
                    return Define().DefineDefaultConstructor(attrs);
                }) {
                MethodAttributes = constructorAttributes
            };
            _actions.Add(() => ctorBuilder.Define());
            return this;
        }

        public IFieldBuilder NewField(string fieldName, Type fieldType) {
            var fieldBuilder = new FluentFieldBuilder(
                fieldName,
                fieldType,
                (name,
                type,
                requiredCustomModifiers,
                optionalCustomModifiers,
                fieldAttributes) => {
                    return Define()
                        .DefineField(
                            name,
                            type,
                            requiredCustomModifiers,
                            optionalCustomModifiers,
                            fieldAttributes);
                });

            _actions.Add(() => fieldBuilder.Define());
            return fieldBuilder;
        }

        public IFieldBuilder NewField(string fieldName, Type fieldType, params IGenericParameterBuilder[] genericParameters) {
            if (fieldType.IsConstructedGenericType == true) {
                throw new ArgumentException("Must be generic type definition", nameof(fieldType));
            }

            var fieldBuilder = new FluentFieldBuilder(
                fieldName,
                fieldType,
                (name,
                type,
                requiredCustomModifiers,
                optionalCustomModifiers,
                fieldAttributes) => {
                    return Define()
                        .DefineField(
                            name,
                            type.MakeGenericType(genericParameters.Select(g => g.AsType()).ToArray()),
                            requiredCustomModifiers,
                            optionalCustomModifiers,
                            fieldAttributes);
                });

            _actions.Add(() => fieldBuilder.Define());
            return fieldBuilder;
        }

        public IFieldBuilder NewField(string fieldName, IGenericParameterBuilder genericParameterBuilder) {
            var fieldBuilder = new FluentFieldBuilder(
                fieldName,
                null,
                (name,
                type,
                requiredCustomModifiers,
                optionalCustomModifiers,
                fieldAttributes) => {
                    return Define().DefineField(
                            name,
                            genericParameterBuilder.AsType(),
                            requiredCustomModifiers,
                            optionalCustomModifiers,
                            fieldAttributes);
                });

            _actions.Add(() => fieldBuilder.Define());
            return fieldBuilder;
        }

        public ITypeBuilder NewField(string fieldName, Type fieldType, Action<IFieldBuilder> fieldBuilder) {
            fieldBuilder(NewField(fieldName, fieldType));
            return this;
        }

        public ITypeBuilder NewMethod(string methodName, Action<IMethodBuilder> action) {
            var builder = new FluentMethodBuilder(methodName, DefineMethod)
                .CallingConvention(CallingConventions.HasThis);

            action(builder);
            _actions.Add(() => builder.Define());
            return this;
        }

        public IMethodBuilder NewMethod(
            string methodName,
            MethodAttributes methodAttributes,
            CallingConventions callingConvention,
            Type returnType) {
            return NewMethod(methodName)
                .MethodAttributes(methodAttributes)
                .CallingConvention(callingConvention)
                .Returns(returnType);
        }

        public IMethodBuilder NewMethod(string methodName) {
            var builder = new FluentMethodBuilder(methodName, DefineMethod);
            _actions.Add(() => builder.Define());
            return builder;
        }

        public IPropertyBuilder NewProperty(
            string propertyName,
            Type propertyType) {
            var builder = new FluentPropertyBuilder(
                this,
                propertyName,
                propertyType,
                (name,
                attr,
                calling,
                returnType,
                returnTypeRequired,
                returnTypeOptional,
                parameterTypes,
                parameterTypesRequired,
                parameterTypesOptional) => {
                    return Define().DefineProperty(
                            name,
                            attr,
                            calling,
                            returnType,
                            returnTypeRequired,
                            returnTypeOptional,
                            parameterTypes,
                            parameterTypesRequired,
                            parameterTypesOptional);
                });

            _actions.Add(() => builder.Define());
            return builder;
        }

        public ITypeBuilder NewProperty(string propertyName, Type propertyType, Action<IPropertyBuilder> propertyBuilder) {
            propertyBuilder(NewProperty(propertyName, propertyType));
            return this;
        }

        public IEventBuilder NewEvent(string eventName, Type eventType) {
            var builder = new FluentEventBuilder(
                eventName,
                eventType,
                (name, attrs, type) => {
                    return Define().DefineEvent(
                            name,
                            attrs,
                            type);
                });

            _actions.Add(() => builder.Define());
            return builder;
        }

        public ITypeBuilder NewEvent(string eventName, Type eventType, Action<IEventBuilder> eventBuilder) {
            eventBuilder(NewEvent(eventName, eventType));
            return this;
        }

        public ITypeBuilder NewNestedType(string typeName) {
            var builder = new FluentTypeBuilder(
                typeName,
                (name, attrs, parent, interfaces) => {
                    return Define().DefineNestedType(
                            name,
                            attrs,
                            parent,
                            interfaces);
                });

            _actions.Add(() => builder.Define());
            return builder;
        }

        public IGenericParameterBuilder NewGenericParameter(string parameterName) {
            _genericParameters = _genericParameters ?? new List<FluentGenericParameterBuilder>();
            var builder = new FluentGenericParameterBuilder(parameterName);
            _genericParameters.Add(builder);
            return builder;
        }

        public ITypeBuilder NewGenericParameter(string parameterName, Action<IGenericParameterBuilder> action) {
            var builder = NewGenericParameter(parameterName);
            action?.Invoke(builder);
            return this;
        }

        public Type GetGenericParameterType(string parameterName) {
            if (_genericParameters.Any() == true) {
                BuildGenericParameters();

                return _genericParameters
                    .FirstOrDefault(p => p.ParameterName == parameterName)
                    ?.AsType();
            }

            return null;
        }

        public ITypeBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute) {
            _customAttributes = _customAttributes ?? new List<CustomAttributeBuilder>();
            _customAttributes.Add(customAttribute);
            return this;
        }

        public TypeBuilder Define() {
            if (_typeBuilder == null) {
                _typeBuilder = _moduleBuilder.DefineType(
                    _typeName,
                    TypeAttributes,
                    _baseType,
                    _interfaces.ToArray());

                BuildGenericParameters();

                _customAttributes.SetCustomAttributes(a => _typeBuilder.SetCustomAttribute(a));
            }

            return _typeBuilder;
        }

        public Type CreateType() {
            Define();

            if (_typeBuilder.IsCreated() == false) {
                foreach (var action in _actions) {
                    action();
                }

                _typeInfo = _typeBuilder.CreateTypeInfo();
            }

            return _typeInfo.AsType();
        }

        private void ThrowIfAlreadyBuilt() {
            if (_typeBuilder != null) {
                throw new InvalidOperationException("Type has been built");
            }
        }

        private void BuildGenericParameters() {
            Define();

            if (_genericParameters != null &&
                _genericParemetersBuilt == false) {
                _genericParemetersBuilt = true;

                var genericParms = _typeBuilder.DefineGenericParameters(
                    _genericParameters.Select(g => g.ParameterName).ToArray());

                for (int i = 0; i < _genericParameters.Count; i++) {
                    _genericParameters[i].Build(genericParms[i]);
                }
            }
        }

        private MethodBuilder DefineMethod(
            string name,
            MethodAttributes attributes,
            CallingConventions convention,
            Type returnType,
            Type[] returnTypeRequiredCustomModifiers,
            Type[] returnTypeOptionalCustomModifiers,
            Type[] parameterTypes,
            Type[][] parameterTypeRequiredCustomModifiers,
            Type[][] parameterTypeOptionalCustomModifiers) {
            return Define().DefineMethod(
                name,
                attributes,
                convention,
                returnType,
                returnTypeRequiredCustomModifiers,
                returnTypeOptionalCustomModifiers,
                parameterTypes,
                parameterTypeRequiredCustomModifiers,
                parameterTypeOptionalCustomModifiers);
        }
    }
}