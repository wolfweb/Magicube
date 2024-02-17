using Magicube.Core;
using Magicube.Data.Abstractions;
using Magicube.Data.Abstractions.EfDbContext;
using Magicube.Data.Abstractions.SqlBuilder;
using Magicube.Data.Abstractions.SqlBuilder.Models;
using Magicube.Data.Migration;
using Magicube.Data.MySql;
using Magicube.TestBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Magicube.Data.ProviderTest {
    public class DataModelMappingTest {
        private readonly ITestOutputHelper _testOutputHelper;

        public DataModelMappingTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData("server=192.168.3.207;database=demo;user id=root;password=123456;")]
        public void Func_DataTable_To_DataModel_Test(string conn) {
            var services = new ServiceCollection();

            var container = services
                .AddLogging(builder => {
                    builder.AddProvider(new XUnitLoggerProvider(_testOutputHelper));
                })
                .UseMySQL(new DatabaseOptions { Value = conn })
                .AddMigrationAssembly(typeof(DbTable).Assembly)
                .BuildServiceProvider();

            var operater = container.GetRequiredService<ISqlBuilder>();
            var rawDataOperator = container.GetRequiredService<IDbContext>() as DefaultDbContext;

            var rawSql = operater.GetTableSchemas("demo");

            var tbModels = rawDataOperator.SqlQuery<TableSchemaModel>(rawSql);

            foreach (var group in tbModels.GroupBy(x=>x.TableName)) {
                var result = group.ExportToDataModel(x => operater.SqlCompiler.GetEntityType(x));
                _testOutputHelper.WriteLine(result);
            }
        }
    }
}
