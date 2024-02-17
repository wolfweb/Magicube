using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;

namespace Magicube.Data.GraphQL {
    public class GraphQLContext : Dictionary<string, object> {
        public ClaimsPrincipal  User                 { get; set; }
        public HttpContext      HttpContext          { get; set; }
        public IServiceProvider ServiceProvider      { get; set; }
        public SemaphoreSlim    ExecutionContextLock { get; } = new SemaphoreSlim(1, 1);
    }
}
