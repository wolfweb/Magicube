using Microsoft.Extensions.FileProviders;

namespace Magicube.Core.IO {
    public interface IWebFileProvider : IFileProvider {
        string   WebRootPath { get; }
        void     CreateDirectory(string path);
        void     CreateFile(string path);
        void     DeleteDirectory(string path);
        void     DeleteFile(string filePath);        
        long     FileLength(string path);        
        string   GetAbsolutePath(params string[] paths);        
        string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true);        
        bool     IsDirectory(string path);
        bool     IsAbsolutePath(string path);
        string   MapPath(string path);        
    }
}
