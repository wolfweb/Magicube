using System;
using System.IO;
using System.Threading.Tasks;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Resource.TagHelpers {
    [HtmlTargetElement("script", Attributes = NameAttributeName)]
    [HtmlTargetElement("script", Attributes = SrcAttributeName)]
    [HtmlTargetElement("script", Attributes = AtAttributeName)]
    public class ScriptTagHelper : TagHelper {
        private const string NameAttributeName = "asp-name";
        private const string SrcAttributeName = "asp-src";
        private const string AtAttributeName = "at";
        private const string AppendVersionAttributeName = "asp-append-version";

        [HtmlAttributeName(NameAttributeName)]
        public string Name { get; set; }

        [HtmlAttributeName(SrcAttributeName)]
        public string Src { get; set; }

        [HtmlAttributeName(AppendVersionAttributeName)]
        public bool? AppendVersion { get; set; }

        [HtmlAttributeName(AtAttributeName)]
        public ResourceLocation At { get; set; }

        public string CdnSrc      { get; set; }
        public string DebugSrc    { get; set; }
        public string DebugCdnSrc { get; set; }
        public bool?  UseCdn      { get; set; }
        public string Condition   { get; set; }
        public string Culture     { get; set; }
        public bool?  Debug       { get; set; }
        public string DependsOn   { get; set; }
        public string Version     { get; set; }

        private readonly IResourceManager _resourceManager;

        public ScriptTagHelper(IResourceManager resourceManager) {
            _resourceManager = resourceManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            output.SuppressOutput();

            if (Name.IsNullOrEmpty() && !Src.IsNullOrEmpty()) {
                RequireSettings setting;

                if (DependsOn.IsNullOrEmpty()) {
                    setting = _resourceManager.RegisterUrl("script", Src, DebugSrc);
                } else {
                    var name = Src.ToLowerInvariant();

                    var definition = _resourceManager.InlineManifest.DefineScript(name);
                    definition.SetUrl(Src, DebugSrc);

                    if (!Version.IsNullOrEmpty()) {
                        definition.SetVersion(Version);
                    }

                    if (!CdnSrc.IsNullOrEmpty()) {
                        definition.SetCdn(CdnSrc, DebugCdnSrc);
                    }

                    if (!Culture.IsNullOrEmpty()) {
                        definition.SetCultures(Culture.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (!DependsOn.IsNullOrEmpty()) {
                        definition.SetDependencies(DependsOn.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    if (AppendVersion.HasValue) {
                        definition.ShouldAppendVersion(AppendVersion);
                    }

                    if (!Version.IsNullOrEmpty()) {
                        definition.SetVersion(Version);
                    }

                    setting = _resourceManager.RegisterResource("script", name);
                }

                if (At != ResourceLocation.Unspecified) {
                    setting.AtLocation(At);
                }

                if (!Condition.IsNullOrEmpty()) {
                    setting.UseCondition(Condition);
                }

                if (Debug != null) {
                    setting.UseDebugMode(Debug.Value);
                }

                if (!Culture.IsNullOrEmpty()) {
                    setting.UseCulture(Culture);
                }

                if (AppendVersion.HasValue) {
                    setting.ShouldAppendVersion(AppendVersion);
                }

                foreach (var attribute in output.Attributes) {
                    setting.SetAttribute(attribute.Name, attribute.Value.ToString());
                }

                if (At == ResourceLocation.Unspecified || At == ResourceLocation.Inline) {
                    using var sw = new StringWriter();
                    _resourceManager.RenderLocalScript(setting, sw);
                    output.Content.AppendHtml(sw.ToString());
                }
            } else if (!Name.IsNullOrEmpty() && Src.IsNullOrEmpty()) {
                var setting = _resourceManager.RegisterResource("script", Name);

                if (At != ResourceLocation.Unspecified) {
                    setting.AtLocation(At);
                }

                if (UseCdn != null) {
                    setting.UseCdn(UseCdn.Value);
                }

                if (!Condition.IsNullOrEmpty()) {
                    setting.UseCondition(Condition);
                }

                if (Debug != null) {
                    setting.UseDebugMode(Debug.Value);
                }

                if (!Culture.IsNullOrEmpty()) {
                    setting.UseCulture(Culture);
                }

                if (AppendVersion.HasValue) {
                    setting.ShouldAppendVersion(AppendVersion);
                }

                if (!Version.IsNullOrEmpty()) {
                    setting.UseVersion(Version);
                }

                if (!DependsOn.IsNullOrEmpty()) {
                    setting.SetDependencies(DependsOn.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }

                foreach (var attribute in output.Attributes) {
                    setting.SetAttribute(attribute.Name, attribute.Value.ToString());
                }

                if (At != ResourceLocation.Unspecified) {
                    var childContent = await output.GetChildContentAsync();
                    if (!childContent.IsEmptyOrWhiteSpace) {
                        _resourceManager.InlineManifest.DefineScript(Name)
                           .SetInnerContent(childContent.GetContent());
                    }

                    if (At == ResourceLocation.Inline) {
                        using var sw = new StringWriter();
                        _resourceManager.RenderLocalScript(setting, sw);
                        output.Content.AppendHtml(sw.ToString());
                    }
                } else {
                    using var sw = new StringWriter();
                    _resourceManager.RenderLocalScript(setting, sw);
                    output.Content.AppendHtml(sw.ToString());
                }
            } else if (!Name.IsNullOrEmpty() && !Src.IsNullOrEmpty()) {
                var definition = _resourceManager.InlineManifest.DefineScript(Name);
                definition.SetUrl(Src, DebugSrc);

                if (!Version.IsNullOrEmpty()) {
                    definition.SetVersion(Version);
                }

                if (!CdnSrc.IsNullOrEmpty()) {
                    definition.SetCdn(CdnSrc, DebugCdnSrc);
                }

                if (!Culture.IsNullOrEmpty()) {
                    definition.SetCultures(Culture.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (!DependsOn.IsNullOrEmpty()) {
                    definition.SetDependencies(DependsOn.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (AppendVersion.HasValue) {
                    definition.ShouldAppendVersion(AppendVersion);
                }

                if (!Version.IsNullOrEmpty()) {
                    definition.SetVersion(Version);
                }

                if (At != ResourceLocation.Unspecified) {
                    var setting = _resourceManager.RegisterResource("script", Name);

                    setting.AtLocation(At);

                    if (UseCdn != null) {
                        setting.UseCdn(UseCdn.Value);
                    }

                    if (!Condition.IsNullOrEmpty()) {
                        setting.UseCondition(Condition);
                    }

                    if (Debug != null) {
                        setting.UseDebugMode(Debug.Value);
                    }

                    if (!Culture.IsNullOrEmpty()) {
                        setting.UseCulture(Culture);
                    }

                    foreach (var attribute in output.Attributes) {
                        setting.SetAttribute(attribute.Name, attribute.Value.ToString());
                    }

                    if (At == ResourceLocation.Inline) {
                        using var sw = new StringWriter();
                        _resourceManager.RenderLocalScript(setting, sw);
                        output.Content.AppendHtml(sw.ToString());
                    }
                }
            } else if (Name.IsNullOrEmpty() && Src.IsNullOrEmpty()) {
                var childContent = await output.GetChildContentAsync();

                var builder = new TagBuilder("script");
                builder.InnerHtml.AppendHtml(childContent);
                builder.TagRenderMode = TagRenderMode.Normal;

                foreach (var attribute in output.Attributes) {
                    builder.Attributes.Add(attribute.Name, attribute.Value.ToString());
                }

                if (At == ResourceLocation.Head) {
                    _resourceManager.RegisterHeadScript(builder);
                } else if (At == ResourceLocation.Inline) {
                    output.Content.SetHtmlContent(builder);
                } else {
                    _resourceManager.RegisterFootScript(builder);
                }
            }
        }
    }
}
