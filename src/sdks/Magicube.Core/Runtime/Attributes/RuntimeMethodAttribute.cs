using System;

namespace Magicube.Core.Runtime.Attributes {
    [AttributeUsage(AttributeTargets.Method)]
    public class RuntimeMethodAttribute : Attribute{
        public string  Tag      { get; set; } = "Core";
        public string  Title    { get; set; }
        public string  Descript { get; set; }
    }
}
