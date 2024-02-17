using System;

namespace Magicube.Data.Abstractions.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class IndexFieldAttribute : Attribute {
        public string Name     { get; set; }
        public bool   IsUnique { get; set; }

        public IndexFieldAttribute() { }

        public IndexFieldAttribute(string name) { 
            Name = name;
        }
    }
}
