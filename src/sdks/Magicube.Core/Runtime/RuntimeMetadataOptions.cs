using System;
using System.Collections.Generic;

namespace Magicube.Core.Runtime {
    public class StaticRuntimeMetadataOptions {
        public IList<Type> ExportTypes { get; private set; }

        public StaticRuntimeMetadataOptions() {
            ExportTypes = new List<Type>();
        }

        public StaticRuntimeMetadataOptions RegisterRuntimeMetadata(Type t) {
            if (!ExportTypes.Contains(t)) {
                ExportTypes.Add(t);
            }
            return this;
        }
    }
}
