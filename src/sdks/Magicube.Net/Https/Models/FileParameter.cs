using System.IO;

namespace Magicube.Net {
    public class FileParameter {
        public Stream Stream      { get; set; }
        public string FileName    { get; set; }
        public string ContentType { get; set; }
        public FileParameter(Stream stream) : this(stream, null) { }
        public FileParameter(Stream stream, string name) : this(stream, name, null) { }
        public FileParameter(Stream stream, string name, string contenttype) {
            stream.Seek(0, SeekOrigin.Begin);
            Stream          = stream;
            FileName        = name;
            ContentType     = contenttype;
        }
    }
}
