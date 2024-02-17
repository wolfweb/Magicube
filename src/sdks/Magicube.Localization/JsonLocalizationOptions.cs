using Microsoft.Extensions.Localization;
using System;
using System.Globalization;
using System.Text;

namespace Magicube.Localization {
    public class JsonLocalizationOptions : LocalizationOptions {
        public JsonLocalizationOptions() {
            ResourcesPath = "Langs";
        }

        public TimeSpan    CacheDuration  { get; set; } = TimeSpan.FromMinutes(30);
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("zh-CN");
        public Encoding    FileEncoding   { get; set; } = Encoding.UTF8;
        public string      LangFilePrefix { get; set; } = "lang-";
        public bool        NeedMerge      { get; set; } = true;
    }
}
