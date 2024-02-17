using Magicube.Data.Abstractions;
using Magicube.Data.Neo4j;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Data.ProviderTest {
    public class Neo4jRepositoryTest {
        private readonly IOptions<Neo4jOptions> options;
        public Neo4jRepositoryTest() {
            var neo4jOptionMock = new Mock<IOptions<Neo4jOptions>>();
            neo4jOptionMock.SetupGet(x => x.Value).Returns(new Neo4jOptions {
                Uri = "http://localhost:7474/db/data",
                User = "neo4j",
                Pwd = "4226361"
            });

            options = neo4jOptionMock.Object;
        }

        [Fact]
        public async Task Func_Neo4jRep_Test() {
            INeo4jRepository<Person, long> rep = new Neo4jRepository<Person, long>(new Neo4jDbContext(options));

            var result = await rep.All();

            foreach(var item in result) {
                var related = await rep.GetRelated<Person, Relationship>(x => x.Id == item.Id);
                foreach (var it in related) {
                    Trace.WriteLine($"{item.Name}: {it.Value.Name} : {it.Key.Name}");
                    await rep.DeleteRelationship<Person, Relationship>(x => x.Id == item.Id, x1 => x1.Id == it.Key.Id, it.Value);
                }
            }

            foreach (var item in result) {
                await rep.Delete(x => x.Name == item.Name);
            }

            await rep.Add(new Person {
                Id = 1,
                Name = "小红",
                Gender = Gender.Female,
                Age = 30
            });

            await rep.Add(new Person {
                Id = 2,
                Name = "小明",
                Gender = Gender.Male,
                Age = 30
            });

            await rep.Add(new Person {
                Id = 3,
                Name = "小静",
                Gender = Gender.Female,
                Age = 30
            });

            await rep.Add(new Person {
                Id = 4,
                Name = "小亮",
                Gender = Gender.Male,
                Age = 30
            });

            var user = await rep.Single(x => x.Name == "小静");
            user.Name = "晓静";
            await rep.Update(x => x.Name == "小静", user);

            Person user1 = await rep.Single(x => x.Name == "小红"),
                user2 = await rep.Single(x => x.Name == "小亮"),
                user3 = await rep.Single(x => x.Name == "晓静");

            if (user1 != null && user2 != null && user3 != null) {
                await rep.Relate<Person, Relationship>(x1 => x1.Id == user1.Id, x2 => x2.Id == user2.Id, new Relationship { Name = "friend" });
                await rep.Relate<Person, Relationship>(x1 => x1.Id == user1.Id, x2 => x2.Id == user3.Id, new Relationship { Name = "friend" });
            }
        }
    }

    public class Person: Entity<long>{
        public string Name   { get; set; }
        public int    Age    { get; set; }
        public Gender Gender { get; set; }
    }

    public enum Gender {
        Female,
        Male
    }
}
