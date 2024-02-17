using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Web.Attributes;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;

namespace Magicube.Web.UI.TagHelpers {
    public interface IElementBuilder {
        int         Order       { get; }
        bool        Show        { get; }
        ViewContext ViewContext { get; set; }
        bool CanApply(PropertyComponentContext property);
        string Render(PropertyComponentContext property, FormTagHelperContext context);
    }

    public abstract class FormElement : IElementBuilder {
        protected readonly HtmlEncoder _htmlEncoder;
        protected readonly IHtmlGenerator _htmlGenerator;
        protected FormTagHelperContext _formTagHelperContext;

        public ViewContext ViewContext { get; set; }
        public abstract int  Order { get; }
        public virtual  bool Show  => true;

        public FormElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) {
            _htmlEncoder = htmlEncoder;
            _htmlGenerator = htmlGenerator;
        }

        public virtual string Render(PropertyComponentContext property, FormTagHelperContext context) {
            _formTagHelperContext = context;

            var (displayName, attr) = property.GetDisplay();
            var container = BuilderItemGroup(property);
            var input = BuildElement(displayName, property);
            if (Show) {
                var label = BuildLabel(displayName, property);
                container.InnerHtml.AppendHtml(label);
                container.InnerHtml.AppendHtml(input);
                var validation = BuildValidationMessageHtml(property);
                container.InnerHtml.AppendHtml(validation);
                if(attr != null && !attr.Prompt.IsNullOrEmpty()) {
                    container.InnerHtml.AppendHtml(BuildPrompt(attr.Prompt));
                }
            } else {
                container.InnerHtml.AppendHtml(input);
            }
            return container.RenderTag(_htmlEncoder);
        }

        protected IHtmlContent BuildPrompt(string prompt) {
            var tagBuilder = new TagBuilder("small");
            tagBuilder.AddCssClass("form-text text-muted");
            tagBuilder.InnerHtml.AppendHtml(prompt);
            return tagBuilder;
        }

        protected abstract string BuildElement(string display, PropertyComponentContext property);

        protected virtual TagBuilder BuilderItemGroup(PropertyComponentContext property) {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("form-group");
            return tagBuilder;
        }

        protected TagBuilder BuildInput(string displayName, PropertyComponentContext property) {
            var builder = new TagBuilder("input");

            SetValue(property, builder);

            builder.Attributes.Add("placeholder", displayName);
            if(property.Readonly()) builder.Attributes.Add("readonly", "readonly");

            builder.TagRenderMode = TagRenderMode.SelfClosing;

            return builder;
        }

        protected string BuildLabel(string displayName, PropertyComponentContext property, string className = null) {
            var builder = new TagBuilder("label");
            builder.AddCssClass("form-label");
            builder.Attributes.Add("for", property.Property.Name);
            builder.InnerHtml.Append(displayName);
            return builder.RenderTag(_htmlEncoder);
        }

        protected string BuildValidationMessageHtml(PropertyComponentContext property) {
            var builder = new TagBuilder("div");
            builder.AddCssClass("invalid-feedback");
            return builder.RenderTag(_htmlEncoder);
        }

        protected void SetValue(PropertyComponentContext property, TagBuilder builder) {
            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(property.Property.PropertyType) && !property.Property.PropertyType.Equals(typeof(string));
            if (_formTagHelperContext.Model != null) {
                if (_formTagHelperContext.Model is IEntityViewModel viewModel) {
                    var value = property.Property.Value(viewModel);
                    if (value == null) {
                        if (isEnumerable) {
                            builder.Attributes.Add("value", "[]");
                        } else {
                            builder.Attributes.Add("value", "");
                        }
                    } else {
                        if (isEnumerable) {
                            builder.Attributes.Add("value", Json.Stringify(value));
                        } else {
                            builder.Attributes.Add("value", value.ToString());
                        }
                    }
                } else {
                    var value = property.GetValue(_formTagHelperContext.Model);
                    if (value == null) {
                        if (isEnumerable) {
                            builder.Attributes.Add("value", "[]");
                        } else {
                            builder.Attributes.Add("value", "");
                        }
                    } else {
                        if (isEnumerable) {
                            builder.Attributes.Add("value", Json.Stringify(value));
                        } else {
                            builder.Attributes.Add("value", value.ToString());
                        }
                    }
                }
            }
        }

        protected object GetValue(PropertyComponentContext property) {
            if (_formTagHelperContext.Model != null) {
                if (_formTagHelperContext.Model is IEntityViewModel viewModel) {
                    return property.Property.Value(viewModel)?.ToString();
                } else
                    return property.GetValue(_formTagHelperContext.Model)?.ToString();
            }
            return null;
        }

