using Neo4jClient;

namespace Magicube.Data.Neo4j {
    public interface INeo4jDbContext {
        GraphClient Client { get; }
    }
}
