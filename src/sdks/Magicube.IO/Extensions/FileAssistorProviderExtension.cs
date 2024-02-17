using Magicube.Core.IO;
using SevenZipExtractor;
using System.Collections.Generic;
using System.IO;

namespace Magicube.IO {
    public static class FileAssistorProviderExtension {
        public static IEnumerable<ArchiveEntry> UnCompress(this FileAssistorProvider assistor, Stream stream) {
            using(var archiveFile = new ArchiveFile(stream)) {
                foreach(var entry in archiveFile.Entries) {
                    var mem = new MemoryStream();
                    entry.Extract(mem);
                    mem.Seek(0, SeekOrigin.Begin);
                    var item = new ArchiveEntry {
                        Type    = entry.IsFolder ? ArchiveEntryType.Directory : ArchiveEntryType.File,
                        Content = mem
                    };

                    item.Name     = item.Type == ArchiveEntryType.File ? Path.GetFileNameWithoutExtension(entry.FileName) : Path.GetDirectoryName(entry.FileName.TrimEnd('/'));
                    item.FullName = item.FullName;
                    if (item.Type == ArchiveEntryType.File)
                        item.Extension = Path.GetExtension(entry.FileName);

                    yield return item;
                }
            }
        }
    }
}
