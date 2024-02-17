using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Web.UI.TagHelpers {
    [RestrictChildren("table-column")]
    public class TableColumnsTagHelper : TagHelper {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            output.SuppressOutput();
            await output.GetChildContentAsync();
        }
    }

    [HtmlTargetElement("table-column", ParentTag = "table-columns", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TableColumnTagHelper : TagHelper {
        [HtmlAttributeName("for")]
        public string For { get; set; }

        [HtmlAttributeName("title")]
        public LocalizedHtmlString Title { get; set; }

        [HtmlAttributeName("visible")]
        public bool Visible { get; set; } = true;

        [HtmlAttributeName("header-class")]
        public string HeaderClass { get; set; } = "";

        [HtmlAttributeName("column-class")]
        public string ColumnClass { get; set; }

        [HtmlAttributeName("header-style")]
        public string HeaderStyle { get; set; }

        [HtmlAttributeName("column-style")]
        public string ColumnStyle { get; set; }

        [HtmlAttributeName("sortable")]
        public bool Sortable { get; set; } = false;

        [HtmlAttributeName("special-sort")]
        public string SpecialSort { get; set; }

        [HtmlAttributeName("column-action")]
        public string ColumnAction { get; set; }

        [HtmlAttributeName("column-controller")]
        public string ColumnController { get; set; }

        [HtmlAttributeName("custom-link")]
        public string CustomLink { get; set; }

        [HtmlAttributeName("link-target")]
        public string LinkTarget { get; set; }

        [HtmlAttributeName("on-click")]
        public string OnClick { get; set; }

        [HtmlAttributeName("selectable")]
        public bool Selecteable { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];

            parentContext.TableColumns.Add(new TableColumnTagHelper {
                For              = For,
                Title            = Title,
                Visible          = Visible,
                ColumnAction     = ColumnAction,
                ColumnClass      = ColumnClass,
                ColumnController = ColumnController,
                ColumnStyle      = ColumnStyle,
                CustomLink       = CustomLink,
                HeaderStyle      = HeaderStyle,
                HeaderClass      = HeaderClass,
                Sortable         = Sortable,
                OnClick          = OnClick,
                SpecialSort      = SpecialSort,
                LinkTarget       = LinkTarget,
                Selecteable      = Selecteable
            });
        }
    }
}