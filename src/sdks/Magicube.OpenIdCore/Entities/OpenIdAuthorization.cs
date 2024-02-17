using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.OpenIdCore.Entities {
    public class OpenIdAuthorization : Entity<string> {
        public OpenIdAuthorization() {
            Id = Guid.NewGuid().ToString();
        }
        public string                   Type             { get; set; }
        public string                   Status           { get; set; }        
        public string                   Subject          { get; set; }
                                        
        [ColumnExtend(Size = 4000)]     
        public JObject                  Properties       { get; set; }       
                                        
        public DateTimeOffset?          CreationDate     { get; set; }
                                        
        [ForeignKey(IdKey)]             
        public OpenIdApplication        Application      { get; set; }
                                        
        public string                   ConcurrencyToken { get; set; } = Guid.NewGuid().ToString();

        [ColumnExtend(Size = 2000)]     
        public ImmutableArray<string>   Scopes           { get; set; } = ImmutableArray.Create<string>();

        [ColumnExtend(Size = 2000)]
        public ICollection<OpenIdToken> Tokens           { get; set; }
    }
}
