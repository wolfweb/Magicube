using System;
using System.ComponentModel;

namespace Magicube.Core {
#if !DEBUG
    using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class EnumExtensions {
        public static string GetDescString(this Enum value) => ConvertTo<DescriptionAttribute>(value)?.Description ?? value.ToString();

        public static T ConvertTo<T>(this Enum value) where T : Attribute {
            return value.GetType().GetAttribute<T>(value.ToString());
        }
    }
}
