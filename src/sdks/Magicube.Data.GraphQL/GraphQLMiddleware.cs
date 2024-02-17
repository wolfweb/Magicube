using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;

namespace Magicube.Data.GraphQL {
    public class GraphQLMiddleware {
        private readonly RequestDelegate _next;
        private readonly GraphQLOption _settings;
        private readonly IDocumentExecuter _executer;

        public GraphQLMiddleware(
            RequestDelegate next,
            IOptions<GraphQLOption> options,
            IDocumentExecuter executer) {
            _next     = next;
            _settings = options.Value;
            _executer = executer;
        }

        private bool IsGraphQLRequest(HttpContext context) {
            return context.Request.Path.StartsWithNormalizedSegments(_settings.Path, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class PathStringExtensions {
        public static bool StartsWithNormalizedSegments(this PathString path, PathString other, StringComparison comparisonType) {
            if (other.HasValue && other.Value.EndsWith('/')) {
                return path.StartsWithSegments(other.Value.Substring(0, other.Value.Length - 1), comparisonType);
            }

            return path.StartsWithSegments(other, comparisonType);
        }
    }
}
