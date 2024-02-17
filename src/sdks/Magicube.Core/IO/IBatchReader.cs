using System;
using System.Collections.Generic;

namespace Magicube.Core.IO {
    public interface IBatchReader : IDisposable {
        IEnumerable<BatchReaderContext> Reader(int size, Func<(int, string), bool> checkState = null);
        IEnumerable<BatchReaderContext> Reader(int begin, int size, Func<(int, string), bool> checkState = null);
        BatchReaderContext ReaderBlock(int begin, int size, Func<(int, string), bool> checkState = null);
    }

    public class BatchReaderContext {
        public int                 Line  { get; set; }
        public IEnumerable<string> Datas { get; set; }
    }
}
