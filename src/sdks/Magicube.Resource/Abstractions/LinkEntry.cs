using Magicube.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magicube.Resource {
    public class LinkEntry {
        private readonly TagBuilder _builder = new TagBuilder("link");

        public string Condition { get; set; }

        public LinkEntry() {
            _builder.TagRenderMode = TagRenderMode.SelfClosing;
        }

        public string Rel {
            get {
                _builder.Attributes.TryGetValue("rel", out string value);
                return value;
            }
            set { SetAttribute("rel", value); }
        }

        public string Type {
            get {
                _builder.Attributes.TryGetValue("type", out string value);
                return value;
            }
            set { SetAttribute("type", value); }
        }

        public string Title {
            get {
                _builder.Attributes.TryGetValue("title", out string value);
                return value;
            }
            set { SetAttribute("title", value); }
        }

        public string Href {
            get {
                _builder.Attributes.TryGetValue("href", out string value);
                return value;
            }
            set { SetAttribute("href", value); }
        }

        public bool AppendVersion { get; set; }

        public IHtmlContent GetTag() {
            if (!Condition.IsNullOrEmpty()) {
                var htmlBuilder = new HtmlContentBuilder();
                htmlBuilder.AppendHtml("<!--[if " + Condition + "]>");
                htmlBuilder.AppendHtml(_builder);
                htmlBuilder.AppendHtml("<![endif]-->");
                return htmlBuilder;
            }

            return _builder;
        }

        public LinkEntry AddAttribute(string name, string value) {
            _builder.MergeAttribute(name, value);
            return this;
        }

        public LinkEntry SetAttribute(string name, string value) {
            _builder.MergeAttribute(name, value, true);
            return this;
        }
    }
}
