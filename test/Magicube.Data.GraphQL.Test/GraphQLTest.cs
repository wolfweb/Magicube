using GraphQL;
using GraphQL.NewtonsoftJson;
using Magicube.Core;
using Magicube.Core.Models;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Migration;
using Magicube.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Data.GraphQL.Test {
    public class GraphQLTest {
        [Theory]
        [InlineData($"Data Source=magicube.db")]
        public async Task Func_GraphQL_Test(string conn) {
            var mockEnvironment = new Mock<IHostEnvironment>();
            var services = new ServiceCollection();

            var container = services
                .AddLogging()
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .UseSqlite(new DatabaseOptions { Value = conn })
                .AddGraphQL()
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();

            await Initialize(container);

            var app = container.GetService<Application>();
            app.ServiceProvider = container;

            var graphqlProvider = container.GetService<IGraphQLProvider>();

            //var schema = new Schema {
            //    Query    = new ObjectGraphType { Name = "Query" },
            //    Mutation = new ObjectGraphType { Name = "Mutation" },
            //    //Subscription = new ObjectGraphType { Name = "Subscription" }
            //};

            //schema.FieldMiddleware.Use(new DynamicEntityFieldMiddleware());

            //ISchemaBuilder schemaBuilder = new DynamicEntitySchemaQuery(app);
            //await schemaBuilder.BuildAsync(schema);
            //schema.Initialize();

            //var graphQLCtx = new GraphQLContext {
            //    ServiceProvider = container
            //};

            #region query
            var json = await graphqlProvider.ExecuteAsync("query { articleInfo (where: {id_lt: 100, or: {id: 101} } orderBy:{id: \"desc\" } first: 10 skip: 10) {id title} }");
            Assert.True(!json.IsNullOrEmpty());
            #endregion

            #region query
            json = await graphqlProvider.ExecuteAsync("query { articleInfo {id title} }");
            Assert.True(!json.IsNullOrEmpty());
            #endregion

            #region update
            /*
            {
                {
                    "articleInfo",
                    new Dictionary<string, object> {
                        ["id"] = 530,
                        ["title"]= DateTime.Now.ToString()
                    }
                }
            }
             */

            json = await graphqlProvider.ExecuteAsync(
                "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo){id title} }",
                new GraphQLArguementBuilder("articleInfo").Add("id", 530).Add("title", DateTime.Now.ToString())
                );
            Assert.True(!json.IsNullOrEmpty());
            #endregion

            #region update with filter
            /*
            {
                {
                    "articleInfo",
                    new Dictionary<string, object> {
                        //["id"] = 502,
                        ["title"]= DateTime.Now.ToString()
                    }
                }
            }
             */
            json = await graphqlProvider.ExecuteAsync(
                "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo, where: {id: 570 }){id title} }", 
                new GraphQLArguementBuilder("articleInfo").Add("title", DateTime.Now.ToString())
                );
            Assert.True(!json.IsNullOrEmpty());
            #endregion

            #region create
            /*
            {
                {
                    "articleInfo",
                    new Dictionary<string, object> {
                        ["title"]="¹þ¹þ"
                    }
                }
            }
             */
            json = await graphqlProvider.ExecuteAsync(
                "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo){id title} }", 
                new GraphQLArguementBuilder("articleInfo").Add("title","¹þ¹þ")
                );
            Assert.True(!json.IsNullOrEmpty());
            #endregion

            #region delete
            json = await graphqlProvider.ExecuteAsync("mutation ($articleInfo:ArticleInfoInput){ delete(articleInfo:$articleInfo, where: {id_gt: 600}){ id } }");
            Assert.True(!json.IsNullOrEmpty());
            #endregion
        }

        [Theory]
        [InlineData($"Data Source=magicube.db")]
        public async Task Func_Graphql_Builder_Test(string conn) {
            const string contentType = "ArticleInfo";

            var mockEnvironment = new Mock<IHostEnvironment>();
            var services = new ServiceCollection();

            var container = services
                .AddLogging()
                .AddSingleton(mockEnvironment.Object)
                .AddCore()
                .AddGraphQL()
                .UseSqlite(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();

            await Initialize(container);

            var app = container.GetService<Application>();
            app.ServiceProvider = container;

            var docExecute = container.GetService<IGraphQLProvider>();

            var query = "mutation ($articleInfo:ArticleInfoInput){ delete(articleInfo:$articleInfo, where : { id_gt : 1 } ) { id } }";
            var builder = new GraphQLOperateBuilder(contentType, GraphQLOperateType.Delete);
            builder.AddField("Id").AddFilter(new GtGraphQLFilter("Id", 1));
            var graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            var result1 = await docExecute.ExecuteAsync<DynamicEntity>(graphqlQuery);

            query = "mutation ($articleInfo:ArticleInfoInput){ create(articleInfo:$articleInfo) { title createAt } }";
            builder = new GraphQLOperateBuilder(contentType, GraphQLOperateType.Create);
            graphqlQuery = builder.AddField("title").AddField("createAt").Build();
            Assert.Equal(query, graphqlQuery);
            result1 = await docExecute.ExecuteAsync<DynamicEntity>(graphqlQuery, new GraphQLArguementBuilder(contentType).Add("title", Guid.NewGuid().ToString("n")).Add("createAt", DateTime.UtcNow.AddDays(-1)));
            Assert.NotNull(result1);

            query = "query { articleInfo ( where : { id_lt : 100, or : { id : 101 } } orderBy : { id : \"desc\" } first : 10 skip : 1 ) { id title } }";
            builder = new GraphQLOperateBuilder(contentType);
            builder.AddField("Id").AddField("Title")
            .AddFilter(new LtGraphQLFilter("Id", 100)).Or.AddFilter(new EqualGraphQLFilter("Id", 101))
            .AddOrder("Id").AddTake(10).AddSkip(1);
            graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            var result = await docExecute.ExecuteAsync<IEnumerable<DynamicEntity>>(graphqlQuery);
            Assert.True(result.Count() == 0);

            query = "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo) { id title } }";
            builder = new GraphQLOperateBuilder(contentType, GraphQLOperateType.AddOrUpdate);
            builder.AddField("Id").AddField("Title");
            graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            result1 = await docExecute.ExecuteAsync<DynamicEntity>(graphqlQuery, new GraphQLArguementBuilder(contentType).Add("title","¹þ¹þ"));

            query = "query { articleInfo { id title } }";
            builder = new GraphQLOperateBuilder(contentType);
            builder.AddField("Id").AddField("Title");
            graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            result = await docExecute.ExecuteAsync<IEnumerable<DynamicEntity>>(graphqlQuery);
            Assert.True(result.Count() > 0);

            query = "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo, where : { id_gt : 0 } ) { id title } }";
            builder = new GraphQLOperateBuilder(contentType, GraphQLOperateType.AddOrUpdate);
            builder.AddField("Id").AddField("Title")
                .AddFilter(new GtGraphQLFilter("Id", 0));
            graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            result1 = await docExecute.ExecuteAsync<DynamicEntity>(graphqlQuery, new GraphQLArguementBuilder(contentType).Add("title","¹þ¹þ1"));

            query = "mutation ($articleInfo:ArticleInfoInput){ addOrUpdate(articleInfo:$articleInfo) { id title } }";
            builder = new GraphQLOperateBuilder(contentType, GraphQLOperateType.AddOrUpdate);
            builder.AddField("Id").AddField("Title");
            graphqlQuery = builder.Build();
            Assert.Equal(query, graphqlQuery);
            result1 = await docExecute.ExecuteAsync<DynamicEntity>(graphqlQuery, new GraphQLArguementBuilder(contentType).Add("title", "¹þ¹þ2").Add("createAt", DateTime.UtcNow.AddDays(-2)));
        }

        private async Task Initialize(IServiceProvider container) {
            var migration = container.GetRequiredService<IMigrationManagerFactory>().GetMigrationManager();
            migration.SchemaUp();

            var tbRep = container.GetRequiredService<IRepository<DbTable, int>>();
            var operater = container.GetRequiredService<ISqlBuilder>();
            var rawDataOperator = container.GetRequiredService<IDbContext>() as DefaultDbContext;

            var tables = InitDatas();

            foreach (var table in tables) {
                var thisTable = tbRep.Get(x => x.Name == table.Name);
                if (thisTable == null)
                    await tbRep.InsertAsync(table);
                else {
                    thisTable.Fields = table.Fields;
                    await tbRep.UpdateAsync(thisTable);
                }
            }
            migration.SchemaUp();
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
                   Title  = "ÎÄÕÂÐÅÏ¢±í",
                   Name   = "ArticleInfo",
                   Description   = "´æ´¢ÎÄÕÂÐÅÏ¢",
                   Fields = articleInfoFields.ToArray(),
                   Status = EntityStatus.Actived
                }
            };
        }
    }
}
