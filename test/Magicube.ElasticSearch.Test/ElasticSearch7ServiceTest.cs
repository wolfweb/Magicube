using System;
using System.Linq;
using Xunit;
using Magicube.Core;
using Magicube.ElasticSearch7;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Magicube.ElasticSearch.Test {
    public class ElasticSearch7ServiceTest {
        const string Url = "http://localhost:9200";

        private readonly IServiceProvider ServiceProvider;

        public ElasticSearch7ServiceTest() {
            ServiceProvider = new ServiceCollection()
                .AddLogging()
                .AddElasticSearchServices(options => {
                    options.Hosts = new[] { Url };
                    options.Debug = true;
                })
                .BuildServiceProvider();
        }

        [Fact]
        public async void Func_CreateIncexWithMapping_Test() {
            IElastic7SearchService ElasticService = ServiceProvider.GetService<IElastic7SearchService>();

            await ElasticService.CreateIndex(new IndexDocMapping { 
                Index = "chuye-courses",
                Fields = new List<IIndexMappingField> { 
                   new TextMappingField{
                       Name = "Title",
                       Analyzer = "ik_max_word",
                       SearchAnalyzer = "ik_smart"
                   },
                   new TextMappingField{
                       Name = "Text",
                       Analyzer = "ik_max_word",
                       SearchAnalyzer = "ik_smart"
                   },
                   new TextMappingField{
                       Name = "Keywords",
                       Analyzer = "ik_max_word",
                       SearchAnalyzer = "ik_smart"
                   },
                   new DenseVectorMappingField{ 
                       Name = "Text_vector",
                       Dimensions = 512
                   }
                }
            });
        }

        [Fact]
        public async Task Func_ElasticOperate_Test() {
            IElastic7SearchService ElasticService = ServiceProvider.GetService<IElastic7SearchService>();

            await ElasticService.DropIndex<FooSearchModel, int>();
            
            await ElasticService.Create<FooSearchModel, int>(new FooSearchModel { 
               Id        = 1,
               Sort      = 90,
               Title     = "wolfweb",
               CreateAt  = DateTime.Now
            });

            var res = await ElasticService.Query<FooSearchModel, int>(10);
            Assert.NotNull(res);
            Assert.True(res.Items.Count() == 1);
        }

        [Fact]
        public async void Func_ElasticIndex_Test() {
            IElastic7SearchService ElasticService = ServiceProvider.GetService<IElastic7SearchService>();

            var res = await ElasticService.ExistIndex<FooSearchModel, int>();
            if(res)
                await ElasticService.DropIndex<FooSearchModel, int>();
            await ElasticService.CreateIndex<FooSearchModel, int>();
            Assert.True(res);

            for(var i = 0; i < 100; i++) {
                var foo = new FooSearchModel {
                    Id    = i,
                    Sort  = i * 2 + 1,
                    Title = Guid.NewGuid().ToString("N").Substring(6),
                    CreateAt = DateTime.UtcNow
                };
                await ElasticService.Create<FooSearchModel, int>(foo);
            }

            var list = await ElasticService.Query<FooSearchModel, int>(50);
            Assert.True(list != null);
            Assert.True(list.Total ==100);
            Assert.True(list.Items.Count() == 50);
        }

        [Fact]
        public async void Func_ElasticDynamicIndex_Test() {
            IElastic7SearchService ElasticService = ServiceProvider.GetService<IElastic7SearchService>();

            var id = "1";
            var doc = new IndexDocument {
                Index = typeof(FooSearchModel).Name.ToLower(),
                Type  = typeof(FooSearchModel).Name.ToLower(),
                Id    = id
            };

            await ElasticService.Delete(doc);

            var entity = ElasticService.Get(new IndexDocument {
                Index = typeof(FooSearchModel).Name.ToLower(),
                Type  = typeof(FooSearchModel).Name.ToLower(),
                Id    = id
            });

            Assert.Null(entity);

            doc.Document = new {
                Id        = 1,
                Age       = 100,
                Name      = "wolfweb",
                Thumbnail = "",
                CreateAt  = DateTime.UtcNow
            };

            await ElasticService.Create(doc);

            entity = ElasticService.Get(new IndexDocument {
                Index = typeof(FooSearchModel).Name.ToLower(),
                Type  = typeof(FooSearchModel).Name.ToLower(),
                Id    = id
            });
            Assert.NotNull(entity);

            var foo = await ElasticService.Get<FooSearchModel, int>(id.AsInt());
            Assert.NotNull(foo);

            foo.Title = "wolfweb1";
            await ElasticService.Update<FooSearchModel, int>(foo);

            entity = ElasticService.Get(new IndexDocument {
                Index = typeof(FooSearchModel).Name.ToLower(),
                Type  = typeof(FooSearchModel).Name.ToLower(),
                Id    = id
            });
            Assert.True(entity.Document.name == "wolfweb1");
        }
    }
}
