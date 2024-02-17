using Magicube.Core.AutoMap;
using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Eventbus;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Magicube.Data.Services {
    public class AutoMapCommonService<TEntity, TKey, TViewModel> : EntityBaseService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TViewModel : EntityViewModel<TEntity, TKey> {
        protected readonly IMapperProvider Mapper;

        public AutoMapCommonService(
            IMapperProvider mapper, 
            IEventProvider eventProvider,
            IRepository<TEntity, TKey> repository
            ) : base(eventProvider, repository) {
            Mapper        = mapper;
        }

        public async Task<TViewModel> AddOrUpdateAsync(TViewModel model) {
            model.NotNull();
            var entity = Mapper.Map<TViewModel, TEntity>(model);
            ParseEntity(entity, model);
            entity = await AddOrUpdateAsync(entity);
            model.Id = entity.Id;
            return model;
        }

        public new async ValueTask<TViewModel> GetAsync(TKey key) {
            var result = Mapper.Map<TEntity, TViewModel>(await base.GetAsync(key));
            return result;
        }

        public PageResult<TViewModel, int> Page(PageSearchModel model, Expression<Func<TEntity, bool>> predicate = null) {
            if (model.PageIndex == -1) return PageResult<TViewModel, int>.Empty(-1);

            var query = Repository.Datas;
            if (predicate != null) {
                query = query.Where(predicate);
            }

            var result = query.Select(BuildEntity).Skip(model.PageSize * model.PageIndex++).Take(model.PageSize).ToArray();
            return new PageResult<TViewModel, int>(model.PageIndex, result.Select(x => Mapper.Map<TEntity, TViewModel>(x)));
        }

        protected virtual void    ParseEntity(TEntity entity, TViewModel viewModel) { }

        protected virtual TEntity BuildEntity(TEntity entity) {
            var ViewModelProperties = TypeAccessor.Get<TViewModel>().Properties;
            var EntityProperties    = TypeAccessor.Get<TEntity>().Properties;
            var instance = New<TEntity>.Instance();
            foreach (var property in ViewModelProperties) {
                var p = EntityProperties.FirstOrDefault(x => x.Name == property.Name);
                if (p.GetCustomAttribute<QueryListFilterIgnoreAttribute>() != null) continue;

                if (p != null && p.CanWrite) {
                    p.SetValue(instance, property.GetValue(entity));
                }
            }
            return instance;
        }
    }
}