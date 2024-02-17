using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.MySql;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;
using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System.Linq;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using Magicube.Data.Abstractions.SqlBuilder.Models;
using Magicube.Data.PostgreSql;
using Magicube.Data.Migration;
using Magicube.Core.Models;
using System.Threading.Tasks;
using Magicube.Core;
using Magicube.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions.EfDbContext;

namespace Magicube.DynamicForm.Test {
    public class DynamicFrmTest {
        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public void Func_MySql_Dynamic_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .UseMySQL(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();  

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var tbRep = container.GetRequiredService<IRepository<DbTable, int>>();
            var operater = container.GetRequiredService<ISqlBuilder>();
            var rawDataOperator = container.GetRequiredService<IDbContext>() as DefaultDbContext;

            var tables = InitDatas();
            var rawSql = operater.GetTableSchemas("demo");

            foreach (var table in tables) {
                var thisTable = tbRep.Get(x => x.Name == table.Name);
                if (thisTable == null)
                    tbRep.Insert(table);
                else {
                    thisTable.Fields = table.Fields;
                    tbRep.Update(thisTable);
                }
            }

            migration.SchemaUp();

            foreach (var table in tables) {
                var tableName = table.Name;
                DynamicEntity entity = new DynamicEntity(table);
                entity.Id = 1;

                rawSql = operater.Query(entity.TableName).Where(nameof(entity.Id), entity.Id).Build();

                entity = rawDataOperator.Get(rawSql);
                if (entity!=null) {
                    rawSql = operater.Delete(entity.TableName).Where(Entity.IdKey, entity.Id).Build();
                    rawDataOperator.Execute(rawSql);
                }

                rawSql = operater.Query(tableName).WhereGt(Entity.IdKey, 2).Build();
                var entities = rawDataOperator.Gets(rawSql);
                foreach(var it in entities) {
                    rawSql = operater.Update(it.TableName).Where(Entity.IdKey, it.Id)
                        .Set("Title", "wolfweb1")
                        .Build();
                    rawDataOperator.Execute(rawSql);
                }

                entity = new DynamicEntity(tableName);
                dynamic wrap = entity;
                wrap.Title = "wolfweb";
                wrap.CreateAt = DateTime.UtcNow;

                rawSql = operater.Insert(entity.TableName)
                    .SetData(entity)
                    .Build();

                var result = rawDataOperator.Execute<long>(rawSql);
            }

            Console.WriteLine("hello");
        }

        [Theory]
        [InlineData("Data Source=magicube.db")]
        public void Func_Sqlite_Dynamic_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .UseSqlite(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var tbRep = container.GetRequiredService<IRepository<DbTable, int>>();
            var operater = container.GetRequiredService<ISqlBuilder>();
            var rawDataOperator = container.GetRequiredService<IDbContext>() as DefaultDbContext;

            var tables = InitDatas();

            foreach (var table in tables) {
                var thisTable = tbRep.Get(x => x.Name == table.Name);
                if (thisTable == null)
                    tbRep.Insert(table);
                else {
                    thisTable.Fields = table.Fields;
                    tbRep.Update(thisTable);
                }
            }
            migration.SchemaUp();

            foreach (var table in tables) {
                var tableName = table.Name;
                DynamicEntity entity = new DynamicEntity(table);
                entity.Id = 1;

                var rawSql = operater.Query(entity.TableName).Where(nameof(entity.Id), entity.Id).Build();

                entity = rawDataOperator.Get(rawSql);
                if (entity != null) {
                    rawSql = operater.Delete(entity.TableName).Where(Entity.IdKey, entity.Id).Build();
                    rawDataOperator.Execute(rawSql);
                }

                rawSql = operater.Query(tableName).WhereGt(Entity.IdKey, 2).Build();
                var entities = rawDataOperator.Gets(rawSql);
                foreach (var it in entities) {
                    rawSql = operater.Update(it.TableName).Where(Entity.IdKey, it.Id)
                        .Set("Title", "wolfweb1")
                        .Build();
                    rawDataOperator.Execute(rawSql);
                }

                entity = new DynamicEntity(tableName);
                dynamic wrap = entity;
                wrap.Title = "wolfweb";
                wrap.CreateAt = DateTime.UtcNow;

                rawSql = operater.Insert(entity.TableName)
                    .SetData(entity)
                    .Build();

                var result = rawDataOperator.Execute<long>(rawSql);
            }

            Console.WriteLine("hello");
        }

