using GraphQL.Types;
using Magicube.Data.Abstractions;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityOrder : InputObjectGraphType {
        public DynamicEntityOrder(DynamicEntity entity) {
            Field<IdGraphType>(Entity.IdKey);
            foreach (var field in entity.Fields) {
                if (field.Value.IsSort) {
                    Field<OrderByGraphType>(field.Key);
                }
            }
        }
    }

    public enum OrderBy {
        Ascending,
        Descending
    }
    public class OrderByGraphType : EnumerationGraphType {
        public OrderByGraphType() {
            Name = "OrderByDirection";
            Description = "the order by direction";
            Add("ASC", OrderBy.Ascending, "orders content items in ascending order");
            Add("DESC", OrderBy.Descending, "orders content items in descending order");
        }
    }
}
