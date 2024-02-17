using Magicube.Core;
using Magicube.Core.Signals;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Magicube.Data.EFCore.Abstractions.Configurations {
    public class DbConfigurationSource : IConfigurationSource, IConfigurationSourceProvider {
        private IConfigurationProvider _configurationProvider;

        public IServiceProvider ServiceProvider       { get; set; }
        public string           Key                   => nameof(DbConfigurationSource);
        public IEnumerable<ConfigurationProviderItem> Sources { get; set; }

        public void Load() {
            var signal = ServiceProvider.GetService<ISignal>();
            var (_, token) = signal.GetToken(Key);
            token.RegisterChangeCallback(obj => {
                _configurationProvider.Load();
                Load();
            }, null);
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            _configurationProvider = new DbConfigurationProvider(this);
            return _configurationProvider;
        }
    }

    public class DbConfigurationProvider : IConfigurationProvider {
        private readonly IConfigurationSourceProvider _configurationSourceProvider;
        private IChangeToken _reloadToken = new ConfigurationReloadToken();

        public DbConfigurationProvider(IConfigurationSourceProvider dbConfigurationSource) {
            _configurationSourceProvider = dbConfigurationSource;
            Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        protected IDictionary<string, string> Data { get; set; }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath) {
            List<string> list = new List<string>();
            if (parentPath == null) {
                foreach (KeyValuePair<string, string> datum in Data) {
                    list.Add(Segment(datum.Key, 0));
                }
            }
            else {
                foreach (KeyValuePair<string, string> datum2 in Data) {
                    if (datum2.Key.Length > parentPath!.Length && datum2.Key.StartsWith(parentPath, StringComparison.OrdinalIgnoreCase) && datum2.Key[parentPath!.Length] == ':') {
                        list.Add(Segment(datum2.Key, parentPath!.Length + 1));
                    }
                }
            }
            list.AddRange(earlierKeys);
            list.Sort(ConfigurationKeyComparer.Instance.Compare);
            return list;
        }

        public IChangeToken GetReloadToken() {
            return _reloadToken;
        }

        public void Load() {
            if (_configurationSourceProvider.ServiceProvider == null) return;

            using (var scoped = _configurationSourceProvider.ServiceProvider.CreateScope()) {
                var repository = scoped.ServiceProvider.GetService<IDbContext>();

                foreach(var item in _configurationSourceProvider.Sources) {
                    var datas = item.Build(repository);
                    foreach (var data in datas) {
                        Set(data.Key, data.Value);
                    }
                }
            }
        }

        public void Set(string key, string value) {
            Data[key] = value;
        }

        public bool TryGet(string key, out string value) {
            return Data.TryGetValue(key, out value);
        }

        private static string Segment(string key, int prefixLength) {
            int num = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            if (num >= 0) {
                return key.Substring(prefixLength, num - prefixLength);
            }
            return key.Substring(prefixLength);
        }
    }
}
