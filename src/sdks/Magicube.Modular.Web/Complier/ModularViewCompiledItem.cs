using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Modular.Web.Complier {
    public class ModularViewCompiledItem : RazorCompiledItem {
        public override string                Identifier { get; }
                                              
        public override string                Kind       { get; }

        public override IReadOnlyList<object> Metadata   { get; }

        public override Type                  Type       { get; }

        public ModularViewCompiledItem(RazorCompiledItemAttribute attr, string modularName) {
            Type       = attr.Type;
            Kind       = attr.Kind;
            Identifier = $"/{modularName}{attr.Identifier}";

            Metadata = Type.GetCustomAttributes(inherit: true).Select(o =>
                o is RazorSourceChecksumAttribute rsca
                    ? new RazorSourceChecksumAttribute(rsca.ChecksumAlgorithm, rsca.Checksum, $"/{modularName}{rsca.Identifier}")
                    : o).ToList();
        }
    }
}
