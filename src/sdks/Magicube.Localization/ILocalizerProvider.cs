using System.Collections.Generic;

namespace Magicube.Localization {
    public interface ILocalizerProvider {
        IList<LocalizerModel> Localizers();
    }
}
