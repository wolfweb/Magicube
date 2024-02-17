using System;

namespace Magicube.Data.Abstractions.Attributes {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SortAttribute : Attribute {
        public int Order { get; }
        public SortAttribute(int order) {
            Order = order;
        }
    }
}
