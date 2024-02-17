using System;
using System.IO;
using System.Linq;
using System.Threading;
using Magicube.Core;
using Magicube.Core.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Magicube.Web {
    public class MagicubeFileProvider : PhysicalFileProvider, IWebFileProvider {
        public string WebRootPath { get; }
        protected Application _application;

        public MagicubeFileProvider(IWebHostEnvironment hostEnvironment, Application application)
            : base(hostEnvironment.ContentRootPath) {
            WebRootPath  = Path.Combine(hostEnvironment.ContentRootPath, "wwwroot");
            _application = application;
        }

        private static void DeleteDirectoryRecursive(string path) {
            Directory.Delete(path, true);
            const int maxIterationToWait = 5;
            var curIteration = 0;

            while (Directory.Exists(path)) {
                curIteration += 1;
                if (curIteration > maxIterationToWait)
                    return;
                Thread.Sleep(100);
            }
        }

        public void CreateDirectory(string path) {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public void CreateFile(string path) {
            if (File.Exists(path))
                return;
            using (File.Create(path)) { }
        }

        public void DeleteDirectory(string path) {
            if (path.IsNullOrEmpty())
                throw new ArgumentNullException(path);

            foreach (var directory in Directory.GetDirectories(path)) {
                DeleteDirectory(directory);
            }

            try {
                DeleteDirectoryRecursive(path);
            } catch (IOException) {
                DeleteDirectoryRecursive(path);
            } catch (UnauthorizedAccessException) {
                DeleteDirectoryRecursive(path);
            }
        }

        public void DeleteFile(string filePath) {
            if (!File.Exists(filePath))
                return;

            File.Delete(filePath);
        }

        public long FileLength(string path) {
            if (!File.Exists(path))
                return -1;

            return new FileInfo(path).Length;
        }

        public string GetAbsolutePath(params string[] paths) {
            var allPaths = paths.ToList();
            allPaths.Insert(0, Root);

            return Path.Combine(allPaths.ToArray());
        }

        public string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true) {
            if (searchPattern.IsNullOrEmpty())
                searchPattern = "*";

            return Directory.GetDirectories(path, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true) {
            if (searchPattern.IsNullOrEmpty())
                searchPattern = "*.*";

            return Directory.GetFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public bool IsDirectory(string path) {
            return Directory.Exists(path);
        }

        public bool IsAbsolutePath(string path) {
            return !(path.StartsWith("~/") || (path.StartsWith("/") && Directory.Exists(path)));
        }
        public string MapPath(string path) {
            path = path.Replace("~/", string.Empty);
            var modular = _application.Modulars?.FirstOrDefault(x => path.StartsWith(x.Descriptor.Name));
            if (modular != null) {
                return Path.Combine(modular.StaticFileRoot, "..\\..\\", path);
            }
            path = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(Root ?? string.Empty, path);
        }
    }
}
