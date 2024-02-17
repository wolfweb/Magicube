using System.Linq;
using Microsoft.AspNetCore.Html;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

namespace Magicube.Web.UI.TagHelpers {
    public class RenderButtons {
        private TableTagHelperContext _parentContext;
        private object _model;

        public RenderButtons(TableTagHelperContext parentContext, object model) {
            _parentContext = parentContext;
            _model = model;
        }

        public TagBuilder Render() {
            var td = new TagBuilder("td");
            td.Attributes.Add("style", $"text-align: center; {_parentContext.ButtonColumnStyle}");
            td.Attributes.Add("class", _parentContext.ButtonColumnClass);

            foreach (var tableButton in _parentContext.TableButtons) {
                if (tableButton.AspCondition != null && tableButton.AspCondition.Invoke(_model)) continue;

                var a = new TagBuilder("a");
                a.Attributes.Add("title", tableButton.Title.Value);
                a.Attributes.Add("class", tableButton.Class);
                a.Attributes.Add("style", tableButton.Style);
                a.Attributes.Add("onclick", tableButton.OnClick);
                a.InnerHtml.SetHtmlContent($"<i class=\"{tableButton.IconClass}\"></i>");

                a.Attributes.Add("target", tableButton.Target);

                var link = tableButton.AspAction?.Invoke(_model);

                a.Attributes.Add("href", link);
                td.InnerHtml.AppendHtml(a);
            }

            return td;
        }
    }
}