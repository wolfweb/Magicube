using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Html;

namespace Magicube.Resource {
    public interface IResourceManager {
        ResourceManifest InlineManifest { get; }

        ResourceDefinition FindResource(RequireSettings settings);

        void NotRequired(string resourceType, string resourceName);

        RequireSettings RegisterUrl(string resourceType, string resourcePath, string resourceDebugPath);

        RequireSettings RegisterResource(string resourceType, string resourceName);

        void RegisterHeadScript(IHtmlContent script);

        void RegisterFootScript(IHtmlContent script);

        void RegisterStyle(IHtmlContent style);

        void RegisterLink(LinkEntry link);

        void RegisterMeta(MetaEntry meta);

        void AppendMeta(MetaEntry meta, string contentSeparator);

        IEnumerable<ResourceRequiredContext> GetRequiredResources(string resourceType);

        IEnumerable<LinkEntry> GetRegisteredLinks();

        IEnumerable<MetaEntry> GetRegisteredMetas();

        IEnumerable<IHtmlContent> GetRegisteredHeadScripts();

        IEnumerable<IHtmlContent> GetRegisteredFootScripts();

        IEnumerable<IHtmlContent> GetRegisteredStyles();

        void RenderMeta(TextWriter writer);

        void RenderHeadLink(TextWriter writer);

        void RenderStylesheet(TextWriter writer);

        void RenderHeadScript(TextWriter writer);

        void RenderFootScript(TextWriter writer);

        void RenderLocalScript(RequireSettings settings, TextWriter writer);

        void RenderLocalStyle(RequireSettings settings, TextWriter writer);
    }
}
