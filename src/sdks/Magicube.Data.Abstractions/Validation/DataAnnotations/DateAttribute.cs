namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateAttribute : DataTypeAttribute {
        public DateAttribute() : base(DataType.Date) {
        }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "{0} 不是一个有效的日期";
            }

            return base.FormatErrorMessage(name);
        }

        public override bool IsValid(object value) {
            if (value == null) return true;

            return DateTime.TryParse(Convert.ToString(value), out DateTime _);
        }
    }
}

