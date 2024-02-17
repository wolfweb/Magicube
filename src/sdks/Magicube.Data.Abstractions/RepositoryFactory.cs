using System;
using Microsoft.Extensions.DependencyInjection;
using Magicube.Core;
using System.Linq;

namespace Magicube.Data.Abstractions {
    public class RepositoryFactory {
        private readonly IServiceProvider _serviceProvider;
        public RepositoryFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public IRepository<TEntity,TKey> Retrive<TEntity, TKey>(string identity) where TEntity : class, IEntity<TKey> {
            var dbContext = _serviceProvider.GetService<IDbContext>(identity);
            var services = _serviceProvider.GetService<IServiceCollection>();
            var serviceItem = services.FirstOrDefault(x => x.ServiceType == typeof(IRepository<,>));
            var repositoryType = serviceItem.ImplementationType.MakeGenericType(typeof(TEntity), typeof(TKey));
            return (IRepository<TEntity, TKey>)ActivatorUtilities.CreateInstance(_serviceProvider, repositoryType, dbContext);
        }
    }
}
