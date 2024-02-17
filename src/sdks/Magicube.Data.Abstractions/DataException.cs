using Magicube.Core;
using System;

namespace Magicube.Data.Abstractions {
    public class DataException : MagicubeException {
        public DataException(string message) : this(10000, message) {
        }

        public DataException(int code, string messageFormat, params object[] args) : base(code, messageFormat, args) {
        }

        public DataException(int code, string message, Exception innerException) : base(code, message, innerException) {
        }
    }
}
