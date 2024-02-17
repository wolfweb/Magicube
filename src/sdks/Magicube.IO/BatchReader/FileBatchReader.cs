using System.Collections.Generic;
using System.Collections;
using System.IO;
using Magicube.Core.IO;
using System;
using System.Linq;

namespace Magicube.IO {
    public class FileBatchReader : IEnumerable, IBatchReader {
        private readonly StreamReader _reader;

        public FileBatchReader(string file) {
            _reader = new StreamReader(File.OpenRead(file));
        }

		public IEnumerable<BatchReaderContext> Reader(int size, Func<(int, string), bool> checkState = null) {
			var container = new List<string>();
			var enumerator = GetEnumerator();
			var ctx = new BatchReaderContext();
			while (enumerator.MoveNext()) {
				++ctx.Line;
				if (checkState != null && !checkState((ctx.Line, enumerator.Current.ToString())))
					continue;

				if (container.Count < size) {
					container.Add(enumerator.Current.ToString());
					if (container.Count == size) {
						ctx.Datas = container;
						yield return ctx;
						container.Clear();
					}
					continue;
				}
				container.Clear();
				container.Add(enumerator.Current.ToString());
			}
			if (ctx.Datas.Any()) yield return ctx;
		}

		public IEnumerable<BatchReaderContext> Reader(int begin, int size, Func<(int, string), bool> checkState = null) {
			var enumerator = GetEnumerator();
			var container = new List<string>();
			var ctx = new BatchReaderContext();
			while (enumerator.MoveNext()) {
				if (++ctx.Line <= begin) continue;

				if (checkState != null) {
					if (!checkState.Invoke((ctx.Line, enumerator.Current.ToString()))) continue;
				}

				if (container.Count < size) {
					container.Add(enumerator.Current.ToString());
					if (container.Count == size) {
						ctx.Datas = container;
						yield return ctx;
						container.Clear();
					}
					continue;
				}
				container.Clear();
				container.Add(enumerator.Current.ToString());
			}
			if(ctx.Datas.Any()) yield return ctx;
		}

		public BatchReaderContext ReaderBlock(int begin, int size, Func<(int, string), bool> checkState = null) {
			var enumerator = GetEnumerator();
			var container = new List<string>();
			var ctx = new BatchReaderContext();
			while (enumerator.MoveNext()) {
				if (++ctx.Line <= begin) continue;

				if (checkState != null) {
					if (!checkState.Invoke((ctx.Line, enumerator.Current.ToString()))) continue;
				}

				if (container.Count < size) {
					container.Add(enumerator.Current.ToString());
					if (container.Count == size) break;
					continue;
				}
			}
			ctx.Datas = container;
			return ctx;
		}

		public void Dispose() {
            _reader.Dispose();
			GC.SuppressFinalize(this);
		}

        public IEnumerator GetEnumerator() {
            while (!_reader.EndOfStream) {
                yield return _reader.ReadLine();
            }
        }
    }
}
