using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Web.UI.TagHelpers {
    public class RenderPagination {
        private string _link;
        private int _buttonCount = 0;
        private readonly TableTagHelperContext _parentContext;

        public RenderPagination(TableTagHelperContext parentContext) {
            _parentContext = parentContext;

            _link = $"/{_parentContext.AspController}/{_parentContext.AspAction}/?";
            _link += $"&returnUrl={_parentContext.ReturnUrl}";
        }

        public TagBuilder Render() {
            if (_parentContext.PaginationSettings.PageCount <= 1)
                return new TagBuilder("span");

            var output = new TagBuilder("nav");
            var ul = new TagBuilder("ul");
            ul.Attributes.Add("class", _parentContext.PaginationSettings.Class);

            AddPage(ul, "Primeira Página", "1", 1);

            if (_parentContext.PaginationSettings.CurrentPage <= 5) {
                for (int i = 2; i <= Math.Min(5, _parentContext.PaginationSettings.PageCount - 1); i++) {
                    AddPage(ul, $"Página {i}", i.ToString(), i);
                }
                if (_parentContext.PaginationSettings.PageCount > 5) {
                    AddPage(ul, "Próximas páginas", "...", 6);
                }
            } else {
                AddPage(ul, "Páginas anteriores", "...", _parentContext.PaginationSettings.CurrentPage - 1);

                if (_parentContext.PaginationSettings.PageCount < 10) {
                    var limite = _parentContext.PaginationSettings.CurrentPage - 5;
                    for (int i = _parentContext.PaginationSettings.CurrentPage - limite; i <= Math.Min(_parentContext.PaginationSettings.CurrentPage + (4 - limite), _parentContext.PaginationSettings.PageCount - 1); i++) {
                        AddPage(ul, $"Página {i}", i.ToString(), i);
                    }
                } else {
                    for (int i = _parentContext.PaginationSettings.CurrentPage; i <= Math.Min(_parentContext.PaginationSettings.CurrentPage + 4, _parentContext.PaginationSettings.PageCount - 1); i++) {
                        AddPage(ul, $"Página {i}", i.ToString(), i);
                    }
                }
                if (_parentContext.PaginationSettings.CurrentPage + 5 <= _parentContext.PaginationSettings.PageCount) {
                    AddPage(ul, "Próximas páginas", "...", _parentContext.PaginationSettings.CurrentPage + 5);
                }
            }

            if (_buttonCount < 6) {
                ul.InnerHtml.Clear();
                _buttonCount = 0;
                AddPage(ul, "Primeira Página", "1", 1);
                AddPage(ul, "Páginas anteriores", "...", _parentContext.PaginationSettings.PageCount - (6 - _buttonCount));
                for (int i = _parentContext.PaginationSettings.PageCount - (6 - _buttonCount); _buttonCount < 6; i++) {
                    AddPage(ul, $"Página {i}", i.ToString(), i);
                }
            }

            AddPage(ul, "Última página", _parentContext.PaginationSettings.PageCount.ToString(), _parentContext.PaginationSettings.PageCount);

            output.InnerHtml.AppendHtml(ul);
            return output;
        }

        private void AddPage(TagBuilder ul, string title, string value, int page) {
            var li = new TagBuilder("li");
            if (_parentContext.PaginationSettings.CurrentPage == page)
                li.Attributes.Add("class", "active");

            var a = new TagBuilder("a");
            a.Attributes.Add("aria-label", title);
            a.Attributes.Add("title", title);
            a.Attributes.Add("data-page", page.ToString());
            a.Attributes.Add("href", $"{_link}&page={page}");
            a.InnerHtml.SetHtmlContent(value);

            li.InnerHtml.AppendHtml(a);
            ul.InnerHtml.AppendHtml(li);
            _buttonCount++;
        }
    }
}