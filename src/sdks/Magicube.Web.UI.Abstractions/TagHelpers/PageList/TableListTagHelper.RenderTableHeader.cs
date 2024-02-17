using System.Linq;
using Microsoft.AspNetCore.Html;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

namespace Magicube.Web.UI.TagHelpers {
    public class RenderTableHeader {
        private TableTagHelperContext _parentContext;

        public RenderTableHeader(TableTagHelperContext parentContext) {
            _parentContext = parentContext;
        }

        public TagBuilder Render() {
            if (!_parentContext.RowsSettings.ShowHeader)
                return new TagBuilder("span");

            var output = new TagBuilder("thead");
            var tr = new TagBuilder("tr");
            tr.AddCssClass("table-primary");

            foreach (var tc in _parentContext.TableColumns) {
                if (!tc.Visible)
                    continue;

                var th = new TagBuilder("th");
                th.Attributes.Add("class", tc.HeaderClass);
                th.Attributes.Add("style", tc.HeaderStyle);
                th.Attributes.Add("onclick", tc.OnClick);

                th.InnerHtml.SetHtmlContent(RenderCaption(tc));
                tr.InnerHtml.AppendHtml(th);
            }

            if (_parentContext.TableButtons.Count > 0) {
                var th = new TagBuilder("th");
                th.Attributes.Add("style", $"text-align: center; {_parentContext.ButtonHeaderStyle}");
                th.Attributes.Add("class", _parentContext.ButtonHeaderClass);
                th.InnerHtml.SetHtmlContent(_parentContext.ButtonHeaderTitle);
                tr.InnerHtml.AppendHtml(th);
            }

            output.InnerHtml.AppendHtml(tr);

            return output;
        }

        private string RenderCaption(TableColumnTagHelper tc) {
            var columnName = tc.Title?.Value;
            if (string.IsNullOrEmpty(columnName)) {
                var propertyContext = _parentContext.Properties.FirstOrDefault(x => x.Property.Name == tc.For);
                if( propertyContext == null ) {
                    return tc.For;
                }

                (columnName, var _) = propertyContext.GetDisplay();
            }

            return columnName;
        }
    }
}