using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.AI.Faces {
	public class ArrayBase64<T> where T : struct {
		static Dictionary<Type, ValueContext> _dict = new Dictionary<Type, ValueContext> {
			[typeof(int)]    = new ValueContext(sizeof(int)   , (data, i) => BitConverter.ToInt32(data, i),  x => BitConverter.GetBytes((int)(object)x)),
			[typeof(long)]   = new ValueContext(sizeof(long)  , (data, i) => BitConverter.ToInt64(data, i),  x => BitConverter.GetBytes((long)(object)x)),
			[typeof(short)]  = new ValueContext(sizeof(short) , (data, i) => BitConverter.ToInt16(data, i),  x => BitConverter.GetBytes((short)(object)x)),
			[typeof(float)]  = new ValueContext(sizeof(float) , (data, i) => BitConverter.ToSingle(data, i), x => BitConverter.GetBytes((float)(object)x)),
			[typeof(double)] = new ValueContext(sizeof(double), (data, i) => BitConverter.ToDouble(data, i), x => BitConverter.GetBytes((double)(object)x)),
		};

		public string ToBase64String(T[] data) {
			var valueContext = _dict[typeof(T)];
            return Convert.ToBase64String(data.SelectMany(x => valueContext.ToBytes(x)).ToArray());
		}

		public IEnumerable<T> FromBase64String(string base64) {
			var bytes = Convert.FromBase64String(base64);
			var valueContext = _dict[typeof(T)];
			for (int i = 0; i < bytes.Length; i += valueContext.Size) {
				yield return (T)valueContext.ToT(bytes, i);
			}
		}

		sealed class ValueContext {
			public int Size { get; private set; }

			public Func<byte[], int, object> ToT     { get; }
			public Func<T, byte[]>           ToBytes { get; }

			public ValueContext(int size, Func<byte[], int, object> toT, Func<T, byte[]> toBytes) {
				ToT     = toT;
				Size    = size;
				ToBytes = toBytes;
			}
		}
	}
}