using System.IO;
using System.Text;

namespace Magicube.Core.Encrypts {
    public interface ICryptoEncryptProvider : ICryptoProvider {
        string Encrypt(string data, string charSet = "utf-8");
        byte[] Encrypt(byte[] data);
        byte[] Encrypt(Stream data);
    }
}
