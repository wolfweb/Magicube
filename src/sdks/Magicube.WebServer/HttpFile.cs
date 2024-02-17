using System.IO;

namespace Magicube.WebServer {
    public class HttpFile {
        public HttpFile(string contentType, string fileName, Stream value, string name) {
            ContentType = contentType;
            FileName    = fileName;
            Value       = value;
            Name        = name;
        }
        public readonly string ContentType;
        public readonly string FileName;
        public readonly string Name;
        public readonly Stream Value;
    }

}
