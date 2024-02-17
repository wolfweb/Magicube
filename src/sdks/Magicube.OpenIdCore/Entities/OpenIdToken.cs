using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magicube.OpenIdCore.Entities {
    public class OpenIdToken : Entity<string> {
        public OpenIdToken() {
            Id = Guid.NewGuid().ToString();
        }

        [ForeignKey(IdKey)]
        public OpenIdApplication    Application     { get; set; }

        [ForeignKey(IdKey)]
        public OpenIdAuthorization  Authorization   { get; set; }
        public DateTimeOffset?      CreationDate    { get; set; }
        public DateTimeOffset?      ExpirationDate  { get; set; }
        public string               Payload         { get; set; }
        
        [ColumnExtend(Size = 4000)]
        public JObject              Properties      { get; set; }
        
        public DateTimeOffset?      RedemptionDate  { get; set; }        
        public string               ReferenceId     { get; set; }        
        public string               Status          { get; set; }
        public string               Subject         { get; set; }
        public string               Type            { get; set; }
    }
}
