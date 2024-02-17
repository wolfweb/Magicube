using System;

namespace Magicube.Net {
    public class RemoteCallFailedException : Exception {
        public Int32 Code { get; private set; }

        public RemoteCallFailedException(Int32 code, String message)
            : base(message) {
            Code = code;
        }
    }

}
