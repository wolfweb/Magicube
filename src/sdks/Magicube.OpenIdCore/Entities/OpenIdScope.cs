using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Magicube.OpenIdCore.Entities {
    public class OpenIdScope: Entity<string> {
        public OpenIdScope() {
            Id = Guid.NewGuid().ToString();
        }
        public string                                   Name         { get; set; }
        public JObject                                  Properties   { get; set; }
        public string                                   Description  { get; set; }
        public string                                   DisplayName  { get; set; }

        [ColumnExtend(Size = 4000)]
        public string                                   Descriptions { get; set; }

        public ImmutableArray<OpenidScopeResource>      Resources    { get; set; } = ImmutableArray.Create<OpenidScopeResource>();
        
        public ImmutableArray<OpenIdScopeName>          DisplayNames { get; set; } = ImmutableArray.Create<OpenIdScopeName>();
    }

    public class OpenIdScopeName : Entity {
        [ForeignKey(IdKey)]
        public OpenIdScope OpenIdScope { get; set; }
        public string      Name        { get; set; }
    }

    public class OpenidScopeResource : Entity {
        [ForeignKey(IdKey)]
        public OpenIdScope OpenIdScope { get; set; }
        public string      Resource    { get; set; }
    }
}
