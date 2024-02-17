using System;
using Magicube.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Resource.TagHelpers {
    [HtmlTargetElement("link", Attributes = SrcAttributeName)]
    public class LinkTagHelper : TagHelper {
        private const string SrcAttributeName = "asp-src";
        private const string AppendVersionAttributeName = "asp-append-version";

        [HtmlAttributeName(SrcAttributeName)]
        public string Src { get; set; }

        [HtmlAttributeName(AppendVersionAttributeName)]
        public bool? AppendVersion { get; set; }

        public string Rel       { get; set; }

        public string Type      { get; set; }

        public string Title     { get; set; }

        public string Condition { get; set; }

        private readonly IResourceManager _resourceManager;

        public LinkTagHelper(IResourceManager resourceManager) {
            _resourceManager = resourceManager;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var linkEntry = new LinkEntry();

            if (!Src.IsNullOrEmpty()) {
                linkEntry.Href = Src;
            }

            if (!Rel.IsNullOrEmpty()) {
                linkEntry.Rel = Rel;
            }

            if (!Condition.IsNullOrEmpty()) {
                linkEntry.Condition = Condition;
            }

            if (!Title.IsNullOrEmpty()) {
                linkEntry.Title = Title;
            }

            if (!Type.IsNullOrEmpty()) {
                linkEntry.Type = Type;
            }

            if (AppendVersion.HasValue) {
                linkEntry.AppendVersion = AppendVersion.Value;
            }

            foreach (var attribute in output.Attributes) {
                if (string.Equals(attribute.Name, "href", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                linkEntry.SetAttribute(attribute.Name, attribute.Value.ToString());
            }

            _resourceManager.RegisterLink(linkEntry);

            output.TagName = null;
        }
    }
}