        [Theory]
        [InlineData("server=localhost;database=demo;user id=postgres;password=123456;")]
        public void Func_PostgreSql_Dynamic_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .UsePostgreSql(new DatabaseOptions { Value = conn })
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var tbRep = container.GetRequiredService<IRepository<DbTable, int>>();
            var operater = container.GetRequiredService<ISqlBuilder>();
            var rawDataOperator = container.GetRequiredService<IDbContext>() as DefaultDbContext;

            var tables = InitDatas();
            foreach (var table in tables) {
                if (tbRep.Get(x => x.Name == table.Name) == null)
                    tbRep.Insert(table);

                var _table = tbRep.Get(x => x.Name == table.Name);

                var rawSql = operater.GetTableSchemas("demo");

                var tbModels = rawDataOperator.SqlQuery<TableSchemaModel>(rawSql);

                if (!tbModels.Select(x => x.TableName).Distinct().Contains(table.Name)) {
                    rawSql = operater.CreateTable(table.Name)
                        .WithColumns(_table.Fields.Select(x => new ColumnItem(x.Name) {
                            Type = x.BindType,
                            UniqueKey = x.UniqueKey,
                            PrimaryKey = x.PrimaryKey,
                            AutoIncrement = x.AutoIncrement,
                        }).ToArray())
                        .Build();

                    rawDataOperator.Execute(rawSql);
                }
            }

            Console.WriteLine("hello");
        }

        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public async Task Func_MySql_Dynamic_Repository_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .AddCore()
                .UseMySQL(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();

            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var rep = container.GetRequiredService<IDynamicEntityRepository>();
            var allSupportTypes = await rep.GetAllTypes();
            Assert.True(allSupportTypes.Count() > 0);
            var dynamicEntity = allSupportTypes.First();
            
            var datas = await rep.GetsAsync(dynamicEntity.Name);
            foreach(var data in datas){
                await rep.DeleteAsync(data);
            }

            for(var i = 0; i < 100; i++) {
                var entity = await rep.NewAsync(dynamicEntity.Name);
                foreach(var field in entity.Fields) {
                    if (field.Key == Entity.IdKey) continue;
                    if (field.Key == "Title") {
                        entity[field.Key] = Guid.NewGuid().ToString("n");
                        continue;
                    }
                    entity[field.Key] = entity.DefaultValue(field.Key);
                }
                await rep.InsertAsync(entity);
            }

            var singleData = await rep.GetAsync(dynamicEntity.Name, 10);
            Assert.NotNull(singleData);

            singleData = await rep.GetAsync(dynamicEntity.Name, new[] { "Id", "CreateAt" }, 10);
            Assert.NotNull(singleData);

            datas = await rep.GetsAsync(dynamicEntity.Name, ctx => {
                return ctx.WhereGt(Entity.IdKey, 50);
            });

            Assert.True(datas.Count > 1);

            Console.WriteLine("hello");
        }

        private List<DbTable> InitDatas() {
            var articleInfoFields = new List<DbField>() {
               new DbField{
                   AutoIncrement = true,
                   PrimaryKey    = true,
                   BindType      = typeof(int),
                   Name          = "Id",
                   DbType        = ""
               },
               new DbField {
                   Name     = "Title",
                   BindType = typeof(string),
                   Size     = 255,
                   Nullable = true,
                   UniqueKey = true,
               },
               new DbField {
                   Name     = "CreateAt",
                   BindType = typeof(DateTime),
                   Nullable = false,
               },
               new DbField {
                   Name = "Body",
                   BindType = typeof(string),
                   Size = 2000
               },
               new DbField {
                   Name = "Attribute",
                   BindType = typeof(JObject),
                   Size = 4000,
               }
            };

            return new List<DbTable>() {
                new DbTable{
                   Title  = "文章信息表",
                   Name   = "ArticleInfo",
                   Description   = "存储文章信息",
                   Fields = articleInfoFields.ToArray(),
                   Status = EntityStatus.Actived
                }
            };
        }
    }
}
