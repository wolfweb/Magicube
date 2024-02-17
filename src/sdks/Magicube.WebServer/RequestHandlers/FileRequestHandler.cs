using System;
using System.IO;
using System.Threading.Tasks;

namespace Magicube.WebServer.RequestHandlers {
    public class FileRequestHandler : FileSystemRequestHandler {
        public FileRequestHandler(string urlPath, EventHandler eventHandler, string fileSystemPath)
            : base(fileSystemPath, urlPath, eventHandler) {
            if (string.IsNullOrWhiteSpace(fileSystemPath))
                throw new ArgumentNullException("fileSystemPath");
        }


        public override async Task<MiniWebContext> HandleRequestAsync(MiniWebContext context) {
            string fullFilePath = Path.Combine(context.RootFolderPath, FileSystemPath);

            if (IsCaseSensitiveFileSystem) fullFilePath = GetPathCaseSensitive(fullFilePath);

            if (context.TryReturnFile(new FileInfo(fullFilePath))) return context;

            return context.ReturnHttp404NotFound();
        }
    }
}
