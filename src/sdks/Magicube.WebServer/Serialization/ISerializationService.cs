using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.WebServer.Serialization {
    public interface ISerializationService {
        string Serialize(object obj);

        T Deserialize<T>(string input);

        object Deserialize(string input, Type type);
    }
}
