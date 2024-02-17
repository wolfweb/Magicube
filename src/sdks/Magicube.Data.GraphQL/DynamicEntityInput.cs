using GraphQL.Types;
using Magicube.Data.Abstractions;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityInput : InputObjectGraphType {
        public DynamicEntityInput(DynamicEntity entity) {
            Name = $"{entity.TableName}Input";
            foreach (var field in entity.Fields) {
                if (!GraphQLTypesMapping.GraphTypeMappings.ContainsKey(field.Value.Type)) continue;
                AddField(new FieldType { 
                    Type        = GraphQLTypesMapping.GraphTypeMappings[field.Value.Type],
                    Name        = field.Key,
                    Description = field.Value.Remark
                });
            }
        }
    }
}
