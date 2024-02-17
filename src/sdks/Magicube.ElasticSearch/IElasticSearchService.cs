using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magicube.ElasticSearch {
    public interface IElasticSearchService {
        bool ShowLog { get; }

        /// <summary>
        /// 节点数<=主分片数*（副本数+1）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        Task       CreateIndex<T, TKey>(string index = null) where T : class, ISearchModel<TKey>;
        Task       DropIndex<T, TKey>() where T : class, ISearchModel<TKey>;
        Task<bool> ExistIndex<T, TKey>(string index = null) where T : class, ISearchModel<TKey>;
        
        Task          DropIndex(string index);
        Task          Create(IndexDocument document);
        Task          Delete(IndexDocument document);
        IndexDocument Get(IndexDocument document);
        Task          Update(IndexDocument document);

        Task    Create<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey>;
        Task    Delete<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey>;
        Task<T> Get<T, TKey>(TKey key, string index = null) where T : class, ISearchModel<TKey>;
        Task    Update<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey>;

        Task BulkInsert<T, TKey>(List<T> datas, string index = null) where T : class, ISearchModel<TKey>;
        Task BulkUpdate<T, TKey>(List<T> datas, Dictionary<string, object> fieldDatas = null, string index = "") where T : class, ISearchModel<TKey>;        
    }
}
