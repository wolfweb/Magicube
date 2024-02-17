using Newtonsoft.Json.Linq;

namespace Magicube.Storage.Abstractions {
    public class StorageSetting {
        public string  StorageProvider   { get; set; }
        public string  StorePathTemplate { get; set; }
    }
}
