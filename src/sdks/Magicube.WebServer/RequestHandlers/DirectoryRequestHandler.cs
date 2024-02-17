using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public class DirectoryRequestHandler : FileSystemRequestHandler {
        public DirectoryRequestHandler(string urlPath, EventHandler eventHandler, string fileSystemPath, bool returnHttp404WhenFileWasNoFound = false, IList<string> defaultDocuments = null)
            : base(fileSystemPath, urlPath, eventHandler) {
            if (string.IsNullOrWhiteSpace(fileSystemPath))
                throw new ArgumentNullException("fileSystemPath");

            if (defaultDocuments == null)
                defaultDocuments = new[] { "index.html" };

            if (FileSystemPath == Constants.DirectorySeparatorString)
                throw new RootDirectoryException();

            ReturnHttp404WhenFileWasNoFound = returnHttp404WhenFileWasNoFound;
            DefaultDocuments = defaultDocuments;
        }

        public bool ReturnHttp404WhenFileWasNoFound { get; set; }

        public IList<string> DefaultDocuments { get; set; }

        public static ConcurrentDictionary<string, string> FileCache = new ConcurrentDictionary<string, string>();

        public override async Task<MiniWebContext> HandleRequestAsync(MiniWebContext context) {
            string partialRequestPath = ReplaceFirstOccurrence(context.Request.Url.Path.ToLower(), UrlPath.ToLower(), "");
            string encodedPartialRequestPath = partialRequestPath.Replace("/", Constants.DirectorySeparatorString).TrimStart(Constants.DirectorySeparatorChar);
            string fullFileSystemPath = Path.Combine(context.RootFolderPath, FileSystemPath, encodedPartialRequestPath);

            if (IsCaseSensitiveFileSystem) fullFileSystemPath = GetPathCaseSensitive(fullFileSystemPath);

            if (!string.IsNullOrWhiteSpace(fullFileSystemPath) && !ContainsInvalidPathCharacters(fullFileSystemPath)) {
                FileInfo fileInfo = null;

                try {
                    fileInfo = new FileInfo(fullFileSystemPath);
                } catch (ArgumentException) {
                    return context.ReturnHttp404NotFound();
                }

                if (context.TryReturnFile(fileInfo)) return context;

                var directoryInfo = new DirectoryInfo(fullFileSystemPath);

                if (directoryInfo.Exists) {
                    if (context.Request.Url.Path.EndsWith("/", StringComparison.Ordinal) == false) {
                        string url = context.Request.Url.BasePath + context.Request.Url.Path + "/" + context.Request.Url.Query;
                        context.Response.Redirect(url);
                        return context;
                    }

                    foreach (string defaultDocument in DefaultDocuments) {
                        string path = Path.Combine(fullFileSystemPath, defaultDocument);

                        if (IsCaseSensitiveFileSystem)
                            path = GetPathCaseSensitive(path);

                        if (context.TryReturnFile(new FileInfo(path)))
                            return context;
                    }
                }
            }

            if (ReturnHttp404WhenFileWasNoFound) return context.ReturnHttp404NotFound();

            return context;
        }

        private static bool ContainsInvalidPathCharacters(string filePath) {
            for (var i = 0; i < filePath.Length; i++) {
                int c = filePath[i];

                if (c == '\"' || c == '<' || c == '>' || c == '|' || c == '*' || c == '?' || c < 32)
                    return true;
            }

            return false;
        }

        public static string ReplaceFirstOccurrence(string originalString, string stringToReplace, string replacementString) {
            int pos = originalString.IndexOf(stringToReplace, StringComparison.Ordinal);
            if (pos < 0)
                return originalString;
            return originalString.Substring(0, pos) + replacementString + originalString.Substring(pos + stringToReplace.Length);
        }

        public static string GetFileHash(FileInfo fileInfo) {
            var key = $"{fileInfo.FullName}_{fileInfo.LastWriteTime}";

            string fileHash;

            if (FileCache.TryGetValue(key, out fileHash))
                return fileHash;

            using (FileStream stream = fileInfo.OpenRead()) {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(stream);
                fileHash = BitConverter.ToString(hash).Replace("-", String.Empty);
            }
            FileCache[key] = fileHash;
            return fileHash;
        }

        [Serializable]
        public class RootDirectoryException : Exception {
            public RootDirectoryException(string message = "Mapping a directory to the root of the application is NOT allowed as this is big security concern and would expose application code and internals.")
                : base(message) {
            }
        }
    }
}
