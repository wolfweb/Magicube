using Magicube.Data.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Magicube.Data.Mongodb {
    public class MongoProvider {
        public MongoProvider(IOptionsMonitor<DatabaseOptions> options) {
            var mongoUrl = new MongoUrl(options.CurrentValue.Value);
            var client   = new MongoClient(mongoUrl);
            Database     = client.GetDatabase(mongoUrl.DatabaseName);
        }

        public IMongoDatabase Database { get; }
    }
}
