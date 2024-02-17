using Magicube.Data.Abstractions;
using MongoDB.Driver;

namespace Magicube.Data.Mongodb {
    public interface IMongoDbContext : IDbContext {
        IMongoDatabase Database { get; }
    }
}
