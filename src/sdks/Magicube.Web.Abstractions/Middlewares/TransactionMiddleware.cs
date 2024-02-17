using Magicube.Data.Abstractions.EfDbContext;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Magicube.Web.Middlewares {
    public class TransactionMiddleware {
        private readonly RequestDelegate _next;
        public TransactionMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork transactionScope) {
            transactionScope.BeginTransaction();
            try {
                await _next(context);
            } catch (AggregateException) {
                transactionScope.Rollback();
                throw;
            } catch (Exception) {
                transactionScope.Rollback();
                throw;
            } finally {
                transactionScope.Dispose();
            }
        }
    }
}
