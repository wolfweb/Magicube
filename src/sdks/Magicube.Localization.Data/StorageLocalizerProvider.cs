using Magicube.Data.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Magicube.Localization.Data {
    public abstract class StorageLocalizerProvider : ILocalizerProvider {
        protected readonly IRepository<LocalizerStore, Guid> Repository;
        public StorageLocalizerProvider(
            IHttpContextAccessor httpContextAccessor
            ) {
            Repository    = httpContextAccessor.HttpContext?.RequestServices.GetService<IRepository<LocalizerStore, Guid>>();
        }

        public abstract string ModularName { get; }

        public virtual IList<LocalizerModel> Localizers() {
            var entities = GetLocalizers();
            var localizations = new List<LocalizerModel>();
            foreach (var entity in entities) {
                var lang  = entity.CultureName;
                var datas = entity.Localizers;
                var localization = localizations.Find(x => x.CultureName == lang);
                if (localization == null) {
                    localization = new LocalizerModel { CultureName = lang };
                    localizations.Add(localization);
                }

                foreach (var item in datas) {
                    if (!localization.Localizers.ContainsKey(item.Key)) {
                        localization.Localizers.Add(item.Key, item.Value);
                    }
                }
            }

            return localizations;
        }

        public virtual IEnumerable<LocalizerStore> GetLocalizers() {
            return Repository?.Query(x => x.ModularName == ModularName) ?? new List<LocalizerStore>();
        }
    }
}
