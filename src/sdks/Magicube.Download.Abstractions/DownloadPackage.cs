namespace Magicube.Download.Abstractions {
    public abstract class DownloadPackage {
        protected DownloadPackage(string rawUrl) {
            RawUrl   = rawUrl;
        }
        public string         RawUrl        { get; }
        public DownloadStatus Status        { get; set; } = DownloadStatus.None;
        public string         FileName      { get; set; }
        public double         SaveProgress  { get; set; }
        public int            ParallelCount { get; set; }
    }
}