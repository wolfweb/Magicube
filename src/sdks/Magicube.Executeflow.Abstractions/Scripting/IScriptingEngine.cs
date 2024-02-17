using System.Collections.Generic;

namespace Magicube.Executeflow.Scripting {
    public interface IScriptingEngine {
        bool CanExecute(string region);
        IScriptingScope CreateScope(IEnumerable<GlobalMethod> methods);
    }
}
