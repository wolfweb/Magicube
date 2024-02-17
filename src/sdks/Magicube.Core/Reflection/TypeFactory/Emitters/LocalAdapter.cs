using System;

namespace Magicube.Core.Reflection.Emitters {
    internal class LocalAdapter : ILocal, IAdaptedLocal {
        private readonly IGenericParameterBuilder _genericParameter;

        private readonly IGenericParameterBuilder[] _genericTypeArgs;

        internal LocalAdapter(string name) {
            Name = name;
        }

        internal LocalAdapter(string name, Type localType, int localIndex, bool isPinned, object local = null) {
            Name       = name;
            LocalType  = localType;
            LocalIndex = localIndex;
            IsPinned   = isPinned;
            Local      = local;
        }

        internal LocalAdapter(string name, IGenericParameterBuilder genericParameter, object local = null) {
            _genericParameter = genericParameter;
            Name = name;
            Local = local;
        }

        internal LocalAdapter(string name, Type localTypeDefinition, IGenericParameterBuilder[] genericTypeArgs, object local = null) {
            _genericTypeArgs = genericTypeArgs;
            Name = name;
            LocalType = localTypeDefinition;
            Local = local;
        }

        public object Local { get; set; }

        public string Name { get; }

        public bool IsPinned { get; }

        public int LocalIndex { get; set; }

        public Type LocalType { get; set; }
    }
}