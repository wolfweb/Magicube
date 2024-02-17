using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Magicube.Resource {
    public class ResourceDefinition {
        private string _basePath;

        public ResourceDefinition(ResourceManifest manifest, string type, string name) {
            Manifest = manifest;
            Type = type;
            Name = name;
        }

        private static string Coalesce(params string[] strings) {
            foreach (var str in strings) {
                if (!str.IsNullOrEmpty()) {
                    return str;
                }
            }
            return null;
        }

        public string              Name              { get; private set; }
        public string              Type              { get; private set; }
        public string              Url               { get; private set; }
        public string              UrlCdn            { get; private set; }
        public string              Version           { get; private set; }
        public ResourceManifest    Manifest          { get; private set; }
        public string              UrlDebug          { get; private set; }
        public string[]            Cultures          { get; private set; }
        public ResourcePosition    Position          { get; private set; }
        public AttributeDictionary Attributes        { get; private set; }
        public string              UrlCdnDebug       { get; private set; }
        public string              CdnIntegrity      { get; private set; }
        public List<string>        Dependencies      { get; private set; }
        public string              InnerContent      { get; private set; }
        public bool?               AppendVersion     { get; private set; }
        public bool                CdnSupportsSsl    { get; private set; }
        public string              CdnDebugIntegrity { get; private set; }

        public ResourceDefinition SetAttribute(string name, string value) {
            if (Attributes == null) {
                Attributes = new AttributeDictionary();
            }

            Attributes[name] = value;
            return this;
        }

        public ResourceDefinition SetBasePath(string basePath) {
            _basePath = basePath;
            return this;
        }

        public ResourceDefinition SetUrl(string url) {
            return SetUrl(url, null);
        }

        public ResourceDefinition SetUrl(string url, string urlDebug) {
            if (url.IsNullOrEmpty()) {
                ThrowArgumentNullException(nameof(url));
            }
            Url = url;
            if (urlDebug != null) {
                UrlDebug = urlDebug;
            }
            return this;
        }

        public ResourceDefinition SetCdn(string cdnUrl) {
            return SetCdn(cdnUrl, null, null);
        }

        public ResourceDefinition SetCdn(string cdnUrl, string cdnUrlDebug) {
            return SetCdn(cdnUrl, cdnUrlDebug, null);
        }

        public ResourceDefinition SetCdnIntegrity(string cdnIntegrity) {
            return SetCdnIntegrity(cdnIntegrity, null);
        }

        public ResourceDefinition SetCdnIntegrity(string cdnIntegrity, string cdnDebugIntegrity) {
            if (cdnIntegrity.IsNullOrEmpty()) {
                ThrowArgumentNullException(nameof(cdnIntegrity));
            }
            CdnIntegrity = cdnIntegrity;
            if (cdnDebugIntegrity != null) {
                CdnDebugIntegrity = cdnDebugIntegrity;
            }
            return this;
        }

        public ResourceDefinition SetCdn(string cdnUrl, string cdnUrlDebug, bool? cdnSupportsSsl) {
            if (cdnUrl.IsNullOrEmpty()) {
                ThrowArgumentNullException(nameof(cdnUrl));
            }
            UrlCdn = cdnUrl;
            if (cdnUrlDebug != null) {
                UrlCdnDebug = cdnUrlDebug;
            }
            if (cdnSupportsSsl.HasValue) {
                CdnSupportsSsl = cdnSupportsSsl.Value;
            }
            return this;
        }

        public ResourceDefinition SetVersion(string version) {
            Version = version;
            return this;
        }

        public ResourceDefinition ShouldAppendVersion(bool? appendVersion) {
            AppendVersion = appendVersion;
            return this;
        }

        public ResourceDefinition SetCultures(params string[] cultures) {
            Cultures = cultures;
            return this;
        }

        public ResourceDefinition SetDependencies(params string[] dependencies) {
            if (Dependencies == null) {
                Dependencies = new List<string>();
            }

            Dependencies.AddRange(dependencies);

            return this;
        }

        public ResourceDefinition SetInnerContent(string innerContent) {
            InnerContent = innerContent;

            return this;
        }
        
        public ResourceDefinition SetPosition(ResourcePosition position) {
            Position = position;

            return this;
        }

        public TagBuilder GetTagBuilder(RequireSettings settings,
            string applicationPath,
            IFileVersionProvider fileVersionProvider) {
            string url, filePathAttributeName = null;
            if (settings.DebugMode) {
                url = settings.CdnMode
                    ? Coalesce(UrlCdnDebug, UrlDebug, UrlCdn, Url)
                    : Coalesce(UrlDebug, Url, UrlCdnDebug, UrlCdn);
            } else {
                url = settings.CdnMode
                    ? Coalesce(UrlCdn, Url, UrlCdnDebug, UrlDebug)
                    : Coalesce(Url, UrlDebug, UrlCdn, UrlCdnDebug);
            }

            if (url.IsNullOrEmpty()) {
                url = null;
            }
            if (!settings.Culture.IsNullOrEmpty()) {
                var nearestCulture = FindNearestCulture(settings.Culture);
                if (!nearestCulture.IsNullOrEmpty()) {
                    url = Path.ChangeExtension(url, nearestCulture + Path.GetExtension(url));
                }
            }

            if (url != null && url.StartsWith("~/", StringComparison.Ordinal)) {
                if (!_basePath.IsNullOrEmpty()) {
                    url = _basePath + url.Substring(1);
                } else {
                    url = applicationPath + url.Substring(1);
                }
            }

            if (url != null && ((settings.AppendVersion.HasValue && settings.AppendVersion == true) ||
                (!settings.AppendVersion.HasValue && AppendVersion == true))) {
                url = fileVersionProvider.AddFileVersionToPath(applicationPath, url);
            }

            if (url != null && !settings.DebugMode && !settings.CdnBaseUrl.IsNullOrEmpty() &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("//", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("file://", StringComparison.OrdinalIgnoreCase)) {
                url = settings.CdnBaseUrl + url;
            }

            TagBuilder tagBuilder;
            switch (Type) {
                case "script":
                    tagBuilder = new TagBuilder("script");
                    if (settings.Attributes.Count > 0) {
                        foreach (var kv in settings.Attributes) {
                            tagBuilder.Attributes.Add(kv);
                        }
                    }
                    filePathAttributeName = "src";
                    break;
                case "stylesheet":
                    if (url == null && InnerContent != null) {
                        tagBuilder = new TagBuilder("style") {
                            Attributes = {
                                { "type", "text/css" }
                            }
                        };
                    } else {
                        tagBuilder = new TagBuilder("link") {
                            TagRenderMode = TagRenderMode.SelfClosing,
                            Attributes = {
                                { "type", "text/css" },
                                { "rel", "stylesheet" }
                            }
                        };
                        filePathAttributeName = "href";
                    }
                    break;
                case "link":
                    tagBuilder = new TagBuilder("link") { TagRenderMode = TagRenderMode.SelfClosing };
                    filePathAttributeName = "href";
                    break;
                default:
                    tagBuilder = new TagBuilder("meta") { TagRenderMode = TagRenderMode.SelfClosing };
                    break;
            }

            if (!CdnIntegrity.IsNullOrEmpty() && url != null && url == UrlCdn) {
                tagBuilder.Attributes["integrity"] = CdnIntegrity;
                tagBuilder.Attributes["crossorigin"] = "anonymous";
            } else if (!CdnDebugIntegrity.IsNullOrEmpty() && url != null && url == UrlCdnDebug) {
                tagBuilder.Attributes["integrity"] = CdnDebugIntegrity;
                tagBuilder.Attributes["crossorigin"] = "anonymous";
            }

            if (Attributes != null) {
                tagBuilder.MergeAttributes(Attributes);
            }

            if (settings.HasAttributes) {
                tagBuilder.MergeAttributes(settings.Attributes);
            }

            if (!url.IsNullOrEmpty() && filePathAttributeName != null) {
                tagBuilder.MergeAttribute(filePathAttributeName, url, true);
            } else if (!InnerContent.IsNullOrEmpty()) {
                tagBuilder.InnerHtml.AppendHtml(InnerContent);
            }

            return tagBuilder;
        }

        public string FindNearestCulture(string culture) {
            if (Cultures == null) {
                return null;
            }
            var selectedIndex = Array.IndexOf(Cultures, culture);
            if (selectedIndex != -1) {
                return Cultures[selectedIndex];
            }
            var cultureInfo = new CultureInfo(culture);
            if (cultureInfo.Parent.Name != culture) {
                var selectedCulture = FindNearestCulture(cultureInfo.Parent.Name);
                if (selectedCulture != null) {
                    return selectedCulture;
                }
            }
            return null;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) {
                return false;
            }

            var that = (ResourceDefinition)obj;
            return string.Equals(that.Name, Name) &&
                string.Equals(that.Type, Type) &&
                string.Equals(that.Version, Version);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, Type);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentNullException(string paramName) {
            throw new ArgumentNullException(paramName);
        }
    }
}
