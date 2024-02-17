using System.Collections.Generic;

namespace Magicube.ElasticSearch {
    public class SearchResult<T> {
        public long           Total               { get; set; }
        public int            TotalPages          { get; set; }
        public IEnumerable<T> Items               { get; set; }
        public int            ElapsedMilliseconds { get; set; }
    }
}
