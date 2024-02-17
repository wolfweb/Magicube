using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Magicube.Core {
#if !DEBUG
    using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class CheckExtension {
        public static T NotNull<T>(this T value, [CallerMemberName] string name = null) {
            if (value == null) throw new ArgumentNullException(name);
            return value;
        }

        public static T NotNull<T>(this T value, string message, [CallerMemberName] string name = null) {
            if (value == null) throw new ArgumentNullException(name, message);
            return value;
        }

        public static IReadOnlyList<T> NotEmpty<T>(this IReadOnlyList<T> value, [CallerMemberName] string name = null) {
            if (value.Count == 0) throw new ArgumentException($"{name} 不能为空");
            return value;
        }
    }
}
