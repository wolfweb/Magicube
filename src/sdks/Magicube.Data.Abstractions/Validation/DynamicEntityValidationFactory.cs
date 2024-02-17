using Magicube.Core;
using Magicube.Core.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using PropertyInfo = System.Reflection.PropertyInfo;

namespace Magicube.Data.Abstractions.Validation {
    public class DynamicEntityValidationFactory {
        private static ConcurrentDictionary<string, ValidatorContext> _validations = new ConcurrentDictionary<string, ValidatorContext>(StringComparer.OrdinalIgnoreCase);
        private static object _lockObj = new object();

        static DynamicEntityValidationFactory() {
            var validations = TypeAccessor.Get<RequiredAttribute>().ScanTypes<ValidationAttribute>();

            foreach (var validate in validations) {
                var name = validate.Name.Replace("Attribute", "");
                _validations.TryAdd(name, new ValidatorContext { 
                    Name    = name,
                    Type    = validate,
                    Display = name.ToFriendly(),
                    Context = TypeAccessor.Get(validate, null).Context
                });
            }

            Register<DateAttribute>();
            Register<EqualToAttribute>();
            Register<IntegerAttribute>();
            Register<NumericAttribute>();
            Register<YearAttribute>();
        }

        public static IDictionary<string, ValidatorContext> Validations => _validations;

        public static CustomAttributeBuilder BuildValidAttribute(DbFieldValidator dbFieldValidator) {
            CustomAttributeBuilder builder = null;
            if (_validations.TryGetValue(dbFieldValidator.Provider, out var context)) {
                var typeAccessor = TypeAccessor.Get(context.Type, null).Context;
                var ctor = typeAccessor.Constructors.First().Constructor;
                if(dbFieldValidator.Args != null && dbFieldValidator.Args.Any()) {
                    var args = dbFieldValidator.Args.Select(x => new KeyValuePair<PropertyInfo, object>(typeAccessor.Properties.FirstOrDefault(m => m.Member.Name == x.Key).Member, x.Value));
                    builder = new CustomAttributeBuilder(ctor, Array.Empty<object>(), args.Select(x=>x.Key).ToArray(), args.Select(x => x.Value).ToArray());
                } else {
                    builder = new CustomAttributeBuilder(ctor, Array.Empty<object>());
                }
            }
            return builder;
        }

        public static void Validator(string fieldName, object value, IEnumerable<DbFieldValidator> validators) {
            foreach (var item in validators) {
                if(_validations.TryGetValue(item.Provider, out ValidatorContext context)) {
                    var validator = New<ValidationAttribute>.Creator(context.Type);
                    validator.ErrorMessage = item.ErrorMessage;

                    if ((context.Properties != null && context.Properties.Any()) && (item.Args != null && item.Args.Any())) {
                        foreach (var property in context.Properties) {
                            validator.SetValue(property, item.Args.ContainsKey(property.Name) ? item.Args[property.Name] : default);
                        }
                    }

                    ValidationError validationError;
                    if (!TryValidate(value, new ValidationContext(new object()) { MemberName = fieldName }, validator, out validationError)) {
                        validationError.ThrowValidationException();
                    }
                }
            }
        }

        public static void Register<T>() {
            lock (_lockObj) {
                var type = typeof(T);
                var name = type.Name.Replace("Attribute", "");
                _validations.TryAdd(name, new ValidatorContext { 
                    Name    = name,
                    Type    = type,
                    Display = name.ToFriendly(),
                    Context = TypeAccessor.Get(type, null).Context
                });
            }
        }

        private static bool TryValidate(object value, ValidationContext validationContext, ValidationAttribute attribute, out ValidationError validationError) {
            ValidationResult validationResult = attribute.GetValidationResult(value, validationContext);
            if (validationResult != ValidationResult.Success) {
                validationError = new ValidationError(attribute, value, validationResult);
                return false;
            }
            validationError = null;
            return true;
        }

        sealed class ValidationError {
            private readonly object _value;

            private readonly ValidationAttribute _validationAttribute;

            public ValidationResult ValidationResult { get; }

            internal ValidationError(ValidationAttribute validationAttribute, object value, ValidationResult validationResult) {
                _validationAttribute = validationAttribute;
                ValidationResult = validationResult;
                _value = value;
            }

            internal void ThrowValidationException() {
                throw new ValidationException(ValidationResult, _validationAttribute, _value);
            }
        }
    }    
    
    public class ValidatorContext {
        public string                    Name    { get; set; }
        public string                    Display { get; set; }
        public Type                      Type    { get; set; }
        public ReflectionContext         Context { get; set; }

        public IEnumerable<PropertyInfo> Properties { 
            get {
                return Context?.Properties.Where(x => x.Member.DeclaringType.Equals(Type)).Select(x => x.Member);
            } 
        }
    }
}
