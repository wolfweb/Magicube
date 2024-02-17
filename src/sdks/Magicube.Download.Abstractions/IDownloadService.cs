using Magicube.Core.Tasks;
using Magicube.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Download.Abstractions {
    public interface IDownloadService {
        DownloadStatus Status { get; }

        Task CancelAsync();
        void Cancel();

        Task PauseAsyc();
        Task ResumeAsync();

        Task StartDownloadAsync(string url, IPackageDownloadProvider packageDownloadProvider);
    }

    public class DownloadService : IDownloadService {
        private readonly PauseTokenSource _pauseTokenSource;
        private readonly CancellationTokenSource _stateTokenSource;
        private readonly SemaphoreSlim _singleInstanceSemaphore = new SemaphoreSlim(1, 1);

        public DownloadService() {
            _pauseTokenSource  = new PauseTokenSource();
            _stateTokenSource  = new CancellationTokenSource();
        }

        public DownloadStatus Status { get; private set; }

        public void Cancel() {
            _stateTokenSource.Cancel(true);
            Status = DownloadStatus.Stopped;
        }

        public async Task CancelAsync() {
            Cancel();
            await Task.Yield();
            await ResumeAsync();
        }

        public async Task PauseAsyc() {
            Status = DownloadStatus.Paused;
            await _pauseTokenSource.PauseAsync(_stateTokenSource.Token);
        }

        public async Task ResumeAsync() {
            Status = DownloadStatus.Running;
            await _pauseTokenSource.ResumeAsync(_stateTokenSource.Token);
        }

        public async Task StartDownloadAsync(string url, IPackageDownloadProvider packageDownloadProvider) {
            Status      = DownloadStatus.Created;
            try {
                await _singleInstanceSemaphore.WaitAsync();
                await packageDownloadProvider.StartDownload(url, _pauseTokenSource, _stateTokenSource);
            }
            catch (System.Exception) {
                throw;
            }
            finally {
                _singleInstanceSemaphore.Release();
                await Task.Yield();
            }

        }
    }
}