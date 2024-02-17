namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IntegerAttribute : DataTypeAttribute {
        public IntegerAttribute() : base("integer") {
        }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "{0} 不是有效的整数";
            }

            return base.FormatErrorMessage(name);
        }

        public override bool IsValid(object value) {
            if (value == null) return true;

            return int.TryParse(Convert.ToString(value), out int _);
        }
    }
}
