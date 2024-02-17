namespace Magicube.Executeflow {
    public class NumberValue {
        private double value;
        public static implicit operator NumberValue(double value) {
            return new NumberValue { value = value };
        }
        public static implicit operator double(NumberValue number) {
            return number.value;
        }
        public static NumberValue operator +(NumberValue x, NumberValue y) {
            return x.value + y.value;
        }
    }
}
