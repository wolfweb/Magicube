using Aliyun.OSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.Storage.Aliyun.Models {
    public class AliyunStorageContext {
        public OssClient Client { get; set; }
        public string    Bucket { get; set; }
    }
}
