using System;
using System.IO;
using System.Threading.Tasks;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Resource.TagHelpers {
    [HtmlTargetElement("style", Attributes = NameAttributeName)]
    [HtmlTargetElement("style", Attributes = SrcAttributeName)]
    [HtmlTargetElement("style", Attributes = AtAttributeName)]
    public class StyleTagHelper : TagHelper {
        private static readonly char[] splitSeparators = { ',', ' ' };
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

        public StyleTagHelper(IResourceManager resourceManager) {
            _resourceManager = resourceManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            output.SuppressOutput();

            if (Name.IsNullOrEmpty() && !Src.IsNullOrEmpty()) {
                var setting = _resourceManager.RegisterUrl("stylesheet", Src, DebugSrc);

                foreach (var attribute in output.Attributes) {
                    setting.SetAttribute(attribute.Name, attribute.Value.ToString());
                }

                if (At != ResourceLocation.Unspecified) {
                    setting.AtLocation(At);
                } else {
                    setting.AtLocation(ResourceLocation.Head);
                }

                if (!Condition.IsNullOrEmpty()) {
                    setting.UseCondition(Condition);
                }

                if (AppendVersion.HasValue == true) {
                    setting.ShouldAppendVersion(AppendVersion);
                }

                if (Debug != null) {
                    setting.UseDebugMode(Debug.Value);
                }

                if (!Culture.IsNullOrEmpty()) {
                    setting.UseCulture(Culture);
                }

                if (!DependsOn.IsNullOrEmpty()) {
                    setting.SetDependencies(DependsOn.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries));
                }

                if (At == ResourceLocation.Inline) {
                    using var sw = new StringWriter();
                    _resourceManager.RenderLocalStyle(setting, sw);
                    output.Content.AppendHtml(sw.ToString());
                }
            } else if (!Name.IsNullOrEmpty() && Src.IsNullOrEmpty()) {
                var setting = _resourceManager.RegisterResource("stylesheet", Name);

                foreach (var attribute in output.Attributes) {
                    setting.SetAttribute(attribute.Name, attribute.Value.ToString());
                }

                if (At != ResourceLocation.Unspecified) {
                    setting.AtLocation(At);
                } else {
                    setting.AtLocation(ResourceLocation.Head);
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

                if (AppendVersion.HasValue == true) {
                    setting.ShouldAppendVersion(AppendVersion);
                }

                if (!Version.IsNullOrEmpty()) {
                    setting.UseVersion(Version);
                }

                if (!DependsOn.IsNullOrEmpty()) {
                    setting.SetDependencies(DependsOn.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries));
                }

                var childContent = await output.GetChildContentAsync();
                if (!childContent.IsEmptyOrWhiteSpace) {
                    _resourceManager.InlineManifest.DefineStyle(Name)
                        .SetInnerContent(childContent.GetContent());
                }

                if (At == ResourceLocation.Inline) {
                    using var sw = new StringWriter();
                    _resourceManager.RenderLocalStyle(setting, sw);
                    output.Content.AppendHtml(sw.ToString());
                }
            } else if (!Name.IsNullOrEmpty() && !Src.IsNullOrEmpty()) {
                var definition = _resourceManager.InlineManifest.DefineStyle(Name);
                definition.SetUrl(Src, DebugSrc);

                foreach (var attribute in output.Attributes) {
                    definition.SetAttribute(attribute.Name, attribute.Value.ToString());
                }

                if (!Version.IsNullOrEmpty()) {
                    definition.SetVersion(Version);
                }

                if (!CdnSrc.IsNullOrEmpty()) {
                    definition.SetCdn(CdnSrc, DebugCdnSrc);
                }

                if (!Culture.IsNullOrEmpty()) {
                    definition.SetCultures(Culture.Split(',', StringSplitOptions.RemoveEmptyEntries));
                }

                if (!DependsOn.IsNullOrEmpty()) {
                    definition.SetDependencies(DependsOn.Split(',', StringSplitOptions.RemoveEmptyEntries));
                }

                var setting = _resourceManager.RegisterResource("stylesheet", Name);

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

                if (At != ResourceLocation.Unspecified) {
                    setting.AtLocation(At);
                } else {
                    setting.AtLocation(ResourceLocation.Head);
                }

                if (At == ResourceLocation.Inline) {
                    using var sw = new StringWriter();
                    _resourceManager.RenderLocalStyle(setting, sw);
                    output.Content.AppendHtml(sw.ToString());
                }
            } else if (Name.IsNullOrEmpty() && Src.IsNullOrEmpty()) {
                var childContent = await output.GetChildContentAsync();

                var builder = new TagBuilder("style");
                builder.InnerHtml.AppendHtml(childContent);
                builder.TagRenderMode = TagRenderMode.Normal;

                foreach (var attribute in output.Attributes) {
                    builder.Attributes.Add(attribute.Name, attribute.Value.ToString());
                }

                if (!builder.Attributes.ContainsKey("type")) {
                    builder.Attributes.Add("type", "text/css");
                }

                if (At == ResourceLocation.Inline) {
                    output.Content.SetHtmlContent(builder);
                } else {
                    _resourceManager.RegisterStyle(builder);
                }
            }
        }
    }
}