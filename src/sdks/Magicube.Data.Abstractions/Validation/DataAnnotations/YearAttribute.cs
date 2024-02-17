namespace System.ComponentModel.DataAnnotations {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class YearAttribute : DataTypeAttribute {
        public YearAttribute()
            : base("year") {
        }

        public int Start { get; set; }

        public int End   { get; set; }

        public override string FormatErrorMessage(string name) {
            if (ErrorMessage == null && ErrorMessageResourceName == null) {
                ErrorMessage = "{0} 不是一个有效年份";
            }

            return base.FormatErrorMessage(name);
        }

        public override bool IsValid(object value) {
            if (value == null) {
                return true;
            }

            int retNum;
            var parseSuccess = int.TryParse(Convert.ToString(value), out retNum);

            return parseSuccess && retNum >= Start && retNum <= End;
        }
    }
}

