using System;
using System.Collections.Generic;
using System.Reflection;

namespace Magicube.Core.Runtime {
    public class RuntimeMethodProvider {
        public string                               Tag             { get; set; }
        public string                               Title           { get; set; }
        public MethodInfo                           Method          { get; set; }
        public string                               Descript        { get; set; }
        public bool                                 IsStatic        { get; set; }
        public Type                                 ImplType        { get; set; }
        public Type                                 ReturnType      { get; set; }
        public Type                                 DeclaredType    { get; set; }
        public RuntimeMetadata                      RuntimeProvider { get; set; }
        public IEnumerable<MethodParameterMetadata> Parameters      { get; set; }
    }
}
