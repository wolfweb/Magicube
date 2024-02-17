namespace Magicube.Download.Abstractions {
    public enum DownloadStatus {
        None     ,
        Created  ,
        Running  ,
        Stopped  , // Cancelled
        Paused   ,
        Completed,
        Failed   
    }
}