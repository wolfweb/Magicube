using Magicube.Data.Abstractions.Validation;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EqualToAttribute : ValidationAttribute {
        public EqualToAttribute(string otherProperty) {
            if (otherProperty == null) {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
            OtherPropertyDisplayName = null;
        }

        public string OtherProperty { get; private set; }

        public string OtherPropertyDisplayName { get; set; }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "'{0}' 和 '{1}' 不匹配.";
            }

            var otherPropertyDisplayName = OtherPropertyDisplayName ?? OtherProperty;

            return string.Format(ErrorMessageString, name, otherPropertyDisplayName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var memberNames = new[] { validationContext.MemberName };
            object otherPropertyValue;
            if (typeof(DynamicObject).IsAssignableFrom( validationContext.ObjectType) ){
                DynamicObject instance = validationContext.ObjectInstance as DynamicObject;
                instance.TryGetMember(new KeyGetMemberBinder(OtherProperty), out otherPropertyValue);
            } else {
                PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
                if (otherPropertyInfo == null) {
                    return new ValidationResult(string.Format("无法找到字段 {0}.", OtherProperty), memberNames);
                }

                var displayAttribute = otherPropertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

                if (displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)) {
                    OtherPropertyDisplayName = displayAttribute.Name;
                }
                otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            }

            if (!Equals(value, otherPropertyValue)) {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
            }
            return null;
        }
    }
}

