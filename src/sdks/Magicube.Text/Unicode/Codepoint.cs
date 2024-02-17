using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Magicube.Text.Unicode {
    public class Codepoint : IComparable<Codepoint>, IComparable<uint>, IEquatable<Codepoint>,
        IEquatable<string>, IComparable<string>, IEquatable<char> {

        public readonly uint Value;

        public Codepoint(uint value) {
            Value = value;
        }

        public Codepoint(long value) : this((uint)value) { }

        public Codepoint(string hexValue) {
            if ((hexValue.StartsWith("0x") || hexValue.StartsWith("U+") || hexValue.StartsWith("u+"))) {
                hexValue = hexValue.Substring(2);
            }
            if (!uint.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.CurrentCulture.NumberFormat, out Value)) {
                throw new UnsupportedCodepointException();
            }
        }

        public uint AsUtf32() => Value;

        public IEnumerable<byte> AsUtf32Bytes() {
            var utf32 = AsUtf32();
            var b1 = (byte)(utf32 >> 24);
            yield return b1;
            var b2 = (byte)((utf32 & 0x00FFFFFF) >> 16);
            yield return b2;
            var b3 = (byte)(((ushort)utf32) >> 8);
            yield return b3;
            var b4 = (byte)utf32;
            yield return b4;
        }

        public IEnumerable<ushort> AsUtf16() {
            if (Value <= 0xFFFF) {
                yield return (ushort)Value;
            }
            else if (Value >= 0x10000 && Value <= 0x10FFFF) {
                uint newVal = Value - 0x010000;
                ushort high = (ushort)((newVal >> 10) + 0xD800);
                Debug.Assert(high <= 0xDBFF && high >= 0xD800);
                yield return high;

                ushort low = (ushort)((newVal & 0x03FF) + 0xDC00);
                Debug.Assert(low <= 0xDFFF && low >= 0xDC00);
                yield return low;
            } else {
                throw new UnsupportedCodepointException();
            }
        }

        public IEnumerable<byte> AsUtf16Bytes() {
            var utf16 = AsUtf16();
            foreach (var u16 in utf16) {
                var high = (byte)(u16 >> 8);
                yield return high;
                var low = (byte)u16;
                yield return low;
            }
        }

        public IEnumerable<byte> AsUtf8() {
            if (Value <= 0x007F) {
                yield return (byte)Value;
                yield break;
            }

            if (Value <= 0x07FF) {
                yield return (byte)(0b11000000 | (0b00011111 & (Value >> 6))); 
                yield return (byte)(0b10000000 | (0b00111111 & Value)); 
                yield break;
            }

            if (Value <= 0x0FFFF) {
                yield return (byte)(0b11100000 | (0b00001111 & (Value >> 12))); 
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 6))); 
                yield return (byte)(0b10000000 | (0b00111111 & Value));
                yield break;
            }

            if (Value <= 0x1FFFFF) {
                yield return (byte)(0b11110000 | (0b00000111 & (Value >> 18)));
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 12)));
                yield return (byte)(0b10000000 | (0b00111111 & (Value >> 6))); 
                yield return (byte)(0b10000000 | (0b00111111 & Value));        
                yield break;
            }

            throw new UnsupportedCodepointException();
        }

        public int CompareTo(Codepoint other) {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(uint other) {
            return Value.CompareTo(other);
        }

        public bool Equals(Codepoint other) {
            return Value == other.Value;
        }

        public override bool Equals(object obj) {
            if (obj is Codepoint) {
                return Value == ((Codepoint)obj).Value;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public static bool operator ==(Codepoint a, Codepoint b) {
            return a.Value == b.Value;
        }

        public static bool operator !=(Codepoint a, Codepoint b) {
            return a.Value != b.Value;
        }

        public static bool operator <(Codepoint a, Codepoint b) {
            return a.Value < b.Value;
        }

        public static bool operator >(Codepoint a, Codepoint b) {
            return a.Value > b.Value;
        }

        public static bool operator >=(Codepoint a, Codepoint b) {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Codepoint a, Codepoint b) {
            return a.Value <= b.Value;
        }

        public static implicit operator uint(Codepoint codepoint) {
            return codepoint.Value;
        }

        public static implicit operator Codepoint(uint value) {
            return new Codepoint(value);
        }

        public override string ToString() {
            return $"U+{Value.ToString("X")}";
        }

        public string AsString() {
            return Encoding.UTF8.GetString(AsUtf8().ToArray());
        }

        public bool IsIn(Range range) {
            return range.Contains(this);
        }

        public bool IsIn(MultiRange multirange) {
            return multirange.Contains(this);
        }

        public bool Equals(string other) {
            return AsString() == other;
        }

        public int CompareTo(string other) {
            return AsString().CompareTo(other);
        }

        public bool Equals(char other) {
            var s = AsString();
            return s.Count() == 1 && s[0] == other;
        }

        public static bool operator ==(Codepoint a, string b) {
            return a.Equals(b);
        }

        public static bool operator !=(Codepoint a, string b) {
            return !a.Equals(b);
        }

        public static bool operator ==(Codepoint a, char b) {
            return a.Equals(b);
        }

        public static bool operator !=(Codepoint a, char b) {
            return !a.Equals(b);
        }
    }

    public class Codepoints {
        /// <summary>
        /// right-to-left mark
        /// </summary>
        public static readonly Codepoint RLM = new Codepoint("U+200F");

        /// <summary>
        /// left-to-right mark
        /// </summary>
        public static readonly Codepoint LRM = new Codepoint("U+200E");

        /// <summary>
        /// ZWJ is used to combine multiple emoji codepoints into a single emoji symbol.
        /// </summary>
        public static readonly Codepoint ZWJ = new Codepoint("U+200D");

        /// <summary>
        /// The "combined enclosing keycap" is used by emoji to box icons
        /// </summary>
        public static readonly Codepoint Keycap = new Codepoint("U+20E3");

        public static class VariationSelectors {
            public static readonly Codepoint TextSymbol  = new Codepoint("U+FE0E");
            public static readonly Codepoint EmojiSymbol = new Codepoint("U+FE0F");
        }
    }
}
