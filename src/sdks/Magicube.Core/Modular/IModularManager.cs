using System;
using System.Collections.Generic;

namespace Magicube.Core.Modular {
    public interface IModularManager {
        Application Initialize();

        ModularInfo FindModular(Type type);
        IEnumerable<ModularInfo> FindModular(ModularType modularType);
    }
}
