using System;
using System.Collections.Generic;

namespace Magicube.Data.Abstractions {
    public class PageResult<T> {
        public PageResult(IEnumerable<T> datas) {
            Items = datas;
        }

        public PageResult(int total, int pageSize, IEnumerable<T> datas) : this(datas){
            Total    = total;
            PageSize = pageSize;
        }

        public IEnumerable<T> Items    { get; }
        public int?           Total    { get; }
        public int?           PageSize { get; }
    }

    public class PageResult<T, TKey> {
        public PageResult(TKey key, IEnumerable<T> datas) {
            Cursor = key;
            Items  = datas;
        }

        public IEnumerable<T> Items  { get; }
        public TKey           Cursor { get; }

        public static PageResult<T, TKey> Empty(TKey v) => new PageResult<T, TKey>(v, Array.Empty<T>());
    }
}
