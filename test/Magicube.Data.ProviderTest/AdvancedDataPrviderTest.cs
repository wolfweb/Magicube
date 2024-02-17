using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.Mapping;
using Magicube.Data.Migration;
using Magicube.Data.MySql;
using Magicube.TestBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace Magicube.Data.ProviderTest {
    public class AdvancedDataPrviderTest {
        private readonly IServiceProvider ServiceProvider;
        private readonly ITestOutputHelper _testOutputHelper;
        public AdvancedDataPrviderTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;

            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile($"appsettings.json", false, true)
                                .Build();

            ServiceProvider = new ServiceCollection()
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(testOutputHelper));
                })
                .AddDatabaseCore()
                .AddEntity<Cattle, CattleMapping>()
                .AddEntity<Dog, DotMapping>()
                .AddEntity<Animal, AnimalMapping>()
                .UseMySQL(new DatabaseOptions { Value = "server=localhost;database=demo;user id=root;password=123456;" })
                .BuildServiceProvider();
        }

        [Fact]
        public void Func_Inheritance_Test() {
            var migration = ServiceProvider.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();
            var dbctx = ServiceProvider.GetService<IDbContext>();
            var dogRep = ServiceProvider.GetService<IRepository<Dog, int>>();
            var cattleRep = ServiceProvider.GetService<IRepository<Cattle, int>>();
            var animalRep = ServiceProvider.GetService<IRepository<Animal, int>>();
            Assert.NotNull(dogRep);
            Assert.NotNull(cattleRep);

            var dog = dogRep.Insert(new Dog {
                Name = "Dog1",
                Speed = 100
            });

            Assert.NotNull(dog);
            Assert.True(dog.Id > 0);

            var cattle = cattleRep.Insert(new Cattle {
                Name = "Cattle1",
                PhysicalStrength = 100,
            });

            Assert.NotNull(cattle);
            Assert.True(cattle.Id > 0);

            var animal = animalRep.Insert(new Dog {
                Name = "Dog2",
                Speed = 200
            });
            Assert.NotNull(cattle);
            Assert.True(animal.GetType() == typeof(Dog));

            animal = animalRep.Get(3);
            Assert.NotNull(animal);
            Assert.True(animal.GetType() == typeof(Dog));

            _testOutputHelper.WriteLine(animal.Name);
        }
    }

    public class Animal : Entity<int> {
        public string Name { get; set; }
    }

    public class Dog : Animal {
        public double Speed { get; set; }
    }

    public class Cattle : Animal {
        public double PhysicalStrength { get; set; }
    }

    public class AnimalMapping : EntityTypeConfiguration<Animal> {
        public override void Configure(EntityTypeBuilder<Animal> builder) {
            builder.ToTable("Animal");
        }
    }

    public class DotMapping : EntityTypeConfiguration<Dog> {
        public override void Configure(EntityTypeBuilder<Dog> builder) {
            builder.ToTable("Dog");
        }
    }

    public class CattleMapping : EntityTypeConfiguration<Cattle> {
        public override void Configure(EntityTypeBuilder<Cattle> builder) {
            builder.ToTable("Cattle");
        }
    }
}
