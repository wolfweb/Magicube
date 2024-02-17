using System;
using System.Collections.Generic;

namespace Magicube.Executeflow.Configurations {
    public class ExecuteflowOptions {
        public IList<Type> Activities { get; private set; }

        public ExecuteflowOptions() {
            Activities = new List<Type>();
        }

        public ExecuteflowOptions RegisterActivity(Type t) {
            if (!Activities.Contains(t)) {
                Activities.Add(t);
            }
            return this;
        }
    }
}
