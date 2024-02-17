using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;

namespace Magicube.Modular.Web.Complier {
    public class RazorViewCompiledItem : RazorCompiledItem {
        public RazorViewCompiledItem(Type type, string kind, string identifier, object[] metadata = null) {
            Type       = type;
            Kind       = kind;
            Identifier = identifier;
            Metadata   = metadata ?? Array.Empty<object>();
        }

        public override string                Identifier { get; }
                                              
        public override string                Kind       { get; }

        public override IReadOnlyList<object> Metadata   { get; }

        public override Type                  Type       { get; }
    }
}
