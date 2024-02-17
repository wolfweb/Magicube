using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Magicube.Core {
#if !DEBUG
using System.Diagnostics;
    [DebuggerStepThrough]
#endif
    public static class ExceptionExtensions {
        public static bool IsFatal(this Exception ex) {
            return
                ex is OutOfMemoryException ||
                ex is SecurityException ||
                ex is SEHException;
        }
    }
}
