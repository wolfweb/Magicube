using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Abstractions.ViewModel;
using Magicube.Eventbus;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Magicube.Web {
    public class EntityViewModelService<TEntity, TKey, TViewModel> : EntityBaseService<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TViewModel : EntityViewModel<TEntity, TKey> {
        protected readonly IMapperProvider Mapper;

        public EntityViewModelService(
            Application app,
            IMapperProvider mapper,
            IEventProvider eventProvider
            ) : base(eventProvider, app) {
            Mapper = mapper;
        }

        public virtual async Task<TViewModel> AddOrUpdateAsync(TViewModel model) {
            var entity = model.Build();
            ParseEntity(entity, model);
            entity = await AddOrUpdateAsync(entity);
            model.Id = entity.Id;
            return model;
        }

        public virtual async ValueTask<TViewModel> GetViewModelAsync(TKey key) {
            var entity = await base.GetAsync(key);
            var result = New<TViewModel, TEntity>.Instance(entity);
            return result;
        }

        public virtual PageResult<TViewModel, int> Page(PageSearchModel model, Expression<Func<TEntity, bool>> predicate = null) {
            if (model.PageIndex == -1) return PageResult<TViewModel, int>.Empty(-1);

            var query = Repository.All;
            if (predicate != null) {
                query = query.Where(predicate);
            }

            query = query.OrderByDescending(x => x.Id);

            var result = BuildSelect(query).Skip(model.PageSize * model.PageIndex++).Take(model.PageSize).ToArray();
            return new PageResult<TViewModel, int>(model.PageIndex, result.Select(x => New<TViewModel, TEntity>.Instance(x)).ToArray());
        }

        protected virtual void ParseEntity(TEntity entity, TViewModel viewModel) { }

        protected virtual IQueryable<TEntity> BuildSelect(IQueryable<TEntity> query) {
            var fields = TypeAccessor.Get<TEntity>().Context.Properties.Where(x => !x.Attributes.Any(m => m is NotMappedAttribute))
                .Where(x => {
                    if (x.Member.PropertyType.Equals(typeof(string))) {
                        var attr = x.GetAttribute<ColumnExtendAttribute>();
                        if (attr != null) {
                            return attr.Size < 255;
                        }
                        return true;
                    }

                    return x.Member.PropertyType.IsValueType;
                })
                .Select(x => x.Member.Name);
            return query.SelectMembers(fields.ToArray());
        }
    }
}
