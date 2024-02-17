using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Magicube.Web.UI.TagHelpers {
    [HtmlTargetElement("entity-form")]
    public class EntityFormTagHelper : TagHelper {
        [HtmlAttributeName("asp-model")]
        public ModelExpression Model        { get; set; }
        
        [HtmlAttributeName("asp-action")]
        public string          TargetAction { get; set; }

        [HtmlAttributeName("asp-method")]
        public string          Method       { get; set; } = "post";

        [HtmlAttributeName("display")]
        public LocalizedHtmlString Display  { get; set; }

        [HtmlAttributeName("class")]
        public string Class { get; set; } = "border-light";

        [ViewContext]
        public ViewContext ViewContext      { get; set; }

        private readonly IServiceProvider _serviceProvider;
        private FormTagHelperContext _formTagHelperContext;

        public EntityFormTagHelper(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public override void Init(TagHelperContext context) {
            _formTagHelperContext = new FormTagHelperContext(Model);
            context.Items.Add(typeof(EntityFormTagHelper), _formTagHelperContext);
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            var formGroupBuilder = _serviceProvider.GetRequiredService<FormGroupBuilder>();
            formGroupBuilder.ViewContext = ViewContext;
            var childrenContent = await output.GetChildContentAsync();

            var builder = new StringBuilder();

            foreach (var propertyContext in _formTagHelperContext.Properties) {
                builder.Append(formGroupBuilder.GetFormGroup(propertyContext, _formTagHelperContext));
            }

            var card = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            card.AddCssClass("card mb-3");
            card.AddCssClass(Class);

            var header = BuildCardHeader();
            card.InnerHtml.AppendHtml(header);

            var body = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            body.AddCssClass("card-body");

            var form = new TagBuilder("form");
            form.Attributes.Add("method", Method);
            form.Attributes.Add("action", TargetAction);
            form.Attributes.Add("autocomplete", "off");
            form.InnerHtml.AppendHtml(builder.ToString());
            form.InnerHtml.AppendHtml(childrenContent.GetContent());
            body.InnerHtml.AppendHtml(form);

            card.InnerHtml.AppendHtml(body);

            output.TagName = "div";
            output.Attributes.Add("class", "row");
            output.Content.AppendHtml(card);
        }

        private TagBuilder BuildCardHeader() {
            var header = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            header.AddCssClass("card-header");
            var headerRow = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            headerRow.AddCssClass("row");
            header.InnerHtml.AppendHtml(headerRow);

            var colLeft = new TagBuilder("div") { TagRenderMode = TagRenderMode.Normal };
            colLeft.AddCssClass("col");
            var display = new TagBuilder("h5") { TagRenderMode = TagRenderMode.Normal };
            display.InnerHtml.AppendHtml(Display);
            colLeft.InnerHtml.AppendHtml(display);

            headerRow.InnerHtml.AppendHtml(colLeft);

            return header;
        }
    }
}
