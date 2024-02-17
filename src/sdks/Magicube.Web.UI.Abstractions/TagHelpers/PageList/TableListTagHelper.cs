using System.Threading.Tasks;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Web.UI.TagHelpers {
    [RestrictChildren("table-columns", "table-buttons", "table-settings")]
    [HtmlTargetElement("entity-table")]
    public class EntityTableTagHelper : TagHelper {
        private TableTagHelperContext _tagHelperContext;

        [HtmlAttributeName("id")]
        public string Id { get; set; } = "entity-table";

        [HtmlAttributeName("model")]
        public ModelExpression Model { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "table table-hover table-bordered";

        [HtmlAttributeName("style")]
        public string Style { get; set; } = "";

        [HtmlAttributeName("asp-action")]
        public string AspAction { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string AspController { get; set; }

        [HtmlAttributeName("return-url")]
        public string ReturnUrl { get; set; } = "";

        public override void Init(TagHelperContext context) {
            _tagHelperContext = new TableTagHelperContext(Model) {
                Id            = Id,
                Class         = Class,
                Style         = Style,
                AspAction     = AspAction,
                AspController = AspController,
                ReturnUrl = System.Net.WebUtility.UrlEncode(ReturnUrl ?? "")
            };
            context.Items.Add(typeof(EntityTableTagHelper), _tagHelperContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            ReturnUrl = System.Net.WebUtility.UrlEncode(ReturnUrl ?? "");

            Model.NotNull();
            AspAction.NotNullOrEmpty();
            AspController.NotNullOrEmpty();

            await output.GetChildContentAsync();

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var table = new TagBuilder("table") { TagRenderMode = TagRenderMode.Normal };
            table.Attributes.Add("id", Id);
            table.Attributes.Add("class", Class);
            table.Attributes.Add("style", Style);

            table.InnerHtml.AppendHtml(new RenderTableHeader(_tagHelperContext).Render());
            
            table.InnerHtml.AppendHtml(new RenderTableBody(_tagHelperContext).Render());

            output.Content.AppendHtml(new RenderContainer(table, _tagHelperContext).Render());

            if (_tagHelperContext.PaginationSettings.AllowPagination) {
                output.Content.AppendHtml(new RenderPagination(_tagHelperContext).Render());
            }
        }
    }
}
