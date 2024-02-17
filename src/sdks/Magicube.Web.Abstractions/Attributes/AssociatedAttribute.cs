using System;

namespace Magicube.Web.Attributes {
    /// <summary>
    /// 提供字段之间的联动操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class AssociatedAttribute : Attribute {
        public StatusType Associated { get; set; }
        public string Field { get; set; }
        public AssociatedAttribute(string field, StatusType associated = StatusType.Readonly) {
            Field      = field;
            Associated = associated;
        }
    }
}
