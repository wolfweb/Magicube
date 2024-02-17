using Magicube.Core;

namespace Magicube.Versioning.Abstractions {
    public class VersioningException : MagicubeException {
        //todo: 定义code
        public VersioningException(string message) : base(10, message) {
        }
    }
}