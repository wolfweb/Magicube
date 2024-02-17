using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Web.UI.TagHelpers {
    [RestrictChildren("table-button")]
    public class TableButtonsTagHelper : TagHelper {
        [HtmlAttributeName("header-class")]
        public string HeaderClass { get; set; }

        [HtmlAttributeName("header-style")]
        public string HeaderStyle { get; set; }

        [HtmlAttributeName("column-class")]
        public string ColumnClass { get; set; }

        [HtmlAttributeName("column-style")]
        public string ColumnStyle { get; set; }

        [HtmlAttributeName("header-title")]
        public string HeaderTitle { get; set; } = "操作";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            output.SuppressOutput();
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];
            parentContext.ButtonHeaderClass = HeaderClass;
            parentContext.ButtonHeaderStyle = HeaderStyle;
            parentContext.ButtonColumnClass = ColumnClass;
            parentContext.ButtonColumnStyle = ColumnStyle;
            parentContext.ButtonHeaderTitle = HeaderTitle;

            await output.GetChildContentAsync();
        }
    }

    [HtmlTargetElement("table-button", ParentTag = "table-buttons", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TableButtonTagHelper : TagHelper {
        [HtmlAttributeName("title")]
        public LocalizedHtmlString Title { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "text-secondary font-weight-bold text-md";

        [HtmlAttributeName("icon-class")]
        public string IconClass { get; set; }

        [HtmlAttributeName("style")]
        public string Style { get; set; }

        [HtmlAttributeName("asp-action")]
        public Func<object,string> AspAction { get; set; }

        [HtmlAttributeName("asp-condition")]
        public Func<object,bool> AspCondition { get; set; }

        [HtmlAttributeName("target")]
        public string Target { get; set; } = "_self";

        [HtmlAttributeName("on-click")]
        public string OnClick { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var parentContext = (TableTagHelperContext)context.Items[typeof(EntityTableTagHelper)];
            parentContext.TableButtons.Add(new TableButtonTagHelper {
                Title            = Title,
                Class            = Class,
                Style            = Style,
                Target           = Target,
                OnClick          = OnClick,
                IconClass        = IconClass,
                AspAction        = AspAction,
                AspCondition     = AspCondition,
            });

            output.SuppressOutput();
        }
    }
}