using Magicube.Data.Abstractions;
using Magicube.Data.Mongodb;
using Magicube.Data.MySql;
using Magicube.Data.PostgreSql;
using Magicube.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using Magicube.Data.Abstractions.SqlBuilder;
using System.Data;
using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using Magicube.Data.SqlServer;
using Magicube.Data.LiteDb;
using Magicube.Data.Migration;
using Magicube.Data.DbFactroy;
using Magicube.Core;
using Magicube.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Magicube.TestBase;
using Magicube.Data.Abstractions.EfDbContext;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Magicube.Data.ProviderTest {
    public class DataProviderTest {
        static FooEntity FooOne = new FooEntity { 
            Id        = 1, 
            Name      = "wolfweb", 
            Age       = 10, 
            Born      = DateTime.UtcNow, 
            Password  = "123456", 
            Attribute = JObject.Parse("{}"), 
            Address   = Guid.NewGuid().ToString(), 
            CreateAt  = DateTimeOffset.UtcNow.ToUnixTimeSeconds() 
        };

        private readonly ITestOutputHelper _testOutputHelper;

        public DataProviderTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public void MySql_Provider_Test(string conn) {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services.UseMySQL(new DatabaseOptions { Value = conn })
                .AddDatabseContext<FooDbContext>("demo")
                .AddSingleton<IServiceCollection>(services)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);
            
            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = rep.Get(1);
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Get(1);
            Assert.Null(entity);

            entity = rep.Insert(FooOne);
            Assert.NotNull(entity);

            entity = rep.Get(1);
            Assert.NotNull(entity);

            var sqliteDbContext = container.GetService<IDbContext>("demo") as DbContext;
            Assert.NotNull(sqliteDbContext);
            sqliteDbContext.Database.EnsureCreated();
            var sqliteRep = container.GetService<RepositoryFactory>().Retrive<FooEntity, int>("demo");
            var entity1 = sqliteRep.Get(1);
            Assert.NotNull(sqliteRep);
            if(entity1 != null) sqliteRep.Delete(entity1);
            entity1 = sqliteRep.Get(1);
            Assert.Null(entity1);
        }

        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public async Task MySql_Provider_TestAsync(string conn) {
            IServiceCollection services = new ServiceCollection();

            InitDataMapping(services);

            var container = services.UseMySQL(new DatabaseOptions { Value = conn })
                .AddDatabseContext<FooDbContext>("demo")
                .AddSingleton(services)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);

            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = await rep.GetAsync(1);
            if (entity != null)
                await rep.DeleteAsync(entity);

            entity = await rep.GetAsync(1);
            Assert.Null(entity);

            entity = await rep.InsertAsync(FooOne);
            Assert.NotNull(entity);

            entity = await rep.GetAsync(1);
            Assert.NotNull(entity);

            var sqliteDbContext = container.GetService<IDbContext>("demo") as DbContext;
            sqliteDbContext.Database.EnsureCreated();
            var sqliteRep = container.GetService<RepositoryFactory>().Retrive<FooEntity, int>("demo");
            var entity1 = sqliteRep.Get(1);
            Assert.NotNull(sqliteRep);
            if (entity1 != null) sqliteRep.Delete(entity1);
            entity1 = sqliteRep.Get(1);
            Assert.Null(entity1);
        }

        [Theory]
        [InlineData("mongodb://192.168.3.23:27017/local")]
        public void Mongo_Provider_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .UseMongodb(new DatabaseOptions { Value = conn })
                .BuildServiceProvider();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);
            var mongoRep = container.GetService<IMongoRepository<FooEntity, int>>();
            Assert.NotNull(mongoRep);

            var entity = rep.Get(1);
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Get(1);
            Assert.Null(entity);

            entity = rep.Insert(new FooEntity { Id = 1, Name = "wolfweb", Age = 10, Born = DateTime.UtcNow });
            Assert.NotNull(entity);

            entity = rep.Get(1);
            Assert.NotNull(entity);

            var list = new List<FooEntity>();
            for (var i = 0; i < 10; i++) {
                list.Add(new FooEntity { Id = entity.Id+i+1, Name = $"wolfweb-{i+1}", Age = 10, Born = DateTime.UtcNow });
            }
            rep.Insert(list);
            var datas = rep.Query(x => x.Id > 0);
            Assert.True(datas.Count() > 1);
            rep.Delete(x=>x.Id>0);

            Assert.True(rep.Query(x => x.Id > 0).Count() == 0);
        }

        [Theory]
        [InlineData("server=192.168.3.23;database=demo;user id=sa;password=password01$;")]
        public void SqlServer_Provider_Test(string conn) {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services
                .UseSqlServer(new DatabaseOptions { Value =  conn})
                .AddMigrationAssembly(typeof(FooEntity).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);

            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = rep.Query(x=>x.Id>0).FirstOrDefault();
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Query(x => x.Id > 0).FirstOrDefault();
            Assert.Null(entity);

            entity = rep.Insert(new FooEntity { Name = "wolfweb", Age = 10, Born = DateTime.UtcNow });
            Assert.NotNull(entity);

            entity = rep.Query(x => x.Id > 0).FirstOrDefault();
            Assert.NotNull(entity);
        }

        [Fact]
        public void Sqlite_Provider_Test() {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services
                .UseNLog()
                .UseSqlite(new DatabaseOptions { Value = $"Data Source=magicube.db" })
                .AddMigrationAssembly(typeof(FooEntity).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);
            
            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = rep.Get(1);
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Get(1);
            Assert.Null(entity);

            entity = rep.Insert(FooOne);
            Assert.NotNull(entity);

            entity = rep.Get(1);
            Assert.NotNull(entity);            
        }

        [Fact]
        public void Sqlite_Provider_Transaction_Test() {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services
                .UseSqlite(new DatabaseOptions { Value = $"Data Source=magicube.db" })
                .AddMigrationAssembly(typeof(FooEntity).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);
            
            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = rep.Get(1);
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Get(1);
            Assert.Null(entity);

            entity = rep.Insert(FooOne);
            Assert.NotNull(entity);

            entity = rep.Get(1);
            Assert.NotNull(entity);

            using (var unitOfWorkScoped = transaction.BeginTransaction()) {
                try {
                    entity = rep.Insert(new FooEntity { Id = 2, Name = "wolfweb", Age = 11, Born = DateTime.UtcNow });

                    entity = rep.Insert(new FooEntity { Id = 1, Name = "wolfweb", Age = 10, Born = DateTime.UtcNow });
                }
                catch(AggregateException e) {
                    unitOfWorkScoped.Rollback();
                    _testOutputHelper.WriteLine(e.Message);
                }
                catch (Exception e){
                    unitOfWorkScoped.Rollback();
                    _testOutputHelper.WriteLine(e.Message);
                }
            }

            var res = rep.Query(x => x.Id > 0);
            Assert.True(res.Count() == 1);

            //var sqliteRep = container.GetService<ISqliteRepository<FooEntity, int>>();
            //Assert.NotNull(sqliteRep);
        }

        [Theory]
        [InlineData("server=localhost;database=demo;user id=postgres;password=123456;")]
        public void Postgre_Provider_Test(string conn) {
            var services = new ServiceCollection();

            InitDataMapping(services);

            var container = services
                .UsePostgreSql(new DatabaseOptions { Value = conn })
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetService<IRepository<FooEntity, int>>();
            Assert.NotNull(rep);
            
            var dbctx = container.GetService<IDbContext>() as DbContext;
            Assert.NotNull(dbctx);

            var transaction = container.GetService<IUnitOfWork>();
            Assert.NotNull(transaction);
            Assert.Same(transaction, dbctx);

            var entity = rep.Get(1);
            if (entity != null)
                rep.Delete(entity);

            entity = rep.Get(1);
            Assert.Null(entity);

            entity = rep.Insert(new FooEntity { Id = 1, Name = "wolfweb", Age = 10, Born = DateTime.UtcNow });
            Assert.NotNull(entity);

            entity = rep.Get(1);
            Assert.NotNull(entity);

            //var postgreRep = container.GetService<IPostgreSqlRepository<FooEntity, int>>();
            //Assert.NotNull(postgreRep);
        }

        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public void Func_MySqlBuilder_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .UseMySQL(new DatabaseOptions { Value = conn })
                .BuildServiceProvider();

            var operater = container.GetRequiredService<ISqlBuilder>();

            var sql = operater.Insert("demo").SetData(new {
                Id   = 1,
                Name = "wolfweb"
            }).WithReturnId().Build();
            Assert.Equal("insert into `demo` (Id,Name) values (1,'wolfweb') ;select last_insert_id() as Id;", sql.RawSql());

            sql = operater.Query("demo").Build();
            Assert.Equal("SELECT * FROM `demo`", sql.RawSql());

            sql = operater.Query("demo").Where(Entity.IdKey, 1).Limit(1).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `Id` = 1 LIMIT 1", sql.RawSql());

            sql = operater.Query("demo").WhereIn("id", new[] { 1,3,4,}).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` IN (1, 3, 4)", sql.RawSql());

            sql = operater.Query("demo").WhereIn("id", new[] { "1", "3", "4", }).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` IN ('1', '3', '4')", sql.RawSql());

            sql = operater.Query("demo").WhereNotIn("id", new[] { 1, 3, 4, }).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` NOT IN (1, 3, 4)", sql.RawSql());

            sql = operater.Query("demo").WhereGt("id", 100).WhereLt("id",1000).Or().WhereIn("id", new[] { 1, 3, 4, }).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` > 100 AND `id` < 1000 OR `id` IN (1, 3, 4)", sql.RawSql());

            sql = operater.Query("demo").WhereGt("id", 100).WhereLt("id", 1000).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` > 100 AND `id` < 1000", sql.RawSql());

            sql = operater.Query("demo").WhereGt("id", 100).Or().WhereLt("id", 1000).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` > 100 OR `id` < 1000", sql.RawSql());

            sql = operater.Query("demo").Where("id", 100).Or().WhereIn("id", new[] { 1, 3, 4, }).Or().WhereIn("id", new[] { 7,8,9 }).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` = 100 OR `id` IN (1, 3, 4) OR `id` IN (7, 8, 9)", sql.RawSql());

            sql = operater.Query("demo").Where("id", 100).Or().Where(q=> q.WhereIn("id", new[] { 1, 3, 4, }).WhereIn("id", new[] { 7, 8, 9 })).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `id` = 100 OR (`id` IN (1, 3, 4) AND `id` IN (7, 8, 9))", sql.RawSql());

            sql = operater.Query("demo").Where(q=>q.Where("id", 100).WhereIn("id", new[] { 1, 3, 4, })).Or().Where(q=>q.WhereIn("id", new[] { 7, 8, 9 })).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE (`id` = 100 AND `id` IN (1, 3, 4)) OR (`id` IN (7, 8, 9))", sql.RawSql());

            sql = operater.Query("demo").OrderBy("id").Build();
            Assert.Equal("SELECT * FROM `demo` ORDER BY `id`", sql.RawSql());

            sql = operater.Query("demo").OrderBy("id").Limit(10).Build();
            Assert.Equal("SELECT * FROM `demo` ORDER BY `id` LIMIT 10", sql.RawSql());

            sql = operater.Query("demo").Where(Entity.IdKey, 1).Limit(1).Build();
            Assert.Equal("SELECT * FROM `demo` WHERE `Id` = 1 LIMIT 1", sql.RawSql());

            sql = operater.Query("demo").Select(new[] { "id", "name", "sex", "addr" }).Where(q=>q.Where("id", 1).Where("name", "wolfweb")).Or().Where("id", 100).OrderByDesc("id").Limit(10).Build();
            Assert.Equal("SELECT `id`, `name`, `sex`, `addr` FROM `demo` WHERE (`id` = 1 AND `name` = 'wolfweb') OR `id` = 100 ORDER BY `id` DESC LIMIT 10", sql.RawSql());

            sql = operater.Delete("demo").Build();
            Assert.Equal("delete from `demo`", sql.RawSql());

            sql = operater.Delete("demo").Where("id", 1).Build();
            Assert.Equal("delete from `demo` where (`id` = 1)", sql.RawSql());

            sql = operater.Update("demo").Set("Age", 1).Set("Name", "wolfweb").Where("id", 1).Build();
            Assert.Equal("update `demo` set `Age` = 1,`Name` = 'wolfweb' where (`id` = 1)", sql.RawSql());

            sql = operater.DropTable("demo").Build();
            Assert.Equal("drop table `demo`", sql.RawSql());

            sql = operater.CreateTable("demo").WithColumns(new[] {
                new ColumnItem("Id")      {Type = typeof(long),   PrimaryKey = true, AutoIncrement = true },
                new ColumnItem("Name")    {Type = typeof(string), UniqueKey  = true, Size = 255 },
                new ColumnItem("CreateAt"){Type = typeof(DateTime)},
                new ColumnItem("Float")   {Type = typeof(float)},
                new ColumnItem("Double")  {Type= typeof(double)}
            }).Build();

            Assert.Equal("create table `demo` ( `Id` bigint primary key auto_increment,`Name` varchar(255) unique key default null,`CreateAt` datetime default null,`Float` float default null,`Double` double default null );", sql.RawSql());

            sql = operater.AlterTable("demo").AddColumn(new ColumnItem("Address") { Type = typeof(string), Size = 255 }).Build();

            Assert.Equal("alter table `demo` add `Address` varchar(255) ;", sql.RawSql());

            sql = operater.AlterTable("demo").DropColumn(new ColumnItem("Address") ).Build();
            Assert.Equal("alter table `demo` drop column `Address` ;", sql.RawSql());

            sql = operater.GetTableSchemas("HotDataAnalysic");
            Assert.Equal("select table_name as TableName, column_name as ColumnName, case when left(column_type,locate('(',column_type)-1)='' then column_type else left(column_type,locate('(',column_type)-1) end as DataType, cast(substring(column_type,locate('(',column_type)+1,locate(')',column_type)-locate('(',column_type)-1) as signed) as Length, column_default as DefaultValue, case when column_key = 'PRI' then true else false end as IsPrimaryKey, case when column_key = 'UNI' then true else false end as UniqueKey, case when extra = 'auto_increment' then true else false end as AutoIncrement, case when is_nullable = 'YES' then true else false end as Nullable from information_schema.columns where table_schema='HotDataAnalysic' and table_schema=(select database())", sql.RawSql());
        }
        
        [Fact]
        public void Func_SqlServerBuilder_Test() {
            var services = new ServiceCollection();

            var container = services
                .UseSqlServer(new DatabaseOptions { Value = "server=192.168.10.251;database=demo;user id=root;password=123456;" })
                .BuildServiceProvider();

            var operater = container.GetRequiredService<ISqlBuilder>();

            var sql = operater.Query("demo").Select(new[] { "id", "name", "sex", "addr" }).Where("id", 1).Where("name", "wolfweb").Or().Where("id", 100).OrderBy("id").Limit(10).Build();
            Assert.Equal("select top 10 [id],[name],[sex],[addr] from [demo] where ([id] = 1 and [name] = 'wolfweb') or ([id] = 100) order by [id] asc", sql.RawSql());
        }

        [Fact]
        public void Func_PostgreBuilder_Test() {
            var services = new ServiceCollection();

            var container = services
                .UsePostgreSql(new DatabaseOptions { Value = "server=localhost;database=demo;user id=postgres;password=123456;" })
                .BuildServiceProvider();

            var operater = container.GetRequiredService<ISqlBuilder>();
            var sql = operater.CreateDatabase("demo");
            Assert.Equal("create database demo", sql.RawSql());

            sql = operater.DropTable("DbFields").Build();
            Assert.Equal("drop table \"DbFields\"", sql.RawSql());

            sql = operater.CreateTable("foo").WithColumns(new[] {
                new ColumnItem("Id"){ 
                   AutoIncrement = true,
                   PrimaryKey    = true,
                   Type          = typeof(int)
                },
                new ColumnItem("Name") {
                    Type      = typeof(string),
                    UniqueKey = true,
                    Nullable  = false,
                    Size      = 128
                }
            }).Build();
            Assert.Equal("create table \"foo\" ( \"Id\" serial primary key not null,\"Name\" varchar(128) unique not null );", sql.RawSql());
        }

        [Theory]
        [InlineData("MySql", "server=192.168.3.207;database=demo;user id=root;password=123456;")]
        [InlineData("PostgreSQL", "server=localhost;database=demo;user id=postgres;password=123456;")]
        [InlineData("SQLite", "Data Source=magicube.db")]
        public async Task Func_DataFactory_Test(string name, string conn) {
            var services = new ServiceCollection();

            InitDataMapping(services);

            services.Configure<DatabaseOptions>(x => {
                x.Name  = name;
                x.Value = conn;
            }).AddDatabases();

            var container = services.BuildServiceProvider();

            using(var scope = container.CreateScope()) {
                var migration = scope.ServiceProvider.GetService<IMigrationManagerFactory>().GetMigrationManager();
                Assert.NotNull(migration);
                migration.SchemaUp();
                var rep = scope.ServiceProvider.GetService<IRepository<FooEntity, int>>();
                Assert.NotNull(rep);
                var entity = await rep.GetAsync(1);
                if(entity != null) {
                    await rep.DeleteAsync(entity);
                }
                entity = await rep.GetAsync(1);
                Assert.Null(entity);
            }
        }

        [Fact]
        public void LiteDb_Provider_Test() {
			var services = new ServiceCollection();

			var container = services
				.UseNLog()
				.UseLiteDb(new DatabaseOptions { Value = $"magicube-lite.db" })
				.BuildServiceProvider();

			var rep = container.GetService<IRepository<FooEntity, int>>();
			Assert.NotNull(rep);

			var entity = rep.Get(1);
			if (entity != null)
				rep.Delete(entity);

			entity = rep.Get(1);
			Assert.Null(entity);

			entity = rep.Insert(FooOne);
			Assert.NotNull(entity);

			entity = rep.Get(1);
			Assert.NotNull(entity);
		}


		private void InitDataMapping(IServiceCollection services) {
            services.AddEntity<FooEntity, FooEntityMapping>();
        }
    }

    public class FooDbContext : DefaultDbContext {
        public FooDbContext(DbContextOptions<FooDbContext> options) : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=magicube.db");
        }

        public DbSet<FooEntity> Foos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            new FooEntityMapping().ApplyConfiguration(builder);
        }
    }
}
