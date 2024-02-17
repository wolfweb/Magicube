using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NumericAttribute : DataTypeAttribute {
        private static Regex _regex = new Regex(@"^[\d\.]+$");

        public NumericAttribute() : base("numeric") {
        }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "{0} 不是有效的数字.";
            }

            return base.FormatErrorMessage(name);
        }

        public override bool IsValid(object value) {
            if (value == null) return false;
            return _regex.IsMatch(Convert.ToString(value));
        }
    }
}
