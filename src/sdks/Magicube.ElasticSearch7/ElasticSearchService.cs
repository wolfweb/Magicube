using Elasticsearch.Net;
using Magicube.Core;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magicube.ElasticSearch;
using Magicube.Core.Reflection;

namespace Magicube.ElasticSearch7 {
    public class ElasticSearchService : IElastic7SearchService {
        private readonly ILogger _logger;
        private readonly IElasticClient _client;

        public bool ShowLog { get; }

        public ElasticSearchService(ILogger<IElasticSearchService> logger, IElasticSearchResolve elasticSearchResolve) {
            var _searchOptions = elasticSearchResolve.Option;
            _logger = logger;
            ShowLog = _searchOptions.Debug;

            var nodes = _searchOptions.Hosts.Select(x => new Uri(x)).Select(x => new Node(x));
            var settings = new ConnectionSettings(new StaticConnectionPool(nodes))
                .OnRequestDataCreated(data => {
                    if (_searchOptions.Debug && data.PostData != null) {
                        using (var mem = new MemoryStream()) {
                            data.PostData.Write(mem, data.ConnectionSettings);
                            var str = mem.ToArray().ToString("utf-8");
                            _logger.LogDebug(str);
                        }
                    }
                })
                .OnRequestCompleted(apiCallDetails => {
                    if (_searchOptions.Debug) {
                        var builder = new StringBuilder();
                        if (apiCallDetails.RequestBodyInBytes != null)
                            builder.Append($"{apiCallDetails.HttpMethod} {apiCallDetails.Uri} {apiCallDetails.RequestBodyInBytes.ToString("utf-8")}\r\n");
                        else
                            builder.Append($"{apiCallDetails.HttpMethod} {apiCallDetails.Uri} \r\n");
                        if (apiCallDetails.ResponseBodyInBytes != null)
                            builder.Append($"Status: {apiCallDetails.HttpStatusCode} {apiCallDetails.ResponseBodyInBytes.ToString("utf-8")}");
                        else
                            builder.Append($"Status: {apiCallDetails.HttpStatusCode}");
                        _logger.LogDebug(builder.ToString());
                    }
                });
            _client = new ElasticClient(settings);
        }

        public async Task BulkInsert<T, TKey>(List<T> datas, string index = null) where T : class, ISearchModel<TKey> {
            var descriptor = new BulkDescriptor();

            foreach (var it in datas) {
                descriptor.Index<T>(op => {
                    index = GetIndexName<T>(index);
                    return op.Index(index).Document(it);
                });
            }

            var rep = await _client.BulkAsync(descriptor);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task BulkUpdate<T, TKey>(List<T> datas, Dictionary<string, object> fieldDatas = null, string index = "") where T : class, ISearchModel<TKey> {
            var descriptor = new BulkDescriptor();

            if (fieldDatas != null) {
                foreach (var it in datas) {
                    descriptor.Update<T, Dictionary<string, object>>(x => {
                        index = GetIndexName<T>(index);
                        return x.Index(index).Doc(fieldDatas);
                    });
                }
            } else {
                foreach (var it in datas) {
                    descriptor.Update<T>(x => {
                        if (index.IsNullOrEmpty())
                            index = typeof(T).Name.ToLower();

                        return x.Index(index).Doc(it);
                    });
                }
            }

            var rep = await _client.BulkAsync(descriptor);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }
        
        public async Task Create<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey> {
            var rep = await _client.IndexAsync(t, x => {
                index = GetIndexName<T>(index);

                return x.Index(index).Refresh(Refresh.True);
            });
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task Create(IndexDocument document) {
            var request = new IndexRequest<object>(document.Document, document.Index, document.Id);
            var rep = await _client.IndexAsync(request);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task CreateIndex<T, TKey>(string index = null) where T : class, ISearchModel<TKey> {
            index = GetIndexName<T>(index);
            var rep = await _client.Indices.CreateAsync(index, x => x.Map<T>(it => it.AutoMap()));
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid)
                    _logger.LogError(rep.DebugInformation);
            }
        }

        public async Task CreateIndex(IndexDocMapping mapping) {
            await _client.Indices.CreateAsync(mapping.Index, x => {
                var descriptor = x.Map(selector => selector.Properties(ps => {
                    foreach (var item in mapping.Fields) {
                        ps = item.Build(ps);
                    }
                    return ps;
                }));

                descriptor.Settings(x => {
                    x.NumberOfShards(mapping.NumberOfShards);
                    x.NumberOfReplicas(mapping.NumberOfReplicas);
                    return x;
                });

                return descriptor;
            });
        }

        public async Task<long> Count(IndexDocument document, Func<QueryContainerDescriptor<dynamic>, QueryContainer> query = null) {
            if (document.Index.IsNullOrEmpty()) throw new ArgumentNullException("document index");

            var rep = await _client.CountAsync<dynamic>(x => {
                if (query != null)
                    x = x.Query(query);

                return x.Index(document.Index);
            });

            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }

            return rep.Count;
        }

        public async Task<long> Count<T, TKey>(string index = null, Func<QueryContainerDescriptor<T>, QueryContainer> query = null) where T : class, ISearchModel<TKey> {
            var rep = await _client.CountAsync<T>(x => {
                index = GetIndexName<T>(index);

                if (query != null)
                    x = x.Query(query);

                return x.Index(index);
            });

            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }

            return rep.Count;
        }

