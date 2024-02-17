using System.IO;
using System.Text;

namespace Magicube.WebServer {
    public static class StreamExtensions {
        public static void Write(this Stream stream, string text) {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
