using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Attributes;
using Magicube.Data.Migration;
using Magicube.Data.Sqlite;
using Magicube.TestBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Magicube.Data.ProviderTest {
    public class DataMigrationTest {
        [Theory]
        [InlineData("Data Source=magicube.db")]
        public void Sqlite_Migration_Test(string conn) {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services
                .UseSqlite(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(FooEntity).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();
        }

        private void InitDataMapping(IServiceCollection services) {
            services.AddEntity<FooEntity, FooEntityMapping>();
            services.AddEntity<FooA>();
            services.AddEntity<FooB>();
        }

        [Table("FooA")]
        public class FooA : FooEntity {
            [IndexField(IsUnique = true)]
            public string Email { get; set; }
        }

        [Table("FooB")]
        public class FooB : FooEntity {

        }
    }
}
