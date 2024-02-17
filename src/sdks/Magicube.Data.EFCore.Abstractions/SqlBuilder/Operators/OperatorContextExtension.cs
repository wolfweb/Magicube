using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public static class OperatorContextExtension {
        public static OperatorContext<AlterTableOperator> AddColumn(this OperatorContext<AlterTableOperator> ctx, ColumnItem column) {
            var clause = new AlterClause(AlterType.Add, column);
            ctx.Operator.AppendClause(clause);
            return ctx;
        }

        public static OperatorContext<AlterTableOperator> DropColumn(this OperatorContext<AlterTableOperator> ctx, ColumnItem column) {
            var clause = new AlterClause(AlterType.Drop, column);
            ctx.Operator.AppendClause(clause);
            return ctx;
        }

        public static OperatorContext<CreateSchemaOperator> WithColumns(this OperatorContext<CreateSchemaOperator> ctx, IEnumerable<ColumnItem> columns) {
            var clause = new ColumnsClause(columns);
            ctx.Operator.AppendClause(clause);
            return ctx;
        }

        public static OperatorContext<InsertOperator> SetData(this OperatorContext<InsertOperator> ctx, object obj) {
            if (obj == null) throw new System.ArgumentNullException(nameof(obj));
            var data = BuildDictionaryFromObject(obj);
            return SetData(ctx,data);
        }

        public static OperatorContext<InsertOperator> SetData(this OperatorContext<InsertOperator> ctx, Dictionary<string,object> data) {
            if (data == null) throw new System.ArgumentNullException(nameof(data));
            ctx.Operator.AddOrUpdateClause(new InsertClause(data.Keys.ToArray(), data.Values.ToArray()));
            return ctx;
        }

        public static WhereOperatorContext<TO> Empty<TO>(this OperatorContext<TO> ctx) where TO : AbstractOperator {
            return new WhereOperatorContext<TO>(ctx);
        }

        public static WhereOperatorContext<TO> Where<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("=", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereGt<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause(">", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereGte<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause(">=", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereLt<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("<", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereLte<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("<=", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereNotEq<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("<>", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereIn<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("in", column, value, ctx);
        }

        public static WhereOperatorContext<TO> WhereNotIn<TO>(this OperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            return BuildWhereClause("not in", column, value, ctx);
        }

        public static OperatorContext<InsertOperator> WithReturnId(this OperatorContext<InsertOperator> ctx, bool withReturnId = true) {
            ctx.Operator.ReturnId = withReturnId;
            return ctx;
        }

        private static WhereOperatorContext<TO> BuildWhereClause<TO>(string op, string column, object value, OperatorContext<TO> ctx) where TO : AbstractOperator {
            BuildConditionClause(column, value, op, ctx.Operator);
            return new WhereOperatorContext<TO>(ctx);
        }

        private static void BuildConditionClause(string column, object value, string oper, AbstractOperator @operator) {
            var data = @operator.GetLastClause(x => typeof(AbstractConditionClause).IsAssignableFrom(x.GetType()));
            int group = 0, pos = 0;
            if (data != null) {
                var clause = data as AbstractConditionClause;
                if(clause as ConditionFactorClause == null) @operator.AppendClause(new ConditionFactorClause("and", clause.Group, clause.Pos + 1));
                group = clause.Group;
                pos = clause.Pos + 1;
            }
            @operator.AppendClause(new ConditionClause(column, value, oper, group, pos));
        }

        private static Dictionary<string, object> BuildDictionaryFromObject(object data) {
            var dictionary = new Dictionary<string, object>();
            var props = data.GetType().GetRuntimeProperties();
            foreach (var property in props) {
                var value = property.GetValue(data);
                var name  = property.Name;
                dictionary.Add(name, value);
            }
            return dictionary;
        }
    }
}
