using Magicube.Core;
using System;

namespace Magicube.LightApp.Abstractions {
    public class LightAppException : MagicubeException{
        public string AccessTokenOrAppId { get; set; }

        public LightAppException(string message)
            : this(message, null) {
        }

        public LightAppException(string message, Exception inner) : base(5000, message, inner) {
        }
    }
}
