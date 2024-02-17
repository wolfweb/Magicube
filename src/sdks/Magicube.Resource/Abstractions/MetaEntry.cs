using System.Collections.Generic;
using Magicube.Core;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Magicube.Resource {
    public class MetaEntry {
        private readonly TagBuilder _builder = new TagBuilder("meta");

        public MetaEntry() {
            _builder.TagRenderMode = TagRenderMode.SelfClosing;
        }

        public MetaEntry(string name = null, string property = null, string content = null, string httpEquiv = null, string charset = null) : this() {
            if (!name.IsNullOrEmpty()) {
                Name = name;
            }

            if (!property.IsNullOrEmpty()) {
                Property = property;
            }

            if (!content.IsNullOrEmpty()) {
                Content = content;
            }

            if (!httpEquiv.IsNullOrEmpty()) {
                HttpEquiv = httpEquiv;
            }

            if (!charset.IsNullOrEmpty()) {
                Charset = charset;
            }
        }

        public static MetaEntry Combine(MetaEntry meta1, MetaEntry meta2, string contentSeparator) {
            var newMeta = new MetaEntry();
            Merge(newMeta._builder.Attributes, meta1._builder.Attributes, meta2._builder.Attributes);
            if (!meta1.Content.IsNullOrEmpty() && !meta2.Content.IsNullOrEmpty()) {
                newMeta.Content = meta1.Content + contentSeparator + meta2.Content;
            }

            return newMeta;
        }

        private static void Merge(IDictionary<string, string> d1, params IDictionary<string, string>[] sources) {
            foreach (var d in sources) {
                foreach (var pair in d) {
                    d1[pair.Key] = pair.Value;
                }
            }
        }

        public MetaEntry AddAttribute(string name, string value) {
            _builder.MergeAttribute(name, value);
            return this;
        }

        public MetaEntry SetAttribute(string name, string value) {
            _builder.MergeAttribute(name, value, true);
            return this;
        }

        public string Name {
            get {
                _builder.Attributes.TryGetValue("name", out var value);
                return value;
            }
            set { SetAttribute("name", value); }
        }

        public string Property {
            get {
                _builder.Attributes.TryGetValue("property", out var value);
                return value;
            }
            set { SetAttribute("property", value); }
        }

        public string Content {
            get {
                string value;
                _builder.Attributes.TryGetValue("content", out value);
                return value;
            }
            set { SetAttribute("content", value); }
        }

        public string HttpEquiv {
            get {
                _builder.Attributes.TryGetValue("http-equiv", out var value);
                return value;
            }
            set { SetAttribute("http-equiv", value); }
        }

        public string Charset {
            get {
                _builder.Attributes.TryGetValue("charset", out var value);
                return value;
            }
            set { SetAttribute("charset", value); }
        }

        public IHtmlContent GetTag() {
            return _builder;
        }
    }
}
