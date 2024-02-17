namespace Magicube.Storage.Abstractions.Models {
    public class CloudStorageStore {
        public string Host         { get; set; }
        public string Bucket       { get; set; }
        public string EndPoint     { get; set; }
        public string AccessKey    { get; set; }
        public string AccessSecret { get; set; }
    }
}