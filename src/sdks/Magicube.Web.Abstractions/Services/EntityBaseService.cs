using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magicube.Web {
    public class EntityBaseService<TEntity, TKey> where TEntity : class, IEntity<TKey> {
        private IRepository<TEntity, TKey> _repository;

        protected IEventProvider EventProvider { get; }
        protected IServiceScope  ServiceScope  { get; }

        protected IRepository<TEntity, TKey> Repository {
            get {
                if(_repository == null) {
                    _repository = ServiceScope.GetService<IRepository<TEntity, TKey>>();
                }
                return _repository;
            }
        }

        public EntityBaseService(
            IEventProvider eventProvider,
            Application app
            ) {
            ServiceScope  = app.CreateScope();
            EventProvider = eventProvider;
        }

        public async Task<TEntity> AddOrUpdateAsync(TEntity entity) {
            if (entity.Id.Equals(default(TKey))) {
                await EventProvider.OnCreatingAsync(BuildEventContext(entity));
                entity = await Repository.InsertAsync(entity);
                await EventProvider.OnCreatedAsync(BuildEventContext(entity));
            } else {
                await EventProvider.OnUpdatingAsync(BuildEventContext(entity));
                entity = await Repository.UpdateAsync(entity);
                await EventProvider.OnUpdatedAsync(BuildEventContext(entity));
            }

            return entity;
        }

        public async ValueTask<TEntity> GetAsync(TKey key) {
            var data = await Repository.GetAsync(key);
            await EventProvider.OnLoadedAsync(BuildEventContext(data));
            return data;
        }

        public async void Remove(TKey key) {
            await Task.Yield();
            var entity = await Repository.GetAsync(key);
            await EventProvider.OnDeletingAsync(BuildEventContext(entity));
            await Repository.DeleteAsync(entity);
            await EventProvider.OnDeletedAsync(BuildEventContext(entity));
        }

        private EventContext<TEntity> BuildEventContext(TEntity entity) {
            return new EventContext<TEntity>(entity);
        }
    }
}
