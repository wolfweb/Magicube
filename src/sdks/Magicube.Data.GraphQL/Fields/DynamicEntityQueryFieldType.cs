using GraphQL.Types;
using System.Threading.Tasks;
using System.Collections.Generic;
using GraphQL;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Magicube.Core;
using System;
using Magicube.Data.Abstractions.EfDbContext;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityQueryFieldType : DynamicEntityFilterFieldType {
        const string LastIdKey = "lastId";
        const string WhereKey  = "where";
        const string OrderKey  = "orderBy";
        const string FirstKey  = "first";
        const string SkipKey   = "skip";
        public DynamicEntityQueryFieldType(DynamicEntity entity, ISchema schema) {
            Type = typeof(ListGraphType<DynamicEntityType>);

            var whereInput = new DynamicEntityWhere(entity);
            var orderByInput = new DynamicEntityOrder(entity);

            Arguments = new QueryArguments(
                new QueryArgument<DynamicEntityWhere> { Name = WhereKey, Description = "filters the dynamic entity items", ResolvedType = whereInput },
                new QueryArgument<DynamicEntityOrder> { Name = OrderKey, Description = "sort order", ResolvedType = orderByInput },
                new QueryArgument<IntGraphType>       { Name = FirstKey, Description = "the first n dynamic entity items", ResolvedType = new IntGraphType() },
                new QueryArgument<IntGraphType>       { Name = SkipKey , Description = "the number of dynamic entity items to skip", ResolvedType = new IntGraphType() }
            );

            Resolver = new LockedAsyncFieldResolver<IEnumerable<DynamicEntity>>(Resolve);

            schema.RegisterType(whereInput);
            schema.RegisterType(orderByInput);
        }

        private async Task<IEnumerable<DynamicEntity>> Resolve(IResolveFieldContext context) {
            var graphContext = (GraphQLContext)context.UserContext;
            var rep = graphContext.ServiceProvider.GetService<IDynamicEntityRepository>();

            JObject where = null;
            JObject order = null;
            int first = 0, skip = 0, lastId = 0;
            if (context.HasArgument(WhereKey)) {
                where = JObject.FromObject(context.Arguments[WhereKey].Value);
            }

            if (context.HasArgument(OrderKey)) {
                order = JObject.FromObject(context.Arguments[OrderKey].Value);
            }

            if (context.HasArgument(FirstKey)) {
                first = context.Arguments[FirstKey].Value.ConvertTo<int>();
                first = Math.Min(first, 100);
            }

            if (context.HasArgument(LastIdKey)) {
                lastId = context.Arguments[LastIdKey].Value.ConvertTo<int>();
            }

            if (context.HasArgument(SkipKey)) {
                skip = context.Arguments[SkipKey].Value.ConvertTo<int>();
            }

            var tbName = context.FieldDefinition.Name;
            var fields = context.SubFields.Keys.ToArray();

            return await rep.GetsAsync(tbName, fields, queryCtx => {
                BuildWhereExpression(where, queryCtx);
                BuildOrderExpression(order, queryCtx);
                return queryCtx;
            }, skip, lastId, first);
        }

        private void BuildWhereExpression(JToken where, SqlKata.Query queryCtx) {
            if (where is JArray array) {
                foreach (var child in array.Children()) {
                    if (child is JObject whereObject) {
                        BuildWhereExpressionInternal(whereObject, queryCtx);
                    }
                }
            } else if (where is JObject whereObject) {
                BuildWhereExpressionInternal(whereObject, queryCtx);
            }
        }

        private void BuildWhereExpressionInternal(JObject where, SqlKata.Query query) {
            if (where != null) {
                foreach (var it in where.Properties()) {
                    var values = it.Name.Split('_', 2);
                    var property = values[0];

                    if (values.Length == 1) {
                        if (string.Equals(values[0], "or", StringComparison.OrdinalIgnoreCase)) {
                            query.Or();
                            query.Where(q => {
                                BuildWhereExpression(it.Value, q);
                                return q;
                            });
                        } else if (string.Equals(values[0], "and", StringComparison.OrdinalIgnoreCase)) {
                            query.Where(q => {
                                BuildWhereExpression(it.Value, q);
                                return q;
                            });                            
                        } else {
                            query.Where(values[0], it.Value.ToObject<object>());
                        }
                    } else {
                        var value = it.Value.ToObject<object>();
                        switch (values[1]) {
                            case "not"            : query.WhereNot(property, value); break;
                            case "gt"             : query.WhereGt(property, value); break;
                            case "gte"            : query.WhereGte(property, value); break;
                            case "lt"             : query.WhereLt(property, value); break;
                            case "lte"            : query.WhereLte(property, value); break;
                            case "in"             : query.WhereIn(property, it.Value.ToObject<object[]>()); break;
                            case "not_in"         : query.WhereNotIn(property, it.Value.ToObject<object[]>()); break;
                            case "contains"       : query.WhereContains(property, it.Value); break;
                            case "not_contains"   : query.WhereNotContains(property, it.Value); break;
                            case "starts_with"    : query.WhereStarts(property,it.Value); break;
                            case "not_starts_with": query.WhereNotStarts(property,it.Value); break;
                            case "ends_with"      : query.WhereEnds(property,it.Value); break;
                            case "not_ends_with"  : query.WhereNotEnds(property, it.Value);break;
                            default: query.Where(property, value); break;
                        }
                    }
                }
            }
        }

        private void BuildOrderExpression(JObject order, SqlKata.Query queryCtx) {
            if (order != null) {
                foreach (var it in order.Properties()) {
                    if (it.Value.ToString().ToLower() == "asc") {
                        queryCtx.OrderBy(it.Name.ToString());
                    } else {
                        queryCtx.OrderByDesc(it.Name.ToString());
                    }
                }
            } 
        }
    }
}
