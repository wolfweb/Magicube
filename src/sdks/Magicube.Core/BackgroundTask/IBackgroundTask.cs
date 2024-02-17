using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Core {
    public interface IBackgroundTask {
        Task Process(CancellationToken cancellationToken);
    }
}
