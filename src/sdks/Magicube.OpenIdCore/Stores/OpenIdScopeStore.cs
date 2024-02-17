using Magicube.Core;
using Magicube.Core.Convertion;
using Magicube.Data.Abstractions;
using Magicube.OpenIdCore.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.OpenIdCore.Stores {
    public class OpenIdScopeStore : IOpenIddictScopeStore<OpenIdScope> {
        private readonly IRepository<OpenIdScope, string> _repository;
        private readonly IRepository<OpenIdScopeName, long> _namesRepository;
        private readonly IRepository<OpenidScopeResource, long> _resourceRepository; 
        public OpenIdScopeStore(
            IRepository<OpenIdScope, string> repository, 
            IRepository<OpenIdScopeName, long> namesRepository, 
            IRepository<OpenidScopeResource, long> resourceRepository) {
            _repository           = repository;
            _namesRepository      = namesRepository;
            _resourceRepository   = resourceRepository;
        }
        public async ValueTask<long> CountAsync(CancellationToken cancellationToken) {
            return await _repository.All.LongCountAsync(cancellationToken);
        }

        public async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdScope>, IQueryable<TResult>> query, CancellationToken cancellationToken) {
            return await query(_repository.All).LongCountAsync(cancellationToken);
        }

        public async ValueTask CreateAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _repository.InsertAsync(scope, cancellationToken);
        }

        public async ValueTask DeleteAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            cancellationToken.ThrowIfCancellationRequested();

            _repository.Delete(scope);
            await Task.CompletedTask;
        }

        public async ValueTask<OpenIdScope> FindByIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await _repository.All.Include(x=>x.Resources).Include(x=>x.DisplayNames).SingleOrDefaultAsync(x => x.Id == identifier, cancellationToken);
        }

        public async ValueTask<OpenIdScope> FindByNameAsync(string name, CancellationToken cancellationToken) {
            if (name.IsNullOrEmpty()) {
                throw new ArgumentException("The scope name cannot be null or empty.", nameof(name));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _repository.All.Include(x => x.Resources).Include(x => x.DisplayNames).SingleOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public IAsyncEnumerable<OpenIdScope> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (names.Any(name => name.IsNullOrEmpty())) {
                throw new ArgumentException("Scope names cannot be null or empty.", nameof(names));
            }

            var dbSet = _namesRepository.All as DbSet<OpenIdScopeName>;

            return dbSet.Where(x => names.Contains(x.Name)).Include(x => x.OpenIdScope).Select(x => x.OpenIdScope).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdScope> FindByResourceAsync(string resource, CancellationToken cancellationToken) {
            if (resource.IsNullOrEmpty()) {
                throw new ArgumentException("The resource cannot be null or empty.", nameof(resource));
            }

            cancellationToken.ThrowIfCancellationRequested();
            var dbSet = _resourceRepository.All as DbSet<OpenidScopeResource>;
            return dbSet.Where(x => x.Resource == resource).Include(x => x.OpenIdScope).Select(x => x.OpenIdScope).AsAsyncEnumerable();
        }

        public async ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIdScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return await query(_repository.All, state).SingleOrDefaultAsync(cancellationToken);
        }

        public ValueTask<string> GetDescriptionAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.Description);
        }

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.Descriptions == null) {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.Descriptions.JsonToObject<KeyValuePair<string, string>[]>().Select(x => new KeyValuePair<CultureInfo, string>(CultureInfo.GetCultureInfo(x.Key), x.Value)).ToImmutableDictionary());
        }

        public ValueTask<string> GetDisplayNameAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.DisplayName);
        }

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.DisplayNames == null) {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.DisplayNames.Select(x => new KeyValuePair<CultureInfo, string>(CultureInfo.CurrentCulture, x.Name)).ToImmutableDictionary());
        }

        public ValueTask<string> GetIdAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.Id);
        }

        public ValueTask<string> GetNameAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.Name);
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.Properties == null) {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(scope.Properties.ToString()));
        }

        public ValueTask<ImmutableArray<string>> GetResourcesAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<ImmutableArray<string>>(scope.Resources.Select(x=>x.Resource).ToImmutableArray());
        }

        public ValueTask<OpenIdScope> InstantiateAsync(CancellationToken cancellationToken) {
            return new ValueTask<OpenIdScope>(new OpenIdScope ());
        }

        public IAsyncEnumerable<OpenIdScope> ListAsync(int? count, int? offset, CancellationToken cancellationToken) {
            var query = _repository.All;

            if (offset.HasValue) {
                query = query.Skip(offset.Value);
            }

            if (count.HasValue) {
                query = query.Take(count.Value);
            }

            return query.Include(x => x.Resources).Include(x => x.DisplayNames).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIdScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return query(_repository.All, state).AsAsyncEnumerable();
        }

        public ValueTask SetDescriptionAsync(OpenIdScope scope, string description, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Description = description;

            return default;
        }

        public ValueTask SetDescriptionsAsync(OpenIdScope scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Descriptions = Json.Stringify(descriptions.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            return default;
        }

        public ValueTask SetDisplayNameAsync(OpenIdScope scope, string name, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.DisplayName = name;

            return default;
        }

        public ValueTask SetDisplayNamesAsync(OpenIdScope scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.DisplayNames = names.Select(x=>new OpenIdScopeName { OpenIdScope = scope, Name = x.Value }).ToImmutableArray();

            return default;
        }

        public ValueTask SetNameAsync(OpenIdScope scope, string name, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Name = name;

            return default;
        }

        public ValueTask SetPropertiesAsync(OpenIdScope scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            if (properties == null || properties.IsEmpty) {
                scope.Properties = null;

                return default;
            }

            scope.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public ValueTask SetResourcesAsync(OpenIdScope scope, ImmutableArray<string> resources, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Resources = resources.Select(x => new OpenidScopeResource { OpenIdScope = scope, Resource = x }).ToImmutableArray();

            return default;
        }

        public async ValueTask UpdateAsync(OpenIdScope scope, CancellationToken cancellationToken) {
            if (scope == null) {
                throw new ArgumentNullException(nameof(scope));
            }

            cancellationToken.ThrowIfCancellationRequested();
            _repository.Update(scope);

            await Task.CompletedTask;
        }
    }
}
