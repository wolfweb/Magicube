using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Magicube.Core.Reflection.Builders {
    internal class FluentPropertyBuilder : IPropertyBuilder {
        private readonly Func<string, PropertyAttributes, CallingConventions, Type, Type[], Type[], Type[], Type[][], Type[][], PropertyBuilder> _define;

        private readonly ITypeBuilder _typeBuilder;

        private readonly string _name;

        private readonly IFieldBuilder _propertyField;

        private readonly Type _propertyType;

        private PropertyBuilder _propertyBuilder;

        private List<CustomAttributeBuilder> _customAttributes;

        private CallingConventions _callingConvention = CallingConventions.HasThis;

        public FluentPropertyBuilder(
            ITypeBuilder typeBuilder,
            string propertyName,
            Type propertyType,
            Func<string, PropertyAttributes, CallingConventions, Type, Type[], Type[], Type[], Type[][], Type[][], PropertyBuilder> define) {
            _define        = define;
            _typeBuilder   = typeBuilder;
            _name          = propertyName;
            _propertyType  = propertyType;
            _propertyField = _typeBuilder.NewField($"_{propertyName.ToSnakeCase()}", _propertyType).Private();
            
            PropertyAttributes = PropertyAttributes.None;

            Getter();
            Setter();
        }

        public PropertyAttributes PropertyAttributes { get; set; }

        public IMethodBuilder SetMethod { get; set; }

        public IMethodBuilder GetMethod { get; set; }

        public IPropertyBuilder Getter(Action<IMethodBuilder> action) {
            action.Invoke(Getter());
            return this;
        }

        public IMethodBuilder Getter() {
            if (GetMethod == null) {
                GetMethod = _typeBuilder
                    .NewMethod($"get_{_name}")
                    .Public()
                    .CallingConvention(_callingConvention)
                    .SpecialName()
                    .Returns(_propertyType);

                GetMethod.Body().LdArg0().LdFld(_propertyField).Ret();
            }

            return GetMethod;
        }

        public IPropertyBuilder Setter(Action<IMethodBuilder> action) {
            action.Invoke(Setter());
            return this;
        }

        public IMethodBuilder Setter() {
            if (SetMethod == null) {
                SetMethod = _typeBuilder
                    .NewMethod($"set_{_name}")
                    .Public()
                    .CallingConvention(_callingConvention)
                    .Param(_propertyType, "value", ParameterAttributes.None)
                    .Returns(typeof(void));

                SetMethod.Body().LdArg0().LdArg1().StFld(_propertyField).Ret();
            }

            return SetMethod;
        }

        public IPropertyBuilder Attributes(PropertyAttributes attributes) {
            PropertyAttributes = attributes;
            return this;
        }

        public IPropertyBuilder CallingConvention(CallingConventions callingConvention) {
            _callingConvention = callingConvention;
            return this;
        }

        public PropertyBuilder Define() {
            if (_propertyBuilder == null) {
                _propertyBuilder = _define(
                    _name,
                    PropertyAttributes,
                    _callingConvention,
                    _propertyType,
                    null,
                    null,
                    null,
                    null,
                    null);

                if (GetMethod != null) {
                    _propertyBuilder.SetGetMethod(GetMethod.Define());
                }

                if (SetMethod != null) {
                    _propertyBuilder.SetSetMethod(SetMethod.Define());
                }

                _customAttributes.SetCustomAttributes(a => _propertyBuilder.SetCustomAttribute(a));
            }

            return _propertyBuilder;
        }

        public IPropertyBuilder SetCustomAttribute(CustomAttributeBuilder customAttribute) {
            _customAttributes ??= new List<CustomAttributeBuilder>();
            _customAttributes.Add(customAttribute);
            return this;
        }
    }
}