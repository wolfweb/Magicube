using System;

namespace Magicube.ElasticSearch {
    public interface ISearchModel<TKey> {
        TKey Id { get; set; }
    }

    public class IndexDocument {
        public string  Id       { get; set; }
        public string  Type     { get; set; }
        public string  Index    { get; set; }
        public dynamic Document { get; set; }
    }
}
