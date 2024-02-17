using System;
using System.Text;

namespace Magicube.Core.Encrypts {
    public static class Base64UrlEncoder {
		private static char _base64PadCharacter = '=';
		private static char _base64Character62 = '+';
		private static char _base64Character63 = '/';
		private static char _base64UrlCharacter62 = '-';
		private static char _base64UrlCharacter63 = '_';
		private static string _doubleBase64PadCharacter = "==";

		public static string Encode(string arg) {
			if (arg == null) throw new ArgumentNullException("arg");

			return Encode(arg.ToByte());
		}

		public static string Encode(byte[] inArray, int offset, int length) {
			if (inArray == null) throw new ArgumentNullException("inArray");

			string s = Convert.ToBase64String(inArray, offset, length);
			s = s.Split(_base64PadCharacter)[0];
			s = s.Replace(_base64Character62, _base64UrlCharacter62);
			s = s.Replace(_base64Character63, _base64UrlCharacter63);
			return s;
		}

		public static string Encode(byte[] inArray) {
			if (inArray == null) throw new ArgumentNullException("inArray");

			string s = Convert.ToBase64String(inArray, 0, inArray.Length);
			s = s.Split(_base64PadCharacter)[0];
			s = s.Replace(_base64Character62, _base64UrlCharacter62);
			s = s.Replace(_base64Character63, _base64UrlCharacter63);

			return s;
		}

		public static byte[] DecodeBytes(string str) {
			if (str == null) {
				throw new ArgumentNullException("str");
			}

			str = str.Replace(_base64UrlCharacter62, _base64Character62);

			str = str.Replace(_base64UrlCharacter63, _base64Character63);

			switch (str.Length % 4) {
				case 0:
					break;
				case 2:
					str += _doubleBase64PadCharacter;
					break;
				case 3:
					str += _base64PadCharacter;
					break;
				default:
					throw new Exception($"format invariant {str}");
			}

			return Convert.FromBase64String(str);
		}

		public static string Decode(string arg) {
			return ByteArrayExtension.ToString(DecodeBytes(arg));
		}
	}
}
