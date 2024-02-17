using Magicube.Core;

namespace Magicube.Storage.Abstractions {
    public class StorageException : MagicubeException {
        public StorageException(string message): base(60000, message) { }
    }
}
