using System.Collections.Generic;

namespace Magicube.Cache.Abstractions {
    public class CacheOptions {
        public CacheOptions() {
            CacheProvider = new List<CacheProvider>();
        }

        public List<CacheProvider>    CacheProvider { get; set; }
    }

    /// <summary>
    /// 缓存提供者
    /// </summary>
    public class CacheProvider {
        public CacheProvider(string name, string display) {
            Name    = name;
            Display = display;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name        { get; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Display     { get; }

        /// <summary>
        /// 视图路径
        /// </summary>
        public string PartialView { get; set; }

        /// <summary>
        /// 试图模型
        /// </summary>
        public object ViewModel  { get; set; }
    }
}
