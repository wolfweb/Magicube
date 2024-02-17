using Magicube.Core;
using System;

namespace Magicube.Executeflow {
    public class ExecuteflowException : MagicubeException {
        private const int code = 15000;
        public ExecuteflowException(string message) : base(code, message) {
        }

        public ExecuteflowException(string messageFormat, params object[] args) : base(code, messageFormat, args) {
        }

        public ExecuteflowException(string message, Exception innerException) : base(code, message, innerException) {
        }
    }
}
