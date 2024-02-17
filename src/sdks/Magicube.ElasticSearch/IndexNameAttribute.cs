using System;

namespace Magicube.ElasticSearch {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IndexNameAttribute : Attribute {
        public string Name { get; set; }
        public IndexNameAttribute(string name) {
            Name = name;
        }
    }
}
