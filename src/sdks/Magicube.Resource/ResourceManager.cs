﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Magicube.Core;
using System.IO;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Resource {
    public class ResourceManager : IResourceManager {
        private readonly Dictionary<ResourceTypeName, RequireSettings> _required = new Dictionary<ResourceTypeName, RequireSettings>();
        private readonly Dictionary<string, ResourceRequiredContext[]> _builtResources;
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly ResourceManagementOptions _options;

        private Dictionary<string, MetaEntry> _metas;
        private ResourceManifest _dynamicManifest;
        private List<IHtmlContent> _headScripts;
        private List<IHtmlContent> _footScripts;
        private HashSet<string> _localScripts;
        private HashSet<string> _localStyles;
        private List<IHtmlContent> _styles;
        private List<LinkEntry> _links;

        public ResourceManager(
            IFileVersionProvider fileVersionProvider,
            IOptions<ResourceManagementOptions> options
            ) {

            _options = options.Value;
            _fileVersionProvider = fileVersionProvider;

            _builtResources = new Dictionary<string, ResourceRequiredContext[]>(StringComparer.OrdinalIgnoreCase);
        }

        public ResourceManifest InlineManifest => _dynamicManifest ??= new ResourceManifest();

        public RequireSettings RegisterResource(string resourceType, string resourceName) {
            if (resourceType == null) {
                return ThrowArgumentNullException<RequireSettings>(nameof(resourceType));
            }

            if (resourceName == null) {
                return ThrowArgumentNullException<RequireSettings>(nameof(resourceName));
            }

            var key = new ResourceTypeName(resourceType, resourceName);
            if (!_required.TryGetValue(key, out var settings)) {
                settings = new RequireSettings(_options) {
                    Type = resourceType,
                    Name = resourceName
                };
                _required[key] = settings;
            }
            _builtResources[resourceType] = null;
            return settings;
        }

        public RequireSettings RegisterUrl(string resourceType, string resourcePath, string resourceDebugPath) {
            if (resourceType == null) {
                return ThrowArgumentNullException<RequireSettings>(nameof(resourceType));
            }

            if (resourcePath == null) {
                return ThrowArgumentNullException<RequireSettings>(nameof(resourcePath));
            }

            if (resourcePath.StartsWith("~/", StringComparison.Ordinal)) {
                resourcePath = _options.ContentBasePath + resourcePath.Substring(1);
            }

            if (resourceDebugPath != null && resourceDebugPath.StartsWith("~/", StringComparison.Ordinal)) {
                resourceDebugPath = _options.ContentBasePath + resourceDebugPath.Substring(1);
            }

            return RegisterResource(
                resourceType,
                GetResourceKey(resourcePath, resourceDebugPath)).Define(d => d.SetUrl(resourcePath, resourceDebugPath));
        }

        public void RegisterHeadScript(IHtmlContent script) {
            if (_headScripts == null) {
                _headScripts = new List<IHtmlContent>();
            }

            _headScripts.Add(script);
        }

        public void RegisterFootScript(IHtmlContent script) {
            if (_footScripts == null) {
                _footScripts = new List<IHtmlContent>();
            }

            _footScripts.Add(script);
        }

        public void RegisterStyle(IHtmlContent style) {
            if (_styles == null) {
                _styles = new List<IHtmlContent>();
            }

            _styles.Add(style);
        }

        public void NotRequired(string resourceType, string resourceName) {
            if (resourceType == null) {
                ThrowArgumentNullException(nameof(resourceType));
                return;
            }

            if (resourceName == null) {
                ThrowArgumentNullException(nameof(resourceName));
                return;
            }

            var key = new ResourceTypeName(resourceType, resourceName);
            _builtResources[resourceType] = null;
            _required.Remove(key);
        }

        public ResourceDefinition FindResource(RequireSettings settings) {
            return FindResource(settings, true);
        }

        private ResourceDefinition FindResource(RequireSettings settings, bool resolveInlineDefinitions) {
            var name = settings.Name ?? "";
            var type = settings.Type;

            var stream = _options.ResourceManifests.SelectMany(x => x.GetResources(type));
            var resource = FindMatchingResource(stream, settings, name);

            if (resource == null && _dynamicManifest != null) {
                stream = _dynamicManifest.GetResources(type);
                resource = FindMatchingResource(stream, settings, name);
            }

            if (resolveInlineDefinitions && resource == null) {
                if (ResolveInlineDefinitions(settings.Type)) {
                    resource = FindResource(settings, false);
                }
            }

            return resource;
        }

        private ResourceDefinition FindMatchingResource(
            IEnumerable<KeyValuePair<string, IList<ResourceDefinition>>> stream,
            RequireSettings settings,
            string name) {
            Version lower = null;
            Version upper = null;
            if (!settings.Version.IsNullOrEmpty()) {
                lower = GetLowerBoundVersion(settings.Version);
                upper = GetUpperBoundVersion(settings.Version);
            }

            ResourceDefinition resource = null;
            foreach (var r in stream) {
                if (String.Equals(r.Key, name, StringComparison.OrdinalIgnoreCase)) {
                    foreach (var resourceDefinition in r.Value) {
                        var version = resourceDefinition.Version != null
                            ? new Version(resourceDefinition.Version)
                            : null;

                        if (lower != null) {
                            if (lower > version || version >= upper) {
                                continue;
                            }
                        }

                        // Use the highest version of all matches
                        if (resource == null
                            || (resourceDefinition.Version != null && new Version(resource.Version) < version)) {
                            resource = resourceDefinition;
                        }
                    }
                }
            }

            return resource;
        }

        private Version GetUpperBoundVersion(string minimumVersion) {
            if (!Version.TryParse(minimumVersion, out var version)) {
                if (Int32.TryParse(minimumVersion, out var major)) {
                    return new Version(major + 1, 0, 0);
                }
            }

            if (version.Build != -1) {
                return new Version(version.Major, version.Minor, version.Build + 1);
            }

            if (version.Minor != -1) {
                return new Version(version.Major, version.Minor + 1, 0);
            }

            return version;
        }

        private Version GetLowerBoundVersion(string minimumVersion) {
            if (!Version.TryParse(minimumVersion, out var version)) {
                if (int.TryParse(minimumVersion, out var major)) {
                    return new Version(major, 0, 0);
                }
            }

            return version;
        }

        private bool ResolveInlineDefinitions(string resourceType) {
            bool anyWereDefined = false;
            foreach (var settings in ResolveRequiredResources(resourceType)) {
                if (settings.InlineDefinition == null) {
                    continue;
                }

                var resource = FindResource(settings, false);
                if (resource == null) {
                    resource = InlineManifest.DefineResource(resourceType, settings.Name).SetBasePath(settings.BasePath);
                    anyWereDefined = true;
                }
                settings.InlineDefinition(resource);
                settings.InlineDefinition = null;
            }
            return anyWereDefined;
        }

        private IEnumerable<RequireSettings> ResolveRequiredResources(string resourceType) {
            foreach (var (key, value) in _required) {
                if (key.Type == resourceType) {
                    yield return value;
                }
            }
        }

        public IEnumerable<LinkEntry> GetRegisteredLinks() => DoGetRegisteredLinks();

        private List<LinkEntry> DoGetRegisteredLinks() {
            return _links ?? EmptyList<LinkEntry>.Instance;
        }

        public IEnumerable<MetaEntry> GetRegisteredMetas() => DoGetRegisteredMetas();

        private Dictionary<string, MetaEntry>.ValueCollection DoGetRegisteredMetas() {
            return _metas?.Values ?? EmptyValueCollection<MetaEntry>.Instance;
        }

        public IEnumerable<IHtmlContent> GetRegisteredHeadScripts() => DoGetRegisteredHeadScripts();

        public List<IHtmlContent> DoGetRegisteredHeadScripts() {
            return _headScripts ?? EmptyList<IHtmlContent>.Instance;
        }

        public IEnumerable<IHtmlContent> GetRegisteredFootScripts() => DoGetRegisteredFootScripts();

        public List<IHtmlContent> DoGetRegisteredFootScripts() {
            return _footScripts ?? EmptyList<IHtmlContent>.Instance;
        }

        public IEnumerable<IHtmlContent> GetRegisteredStyles() => DoGetRegisteredStyles();

        public List<IHtmlContent> DoGetRegisteredStyles() {
            return _styles ?? EmptyList<IHtmlContent>.Instance;
        }

        public IEnumerable<ResourceRequiredContext> GetRequiredResources(string resourceType)
            => DoGetRequiredResources(resourceType);

        private ResourceRequiredContext[] DoGetRequiredResources(string resourceType) {
            if (_builtResources.TryGetValue(resourceType, out var requiredResources) && requiredResources != null) {
                return requiredResources;
            }

            var allResources = new ResourceDictionary();
            foreach (var settings in ResolveRequiredResources(resourceType)) {
                var resource = FindResource(settings);
                if (resource == null) {
                    throw new InvalidOperationException($"Could not find a resource of type '{settings.Type}' named '{settings.Name}' with version '{settings.Version ?? "any"}'.");
                }

                ExpandDependencies(resource, settings, allResources);
            }

            requiredResources = new ResourceRequiredContext[allResources.Count];
            int i, first = 0, byDependency = allResources.FirstCount, last = allResources.Count - allResources.LastCount;
            foreach (DictionaryEntry entry in allResources) {
                var settings = (RequireSettings)entry.Value;
                if (settings.Position == ResourcePosition.First) {
                    i = first++;
                } else if (settings.Position == ResourcePosition.Last) {
                    i = last++;
                } else {
                    i = byDependency++;
                }

                requiredResources[i] = new ResourceRequiredContext {
                    Settings = settings,
                    Resource = (ResourceDefinition)entry.Key,
                    FileVersionProvider = _fileVersionProvider
                };
            }

            return _builtResources[resourceType] = requiredResources;
        }

        protected virtual void ExpandDependencies(
            ResourceDefinition resource,
            RequireSettings settings,
            ResourceDictionary allResources) {
            if (resource == null) {
                return;
            }
            allResources.AddExpandingResource(resource, settings);

            List<string> dependencies = null;
            if (resource.Dependencies != null) {
                dependencies = new List<string>(resource.Dependencies);
                if (settings.Dependencies != null) {
                    dependencies.AddRange(settings.Dependencies);
                }
            } else if (settings.Dependencies != null) {
                dependencies = new List<string>(settings.Dependencies);
            }

            var dependencySettings = (((RequireSettings)allResources[resource])
                    ?.NewAndCombine(settings)
                ?? new RequireSettings(_options) {
                    Name = resource.Name,
                    Type = resource.Type,
                    Position = resource.Position
                }
                    .Combine(settings))
                    .CombinePosition(settings)
                    ;

            if (dependencies != null) {
                var tempSettings = new RequireSettings();

                for (var i = 0; i < dependencies.Count; i++) {
                    var d = dependencies[i];
                    var idx = d.IndexOf(':');
                    var name = d;
                    string version = null;
                    if (idx != -1) {
                        name = d.Substring(0, idx);
                        version = d[(idx + 1)..];
                    }

                    tempSettings.Type = resource.Type;
                    tempSettings.Name = name;
                    tempSettings.Version = version;

                    var dependency = FindResource(tempSettings);
                    if (dependency == null) {
                        continue;
                    }

                    ExpandDependencies(dependency, dependencySettings, allResources);
                }
            }

            settings.UpdatePositionFromDependency(dependencySettings);
            allResources.AddExpandedResource(resource, dependencySettings);
        }

        public void RegisterLink(LinkEntry link) {
            if (_links == null) {
                _links = new List<LinkEntry>();
            }

            var href = link.Href;

            if (href != null && href.StartsWith("~/", StringComparison.Ordinal)) {
                link.Href = _options.ContentBasePath + href.Substring(1);
            }

            if (link.AppendVersion) {
                link.Href = _fileVersionProvider.AddFileVersionToPath(_options.ContentBasePath, link.Href);
            }

            _links.Add(link);
        }

        public void RegisterMeta(MetaEntry meta) {
            if (meta == null) {
                return;
            }

            if (_metas == null) {
                _metas = new Dictionary<string, MetaEntry>();
            }

            var index = meta.Name ?? meta.Property ?? meta.HttpEquiv ?? "charset";

            _metas[index] = meta;
        }

        public void AppendMeta(MetaEntry meta, string contentSeparator) {
            if (meta == null) {
                return;
            }

            var index = meta.Name ?? meta.Property ?? meta.HttpEquiv;

            if (index.IsNullOrEmpty()) {
                return;
            }

            if (_metas == null) {
                _metas = new Dictionary<string, MetaEntry>();
            }

            if (_metas.TryGetValue(index, out var existingMeta)) {
                meta = MetaEntry.Combine(existingMeta, meta, contentSeparator);
            }

            _metas[index] = meta;
        }

        public void RenderMeta(TextWriter writer) {
            var first = true;

            foreach (var meta in DoGetRegisteredMetas()) {
                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                meta.GetTag().WriteTo(writer, NullHtmlEncoder.Default);
            }
        }

        public void RenderHeadLink(TextWriter writer) {
            var first = true;

            var registeredLinks = DoGetRegisteredLinks();
            for (var i = 0; i < registeredLinks.Count; i++) {
                var link = registeredLinks[i];
                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                link.GetTag().WriteTo(writer, NullHtmlEncoder.Default);
            }
        }

        public void RenderStylesheet(TextWriter writer) {
            var first = true;

            var styleSheets = DoGetRequiredResources("stylesheet");

            foreach (var context in styleSheets) {
                if (context.Settings.Location == ResourceLocation.Inline) {
                    continue;
                }

                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, _options.ContentBasePath);
            }

            var registeredStyles = DoGetRegisteredStyles();
            for (var i = 0; i < registeredStyles.Count; i++) {
                var context = registeredStyles[i];
                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, NullHtmlEncoder.Default);
            }
        }

        public void RenderHeadScript(TextWriter writer) {
            var headScripts = DoGetRequiredResources("script");

            var first = true;

            foreach (var context in headScripts) {
                if (context.Settings.Location != ResourceLocation.Head) {
                    continue;
                }

                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, _options.ContentBasePath);
            }

            var registeredHeadScripts = DoGetRegisteredHeadScripts();
            for (var i = 0; i < registeredHeadScripts.Count; i++) {
                var context = registeredHeadScripts[i];
                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, NullHtmlEncoder.Default);
            }
        }

        public void RenderFootScript(TextWriter writer) {
            var footScripts = DoGetRequiredResources("script");

            var first = true;
            foreach (var context in footScripts) {
                if (context.Settings.Location != ResourceLocation.Foot) {
                    continue;
                }

                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, _options.ContentBasePath);
            }

            var registeredFootScripts = DoGetRegisteredFootScripts();
            for (var i = 0; i < registeredFootScripts.Count; i++) {
                var context = registeredFootScripts[i];
                if (!first) {
                    writer.Write(Environment.NewLine);
                }

                first = false;

                context.WriteTo(writer, NullHtmlEncoder.Default);
            }
        }

        public void RenderLocalScript(RequireSettings settings, TextWriter writer) {
            var localScripts = DoGetRequiredResources("script");
            var first = true;
            foreach (var context in localScripts) {
                if ((context.Settings.Location == ResourceLocation.Unspecified || context.Settings.Location == ResourceLocation.Inline) &&
                    (_localScripts.Add(context.Settings.Name) || context.Settings.Name == settings.Name)) {
                    if (!first) {
                        writer.Write(Environment.NewLine);
                    }

                    first = false;

                    context.WriteTo(writer, _options.ContentBasePath);
                }
            }
        }

        public void RenderLocalStyle(RequireSettings settings, TextWriter writer) {
            var localStyles = DoGetRequiredResources("stylesheet");
            _localStyles ??= new HashSet<string>(localStyles.Length);

            var first = true;

            foreach (var context in localStyles) {
                if (context.Settings.Location == ResourceLocation.Inline &&
                    (_localStyles.Add(context.Settings.Name) || context.Settings.Name == settings.Name)) {
                    if (!first) {
                        writer.Write(Environment.NewLine);
                    }

                    first = false;

                    context.WriteTo(writer, _options.ContentBasePath);
                }
            }
        }

        private readonly struct ResourceTypeName : IEquatable<ResourceTypeName> {
            public readonly string Type;
            public readonly string Name;

            public ResourceTypeName(string resourceType, string resourceName) {
                Type = resourceType;
                Name = resourceName;
            }

            public bool Equals(ResourceTypeName other) {
                return Type == other.Type && Name == other.Name;
            }

            public override int GetHashCode() {
                return HashCode.Combine(Type, Name);
            }

            public override string ToString() => "(" + Type + ", " + Name + ")";
        }

        private string GetResourceKey(string releasePath, string debugPath) {
            if (_options.DebugMode && !debugPath.IsNullOrEmpty()) {
                return debugPath;
            } else {
                return releasePath;
            }
        }

        private static class EmptyList<T> {
            public static readonly List<T> Instance = new List<T>();
        }

        private static class EmptyValueCollection<T> {
            public static readonly Dictionary<string, T>.ValueCollection Instance = new Dictionary<string, T>.ValueCollection(new Dictionary<string, T>());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowArgumentNullException(string paramName) {
            ThrowArgumentNullException<object>(paramName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static T ThrowArgumentNullException<T>(string paramName) {
            throw new ArgumentNullException(paramName);
        }
    }
}
