using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Magicube.Localization {
    public class JsonStringLocalizerFactory : IStringLocalizerFactory {
        private readonly LocalizerService _localizerService;
        private readonly bool _fallBackToParentCulture;

        public JsonStringLocalizerFactory(
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            LocalizerService localizerService
            ) {
            _fallBackToParentCulture = requestLocalizationOptions.Value.FallBackToParentUICultures;
            _localizerService        = localizerService;
        }

        public IStringLocalizer Create(Type resourceSource) {
            return new JsonStringLocalizer(_fallBackToParentCulture, _localizerService);
        }

        public IStringLocalizer Create(string baseName, string location) {
            return new JsonStringLocalizer(_fallBackToParentCulture, _localizerService);
        }
    }

    internal class JsonStringLocalizer : IStringLocalizer {
        private readonly bool _fallBackToParentCulture;
        private readonly LocalizerService _localizerService;

        public JsonStringLocalizer(
            bool fallBackToParentCulture,
            LocalizerService localizerService
            ) {
            _localizerService        = localizerService;
            _fallBackToParentCulture = fallBackToParentCulture;
        }

        public virtual LocalizedString this[string name] {
            get {
                if (name == null) {
                    throw new ArgumentNullException(nameof(name));
                }
                Trace.WriteLine($"get localizedString by name=>{name}");
                var value = _localizerService.GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments] {
            get {
                if (name == null) {
                    throw new ArgumentNullException(nameof(name));
                }
                
                Trace.WriteLine($"get localizedString by name=>{name}");
                var format = _localizerService.GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
            return includeParentCultures
                 ? _localizerService.Localizations
                     .Select(
                         l => {
                             var value = _localizerService.GetString(l.CultureName);
                             return new LocalizedString(l.CultureName, value ?? l.CultureName, resourceNotFound: value == null);
                         }
                     )
                 : _localizerService.Localizations
                     .Where(l => l.CultureName.Equals(CultureInfo.CurrentUICulture.Name))
                     .SelectMany(l => l.Localizers.Select(x => new LocalizedString(x.Key, x.Value, false)));
        }

        public IStringLocalizer WithCulture(CultureInfo culture) {
            return this;
        }
    }
}
