using Microsoft.Extensions.Configuration;

namespace Magicube.Core {
#if !DEBUG
    using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class ConfiuratioinExtension {
        public static T BindTo<T>(this IConfiguration configuration) {
            return BindTo<T>(configuration, typeof(T).Name);
        }

        public static T BindTo<T>(this IConfiguration configuration, string name) {
            var result = New<T>.Instance();
            configuration.Bind(name, result);
            return result;
        }
    }
}
