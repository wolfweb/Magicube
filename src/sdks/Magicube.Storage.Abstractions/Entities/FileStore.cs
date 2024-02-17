using Magicube.Data.Abstractions;
using System;

namespace Magicube.Storage.Abstractions.Entities {
    /// <summary>
    /// 主键id为文件的md5-hash, 有客户端计算生成（云存储的时候文件不经过服务器，服务器要计算的话需要从云存储下载完整文件之后才能得到hash值）
    /// </summary>
    public class FileStore : Entity<Guid> { 
        public string Name     { get; set; }
        
        public string Path     { get; set; }

        public long   Length   { get; set; }

        public long   CreateAt { get; set; }
    }
}
