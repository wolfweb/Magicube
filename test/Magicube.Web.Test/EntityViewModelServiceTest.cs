using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Migration;
using Magicube.Data.MySql;
using Magicube.Eventbus;
using Magicube.TestBase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Web.Test {
    public class EntityViewModelServiceTest {
        private readonly IServiceProvider ServiceProvider;

        public EntityViewModelServiceTest() {
            ServiceProvider = new ServiceCollection()
                .AddCore()
                .AddEventCore()
                .AddDatabaseCore()
                .UseMySQL(new DatabaseOptions { 
                    Value = "server=localhost;database=demo;user id=root;password=123456;"
                })
                .AddTransient<FooEntityViewModelService>()
                .AddEntity<FooEntity, FooEntityMapping>()
                .BuildServiceProvider();
        }
        [Fact]
        public async Task Func_EntityViewModel_Should_Return_Entity() {
            var migration = ServiceProvider.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            using(var scope = ServiceProvider.CreateScope()) {
                var service = scope.ServiceProvider.GetService<FooEntityViewModelService>();
                dynamic viewModel = new FooViewModel();
                viewModel.CreateAt = DateTime.UtcNow;
                viewModel.Password = "123456";
                viewModel.VerifyPwd = "123456";
                var result = await service.AddOrUpdateAsync(viewModel);
                Assert.NotNull(result);
            }

            using (var scope = ServiceProvider.CreateScope()) {
                var service = scope.ServiceProvider.GetService<FooEntityViewModelService>();
                var page = service.Page(new PageSearchModel {
                    PageSize = 5
                });

                foreach (dynamic item in page.Items) {
                    item.Name = Guid.NewGuid().ToString("n");
                    item.Address = Guid.NewGuid().ToString("n");
                    item.VerifyPwd = item.Password;
                    await service.AddOrUpdateAsync(item);
                }
            }

        }
    }

    public class FooEntityViewModelService : EntityViewModelService<FooEntity, int, FooViewModel> {
        public FooEntityViewModelService(IMapperProvider mapper, IEventProvider eventProvider, Application app) : base(app, mapper, eventProvider) {
        }
    }
}
