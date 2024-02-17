using Magicube.Core;
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
using static OpenIddict.Abstractions.OpenIddictConstants;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Magicube.OpenIdCore.Stores {
    public class OpenIdTokenStore : IOpenIddictTokenStore<OpenIdToken> {
        private readonly IRepository<OpenIdToken, string> _repository;
        public OpenIdTokenStore(IRepository<OpenIdToken, string> repository) {
            _repository = repository;
        }
        public async ValueTask<long> CountAsync(CancellationToken cancellationToken) {
            return await _repository.All.LongCountAsync(cancellationToken);
        }

        public async ValueTask<long> CountAsync<TResult>(Func<IQueryable<OpenIdToken>, IQueryable<TResult>> query, CancellationToken cancellationToken) {
            return await query(_repository.All).LongCountAsync(cancellationToken);
        }

        public async ValueTask CreateAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await _repository.InsertAsync(token, cancellationToken);
        }

        public async ValueTask DeleteAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            _repository.Delete(token);
            await Task.CompletedTask;
        }

        public IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Include(x=>x.Authorization).Where(x => x.Subject == subject && x.Application.Id == client).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (status.IsNullOrEmpty()) {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Include(x=>x.Authorization).Where(x => x.Subject == subject && x.Application.Id == client && x.Status == status).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdToken> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (client.IsNullOrEmpty()) {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (status.IsNullOrEmpty()) {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            if (type.IsNullOrEmpty()) {
                throw new ArgumentException("The type cannot be null or empty.", nameof(type));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x=>x.Application).Include(x=>x.Authorization).Where(x => x.Subject == subject && x.Application.Id == client && x.Status == status && x.Type == type).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdToken> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x => x.Application).Include(x => x.Authorization).Where(x => x.Application.Id == identifier).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<OpenIdToken> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x => x.Application).Include(x => x.Authorization).Where(x => x.Authorization.Id == identifier).AsAsyncEnumerable();
        }

        public async ValueTask<OpenIdToken> FindByIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _repository.All.Include(x => x.Application).Include(x => x.Authorization).FirstOrDefaultAsync(x => x.Id == identifier, cancellationToken);
        }

        public async ValueTask<OpenIdToken> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken) {
            if (identifier.IsNullOrEmpty()) {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return await _repository.All.Include(x => x.Application).Include(x => x.Authorization).FirstOrDefaultAsync(x => x.ReferenceId == identifier, cancellationToken);
        }

        public IAsyncEnumerable<OpenIdToken> FindBySubjectAsync(string subject, CancellationToken cancellationToken) {
            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            cancellationToken.ThrowIfCancellationRequested();

            return _repository.All.Include(x => x.Application).Include(x => x.Authorization).Where(x => x.Subject == subject).AsAsyncEnumerable();
        }

        public ValueTask<string> GetApplicationIdAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Application.Id?.ToString(CultureInfo.InvariantCulture));
        }

        public async ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<OpenIdToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            return await query(_repository.All, state).SingleOrDefaultAsync(cancellationToken);
        }

        public ValueTask<string> GetAuthorizationIdAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Authorization.Id);
        }

        public ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<DateTimeOffset?>(token.CreationDate);
        }

        public ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<DateTimeOffset?>(token.ExpirationDate);
        }

        public ValueTask<string> GetIdAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Id);
        }

        public ValueTask<string> GetPayloadAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Payload);
        }

        public ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Properties == null) {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(token.Properties.ToString()));
        }

        public ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<DateTimeOffset?>(token.RedemptionDate);
        }

        public ValueTask<string> GetReferenceIdAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.ReferenceId);
        }

        public ValueTask<string> GetStatusAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Status);
        }

        public ValueTask<string> GetSubjectAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Subject);
        }

        public ValueTask<string> GetTypeAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Type);
        }

        public ValueTask<OpenIdToken> InstantiateAsync(CancellationToken cancellationToken) {
            return new ValueTask<OpenIdToken>(new OpenIdToken ()); 
        }

        public IAsyncEnumerable<OpenIdToken> ListAsync(int? count, int? offset, CancellationToken cancellationToken) {
            var query = _repository.All;

            if (offset.HasValue) {
                query = query.Skip(offset.Value);
            }

            if (count.HasValue) {
                query = query.Take(count.Value);
            }

            return query.AsAsyncEnumerable();
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<OpenIdToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            return query(_repository.All, state).AsAsyncEnumerable();
        }

        public async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken) {
            List<Exception> exceptions = null;
            var result = 0L;
            var date = threshold.UtcDateTime;

            for (var offset = 0; offset < 100_000; offset = offset + 1_000) {
                cancellationToken.ThrowIfCancellationRequested();
                try {

                    var count = await
                        (from token in _repository.All
                         where token.CreationDate < date
                         where (token.Status != Statuses.Inactive && token.Status != Statuses.Valid) ||
                               (token.Authorization != null && token.Authorization.Status != Statuses.Valid) ||
                                token.ExpirationDate < DateTime.UtcNow
                         orderby token.Id
                         select token).Take(1_000).ExecuteDeleteAsync(cancellationToken);

                    if (count is 0) {
                        break;
                    }

                    result += count;
                }catch(Exception exception) {
                    exceptions ??= new List<Exception>(capacity: 1);
                    exceptions.Add(exception);
                }
            }

            if (exceptions is not null) {
                throw new AggregateException("Error:", exceptions);
            }

            return result;
        }

        public async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken) {
            return await(from token in _repository.All
                where token.Authorization!.Id!.Equals(identifier)
                select token).ExecuteUpdateAsync(entity => entity.SetProperty(token => token.Status, Statuses.Revoked), cancellationToken);
        }

        public ValueTask SetApplicationIdAsync(OpenIdToken token, string identifier, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (identifier.IsNullOrEmpty()) {
                token.Application = null;
            } else {
                token.Application = new OpenIdApplication { Id = identifier };
            }

            return default;
        }

        public ValueTask SetAuthorizationIdAsync(OpenIdToken token, string identifier, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (identifier.IsNullOrEmpty()) {
                token.Authorization = null;
            } else {
                token.Authorization = new OpenIdAuthorization { Id = identifier };
            }

            return default;
        }

        public ValueTask SetCreationDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            token.CreationDate = date?.UtcDateTime;

            return default;
        }

        public ValueTask SetExpirationDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            token.ExpirationDate = date?.UtcDateTime;

            return default;
        }

        public ValueTask SetPayloadAsync(OpenIdToken token, string payload, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            token.Payload = payload;

            return default;
        }

        public ValueTask SetPropertiesAsync(OpenIdToken token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (properties == null || properties.IsEmpty) {
                token.Properties = null;

                return default;
            }

            token.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public ValueTask SetRedemptionDateAsync(OpenIdToken token, DateTimeOffset? date, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            token.RedemptionDate = date?.UtcDateTime;

            return default;
        }

        public ValueTask SetReferenceIdAsync(OpenIdToken token, string identifier, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            token.ReferenceId = identifier;

            return default;
        }

        public ValueTask SetStatusAsync(OpenIdToken token, string status, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (status.IsNullOrEmpty()) {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            token.Status = status;

            return default;
        }

        public ValueTask SetSubjectAsync(OpenIdToken token, string subject, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (subject.IsNullOrEmpty()) {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            token.Subject = subject;

            return default;
        }

        public ValueTask SetTypeAsync(OpenIdToken token, string type, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            if (type.IsNullOrEmpty()) {
                throw new ArgumentException("The token type cannot be null or empty.", nameof(type));
            }

            token.Type = type;

            return default;
        }

        public async ValueTask UpdateAsync(OpenIdToken token, CancellationToken cancellationToken) {
            if (token == null) {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            _repository.Update(token);
            await Task.CompletedTask;
        }
    }
}
