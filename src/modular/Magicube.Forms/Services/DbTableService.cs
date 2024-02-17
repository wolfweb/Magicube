using Magicube.Data.Abstractions;
using Magicube.Forms.ViewModels;
using Magicube.Eventbus;
using Magicube.Core;
using Magicube.Web;
using System;

namespace Magicube.Forms.Services {
    public class DbTableService : EntityViewModelService<DbTable, int, DbTableViewModel> {
        public DbTableService(
            IMapperProvider mapper,
            IEventProvider eventProvider,
            IRepository<DbTable, int> tableRepository
            ) : base(mapper, eventProvider, tableRepository) {
        }

        protected override void ParseEntity(DbTable entity, DbTableViewModel viewModel) {
            if (entity.Id > 0) {
                if (entity.UpdateAt == null) entity.UpdateAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
    }
}
