using System.IO;
using System;
using System.Text;

namespace Magicube.Medias.Hls {
    public static class M3UFileReaderExtension {
        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri) {
            return reader.GetM3u8FileInfo(baseUri, File.OpenRead(baseUri.OriginalString));
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, Stream stream) {
            reader.WithUri(baseUri);
            return reader.Read(stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, string text) {
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(text), false);
            reader.WithUri(baseUri);
            return reader.Read(stream);
        }

        public static M3UFileInfo GetM3u8FileInfo(this M3UFileReaderBase reader, Uri baseUri, FileInfo file) {
            reader.WithUri(baseUri);
            return reader.Read(file.OpenRead());
        }
    }
}