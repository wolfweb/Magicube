using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Migration;
using Magicube.Data.MySql;
using Magicube.Web.Sites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Identity.Test {
    public class IdentityTest {
        [Theory]
        [InlineData("server=localhost;database=demo;user id=root;password=123456;")]
        public async Task Func_Identity_Test(string conn) {
            var services = new ServiceCollection();

            var moq = new Mock<ISiteManager>();
            moq.Setup(x => x.GetSite()).Returns(new DefaultSite() { UserPasswordCryptoType = "Sha512" });

            var container = services
                .AddMemoryCache()
                .AddCore()
                .UseMySQL(new DatabaseOptions { Value = conn })
                .AddIdentity()
                .Replace(new ServiceDescriptor(typeof(ISiteManager), sp=> moq.Object, ServiceLifetime.Singleton))
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var cryptoOptions = container.GetServices<ICryptoOptions>();

            var userManager = container.GetRequiredService<UserManager<User>>();
            Assert.NotNull(userManager);

            var user = await userManager.FindByNameAsync("wolfweb");
            
            if (user != null) {
                await userManager.DeleteAsync(user);
            }

            var password = "Magicube*123";
            user = new User {
                DisplayName  = "wolfweb",
                CreateAt     = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Email        = "wolfweb@wolfweb.com",
                UserName     = "wolfweb",
                PhoneNumber  = "12345678901",
                Status       = EntityStatus.Pending,
                PasswordHash = Guid.NewGuid().ToString("N"),
            };
            var result = await userManager.CreateAsync(user, password);

            user = await userManager.FindByNameAsync("wolfweb");
            Assert.NotNull(user);

            Assert.True(await userManager.CheckPasswordAsync(user, password));

            var signManager = container.GetRequiredService<SignInManager<User>>();
            Assert.NotNull(signManager);
        }
    }
}
