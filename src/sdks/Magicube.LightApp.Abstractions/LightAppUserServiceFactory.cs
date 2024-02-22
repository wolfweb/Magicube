using Magicube.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Magicube.LightApp.Abstractions {
    public class LightAppUserServiceFactory {
        private readonly Application _app;

        public LightAppUserServiceFactory(Application app) {
            _app = app;
        }

        public Task<LightAppUserViewModel> GetOrAddUser(string code, string identity) {
            using var scoped = _app.CreateScope();
            var service = scoped.ServiceProvider.GetService<ILightAppUserService>(identity);
            return service.GetOrAddUser(code);
        }

        public Task UpdateProfile(long userId, LigthAppUserDataViewModel viewModel) {
            using var scoped = _app.CreateScope();
            var service = scoped.ServiceProvider.GetService<ILightAppUserService>(viewModel.Provider);
            return service.Profile(userId, viewModel);
        }

        sealed class AppUserService : IDisposable {

            public void Dispose() {
                
            }
        }
    }
}
