using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Web.UI.TagHelpers {
    [RestrictChildren("container-settings", "pagination-setttings")]
    public class TableSettingsTagHelper : TagHelper {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            output.SuppressOutput();
            await output.GetChildContentAsync();
        }
    }

    [HtmlTargetElement("rows-setttings", ParentTag = "table-settings", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RowsSettingsTagHelper : TagHelper {
        [HtmlAttributeName("show-header")]
        public bool ShowHeader { get; set; } = true;

        [HtmlAttributeName("row-script")]
        public string RowScript { get; set; } = "";  // use {0} to get row id

        [HtmlAttributeName("on-click")]
        public string OnClick { get; set; } = ""; // on-click on row

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "";

        [HtmlAttributeName("style")]
        public string Style { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];
            parentContext.RowsSettings.ShowHeader = ShowHeader;
            parentContext.RowsSettings.RowScript = RowScript;
            parentContext.RowsSettings.OnClick = OnClick;
            parentContext.RowsSettings.Class = Class;
            parentContext.RowsSettings.Style = Style;
        }
    }


    [HtmlTargetElement("pagination-setttings", ParentTag = "table-settings", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PaginationSettingsTagHelper : TagHelper {
        [HtmlAttributeNotBound]
        public bool AllowPagination { get; set; } = false;

        [HtmlAttributeName("page-count")]
        public int PageCount { get; set; } = 1;

        [HtmlAttributeName("page-index")]
        public int CurrentPage { get; set; } = 1;

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "pagination pagination-sm";

        [HtmlAttributeName("style")]
        public string Style { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];
            parentContext.PaginationSettings.AllowPagination = true;
            parentContext.PaginationSettings.PageCount = PageCount;
            parentContext.PaginationSettings.CurrentPage = CurrentPage;
            parentContext.PaginationSettings.Class = Class;
            parentContext.PaginationSettings.Style = Style;
        }
    }
}