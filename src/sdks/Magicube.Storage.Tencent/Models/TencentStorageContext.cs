using COSXML;

namespace Magicube.Storage.Tencent.Models {
    public class TencentStorageContext {
        public string Bucket    { get; set; }
        public CosXml CosClient { get; set; }
    }
}
