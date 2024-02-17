using Magicube.Web;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Magicube.Identity {
    public class OnLineUserWrapper<T> {
        public T     Identity    { get; }
        public IUser User        { get; set; }

        public OnLineUserWrapper(T identity) {
            Identity = identity;
        }
    }

    public interface IOnlineService<T> {
        void Add(OnLineUserWrapper<T> user);
        void Touch(OnLineUserWrapper<T> user);
        void Remove(OnLineUserWrapper<T> user);
    }

    public class OnlineService<T> : IOnlineService<T> {
        private readonly Timer _timer;
        private readonly OnlineOptions _options;
        private readonly ReaderWriterLockSlim lockSlim = new ();             
        private readonly ConcurrentDictionary<T, OnlineUser> _onlineUsers = new();
        public OnlineService(IOptions<OnlineOptions> options) {
            _options = options.Value;
            _timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        public void Add(OnLineUserWrapper<T> user) {
            lockSlim.EnterWriteLock();
            try {
                _onlineUsers.TryAdd(user.Identity, new OnlineUser(user.User) { LastActived = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });
            }
            finally {
                lockSlim.ExitWriteLock();
            }
        }

        public void Touch(OnLineUserWrapper<T> user) {
            lockSlim.EnterReadLock();

            try {
                if (_onlineUsers.ContainsKey(user.Identity)) {
                    _onlineUsers[user.Identity].LastActived = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
            }
            finally{
                lockSlim.ExitReadLock();
            }
        }

        public void Remove(OnLineUserWrapper<T> user) {
            lockSlim.EnterWriteLock();
            try {
                _onlineUsers.TryRemove(user.Identity, out _);
            }
            finally {
                lockSlim.ExitWriteLock();
            }
        }

        private void TimerCallback(object state) {
            var container = new List<T>();
            foreach(var item in _onlineUsers) {
                var seconds =  DateTimeOffset.UtcNow.ToUnixTimeSeconds() - item.Value.LastActived;
                if (seconds > _options.Threshold) {
                    container.Add(item.Key);
                }
            }

            lockSlim.EnterWriteLock();
            try {
                container.ForEach(x => _onlineUsers.TryRemove(x, out _));
            }
            finally {
                lockSlim.ExitWriteLock();
            }
        }

        sealed record OnlineUser(IUser User) {
            public long LastActived { get; set; }
        }
    }
}
