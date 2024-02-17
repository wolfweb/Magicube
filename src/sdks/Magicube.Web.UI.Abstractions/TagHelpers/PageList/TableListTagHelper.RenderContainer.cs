using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Magicube.Web.UI.TagHelpers {
    [HtmlTargetElement("container-settings", ParentTag = "table-settings", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RenderContainerTagHelper : TagHelper {
        [HtmlAttributeName("display")]
        public LocalizedHtmlString Display { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "border-light";

        internal string InnerContent { get; private set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];
            parentContext.ContainerSettings.Display = Display;
            parentContext.ContainerSettings.Class = Class;
            parentContext.ContainerSettings.InnerContent = (await output.GetChildContentAsync()).GetContent();
        }
    }

    public class RenderContainer {
        private readonly TagBuilder _table;
        private readonly RenderContainerTagHelper _containerSettings;
        public RenderContainer(TagBuilder table, TableTagHelperContext parentContext) {
            _table = table;
            _containerSettings = parentContext.ContainerSettings;
        }

        public TagBuilder Render() {
            var card = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            card.AddCssClass("card mb-3");
            card.AddCssClass(_containerSettings.Class);

            var header = BuildCardHeader(_containerSettings.InnerContent);
            card.InnerHtml.AppendHtml(header);

            var body = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            body.AddCssClass("card-body");
            body.InnerHtml.AppendHtml(_table);

            card.InnerHtml.AppendHtml(body);

            return card;
        }

        private TagBuilder BuildCardHeader(string innerContent) {
            var header = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            header.AddCssClass("card-header");
            var headerRow = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            headerRow.AddCssClass("row");
            header.InnerHtml.AppendHtml(headerRow);

            var colLeft = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            colLeft.AddCssClass("col");
            var display = new TagBuilder("h5") { TagRenderMode = TagRenderMode.Normal };
            display.InnerHtml.AppendHtml(_containerSettings.Display);
            colLeft.InnerHtml.AppendHtml(display);

            headerRow.InnerHtml.AppendHtml(colLeft);

            var colRight = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            colRight.AddCssClass("col-12 col-md-auto");
            colRight.InnerHtml.AppendHtml(innerContent);

            headerRow.InnerHtml.AppendHtml(colRight);
            return header;
        }
    }
}