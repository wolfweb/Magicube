using Microsoft.AspNetCore.Http;

namespace Magicube.Data.GraphQL {
    public class GraphQLOption {
        public PathString Path { get; set; } = "/api/graphql";
    }
}
