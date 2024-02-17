using System;

namespace Magicube.Core.Runtime.Attributes {
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterDescriptorAttribute : Attribute {
        public string Title    { get; set; }
        public string Descript { get; set; }
        public bool   Required { get; set; } = true;
    }
}
