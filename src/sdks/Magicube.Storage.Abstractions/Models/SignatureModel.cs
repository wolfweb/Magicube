using System.Collections.Generic;

namespace Magicube.Storage.Abstractions.Models {
    public class SignatureModel {
        public string                     Provider   { get; set; }
        public string                     FileType   { get; set; }
        public string                     Method     { get; set; }
        public long                       Expiration { get; set; }
        public string                     Path       { get; set; }
        public Dictionary<string, string> Arg        { get; set; }
    }
}
