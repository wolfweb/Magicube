using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.OpenIdCore.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Magicube.OpenIdCore.Stores {
    public class OpenIdAuthorizationStore : IOpenIddictAuthorizationStore<OpenIdAuthorization> {
        private readonly IRepository<OpenIdToken, string> _tokenRepository;
        private readonly IRepository<OpenIdAuthorization,string> _repository;
        public OpenIdAuthorizationStore(
            IRepository<OpenIdToken, string> tokenRepository,
            IRepository<OpenIdAuthorization, string> repository
            ) {
            _repository      = repository;
            _tokenRepository = tokenRepository;
        }

        public async ValueTask<long> CountAsync(CancellationToken cancellationToken) {
            return await _repository.All.LongCountAsync(cancellationToken);
        }

        public async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken) {
            return await query(_repository.All).LongCountAsync(cancellationToken);
        }

        public async ValueTask CreateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _repository.InsertAsync(authorization, cancellationToken);
        }

        public async ValueTask DeleteAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var tokens = _tokenRepository.All.Include(x => x.Authorization).Where(x => x.Authorization == authorization).AsAsyncEnumerable();

            await foreach(var token in tokens) {
                _tokenRepository.Delete(token);
            }

            _repository.Delete(authorization);
            await Task.CompletedTask;
        }

        public IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Where(x => x.Subject == subject && x.Application.Id == client).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (status.IsNullOrEmpty()) {
                throw new ArgumentException("The status cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Where(x => x.Subject == subject && x.Application.Id == client && x.Status == status).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (status.IsNullOrEmpty()) {
                throw new ArgumentException("The status cannot be null or empty.", nameof(client));
            }

            if (type.IsNullOrEmpty()) {
                throw new ArgumentException("The type cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Where(x => x.Subject == subject && x.Application.Id == client && x.Status == status && x.Type == type).AsAsyncEnumerable();
        }

        public async IAsyncEnumerable<OpenIdAuthorization> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes, [EnumeratorCancellation] CancellationToken cancellationToken) {
            await foreach (var authorization in FindAsync(subject, client, status, type, cancellationToken)) {
                if (new HashSet<string>(await GetScopesAsync(authorization, cancellationToken), StringComparer.Ordinal).IsSupersetOf(scopes)) {
                    yield return authorization;
                }
            }
        }

        public IAsyncEnumerable<OpenIdAuthorization> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Where(x => x.Application.Id == identifier).AsAsyncEnumerable();
        }

        public async ValueTask<OpenIdAuthorization> FindByIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await _repository.All.Include(x=>x.Application).Where(x => x.Id == identifier).FirstOrDefaultAsync(cancellationToken);
        }

        public IAsyncEnumerable<OpenIdAuthorization> FindBySubjectAsync(string subject, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Where(x => x.Subject == subject).AsAsyncEnumerable();
        }

        public ValueTask<string> GetApplicationIdAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if(authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Application.Id);
        }

        public async ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIdAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return await query(_repository.All, state).SingleOrDefaultAsync(cancellationToken);
        }

        public ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<DateTimeOffset?>(authorization.CreationDate);
        }

        public ValueTask<string> GetIdAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Id);
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.Properties == null) {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(authorization.Properties.ToString()));
        }

        public ValueTask<ImmutableArray<string>> GetScopesAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<ImmutableArray<string>>(authorization.Scopes);
        }

        public ValueTask<string> GetStatusAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Status);
        }

        public ValueTask<string> GetSubjectAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Subject);
        }

        public ValueTask<string> GetTypeAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Type);
        }

        public ValueTask<OpenIdAuthorization> InstantiateAsync(CancellationToken cancellationToken) {
            return new ValueTask<OpenIdAuthorization>(new OpenIdAuthorization ());
        }

        public IAsyncEnumerable<OpenIdAuthorization> ListAsync(int? count, int? offset, CancellationToken cancellationToken) {
            var query = _repository.All;

            if (offset.HasValue) {
                query = query.Skip(offset.Value);
            }

            if (count.HasValue) {
                query = query.Take(count.Value);
            }

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIdAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return query(_repository.All, state).AsAsyncEnumerable();
        }

        public async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken) {
            var result = 0L;
            var date = threshold.UtcDateTime;

            List<Exception> exceptions = null;
            for (var index = 0; index < 1_000; index++) {
                try {
                    var count = await
                            (from authorization in _repository.All
                             where authorization.CreationDate < date
                             where authorization.Status != Statuses.Valid ||
                                  (authorization.Type == AuthorizationTypes.AdHoc && !authorization.Tokens.Any())
                             orderby authorization.Id
                             select authorization).Take(1_000).ExecuteDeleteAsync(cancellationToken);

                    if (count is 0) {
                        break;
                    }
                    result += count;
                }
                catch (Exception exception) {
                    exceptions ??= new List<Exception>(capacity: 1);
                    exceptions.Add(exception);
                }
            }

            if (exceptions is not null) {
                throw new AggregateException("Error:", exceptions);
            }

            return result;
        }

        public ValueTask SetApplicationIdAsync(OpenIdAuthorization authorization, string identifier, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (identifier.IsNullOrEmpty()) {
                authorization.Application = null;
            } else {
                authorization.Application = new OpenIdApplication { 
                   Id = identifier
                };
            }

            return default;
        }

        public ValueTask SetCreationDateAsync(OpenIdAuthorization authorization, DateTimeOffset? date, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.CreationDate = date;

            return default;
        }

        public ValueTask SetPropertiesAsync(OpenIdAuthorization authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (properties == null || properties.IsEmpty) {
                authorization.Properties = null;

                return default;
            }

            authorization.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public ValueTask SetScopesAsync(OpenIdAuthorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Scopes = scopes;

            return default;
        }

        public ValueTask SetStatusAsync(OpenIdAuthorization authorization, string status, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Status = status;

            return default;
        }

        public ValueTask SetSubjectAsync(OpenIdAuthorization authorization, string subject, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Subject = subject;

            return default;
        }

        public ValueTask SetTypeAsync(OpenIdAuthorization authorization, string type, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Type = type;

            return default;
        }

        public async ValueTask UpdateAsync(OpenIdAuthorization authorization, CancellationToken cancellationToken) {
            if (authorization == null) {
                throw new ArgumentNullException(nameof(authorization));
            }
            cancellationToken.ThrowIfCancellationRequested();

            authorization.ConcurrencyToken = Guid.NewGuid().ToString();

            _repository.Update(authorization);
            await Task.CompletedTask;
        }
    }
}
