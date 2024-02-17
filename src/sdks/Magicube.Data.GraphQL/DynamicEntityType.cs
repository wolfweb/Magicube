using GraphQL.Types;
using Magicube.Data.Abstractions;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityType : ObjectGraphType<DynamicEntity> {
        public DynamicEntityType(DynamicEntity entity) {
            Name = entity.TableName;
            Field(x => x.Id).Description("entity id");
            foreach (var field in entity.Fields) {
                if (field.Key == Entity.IdKey || !GraphQLTypesMapping.GraphTypeMappings.ContainsKey(field.Value.Type) ) continue;

                AddField(new FieldType {
                    Type        = GraphQLTypesMapping.GraphTypeMappings[field.Value.Type],
                    Name        = field.Key,
                    Description = field.Value.Remark
                });
            }
        }
    }
}