        protected void ParseElementAttribute(PropertyComponentContext property, TagBuilder builder) {
            if (property.HasAttributes(out IEnumerable<MutexAttribute> v)) {
                builder.Attributes.Add("data-mutex-field", string.Join(",", v.Select(x => x.Field)));
                builder.Attributes.Add("data-mutex-type", string.Join(",", v.Select(x => x.Mutex.ToString())));
            }

            if (property.HasAttributes(out IEnumerable<AssociatedAttribute> m)) {
                builder.Attributes.Add("data-associated-field", string.Join(",", m.Select(x => x.Field)));
                builder.Attributes.Add("data-associated-type", string.Join(",", m.Select(x => x.Associated.ToString())));
            }
        }

        public abstract bool CanApply(PropertyComponentContext property);
    }

    public class InputElement : FormElement {
        public InputElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public override int Order => 100;

        protected virtual string InputType => string.Empty;
        protected virtual string ClassName => "form-control";
        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var input = BuildInput(displayName, property);

            input.AddCssClass(ClassName);

            if (!InputType.IsNullOrEmpty()) {
                input.Attributes.Add("type", InputType);
            }

            if (property.HasAttribute(out DataTypeAttribute attr)) {
                var type = attr.GetDataTypeName();
                if (!type.IsNullOrEmpty()) {
                    if (input.Attributes.ContainsKey("type")) input.Attributes["type"] = type;
                    else input.Attributes.Add("type", type);
                }
                if (!input.Attributes.ContainsKey("value")) {
                    input.Attributes.Add("value", DateTime.Now.ToString());
                }
            }

            if (!input.Attributes.ContainsKey("type")) {
                input.Attributes.Add("type", "text");
            }

            input.Attributes.Add("name", property.Property.Name);
            input.Attributes.Add("id", property.Property.Name);

            return input.RenderTag(_htmlEncoder);
        }

        public override bool CanApply(PropertyComponentContext property) {
            return property.Property.PropertyType.IsSimpleType();
        }
    }

    public class TextareaElement : FormElement {
        public TextareaElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public override int Order => 110;

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var builder = new TagBuilder("textarea");

            SetValue(property, builder);

            builder.AddCssClass("form-control");

            builder.Attributes.Add("style", "height: 120px");
            builder.Attributes.Add("placeholder", displayName);
            builder.Attributes.Add("name", property.Property.Name);
            builder.Attributes.Add("id", property.Property.Name);

            builder.InnerHtml.Append(builder.Attributes["value"]);

            return builder.RenderTag(_htmlEncoder);
        }

        public override bool CanApply(PropertyComponentContext property) {
            if (property.HasAttribute(out StringLengthAttribute v)) {
                return typeof(string) == property.Property.PropertyType && v.MaximumLength > 255;
            }
            return false;
        }
    }

    public class HiddenElement : InputElement {
        public HiddenElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public override int  Order => 130;
        public override bool Show  => false;

        protected override string InputType => "hidden";

        public override bool CanApply(PropertyComponentContext property) {
            if(property.Property.Name == Entity.IdKey) return true;

            if(property.HasAttribute(out KeyAttribute v)) {
                return true;
            }
            if(property.HasAttribute(out DataTypeAttribute v1)) {
                return v1.CustomDataType == InputType;
            }
            return false;
        }
    }

    public class RangeElement : InputElement {
        public RangeElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public override    int    Order     => 130;

        protected override string InputType => "range";

        protected override string ClassName => "form-control-range custom-range";

        public override bool CanApply(PropertyComponentContext explorer) {
            if (explorer.HasAttribute(out DataTypeAttribute v)) {
                return v.CustomDataType == "range";
            }
            return false;
        }
    }

    public class ReferenceElement : InputElement {
        public ReferenceElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public    override int    Order     => 140;
        public    override bool   Show      => false;
        protected override string InputType => "hidden";

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var input = BuildInput(displayName, property);
            var name = $"{property.GetDisplay()}.Id";
            if (property.HasAttribute(out DisplayAttribute v)) {
                if (!v.Name.IsNullOrEmpty())
                    name = v.Name;
            }

            return input.RenderTag(_htmlEncoder);
        }

        public override bool CanApply(PropertyComponentContext explorer) {
            if(explorer.HasAttribute(out DataTypeAttribute v)) {
                return v.CustomDataType == "reference";
            }
            return false;
        }
    }

    public class CheckboxElement : InputElement {
        public CheckboxElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public IEnumerable<SelectListItem> Items { get; private set; }

        protected override string InputType => "checkbox";
        protected override string ClassName => "custom-control-input";

        public override int Order => 120;

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var stringBuilder = new StringBuilder();
            if (property.Property.PropertyType == typeof(bool)) {
                var el = new TagBuilder("input");
                el.AddCssClass("form-check-input");
                el.Attributes.Add("type", "checkbox");
                el.Attributes.Add("name", property.Property.Name);
                el.Attributes.Add("id", property.Property.Name);
                el.Attributes.Add("value", "true");

                var value = GetValue(property);
                if(value != null && value.ToString().ToLower() == "true") {
                    el.Attributes.Add("checked", "checked");
                }

                ParseElementAttribute(property, el);

                stringBuilder.Append(el.RenderTag(_htmlEncoder));
            } else {
                stringBuilder.Append("<div class='sub-form-group'>");
                foreach (var item in Items) {
                    stringBuilder.Append("<div class='custom-control custom-checkbox custom-control-inline'>");
                    
                    stringBuilder.Append($"<br>");
                    stringBuilder.Append($"<label for='{item.Text}' class='custom-control-label'>{item.Text}</label>");
                    stringBuilder.Append("</div>");
                }
                stringBuilder.Append("</div>");
            }
            return stringBuilder.ToString();
        }

        public override bool CanApply(PropertyComponentContext property) {
            if (property.HasAttribute(out DataItemsAttribute v)) {
                if (v.ChoicesType == ChoicesTypes.CHECKBOX) {
                    Items = v.GetItems(property).Select(x=>new SelectListItem { 
                        Text  = x.Text,
                        Value = x.Value,
                    });
                    return true;
                }
            }

            return property.Property.PropertyType == typeof(bool);
        }

        public override string Render(PropertyComponentContext property, FormTagHelperContext context) {
            _formTagHelperContext = context;

            var (displayName, attr) = property.GetDisplay();

            var container = BuilderItemGroup(property);

            var checkBox = new TagBuilder("div");
            checkBox.AddCssClass("form-check form-switch");
            container.InnerHtml.AppendHtml(checkBox);

            var input = BuildElement(displayName, property);
            var label = BuildLabel(displayName, property);
            checkBox.InnerHtml.AppendHtml(input);
            checkBox.InnerHtml.AppendHtml(label);
            var validation = BuildValidationMessageHtml(property);
            checkBox.InnerHtml.AppendHtml(validation);

            if(attr!= null && !attr.Prompt.IsNullOrEmpty()) {
                container.InnerHtml.AppendHtml(BuildPrompt(attr.Prompt));
            }
            
            return container.RenderTag(_htmlEncoder);
        }
    }

    public class RadioElement : InputElement {
        public RadioElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        public IEnumerable<SelectListItem> Items { get; private set; }
        protected override string InputType => "radio";
        protected override string ClassName => "custom-control-input";
        public override int Order => 120;

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            StringBuilder stringBuilder = new StringBuilder("<div class='sub-form-group'>");
            foreach (var item in Items) {
                stringBuilder.Append("<div class='custom-control custom-radio custom-control-inline'>");
                
                stringBuilder.Append($"<br>");
                stringBuilder.Append($"<label for='{item.Text}' class='custom-control-label'>{item.Text}</label>");
                stringBuilder.Append("</div>");
            }
            stringBuilder.Append("</div>");
            return stringBuilder.ToString();
        }

        public override bool CanApply(PropertyComponentContext property) {
            if (property.HasAttribute(out DataItemsAttribute v)) {
                if (v.ChoicesType == ChoicesTypes.RADIO) {
                    Items = v.GetItems(property).Select(x => new SelectListItem {
                        Value = x.Value,
                        Text  = x.Text,
                    });
                    return true;
                }
            }
            return false;
        }
    }

    public class SelectElement : FormElement {
        public SelectElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var builder = new TagBuilder("select");
            builder.AddCssClass("form-select dropdown");
            builder.Attributes.Add("name", property.Property.Name);
            builder.Attributes.Add("id", property.Property.Name);

            foreach (var item in Items) {
                var value = GetValue(property);
                builder.InnerHtml.AppendHtml($"<option value=\"{item.Value}\" {(value != null && value.ToString() == item.Value ? "selected": "")}>{item.Text}</option>");
            }

            return builder.RenderTag(_htmlEncoder);
        }

        public IEnumerable<SelectListItem> Items { get; private set; }

        public override int Order => 120;

        public override bool CanApply(PropertyComponentContext property) {
            if (property.HasAttribute(out DataItemsAttribute v)) {
                if (v.ChoicesType == ChoicesTypes.SELECT) {
                    Items = v.GetItems(property).Select(x=> new SelectListItem { 
                        Value = x.Value,
                        Text  = x.Text,
                    });
                    return true;
                }
            }
            return false;
        }
    }

    public class ArrayStringElement : TextareaElement {
        public ArrayStringElement(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder) : base(htmlGenerator, htmlEncoder) {
        }

        protected override string BuildElement(string displayName, PropertyComponentContext property) {
            var builder = new TagBuilder("textarea");

            SetValue(property, builder);
            
            builder.AddCssClass("form-control");
            builder.Attributes.Add("data-role", "tagsinput");
            builder.Attributes.Add("placeholder", displayName);
            builder.Attributes.Add("name", property.Property.Name);
            builder.Attributes.Add("id", property.Property.Name);

            builder.InnerHtml.Append(builder.Attributes["value"]);

            return builder.RenderTag(_htmlEncoder);
        }

        public override bool CanApply(PropertyComponentContext property) {
            if (property.Override != null) {
                return property.Override.Property.Member.PropertyType.GetCollectionType() == typeof(string);
            }

            return property.Property.PropertyType.GetCollectionType() == typeof(string);
        }
    }
}
