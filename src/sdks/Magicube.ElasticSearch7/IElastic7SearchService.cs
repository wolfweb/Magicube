using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Magicube.ElasticSearch;
using Nest;

namespace Magicube.ElasticSearch7 {
    public interface IElastic7SearchService: IElasticSearchService {
        Task CreateIndex(IndexDocMapping mapping);

        Task<long> Count(IndexDocument document, Func<QueryContainerDescriptor<dynamic>, QueryContainer> query = null);

        Task<long> Count<T, TKey>(string index = null, Func<QueryContainerDescriptor<T>, QueryContainer> query = null) where T : class, ISearchModel<TKey>;

        Task<IReadOnlyDictionary<IndexName, IndexState>> GetAllIndices();

        Task<KeyValuePair<IndexName, IndexState>> GetIndex(string index);

        Task<KeyValuePair<IndexName, IndexState>> GetIndex<T,TKey>() where T : class, ISearchModel<TKey>;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页</param>
        /// <param name="skip"></param>
        /// <param name="query">查询过滤</param>
        /// <param name="sortor">排序</param>
        /// <param name="fieldFilter">字段过滤</param>
        /// <returns></returns>
        Task<SearchResult<dynamic>> Query(
            string index, 
            int pageSize, 
            int skip = 0, 
            Func<QueryContainerDescriptor<dynamic>, QueryContainer> query = null, 
            Func<SortDescriptor<dynamic>, IPromise<IList<ISort>>> sortor = null,
            Func<SourceFilterDescriptor<dynamic>, ISourceFilter> fieldFilter = null,
            Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> scriptFields = null);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="pageSize">分页</param>
        /// <param name="skip"></param>
        /// <param name="index">索引</param>
        /// <param name="query">查询过滤</param>
        /// <param name="sortor">排序</param>
        /// <param name="fieldFilter">字段过滤</param>
        /// <returns></returns>
        Task<SearchResult<T>> Query<T, TKey>(
            int pageSize, 
            int skip = 0, 
            string index = "",
            Func<QueryContainerDescriptor<T>, QueryContainer> query = null, 
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortor = null,
            Func<SourceFilterDescriptor<T>, ISourceFilter> fieldFilter = null,
            Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> scriptFields = null) where T : class, ISearchModel<TKey>;
    }
}
