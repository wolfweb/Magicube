using System;

namespace Magicube.Core.Encrypts {
    public interface ICryptoProvider : IDisposable{
        string Name  { get; }
        string Group { get; }
    }
}
