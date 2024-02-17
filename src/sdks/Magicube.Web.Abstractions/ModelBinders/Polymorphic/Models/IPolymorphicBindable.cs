using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Magicube.Web.ModelBinders.Polymorphic {
    public interface IPolymorphicBindable {
        Type BindToType { get; }

        bool IsBindable(ModelBindingContext bindingContext);
    }

    public class CustomPolymorphicBindable : IPolymorphicBindable {
        private readonly Func<ModelBindingContext, bool> _isMatchFunc;

        public CustomPolymorphicBindable(Type type, Func<ModelBindingContext, bool> isMatchFunc) {
            BindToType = type;
            _isMatchFunc = isMatchFunc;
        }

        public Type BindToType { get; }

        public bool IsBindable(ModelBindingContext bindingContext) => _isMatchFunc(bindingContext);
    }

    public class DiscriminatorPolymorphicBindable : IPolymorphicBindable {
        private readonly string _discriminatorFieldName;
        private readonly Func<string, bool> _valueMatch;

        public DiscriminatorPolymorphicBindable(Type bindToType, string discriminatorFieldName)
            : this(bindToType, discriminatorFieldName, value => value == bindToType.Name) {
        }

        public DiscriminatorPolymorphicBindable(Type bindToType, string discriminatorFieldName, Func<string, bool> valueMatch) {
            BindToType = bindToType;
            _discriminatorFieldName = discriminatorFieldName;
            _valueMatch = valueMatch;
        }

        public Type BindToType { get; set; }

        public bool IsBindable(ModelBindingContext bindingContext) {
            var fieldName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, _discriminatorFieldName);
            var value = bindingContext.ValueProvider.GetValue(fieldName);

            if (value == ValueProviderResult.None)
                throw new ArgumentNullException(_discriminatorFieldName);

            return _valueMatch(value.FirstValue);
        }
    }

    class TypeInValuePolymorphicBindable : IPolymorphicBindable {
        public const string FieldName = "PolymorphicTypeValue";

        public TypeInValuePolymorphicBindable(Type type) {
            BindToType = type;
        }

        public Type BindToType { get; }

        public bool IsBindable(ModelBindingContext bindingContext) {
            var fieldName = ModelNames.CreatePropertyModelName(bindingContext.ModelName, FieldName);
            var value = bindingContext.ValueProvider.GetValue(fieldName);

            if (value == ValueProviderResult.None)
                return false;

            return Type.GetType(value.FirstValue) == BindToType;
        }
    }
}
