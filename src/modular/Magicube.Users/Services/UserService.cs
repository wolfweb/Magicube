using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Eventbus;
using Magicube.Identity;
using Magicube.Users.ViewModels;
using Magicube.Web;

namespace Magicube.Users.Services {
    public class UserService : EntityViewModelService<User, long, UserViewModel> {
        public UserService(IMapperProvider mapper, IEventProvider eventProvider, IRepository<User, long> repository) : base(mapper, eventProvider, repository) {
        }
    }
}
