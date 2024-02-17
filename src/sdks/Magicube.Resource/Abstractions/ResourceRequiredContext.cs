using System.IO;
using Magicube.Core;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Magicube.Resource {
    public class ResourceRequiredContext {
        private const string NotIE = "!IE";

        public ResourceDefinition   Resource            { get; set; }
        public RequireSettings      Settings            { get; set; }
        public IFileVersionProvider FileVersionProvider { get; set; }

        public void WriteTo(TextWriter writer, string appPath) {
            var tagBuilder = Resource.GetTagBuilder(Settings, appPath, FileVersionProvider);

            if (Settings.Condition.IsNullOrEmpty()) {
                tagBuilder.WriteTo(writer, NullHtmlEncoder.Default);
                return;
            }

            if (Settings.Condition == NotIE) {
                writer.Write("<!--[if " + Settings.Condition + "]>-->");
            } else {
                writer.Write("<!--[if " + Settings.Condition + "]>");
            }

            tagBuilder.WriteTo(writer, NullHtmlEncoder.Default);

            if (!Settings.Condition.IsNullOrEmpty()) {
                if (Settings.Condition == NotIE) {
                    writer.Write("<!--<![endif]-->");
                } else {
                    writer.Write("<![endif]-->");
                }
            }
        }
    }
}