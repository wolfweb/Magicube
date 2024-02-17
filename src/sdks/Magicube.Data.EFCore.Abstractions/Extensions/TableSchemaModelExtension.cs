using Magicube.Data.Abstractions.SqlBuilder.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Data.Abstractions {
#if !DEBUG
	using System.Diagnostics;
	[DebuggerStepThrough]
#endif
    public static class TableSchemaModelExtension {
        const string TemplateClassHeader = "public class {0} : IEntity";
        const string TemplateClassProperty = "{2}public {1} {0} {{ get; set; }}";
        public static string ExportToDataModel(this IEnumerable<TableSchemaModel> schemas, Func<string,Type> action) {
            var builder = new List<string>() {
                 string.Format(TemplateClassHeader,schemas.First().TableName),
                 "{"
            };
            foreach (var schema in schemas) {
                builder.Add(string.Format(TemplateClassProperty, schema.ColumnName, GenerateType(action(schema.DataType)), GeneratePropertyAttribute(schema)));
            }

            builder.Add("  [NotMapped]\n  public JObject Parts { get; set; }");
            builder.Add("}");

            return string.Join("\n", builder);
        }

        private static string GeneratePropertyAttribute(TableSchemaModel model) {
            var attributes = new List<string>();
            if (model.AutoIncrement || model.IsPrimaryKey) {
                attributes.Add("  [KeyAttribute]\n");
            } else if (model.UniqueKey) {
                attributes.Add("  [IndexedAttribute]\n");
            }

            if (model.Length > 0) {
                attributes.Add($"  [ColumnExtendAttribute(Size = {model.Length}, NullAble = {model.Nullable.ToString().ToLower()})]\n");
            }

            attributes.Add("  ");           

            return string.Join("", attributes);
        }

        private static string GenerateType(Type type) {
            var arr = type.FullName.Split('.');
            return arr[arr.Length - 1];
        }
    }
}
