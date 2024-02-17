using System;

namespace Magicube.Core.Models {
	public struct ValueObject : IEquatable<ValueObject>, IConvertible {
		private readonly object _v;
		private readonly Type _v_type;
        public ValueObject(object v) {
			_v = v;
			_v_type = v?.GetType();
		}

		public override string ToString() {
			return _v.ToString();
		}

		public static implicit operator ValueObject(char v) => new ValueObject(v);

        public static implicit operator ValueObject(byte v) => new ValueObject(v);

        public static implicit operator ValueObject(short v) => new ValueObject(v);

        public static implicit operator ValueObject(ushort v) => new ValueObject(v);

        public static implicit operator ValueObject(int v) => new ValueObject(v);

        public static implicit operator ValueObject(uint v) => new ValueObject(v);

        public static implicit operator ValueObject(long v) => new ValueObject(v);

        public static implicit operator ValueObject(ulong v) => new ValueObject(v);

        public static implicit operator ValueObject(float v) => new ValueObject(v);

        public static implicit operator ValueObject(double v) => new ValueObject(v);

        public static implicit operator ValueObject(decimal v) => new ValueObject(v);

        public static implicit operator ValueObject(bool v) => new ValueObject(v);

        public static implicit operator ValueObject(string v) => new ValueObject(v);

        public static implicit operator ValueObject(Guid v) => new ValueObject(v);

        public static implicit operator ValueObject(DateTime dateTime) => new ValueObject(dateTime);

        public static implicit operator ValueObject(DateTimeOffset dateTime) => new ValueObject(dateTime);

        public static implicit operator string(ValueObject v) {
			if (v.Equals(default)) return null;
			if (v._v_type == typeof(Guid)) return ((Guid)v._v).ToString("n");
			if (v._v_type == typeof(DateTime)) {
                var _date = (DateTime)v._v;
				if(_date.Kind == DateTimeKind.Utc) {
					return _date.ToLocalTime().ToString("s");
				}
				return _date.ToString("s");
            }
			if (v._v_type == typeof(DateTimeOffset)) {
                var _date = (DateTimeOffset)v._v;
				if(_date.Offset == TimeSpan.Zero) {
					return _date.ToLocalTime().ToString("s");
				}
				return _date.ToString("s");
            }
			return v.ToString();
		}

		public static implicit operator bool(ValueObject v) {
			if (v.Equals(default)) return false;

			decimal _v = v;
			if (_v > 0) return Convert.ToBoolean(_v);
			if (v._v_type == typeof(bool)) return (bool)v._v;

			bool.TryParse(v, out bool result);
			return result;
		}

		public static implicit operator decimal(ValueObject v) {
			if (v.Equals(default)) return 0;

			double _v = v;
			if (_v > 0) return Convert.ToDecimal(_v);
			if (v._v_type == typeof(decimal)) return (decimal)v._v;

			decimal.TryParse(v, out decimal result);
			return result;
		}

		public static implicit operator double(ValueObject v) {
			if (v.Equals(default)) return 0;

			float _v = v;
			if (_v > 0) return Convert.ToDouble(_v);
			if (v._v_type == typeof(double)) return (double)v._v;

			double.TryParse(v, out double result);
			return result;
		}

		public static implicit operator float(ValueObject v) {
			if (v.Equals(default)) return 0;

			long _v = v;
			if (_v > 0) return Convert.ToSingle(_v);
			if (v._v_type == typeof(float)) return (float)v._v;

			float.TryParse(v, out float result);
			return result;
		}

		public static implicit operator long(ValueObject v) {
			if (v.Equals(default)) return long.MinValue;

			int _v = v;
			if (_v > 0) return Convert.ToInt64(_v);
			if (v._v_type == typeof(long)) return (long)v._v;

			long.TryParse(v, out long result);
			return result;
		}

		public static implicit operator int(ValueObject v) {
			if (v.Equals(default)) return int.MinValue;
			short _v = v;
			if (_v > 0) return Convert.ToInt32(_v);
			if (v._v_type == typeof(int)) return (int)v._v;

			if (v._v_type == typeof(string) && DateTime.TryParse(v, out DateTime d)) return (int)d.ToUnixTimeSeconds();

			if (v._v_type == typeof(DateTime)) {
				DateTime dt = v;
				return (int)dt.ToUnixTimeSeconds();
			}

			if (v._v_type == typeof(DateTimeOffset)) {
				return (int)((DateTimeOffset)v._v).ToUnixTimeSeconds();
			}

			int.TryParse(v, out int result);
			return result;
		}

		public static implicit operator short(ValueObject v) {
			if (v.Equals(default)) return short.MinValue;
			if (v._v_type == typeof(byte)) return Convert.ToInt16((byte)v);
			if (v._v_type == typeof(short)) return (short)v._v;
			if (v._v_type == typeof(char)) return Convert.ToInt16((char)v);
			if (v._v_type == typeof(int)) {
				int _v = (int)v._v;
				if (_v <= short.MaxValue) return Convert.ToInt16(_v);
			}

			short.TryParse(v, out short result);
			return result;
		}

