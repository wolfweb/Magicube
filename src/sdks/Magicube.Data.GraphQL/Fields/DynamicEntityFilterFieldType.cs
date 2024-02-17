using GraphQL.Types;
using Newtonsoft.Json.Linq;
using Magicube.Data.Abstractions.SqlBuilder.Operators;
using System;
using Magicube.Core;

namespace Magicube.Data.GraphQL {
    public abstract class DynamicEntityFilterFieldType : FieldType {
        protected WhereOperatorContext<TO> BuildWhereExpression<TO>(JToken where, OperatorContext<TO> queryCtx) where TO : AbstractOperator{
            WhereOperatorContext<TO> whereOp = null;
            if (where is JArray array) {
                foreach (var child in array.Children()) {
                    if (child is JObject whereObject) {
                        whereOp = BuildWhereExpressionInternal(whereObject, queryCtx);
                    }
                }
            } else if (where is JObject whereObject) {
                whereOp = BuildWhereExpressionInternal(whereObject, queryCtx);
            }
            return whereOp;
        }

        protected WhereOperatorContext<TO> BuildWhereExpressionInternal<TO>(JObject where, OperatorContext<TO> queryCtx) where TO : AbstractOperator {
            if (where == null) {
                return null;
            } else {
                WhereOperatorContext<TO> whereOp = queryCtx.Empty();
                foreach (var it in where.Properties()) {
                    var values = it.Name.Split('_', 2);
                    var property = values[0];

                    if (values.Length == 1) {
                        if (string.Equals(values[0], "or", StringComparison.OrdinalIgnoreCase)) {
                            BuildWhereExpression(it.Value, whereOp.Or());
                        } else if (string.Equals(values[0], "and", StringComparison.OrdinalIgnoreCase)) {
                            BuildWhereExpression(it.Value, whereOp.And());
                        } else {
                            whereOp = whereOp.WhereEq(values[0], it.Value.ToObject<object>());
                        }
                    } else {
                        var value = it.Value.ToObject<object>();
                        switch (values[1]) {
                            case "not"            : whereOp = whereOp.WhereNotEq(property, value)                        ; break;
                            case "gt"             : whereOp = whereOp.WhereGt(property, value)                           ; break;
                            case "gte"            : whereOp = whereOp.WhereGte(property, value)                          ; break;
                            case "lt"             : whereOp = whereOp.WhereLt(property, value)                           ; break;
                            case "lte"            : whereOp = whereOp.WhereLte(property, value)                          ; break;
                            case "in"             : whereOp = whereOp.WhereIn(property, it.Value.ToObject<object[]>())   ; break;
                            case "not_in"         : whereOp = whereOp.WhereNotIn(property, it.Value.ToObject<object[]>()); break;
                            case "contains"       : throw new NotImplementedException()                                  ;
                            case "not_contains"   : throw new NotImplementedException()                                  ;
                            case "starts_with"    : throw new NotImplementedException()                                  ;
                            case "not_starts_with": throw new NotImplementedException()                                  ;
                            case "ends_with"      : throw new NotImplementedException()                                  ;
                            case "not_ends_with"  : throw new NotImplementedException()                                  ;
                            default               : whereOp = whereOp.WhereEq(property, value)                           ; break;
                        }
                    }
                }

                return whereOp;
            }
        }
    }
}
