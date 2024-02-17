using Magicube.Data.Abstractions.ViewModel;
using Magicube.Storage.Abstractions.Entities;
using System.Collections.Generic;

namespace Magicube.Storage.Abstractions.ViewModels {
    /// <summary>
    /// cloud storage form data signature
    /// </summary>
    public class SignatureViewModel {
        /// <summary>
        /// 上传地址
        /// </summary>
        public string                    Url        { get; set; }
        
        /// <summary>
        /// 文件字段名
        /// </summary>
        public string                    Name       { get; set; }

        /// <summary>
        /// 上传所需传递header
        /// </summary>
        public Dictionary<string,string> Headers    { get; set; }

        /// <summary>
        /// 上传所需传递formData
        /// </summary>
        public Dictionary<string,string> FormData   { get; set; }

        /// <summary>
        /// 上传成功地址
        /// </summary>
        public string                    SuccessUrl { get; set; }
        public long                      Length     { get; set; }
    }

    public class CloudStorageViewModel : EntityViewModel<StorageStore, int> {

    }
}
