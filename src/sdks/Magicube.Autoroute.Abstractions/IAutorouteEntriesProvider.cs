using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Autoroute.Abstractions {
    public interface IAutorouteEntriesProvider {
        Task<bool> TryAdd(AutorouteEntry entry);
        Task<bool> TryRemove(AutorouteEntry entry);
        Task<(bool, AutorouteEntry)> TryGetAutorouteEntryAsync(string path);
    }

    public class AuthrouteEntriesProvider : IAutorouteEntriesProvider {
        private int _initialized;
        private ImmutableDictionary<string, AutorouteEntry> _paths = ImmutableDictionary<string, AutorouteEntry>.Empty;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public AuthrouteEntriesProvider(IServiceScopeFactory serviceScopeFactory) {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<(bool,AutorouteEntry)> TryGetAutorouteEntryAsync(string path) {
            await EnsureInitializedAsync();

            if (_paths.TryGetValue(path.TrimEnd('/'), out var entry)) {
                return (true, entry);
            }

            return (false, entry);
        }

        public async Task<bool> TryAdd(AutorouteEntry entry) {
            entry.NotNull();

            await _semaphore.WaitAsync();
            try {
                if (!entry.Path.IsNullOrEmpty())
                    _paths.SetItem(entry.Path, entry);

                return true;
            } catch {
                return false;
            } finally {
                _semaphore.Release();
            }
        }

        public async Task<bool> TryRemove(AutorouteEntry entry) {
            entry.NotNull();

            await _semaphore.WaitAsync();
            try {
                if (!entry.Path.IsNullOrEmpty())
                    _paths.Remove(entry.Path);
                return true;
            } catch {
                return false;
            } finally {
                _semaphore.Release();
            }
        }

        private async Task EnsureInitializedAsync() {
            if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0) {
                using (var scope = _serviceScopeFactory.CreateScope()) {
                    var repository = scope.ServiceProvider.GetService<IRepository<AutorouteEntry, long>>();
                    var datas = await repository.QueryAsync(x => x.Path != null && x.Status == EntityStatus.Actived);
                    foreach (var entry in datas) {
                        _paths.SetItem(entry.Path, entry);
                    }
                }
            }
        }
    }
}
