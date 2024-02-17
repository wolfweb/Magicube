using System.IO;

namespace Magicube.Core.Encrypts {
    public interface ICryptoDecryptProvider : ICryptoEncryptProvider {
        string Decrypt(string data, string charSet = "utf-8");
        byte[] Decrypt(byte[] data);
        byte[] Decrypt(Stream data);
    }
}