        public async Task Delete<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey> {
            var rep = await _client.DeleteAsync(DocumentPath<T>.Id(t.Id.ToString()), x => {
                index = GetIndexName<T>(index);

                return x.Index(index).Refresh(Refresh.True);
            });
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task Delete(IndexDocument document) {
            var request = new DeleteRequest<object>(document.Index, document.Id) {
                Refresh = Refresh.True
            };
            var rep = await _client.DeleteAsync(request);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task DropIndex<T, TKey>() where T : class, ISearchModel<TKey> {
            await DropIndex(GetIndexName<T>(null));
        }

        public async Task DropIndex(string index) {
            if (_client.Indices.Exists(index).Exists) {
                var rep = await _client.Indices.DeleteAsync(index);
                if (ShowLog)
                    _logger.LogDebug(rep.DebugInformation);
                else {
                    if (!rep.IsValid) {
                        _logger.LogError(rep.DebugInformation);
                    }
                }
            }
        }

        public async Task<bool> ExistIndex<T, TKey>(string index = null) where T : class, ISearchModel<TKey> {
            index = GetIndexName<T>(index);

            var resp = await _client.Indices.ExistsAsync(index);
            return resp.Exists;
        }

        public async Task<IReadOnlyDictionary<IndexName, IndexState>> GetAllIndices() {
            var rep = await _client.Indices.GetAsync(new GetIndexRequest(Indices.All));
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
            return rep.Indices;
        }

        public async Task<KeyValuePair<IndexName, IndexState>> GetIndex(string index) {
            var rep = await _client.Indices.GetAsync(new GetIndexRequest(Indices.Index(index)));
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
            return rep.Indices.FirstOrDefault();
        }

        public async Task<KeyValuePair<IndexName, IndexState>> GetIndex<T, TKey>() where T : class, ISearchModel<TKey> {
            return await GetIndex(GetIndexName<T>(null));
        }

        public async Task<T> Get<T, TKey>(TKey key, string index = null) where T : class, ISearchModel<TKey> {
            var rep = await _client.GetAsync(DocumentPath<T>.Id(key.ToString()), x => {
                index = GetIndexName<T>(index);

                return x.Index(index);
            });
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
            return rep.Source;
        }

        public IndexDocument Get(IndexDocument document) {
            var request = new GetRequest<dynamic>(document.Index, document.Id);
            var rep = _client.Get<dynamic>(request);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }

            return rep.Source != null ? new IndexDocument { Document = rep.Source, Id = rep.Id, Index = rep.Index, Type = rep.Type } : null;
        }
                
        public async Task<SearchResult<dynamic>> Query(
            string index, 
            int pageSize, 
            int skip = 0, 
            Func<QueryContainerDescriptor<dynamic>, QueryContainer> query = null, 
            Func<SortDescriptor<dynamic>, IPromise<IList<ISort>>> sortor = null, 
            Func<SourceFilterDescriptor<dynamic>, ISourceFilter> fieldFilter = null,
            Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> scriptFields = null) {
            if (index.IsNullOrEmpty()) throw new InvalidOperationException("doc index name required!");
            var response = await _client.SearchAsync<dynamic>(x => {
                x = x.Index(index);

                if (query != null)
                    x = x.Query(query);

                if (sortor != null)
                    x = x.Sort(sortor);

                if (fieldFilter != null)
                    x.Source(fieldFilter);

                if(scriptFields != null)
                    x.ScriptFields(scriptFields);

                return x.Skip(skip).Size(pageSize);
            });

            return new SearchResult<dynamic> { 
                Items      = response.Documents,
                Total      = response.Total,
                TotalPages = (int)Math.Ceiling(response.Total / (double)pageSize)
            };
        }

        public async Task<SearchResult<T>> Query<T, TKey>(
            int pageSize, 
            int skip = 0, 
            string index = "", 
            Func<QueryContainerDescriptor<T>, QueryContainer> query = null, 
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortor = null, 
            Func<SourceFilterDescriptor<T>, ISourceFilter> fieldFilter = null,
            Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> scriptFields = null) where T : class, ISearchModel<TKey> {
            var response = await _client.SearchAsync<T>(x => {
                index = GetIndexName<T>(index);

                x = x.Index(index);

                if (query != null)
                    x = x.Query(query);

                if (sortor != null)
                    x = x.Sort(sortor);

                if (fieldFilter != null)
                    x.Source(fieldFilter);

                if (scriptFields != null)
                    x.ScriptFields(scriptFields);

                return x.Skip(skip).Size(pageSize);
            });
            return new SearchResult<T> {
                Items = response.Documents,
                Total = response.Total,
                TotalPages = (int)Math.Ceiling(response.Total / (double)pageSize)
            };
        }
        
        public async Task Update<T, TKey>(T t, string index = null) where T : class, ISearchModel<TKey> {
            var rep = await _client.UpdateAsync(DocumentPath<T>.Id(t.Id.ToString()), x => {
                index = GetIndexName<T>(index);
                return x.Index(index).Doc(t).Refresh(Refresh.True);
            });
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        public async Task Update(IndexDocument document) {
            var request = new UpdateRequest<object, object>(document.Index, document.Id) {
                Doc = document.Document,
                Refresh = Refresh.True,
            };

            var rep = await _client.UpdateAsync(request);
            if (ShowLog)
                _logger.LogDebug(rep.DebugInformation);
            else {
                if (!rep.IsValid) {
                    _logger.LogError(rep.DebugInformation);
                }
            }
        }

        private string GetIndexName<T>(string index = null) {
            if (!index.IsNullOrEmpty()) return index;
            var attr = TypeAccessor.Get<T>().Context.Attributes.FirstOrDefault(x => x is IndexNameAttribute);
            if( attr != null ) return ((IndexNameAttribute)attr).Name;
            return TypeAccessor.Get<T>().Context.Type.Name.ToLower();
        }
    }
}
