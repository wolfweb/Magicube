using System;
using System.Collections.Generic;

namespace Magicube.Executeflow {
    public interface IActivityProvider {
        IEnumerable<IActivity> Activities { get; }
        IActivity Retrieve(Type assemblyInfo);
    }
}
