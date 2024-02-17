using System;

namespace Magicube.Storage.Upyun.Models {
    public enum UpyunFileType {
        File, Folder
    }
    public class UpyunFile {
        public long Size          { get; set; }
        public string Name        { get; set; }
        public DateTime Date      { get; set; }
        public UpyunFileType Type { get; set; }
    }
}
