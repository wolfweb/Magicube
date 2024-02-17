using Magicube.Download.Abstractions;
using System.IO;

namespace Magicube.Medias.Hls {
    public class DownloadM3u8Package : DownloadPackage {
        private string _storageFolder;
             
        public DownloadM3u8Package(string rawUrl) : base(rawUrl) {
        }

        public string      StorageFolder { 
            get { return _storageFolder; } 
            set { _storageFolder = value; } 
        }

        public string      M3UFolder     { 
            get {
                return Path.Combine(_storageFolder, "M3U");
            } 
        } 

        public M3UFileInfo M3UFile       { get; set; }
    }
}