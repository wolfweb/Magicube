using Nest;
using System;

namespace Magicube.ElasticSearch.Test {
    public class WorkElasticModel: ISearchModel<string> {
        public string   Id         { get; set; }
        public int      Valid      { get; set; }
        public int      UserId     { get; set; }
        public int      WorkId     { get; set; }
        [Text(Analyzer = "ik_max_word", SearchAnalyzer = "ik_smart")]
        public string   Text       { get; set; }
        public string   Title      { get; set; }
        public int      Visibility { get; set; }
        public DateTime CreateAt   { get; set; }
    }
}
