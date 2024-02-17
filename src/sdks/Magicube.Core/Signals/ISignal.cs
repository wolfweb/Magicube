using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Threading;

namespace Magicube.Core.Signals {
    public interface ISignal {
        (bool,IChangeToken) GetToken(string key);

        void SignalToken(string key);
    }

    public class Signal : ISignal {
        private readonly ConcurrentDictionary<string, ChangeTokenInfo> _changeTokens;

        public Signal() {
            _changeTokens = new ConcurrentDictionary<string, ChangeTokenInfo>();
        }

        public (bool, IChangeToken) GetToken(string key) {
            if (_changeTokens.ContainsKey(key)) {
                return (true, _changeTokens[key].ChangeToken);
            }

            lock (this) {
                var cancellationTokenSource = new CancellationTokenSource();
                var changeToken = new CancellationChangeToken(cancellationTokenSource.Token);
                var tokenInfo = new ChangeTokenInfo(changeToken, cancellationTokenSource);

                _changeTokens.TryAdd(key, tokenInfo);

                return (false, tokenInfo.ChangeToken);
            }
        }

        public void SignalToken(string key) {
            if (_changeTokens.TryRemove(key, out ChangeTokenInfo changeTokenInfo)) {
                changeTokenInfo.TokenSource.Cancel();
            }
        }

        private struct ChangeTokenInfo {
            public ChangeTokenInfo(IChangeToken changeToken, CancellationTokenSource tokenSource) {
                ChangeToken = changeToken;
                TokenSource = tokenSource;
            }
                                           
            public IChangeToken            ChangeToken { get; }

            public CancellationTokenSource TokenSource { get; }
        }
    }
}
