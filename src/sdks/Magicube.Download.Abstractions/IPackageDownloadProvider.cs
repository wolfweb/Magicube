using Magicube.Core.Tasks;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Download.Abstractions {
    public interface IPackageDownloadProvider {
        string Identity { get; }
        Task StartDownload(string url, PauseTokenSource pauseTokenSource, CancellationTokenSource cancellationTokenSource);
    }
}
