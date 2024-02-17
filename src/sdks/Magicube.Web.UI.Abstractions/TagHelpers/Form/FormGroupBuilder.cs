using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Magicube.Web.UI.TagHelpers {
    public class FormGroupBuilder {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IHtmlHelper _htmlHelper;
        private readonly IEnumerable<IElementBuilder> _elementBuilders;

        public FormGroupBuilder(
            IHtmlGenerator htmlGenerator,
            IHtmlHelper htmlHelper,
            HtmlEncoder htmlEncoder,
            IEnumerable<IElementBuilder> elementBuilders
            ) {
            _htmlGenerator   = htmlGenerator;
            _htmlEncoder     = htmlEncoder;
            _htmlHelper      = htmlHelper;
            _elementBuilders = elementBuilders.OrderByDescending(x => x.Order);
        }

        private ViewContext _viewContext;
        public ViewContext ViewContext {
            get { return _viewContext; }
            set {
                _viewContext = value;
                (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
            }
        }

        public string GetFormGroup(PropertyComponentContext property, FormTagHelperContext context) {
            if (property.HasAttribute(out NoUIRenderAttribute _)) return string.Empty;            
            if (property.Property.CanWrite ) {
                var builder = _elementBuilders.FirstOrDefault(x => x.CanApply(property));
                if (builder == null) {
                    return GenerateComplexFormGroups(property);
                } else {
                    builder.ViewContext = ViewContext;
                    return builder.Render(property, context);
                }
            } else {
                return string.Empty;
            }
        }

        private string GenerateComplexFormGroups(PropertyComponentContext property) {
            StringBuilder builder = new StringBuilder();
            string label = BuildLabelHtml(property);
            //foreach (var prop in property.Properties) {
            //    builder.Append(await GetFormGroup(prop));
            //}

            return $@"<div class='form-group'>
                    {label}
                    <div class=""sub-form-group"">
                        {builder}
                    </div></div>";
        }

        private string BuildLabelHtml(PropertyComponentContext property) {
            var builder = new TagBuilder("label");
            builder.Attributes.Add("for", property.Property.Name);

            return builder.RenderTag(_htmlEncoder);
        }       
    }
}
