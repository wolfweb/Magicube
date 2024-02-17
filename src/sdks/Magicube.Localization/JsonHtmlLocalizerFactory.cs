using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Diagnostics;

namespace Magicube.Localization {
    public class JsonHtmlLocalizerFactory : IHtmlLocalizerFactory {
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        public JsonHtmlLocalizerFactory(IStringLocalizerFactory stringLocalizerFactory) {
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        public IHtmlLocalizer Create(Type resourceSource) {
            return new JsonHtmlLocalizer(_stringLocalizerFactory.Create(resourceSource));
        }

        public IHtmlLocalizer Create(string baseName, string location) {
            Trace.WriteLine($"create html localizer with baseName: {baseName}, location: {location}");
            return new JsonHtmlLocalizer(_stringLocalizerFactory.Create(baseName, location));
        }
    }

    internal class JsonHtmlLocalizer : HtmlLocalizer {
        public JsonHtmlLocalizer(IStringLocalizer localizer) : base(localizer) {
        }
    }
}
