using Cysharp.Text;
using System.Linq;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class StringExtension {
        public static string Format<T1>(this string v, T1 arg1) {
            return ZString.Format(v, arg1);
        }

        public static string Format<T1,T2>(this string v, T1 arg1, T2 arg2) {
            return ZString.Format(v, arg1, arg2);
        }

        public static string Format<T1, T2, T3>(this string v, T1 arg1, T2 arg2, T3 arg3) {
            return ZString.Format(v, arg1, arg2, arg3);
        }

        public static string Format<T1, T2, T3, T4>(this string v, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            return ZString.Format(v, arg1, arg2, arg3, arg4);
        }

        public static string Format<T1, T2, T3, T4, T5>(this string v, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) {
            return ZString.Format(v, arg1, arg2, arg3, arg4, arg5);
        }

        public static string Format<T1, T2, T3, T4, T5, T6>(this string v, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) {
            return ZString.Format(v, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string Format<T1, T2, T3, T4, T5, T6, T7>(this string v, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) {
            return ZString.Format(v, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static string Format<T1, T2, T3, T4, T5, T6, T7, T8>(this string v, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) {
            return ZString.Format(v, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static string Concat(this string v, params object[] args) {
            var list = args.ToList();
            list.Insert(0, v);
            return ZString.Concat(list);
        }

        public static string Join(this string[] v, string separator) {
            return ZString.Join(separator, v);
        }
    }
}
