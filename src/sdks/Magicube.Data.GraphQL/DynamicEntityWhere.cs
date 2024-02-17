using GraphQL.Types;
using Magicube.Data.Abstractions;
using System;
using System.Collections.Generic;

namespace Magicube.Data.GraphQL {
    public class DynamicEntityWhere : WhereInputObjectGraphType {
        public DynamicEntityWhere(DynamicEntity entity) {
            Description = $"the {entity.TableName} content item filters";

            AddScalarFilterFields<IdGraphType>(Entity.IdKey, "the id");

            BuildWhereFields(entity);

            var whereInputType = new ListGraphType(this);
            
            var whereOrField = new FieldType {
                Type        = typeof(ListGraphType<DynamicEntityWhere>),
                Name        = "Or",
                Description = "OR logical operation"
            };
            whereOrField.ResolvedType = whereInputType;

            AddField(whereOrField);

            var whereAndField = new FieldType {
                Type        = typeof(ListGraphType<DynamicEntityWhere>),
                Name        = "And",
                Description = "AND logical operation"
            };
            whereAndField.ResolvedType = whereInputType;

            AddField(whereAndField);
        }

        private void BuildWhereFields(DynamicEntity entity) {
            foreach (var field in entity.Fields) {
                if (field.Value.IsFilter && field.Key != Entity.IdKey) {
                    if (!GraphQLTypesMapping.GraphTypeMappings.ContainsKey(field.Value.Type)) throw new DataException("unknow field type for graphql");
                    AddScalarFilterFields(GraphQLTypesMapping.GraphTypeMappings[field.Value.Type], field.Key, field.Value.Remark);
                }
            }
        }
    }

    public class WhereInputObjectGraphType : WhereInputObjectGraphType<object> {
    }

    public class WhereInputObjectGraphType<TSourceType> : InputObjectGraphType<TSourceType> {
        public static Dictionary<string, string> EqualityOperators = new Dictionary<string, string>
        {
            { ""    , "is equal to" },
            { "_not", "is not equal to" }
        };

        public static Dictionary<string, string> MultiValueComparisonOperators = new Dictionary<string, string>
        {
            { "_in"    , "is in collection" },
            { "_not_in", "is not in collection" }
        };

        public static Dictionary<string, string> NonStringValueComparisonOperators = new Dictionary<string, string>
        {
            { "_gt" , "is greater than" },
            { "_gte", "is greater than or equal" },
            { "_lt" , "is less than" },
            { "_lte", "is less than or equal" }
        };

        public static Dictionary<string, string> StringComparisonOperators = new Dictionary<string, string>
        {
            {"_contains"       , "contains the string"},
            {"_not_contains"   , "does not contain the string"},
            {"_starts_with"    , "starts with the string"},
            {"_not_starts_with", "does not start with the string"},
            {"_ends_with"      , "ends with the string"},
            {"_not_ends_with"  , "does not end with the string"}
        };

        public void AddScalarFilterFields<TGraphType>(string fieldName, string description) {
            AddScalarFilterFields(typeof(TGraphType), fieldName, description);
        }

        public void AddScalarFilterFields(Type graphType, string fieldName, string description) {
            AddEqualityFilters(graphType, fieldName, description);
            AddMultiValueFilters(graphType, fieldName, description);

            if (graphType == typeof(StringGraphType)) {
                AddStringFilters(graphType, fieldName, description);
            } else if (graphType == typeof(DateTimeGraphType) || graphType == typeof(IdGraphType)) {
                AddNonStringFilters(graphType, fieldName, description);
            }
        }

        private void AddEqualityFilters(Type graphType, string fieldName, string description) {
            AddFilterFields(graphType, EqualityOperators, fieldName, description);
        }

        private void AddStringFilters(Type graphType, string fieldName, string description) {
            AddFilterFields(graphType, StringComparisonOperators, fieldName, description);
        }

        private void AddNonStringFilters(Type graphType, string fieldName, string description) {
            AddFilterFields(graphType, NonStringValueComparisonOperators, fieldName, description);
        }

        private void AddMultiValueFilters(Type graphType, string fieldName, string description) {
            var wrappedType = typeof(ListGraphType<>).MakeGenericType(graphType);
            AddFilterFields(wrappedType, MultiValueComparisonOperators, fieldName, description);
        }

        private void AddFilterFields(Type graphType, IDictionary<string, string> filters, string fieldName, string description) {
            foreach (var filter in filters) {
                AddField( new FieldType { 
                    Type        = graphType, 
                    Name        = fieldName + filter.Key, 
                    Description = $"{description} {filter.Value}" 
                });
            }
        }
    }
}
