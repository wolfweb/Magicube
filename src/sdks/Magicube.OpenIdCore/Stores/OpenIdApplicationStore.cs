using Magicube.Core;
using Magicube.Core.Convertion;
using Magicube.Data.Abstractions;
using Magicube.OpenIdCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    public class OpenIdApplicationStore : IOpenIddictApplicationStore<OpenIdApplication> {
        private readonly IRepository<OpenIdToken, string> _tokenRepository;
        private readonly IRepository<OpenIdApplication, string> _repository;
        private readonly IRepository<OpenIdAppLogoutUri,long> _logoutUriRepository;
        private readonly IRepository<OpenIdAppRedirectUri, long> _redirectUriRepository;
        private readonly IRepository<OpenIdAuthorization, string> _authorizationRepository;

        public OpenIdApplicationStore(
            IRepository<OpenIdToken, string> tokenRepository,
            IRepository<OpenIdApplication, string> repository, 
            IRepository<OpenIdAppLogoutUri, long> logoutUriRepository, 
            IRepository<OpenIdAppRedirectUri, long> redirectUriRepository,
            IRepository<OpenIdAuthorization, string> authorizationRepository) {
            _repository               = repository;
            _tokenRepository          = tokenRepository;
             _logoutUriRepository     = logoutUriRepository;
            _redirectUriRepository    = redirectUriRepository;
            _authorizationRepository  = authorizationRepository;
        }

        public async ValueTask<long> CountAsync(CancellationToken cancellationToken) {
            return await _repository.All.LongCountAsync(cancellationToken);
        }

        public async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken) {
            return await query(_repository.All).LongCountAsync(cancellationToken);
        }

        public async ValueTask CreateAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            await _repository.InsertAsync(application, cancellationToken);
        }

        public async ValueTask DeleteAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var tokens = _tokenRepository.All.Include(x => x.Application).Where(x => x.Application == application).AsAsyncEnumerable();
            var authorizations = _authorizationRepository.All.Include(x => x.Application).Where(x => x.Application == application).AsAsyncEnumerable();

            await foreach (var token in tokens) {
                 _tokenRepository.Delete(token);
            }

            await foreach(var authorization in authorizations) {
                _authorizationRepository.Delete(authorization);
            }

            _repository.Delete(application);
            await Task.CompletedTask;
        }

        public async ValueTask<OpenIdApplication> FindByClientIdAsync(string identifier, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(identifier.IsNullOrEmpty()) throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));

            return await _repository.GetAsync(x => x.ClientId == identifier);
        }

        public async ValueTask<OpenIdApplication> FindByIdAsync(string identifier, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (identifier.IsNullOrEmpty()) throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));

            return await _repository.GetAsync(x => x.Id == identifier);
        }

        public IAsyncEnumerable<OpenIdApplication> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (address.IsNullOrEmpty()) throw new ArgumentException("The address cannot be null or empty.", nameof(address));

            var dbSet = _logoutUriRepository.All as DbSet<OpenIdAppLogoutUri>;

            return dbSet.Where(x => x.LogoutRedirectUri == address).Include(x => x.Application).Select(x => x.Application).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdApplication> FindByRedirectUriAsync(string address, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if(address.IsNullOrEmpty()) throw new ArgumentException("The address cannot be null or empty.", nameof(address));

            var dbSet = _redirectUriRepository.All as DbSet<OpenIdAppRedirectUri>;
            return dbSet.Where(x => x.RedirectUri == address).Include(x => x.Application).Select(x => x.Application).AsAsyncEnumerable();
        }

        public ValueTask<string> GetApplicationTypeAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            return new ValueTask<string>(application?.Type);
        }

        public async ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIdApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return await query(_repository.All, state).FirstOrDefaultAsync(cancellationToken);
        }

        public ValueTask<string> GetClientIdAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.ClientId);
        }

        public ValueTask<string> GetClientSecretAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.ClientSecret);
        }

        public ValueTask<string> GetClientTypeAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.Type);
        }

        public ValueTask<string> GetConsentTypeAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.ConsentType);
        }

        public ValueTask<string> GetDisplayNameAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.DisplayName);
        }

        public ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.DisplayNames.IsNullOrEmpty()) {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(application.DisplayNames.JsonToObject<KeyValuePair<string, string>[]>().Select(x => new KeyValuePair<CultureInfo, string>(CultureInfo.GetCultureInfo(x.Key), x.Value)).ToImmutableDictionary());
        }

        public ValueTask<string> GetIdAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.Id);
        }

        public ValueTask<JsonWebKeySet> GetJsonWebKeySetAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (application.JsonWebKeySet.IsNullOrEmpty()) {
                return new(result: null);
            }

            var webKey = JsonWebKeySet.Create(application.JsonWebKeySet);
            return new(webKey);
        }

        public ValueTask<ImmutableArray<string>> GetPermissionsAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<ImmutableArray<string>>(application.Permissions);
        }

        public ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<ImmutableArray<string>>(application.PostLogoutRedirectUris.Select(x => x.LogoutRedirectUri).ToImmutableArray());
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if(application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Properties == null) {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(application.Properties.ToString()));
        }

        public ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<ImmutableArray<string>>(application.RedirectUris.Select(x=>x.RedirectUri).ToImmutableArray());
        }

        public ValueTask<ImmutableArray<string>> GetRequirementsAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<ImmutableArray<string>>(application.Requirements);
        }

        public ValueTask<ImmutableDictionary<string, string>> GetSettingsAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            if (application.Settings.IsNullOrEmpty()) return new(ImmutableDictionary.Create<string, string>());

            var dict = application.Settings.JsonToObject<Dictionary<string, string>>(); 
            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var item in dict) {
                builder.Add(item.Key, item.Value);
            }

            return new(builder.ToImmutable());
        }

        public ValueTask<OpenIdApplication> InstantiateAsync(CancellationToken cancellationToken) {
            return new ValueTask<OpenIdApplication>(new OpenIdApplication());
        }

        public IAsyncEnumerable<OpenIdApplication> ListAsync(int? count, int? offset, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            var query = _repository.All;

            if (offset.HasValue) {
                query = query.Skip(offset.Value);
            }

            if (count.HasValue) {
                query = query.Take(count.Value);
            }

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIdApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return query(_repository.All, state).AsAsyncEnumerable();
        }

        public ValueTask SetApplicationTypeAsync(OpenIdApplication application, string type, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            application.Type = type;
            return ValueTask.CompletedTask;
        }

        public ValueTask SetClientIdAsync(OpenIdApplication application, string identifier, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.ClientId = identifier;

            return default;
        }

        public ValueTask SetClientSecretAsync(OpenIdApplication application, string secret, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.ClientSecret = secret;

            return default;
        }

        public ValueTask SetClientTypeAsync(OpenIdApplication application, string type, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.Type = type;

            return default;
        }

        public ValueTask SetConsentTypeAsync(OpenIdApplication application, string type, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.ConsentType = type;

            return default;
        }

        public ValueTask SetDisplayNameAsync(OpenIdApplication application, string name, CancellationToken cancellationToken) {
            if (application is null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.DisplayName = name;

            return default;
        }

        public ValueTask SetDisplayNamesAsync(OpenIdApplication application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.DisplayNames = Json.Stringify(names.Select(x => new KeyValuePair<string, string>(x.Key.Name, x.Value)));

            return default;
        }

        public ValueTask SetJsonWebKeySetAsync(OpenIdApplication application, Microsoft.IdentityModel.Tokens.JsonWebKeySet set, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            application.JsonWebKeySet = set is not null ? Json.Stringify(set) : null;

            return default;
        }

        public ValueTask SetPermissionsAsync(OpenIdApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.Permissions = permissions;

            return default;
        }

        public ValueTask SetPostLogoutRedirectUrisAsync(OpenIdApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.PostLogoutRedirectUris = addresses.Select(x=> new OpenIdAppLogoutUri {
               Application       = application,
               LogoutRedirectUri = x
            }).ToImmutableArray();

            return default;
        }

        public ValueTask SetPropertiesAsync(OpenIdApplication application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            if (properties == null || properties.IsEmpty) {
                application.Properties = null;

                return default;
            }

            application.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public ValueTask SetRedirectUrisAsync(OpenIdApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.RedirectUris = addresses.Select(x=>new OpenIdAppRedirectUri { 
                Application  = application,
                RedirectUri  = x
            }).ToImmutableArray();

            return default;
        }

        public ValueTask SetRequirementsAsync(OpenIdApplication application, ImmutableArray<string> requirements, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            application.Requirements = requirements;

            return default;
        }

        public ValueTask SetSettingsAsync(OpenIdApplication application, ImmutableDictionary<string, string> settings, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();

            application.Settings = settings is not null ? Json.Stringify(settings) : null;
            
            return default;
        }

        public async ValueTask UpdateAsync(OpenIdApplication application, CancellationToken cancellationToken) {
            if (application == null) {
                throw new ArgumentNullException(nameof(application));
            }

            cancellationToken.ThrowIfCancellationRequested();

            application.ConcurrencyToken = Guid.NewGuid().ToString();

            await _repository.UpdateAsync(application, cancellationToken);
        }
    }
}