		public static implicit operator char(ValueObject v) {
			if (v.Equals(default)) return char.MinValue;
			if (v._v_type == typeof(byte)) return Convert.ToChar((byte)v);
			if (v._v_type == typeof(short)) return Convert.ToChar((short)v);
			if (v._v_type == typeof(char)) return (char)v._v;
			if (v._v_type == typeof(int)) {
				int _v = (int)v._v;
				if (_v <= ushort.MaxValue) return Convert.ToChar(_v);
			}

			char.TryParse(v, out char result);
			return result;
		}

		public static implicit operator byte(ValueObject v) {
			if (v.Equals(default)) return byte.MinValue;
			if (v._v_type == typeof(byte)) return (byte)v._v;
			if (v._v_type == typeof(short)) return Convert.ToByte((short)v);
			if (v._v_type == typeof(char)) return Convert.ToByte((char)v);
			if (v._v_type == typeof(int)) {
				int _v = (int)v._v;
				if (_v <= byte.MaxValue) return Convert.ToByte(_v);
			}

			byte.TryParse(v, out byte result);
			return result;
		}

		public static implicit operator DateTimeOffset(ValueObject v) {
			if (v.Equals(default)) return DateTime.MinValue;

			var type = v._v.GetType();

			Func<long, DateTimeOffset> convert = d => {
				if (Math.Round(DateTimeOffset.Now.ToUnixTimeMilliseconds() / (d * 1f)) == 1000)
					return DateTimeOffset.FromUnixTimeSeconds(d);
				return DateTimeOffset.FromUnixTimeMilliseconds(d);
			};

			if (type == typeof(string)) {
				long _v = v;
				if (_v > 0) return convert(_v);
			}
			if (type == typeof(long) || type == typeof(int)) {
				return convert(v);
			}

			DateTimeOffset.TryParse(v, out DateTimeOffset result);
			return result;
		}

		public static implicit operator DateTime(ValueObject v) {
			if (v.Equals(default)) return DateTime.MinValue;

			DateTimeOffset result = v;
			return result.UtcDateTime;
		}

		public static implicit operator Guid(ValueObject v) {
			if (v.Equals(default)) return Guid.Empty;
			Guid.TryParse(v, out Guid result);
			return result;
		}

		public bool Equals(ValueObject other) {
			return this == other;
		}

		public bool IsNull => _v == null;

		public static bool operator ==(ValueObject first, ValueObject second) {
			if (first.IsNull) return second.IsNull;
			if (second.IsNull) return false;

			if(first._v_type == second._v_type) {
				return first._v == second._v;
			}

			try {
				return Convert.ChangeType(second._v, first._v_type).Equals(first._v);
			} catch {
				return false;
			}
		}

		public static bool operator !=(ValueObject first, ValueObject second) {
			if (first.IsNull) return !second.IsNull;
			if (second.IsNull) return true;
			return first.ToString() != second.ToString();
		}

		public override bool Equals(object obj) {
			return Equals(new ValueObject(obj));
		}

		public override int GetHashCode() {
			unchecked {
				int hashcode = 1430287;
				if (!IsNull) hashcode = hashcode * 7302013 ^ _v.GetHashCode();
				return hashcode;
			}
		}

        public TypeCode GetTypeCode() => TypeCode.Object;

        public bool ToBoolean(IFormatProvider provider) => this;

		public byte ToByte(IFormatProvider provider) => this;

		public char ToChar(IFormatProvider provider) => this;

		public DateTime ToDateTime(IFormatProvider provider) => this;

		public decimal ToDecimal(IFormatProvider provider) => this;

		public double ToDouble(IFormatProvider provider) => this;

		public short ToInt16(IFormatProvider provider) => this;

		public int ToInt32(IFormatProvider provider) => this;

		public long ToInt64(IFormatProvider provider) => this;

		public sbyte ToSByte(IFormatProvider provider) => (sbyte)(byte)this;

		public float ToSingle(IFormatProvider provider) => this;

		public string ToString(IFormatProvider provider) => this;

        public object ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == null) throw new ArgumentNullException("conversionType");

			if (conversionType == typeof(ValueObject)) return this;

			if (conversionType == typeof(DateTimeOffset)) return (DateTimeOffset)this;
			return Type.GetTypeCode(conversionType) switch {
				TypeCode.DateTime => (DateTime)this,
				TypeCode.Boolean  => (bool)this,
				TypeCode.Decimal  => (decimal)this,
				TypeCode.Double   => (double)this,
				TypeCode.Single   => (float)this,
				TypeCode.String   => (string)this,
				TypeCode.UInt16   => checked((ushort)(short)this),
				TypeCode.UInt32   => (uint)(int)this,
				TypeCode.UInt64   => (ulong)(long)this,
				TypeCode.Object   => this,
				TypeCode.SByte    => (sbyte)(int)this,
				TypeCode.Int16    => (short)(int)this,
				TypeCode.Int32    => (int)this,
				TypeCode.Int64    => (long)this,
				TypeCode.Byte     => checked((byte)this),
				TypeCode.Char     => checked((char)this),
				_ => throw new NotSupportedException(),
			};
		}

        public ushort ToUInt16(IFormatProvider provider) {
			return checked((ushort)(short)this);
		}

        public uint ToUInt32(IFormatProvider provider) {
			return (uint)(int)this;
		}

        public ulong ToUInt64(IFormatProvider provider) {
            return (ulong)(long)this;
		}

		public T Value<T>() => (T)Convert.ChangeType(this, typeof(T));
    }
}
