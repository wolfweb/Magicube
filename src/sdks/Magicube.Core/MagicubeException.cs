using System;

namespace Magicube.Core {
    public class MagicubeException : Exception {
        public int Code { get; }

        public MagicubeException(int code, string message) : base(message) {
            Code = code;
        }

        public MagicubeException(int code, string formater, params object[] args)
            : base(string.Format(formater, args)) {
            Code = code;
        }

        public MagicubeException(int code, string message, Exception innerException)
            : base(message, innerException) {
            Code = code;
        }
    }
}
