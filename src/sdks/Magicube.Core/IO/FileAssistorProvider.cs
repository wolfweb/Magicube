using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Magicube.Core.IO {
    public class FileAssistorProvider {
        public Stream ZipCompress(Dictionary<string, Stream> fs) {
            var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true)) {
                foreach (var item in fs) {
                    var readmeEntry = archive.CreateEntry(item.Key);
                    using (Stream writer = readmeEntry.Open()) {
                        item.Value.CopyTo(writer);
                    }
                }
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public IEnumerable<ArchiveEntry> ZipUnCompress(Stream stream, Encoding encoding = null) {
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, true, encoding ?? Encoding.UTF8)) {
                foreach (var entry in archive.Entries) {
                    var mem = new MemoryStream();
                    entry.Open().CopyTo(mem);
                    mem.Seek(0, SeekOrigin.Begin);
                    var item = new ArchiveEntry {
                        Type    = entry.CompressedLength == 0 ? ArchiveEntryType.Directory : ArchiveEntryType.File,
                        Content = mem
                    };
                    item.Name = item.Type == ArchiveEntryType.File ? Path.GetFileNameWithoutExtension(entry.FullName) : Path.GetDirectoryName(entry.FullName.TrimEnd('/'));
                    item.FullName = entry.FullName;
                    if (item.Type == ArchiveEntryType.File)
                        item.Extension = Path.GetExtension(entry.FullName);
                    yield return item;
                }
            }
        }
    }

    public class ArchiveEntry {
        public string           Name      { get; set; }
        public Stream           Content   { get; set; }
        public string           FullName  { get; set; }
        public string           Extension { get; set; }
        public ArchiveEntryType Type      { get; set; }
    }

    public enum ArchiveEntryType {
        File,
        Directory
    }
}
