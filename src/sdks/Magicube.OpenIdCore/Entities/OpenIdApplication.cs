using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Magicube.OpenIdCore.Entities {
    public class OpenIdApplication : Entity<string> {
        public OpenIdApplication() {
            Id = Guid.NewGuid().ToString();
        }
        public string                                      Type                   { get; set; }
        public string                                      ClientId               { get; set; }
        
        [ColumnExtend(Size = 4000)]
        public JObject                                     Properties             { get; set; }
        
        public string                                      ConsentType            { get; set; }
        public string                                      DisplayName            { get; set; }
        public string                                      ClientSecret           { get; set; }
        
        [ColumnExtend(Size = 4000)]
        public ImmutableArray<string>                      Roles                  { get; set; } = ImmutableArray.Create<string>();
        
        [ColumnExtend(Size = 4000)]
        public ImmutableArray<string>                      Permissions            { get; set; } = ImmutableArray.Create<string>();
        
        [ColumnExtend(Size = 4000)]
        public ImmutableArray<string>                      Requirements           { get; set; } = ImmutableArray.Create<string>();
        
        public string                                      DisplayNames           { get; set; }
        public ImmutableArray<OpenIdAppRedirectUri>        RedirectUris           { get; set; }
        public string                                      ConcurrencyToken       { get; set; } = Guid.NewGuid().ToString();
        public ImmutableArray<OpenIdAppLogoutUri>          PostLogoutRedirectUris { get; set; }

        public string                                      JsonWebKeySet          { get; set; }
        public string                                      Settings               { get; set; }
    }

    public class OpenIdAppLogoutUri : Entity {
        public string LogoutRedirectUri { get; set; }

        [ForeignKey(IdKey)]
        public OpenIdApplication Application { get; set; }
    }

    public class OpenIdAppRedirectUri : Entity {
        public string RedirectUri { get; set; }

        [ForeignKey(IdKey)]
        public OpenIdApplication Application { get; set; }
    }
}
