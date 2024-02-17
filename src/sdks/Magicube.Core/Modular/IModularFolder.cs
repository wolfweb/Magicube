using System.Collections.Generic;

namespace Magicube.Core.Modular {
    public interface IModularFolder {
        IEnumerable<ModularInfo> LoadModulars(Application application);
    }
}
