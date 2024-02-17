using System.Collections.Generic;

namespace Magicube.Localization {
    public class LocalizerModel {
        public LocalizerModel() {
            Localizers = new Dictionary<string, string>();
        }
        public string                    CultureName { get; set; }
        public Dictionary<string,string> Localizers  { get; set; }
    }
}
