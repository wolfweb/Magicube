using Magicube.Core.IO;
using Magicube.Core;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Magicube.Localization {
    public abstract class FileLocalizerProvider : ILocalizerProvider {
        protected readonly IWebFileProvider FileProvider;
        private readonly JsonLocalizationOptions _localizationOptions;
        public FileLocalizerProvider(IOptions<JsonLocalizationOptions> options, IWebFileProvider fileProvider) {
            FileProvider         = fileProvider;
            _localizationOptions = options.Value;
        }
        public virtual IList<LocalizerModel> Localizers() {
            var langsPath = ResourcePaths();
            var localizations = new List<LocalizerModel>();
            var myFiles = langsPath.SelectMany(x => {
                if (Directory.Exists(x)) {
                    return Directory.GetFiles(x, $"{_localizationOptions.LangFilePrefix}*.json", SearchOption.AllDirectories);
                }
                return Array.Empty<string>();
            });

            foreach (string file in myFiles) {
                var lang = Path.GetFileNameWithoutExtension(file).Substring(_localizationOptions.LangFilePrefix.Length);
                var datas = Json.Parse<Dictionary<string, string>>(File.ReadAllText(file, _localizationOptions.FileEncoding));

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

        public abstract IEnumerable<string> ResourcePaths();
    }
}
