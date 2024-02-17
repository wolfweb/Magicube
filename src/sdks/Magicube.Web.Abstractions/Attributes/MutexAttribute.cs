using System;

namespace Magicube.Web.Attributes {
    /// <summary>
    /// 提供字段之间的互斥操作，一个可用另一个不可以，一个可显现另一个不可以，一个可选中另一个不可以
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class MutexAttribute : Attribute {
        public StatusType Mutex { get; set; }
        public string Field { get; set; }
        public MutexAttribute(string field, StatusType mutex = StatusType.Readonly) {
            Field = field;
            Mutex = mutex;
        }
    }

    public enum StatusType {
        Readonly,
        Hidden,
        Checked
    }
}
