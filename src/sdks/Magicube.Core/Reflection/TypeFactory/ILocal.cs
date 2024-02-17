using System;

namespace Magicube.Core.Reflection {
    public interface ILocal {
        string Name { get; }

        bool IsPinned { get; }

        int LocalIndex { get; }

        Type LocalType { get; }
    }
}