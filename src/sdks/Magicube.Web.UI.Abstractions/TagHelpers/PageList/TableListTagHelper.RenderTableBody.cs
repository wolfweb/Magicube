using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Magicube.Core.Reflection;
using Magicube.Data;
using Magicube.Data.Abstractions.ViewModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magicube.Web.UI.TagHelpers {
    public class RenderTableBody {
        private TableTagHelperContext _parentContext;

        public RenderTableBody(TableTagHelperContext parentContext) {
            _parentContext = parentContext;
        }

        public TagBuilder Render() {
            var output = new TagBuilder("tbody");
            int i = 1;
            foreach (var model in (IEnumerable)_parentContext.Model) {
                var typeAccessor = TypeAccessor.Get(model.GetType(), model).Context;

                var tr = new TagBuilder("tr");
                tr.AddCssClass("table-light");
                tr.Attributes.Add("id", $"{_parentContext.Id}-row-{i}");

                foreach (var tc in _parentContext.TableColumns) {
                    if (!tc.Visible)
                        continue;

                    var td = new TagBuilder("td");
                    td.Attributes.Add("class", tc.ColumnClass);
                    td.Attributes.Add("style", tc.ColumnStyle);
                    td.Attributes.Add("name", tc.For);
                    td.Attributes.Add("onclick", tc.OnClick);
                    IEntityViewModel viewModel = typeof(IEntityViewModel).IsAssignableFrom(_parentContext.ModelType) ? (IEntityViewModel)model : null;

                    if (tc.Selecteable) {
                        var renderTag = new TagBuilder("input");
                        renderTag.Attributes.Add("type", "checkbox");
                        renderTag.Attributes.Add("name", tc.For);

                        td.InnerHtml.AppendHtml(renderTag);
                        tr.InnerHtml.AppendHtml(td);
                        continue;
                    }

                    foreach (var property in _parentContext.Properties) {
                        if (property.Property.Name == tc.For) {
                            var columnValue = property.GetValue(viewModel);

                            property.HasAttribute(out DisplayFormatAttribute displayFormat);

                            if (displayFormat != null) {
                                var dataFormat = displayFormat.DataFormatString;
                                columnValue = string.Format(dataFormat, columnValue);
                            }

                            if (property.Property.PropertyType == typeof(bool)) {
                                td.InnerHtml.SetHtmlContent((bool)columnValue ? "<i class=\"fa fa-check-square-o\">" : "<i class=\"fa fa-square-o\">");
                            } else {
                                if (string.IsNullOrEmpty(tc.CustomLink)) {
                                    td.InnerHtml.SetHtmlContent(columnValue?.ToString());
                                } else {
                                    var a = new TagBuilder("a");
                                    a.Attributes.Add("class", tc.ColumnClass);
                                    a.Attributes.Add("style", tc.ColumnStyle);
                                    a.Attributes.Add("target", tc.LinkTarget);

                                    var link = tc.CustomLink + (tc.CustomLink.Contains("?") ? "" : "?");
                                    link += $"&returnUrl={_parentContext.ReturnUrl}";

                                    a.Attributes.Add("href", link);
                                    a.InnerHtml.SetHtmlContent(columnValue.ToString());
                                    td.InnerHtml.SetHtmlContent(a);
                                }
                            } 

                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(_parentContext.RowsSettings.RowScript)) {
                        tr.InnerHtml.AppendHtml(string.Format(_parentContext.RowsSettings.RowScript, $"{_parentContext.Id}-row-{i}"));
                    }

                    tr.InnerHtml.AppendHtml(td);
                }

                if (_parentContext.TableButtons.Count > 0) {
                    var buttonsRender = new RenderButtons(_parentContext, model);
                    tr.InnerHtml.AppendHtml(buttonsRender.Render());
                }

                output.InnerHtml.AppendHtml(tr);
            }


            return output;
        }
    }
}