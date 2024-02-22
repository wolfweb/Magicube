using Magicube.Core;
using Magicube.Eventbus;
using Magicube.Identity;
using Magicube.Users.ViewModels;
using Magicube.Web;

namespace Magicube.Users.Services {
    public class UserService : EntityViewModelService<User, long, UserViewModel> {
        public UserService(IMapperProvider mapper, IEventProvider eventProvider, Application app) : base(app, mapper, eventProvider) {
        }
    }
}
