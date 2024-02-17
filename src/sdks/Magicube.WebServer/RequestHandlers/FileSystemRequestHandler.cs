using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Magicube.WebServer.RequestHandlers {
    public abstract class FileSystemRequestHandler : RequestHandler {
        private static readonly ConcurrentDictionary<string, string> Paths = new ConcurrentDictionary<string, string>();

        static FileSystemRequestHandler() {
            string path = Path.GetTempFileName();

            IsCaseSensitiveFileSystem = !File.Exists(path.ToUpper());
        }

        protected FileSystemRequestHandler(string fileSystemPath, string urlPath, EventHandler handler) : base(urlPath, handler) {
            FileSystemPath = fileSystemPath.Replace("/", Constants.DirectorySeparatorString).TrimStart('~', Constants.DirectorySeparatorChar).TrimEnd('/', Constants.DirectorySeparatorChar);
        }

        public static bool IsCaseSensitiveFileSystem { get; set; }

        public string FileSystemPath { get; set; }

        protected string GetPathCaseSensitive(string path) {
            if (Paths.ContainsKey(path)) return Paths[path];

            string[] segments = path.Split(new[] { Constants.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            var current = new DirectoryInfo(FileSystemPath).Root;

            for (int i = 0; i < segments.Length; i++) {
                string result = GetPathSegmentCaseSensitive(segments[i], current);

                if (i == segments.Length - 1) {
                    if (result != null) Paths[path] = result;

                    return result;
                }

                if (result == null) return null;

                current = new DirectoryInfo(result);
            }

            return null;
        }

        private string GetPathSegmentCaseSensitive(string name, DirectoryInfo directory) {
            if (directory == null) return null;

            FileSystemInfo match = directory.GetFileSystemInfos().FirstOrDefault(f => f.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

            return match != null ? match.FullName : null;
        }
    }
}
