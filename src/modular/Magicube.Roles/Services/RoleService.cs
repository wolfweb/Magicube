using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Web;
using Magicube.Eventbus;
using Magicube.Identity;
using Magicube.Roles.ViewModels;

namespace Magicube.Roles.Services {
    public class RoleService : EntityViewModelService<Role, int, RoleViewModel> {
        public RoleService(IMapperProvider mapper, IEventProvider eventProvider, IRepository<Role, int> repository) : base(mapper, eventProvider, repository) {
        }
    }
}
