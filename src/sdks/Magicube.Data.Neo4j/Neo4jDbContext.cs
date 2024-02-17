using Microsoft.Extensions.Options;
using Neo4jClient;
using System;

namespace Magicube.Data.Neo4j {
    public class Neo4jDbContext : INeo4jDbContext {
        private readonly Neo4jOptions _neo4JOptions;
        public GraphClient Client { get; }

        public Neo4jDbContext(IOptions<Neo4jOptions> options) {
            _neo4JOptions = options.Value;
            Client = new GraphClient(new Uri(_neo4JOptions.Uri), _neo4JOptions.User, _neo4JOptions.Pwd);
            Client.ConnectAsync();
        }
    }
}
