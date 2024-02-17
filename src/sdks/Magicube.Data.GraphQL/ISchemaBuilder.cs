using GraphQL.Types;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Magicube.Data.Abstractions;
using Magicube.Core;

namespace Magicube.Data.GraphQL {
    public interface ISchemaBuilder {
        Task<IChangeToken> BuildAsync(ISchema schema);
    }

    public class DynamicEntitySchemaQuery : ISchemaBuilder {
        private readonly Application _application;

        public DynamicEntitySchemaQuery(Application application) {
            _application = application;
        }

        public async Task<IChangeToken> BuildAsync(ISchema schema) {
            var serviceProvider = _application.ServiceProvider;

            var dynamicEntityRepository = serviceProvider.GetService<IDynamicEntityRepository>();

            foreach (var type in await dynamicEntityRepository.GetAllTypes()) {
                DynamicEntity entity = type.Name;

                //声明类型
                var typeType = new DynamicEntityType(entity) {
                    Name        = type.Name,
                    Description = $"Represents a {type.Desc}."
                };

                var query = new DynamicEntityQueryFieldType(entity, schema) {
                    Name = type.Name
                };

                var create = new DynamicEntityCreateFieldType(entity, schema) { 
                    Name = "Create"
                };

                var addOrUpdate = new DynamicEntityAddOrUpdateFieldType(entity, schema) {
                    Name = "AddOrUpdate"
                };

                var delete = new DynamicEntityDeleteFieldType(entity, schema) {
                    Name = "Delete"
                };

                var update = new DynamicEntityUpdateFieldType(entity, schema) {
                    Name = "Update"
                };

                schema.Query.AddField(query);
                schema.Mutation.AddField(create);
                schema.Mutation.AddField(addOrUpdate);
                schema.Mutation.AddField(delete);
                schema.Mutation.AddField(update);

                schema.RegisterType(typeType);
            }

            return dynamicEntityRepository.ChangeToken;
        }
    }
}
