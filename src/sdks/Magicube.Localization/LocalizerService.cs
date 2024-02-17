using Magicube.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Magicube.Localization {
    public class LocalizerService {
        private IEnumerable<LocalizerModel> _localizers;
        private readonly ILogger<LocalizerService> _logger;
        private readonly JsonLocalizationOptions _localizationOptions;
        private readonly IEnumerable<ILocalizerProvider> _localizerProviders;
        
        public LocalizerService(ILogger<LocalizerService> logger, IEnumerable<ILocalizerProvider> localizerProviders, IOptions<JsonLocalizationOptions> options) {
            _localizerProviders  = localizerProviders;
            _localizationOptions = options.Value;
            _logger              = logger;
            EnsureMerged();            
        }

        public IEnumerable<LocalizerModel> Localizations => _localizers;

        public void EnsureMerged() {
            if (_localizationOptions.NeedMerge) {
                _localizers = _localizerProviders.SelectMany(x => x.Localizers())
                    .Aggregate(new List<LocalizerModel>(), (x, y) => {
                        var it = x.Find(m => m.CultureName == y.CultureName);
                        if (it == null) {
                            x.Add(y);
                        } else {
                            foreach(var m in y.Localizers) {
                                if (!it.Localizers.ContainsKey(m.Key)) {
                                    it.Localizers.Add(m.Key, m.Value);
                                }
                            }
                        }
                        return x;
                    });
                _localizationOptions.NeedMerge = false;
            }
        }

        public string GetString(string name, CultureInfo cultureInfo = null, bool shouldTryDefaultCulture = true) {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if (_localizers == null) throw new ArgumentNullException("localizer need init");

            if (cultureInfo == null) {
                cultureInfo = CultureInfo.CurrentUICulture;
                Trace.WriteLine($"use current culture {cultureInfo.Name}");
            }

            var localization = _localizers.FirstOrDefault(x => x.CultureName == cultureInfo.Name);

            if (localization == null) return null;

            var valuesSection = localization.Localizers.FirstOrDefault(l => l.Key == name);

            if (!valuesSection.Value.IsNullOrEmpty()) {
                return valuesSection.Value;
            }

            if (!cultureInfo.Equals(_localizationOptions.DefaultCulture) && !cultureInfo.Equals(cultureInfo.Parent)) {
                _logger.LogError($"{name} is using parent culture instead of current ui culture, {_localizationOptions.DefaultCulture.Name}");
                return GetString(name, cultureInfo.Parent, shouldTryDefaultCulture);
            }

            if (shouldTryDefaultCulture && !cultureInfo.Equals(_localizationOptions.DefaultCulture)) {
                _logger.LogError($"{name} is using default option culture instead of current ui culture");
                return GetString(name, _localizationOptions.DefaultCulture, false);
            }

            _logger.LogError($"{name} does not contains any translation");
            return null;
        }
    }
}
