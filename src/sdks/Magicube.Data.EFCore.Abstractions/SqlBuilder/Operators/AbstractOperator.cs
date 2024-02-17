using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public abstract class AbstractOperator {
        protected readonly SqlCompiler _compiler;
        protected readonly IList<Clause> Clauses;
        public AbstractOperator(SqlCompiler compiler) {
            _compiler = compiler;
            Clauses = new List<Clause>();
        }
        public          string TableName { get; protected set; }

        public abstract string Action    { get; }

        public virtual SqlResult Compile() {
            var sql = new SqlResult();
            var results = BuildStructure(sql);
            sql.Sql = string.Join(" ", results);
            return sql;
        }

        public void AddOrUpdateClause(Clause clause) {
            var it = Clauses.FirstOrDefault(x => x.GetType() == clause.GetType());
            if (it == null) {
                Clauses.Add(clause);
            } else {
                it = clause;
            }
        }

        public void AddOrUpdateClause(Func<Clause, bool> filter, Func<Clause, Clause> update) {
            var clause = GetClause(filter);
            bool shouldAdd = clause == null;
            clause = update(clause);
            if (shouldAdd) AppendClause(clause);
        }

        public void AppendClause(Clause clause) {
            Clauses.Add(clause);
        }

        public Clause GetClause(Func<Clause, bool> func) {
            return Clauses.FirstOrDefault(func);
        }

        public Clause GetLastClause(Func<Clause, bool> func) {
            return Clauses.LastOrDefault(func);
        }

        protected abstract string[] BuildStructure(SqlResult sql);

        protected string BuildCondition(SqlResult sql) {
            var groups = Clauses.Where(x => typeof(AbstractConditionClause).IsAssignableFrom(x.GetType())).Cast<AbstractConditionClause>().GroupBy(x => x.Group);
            if (groups.Count() == 0) return null;

            var builder = new StringBuilder("where (");
            var groupKey = 0;
            foreach (var group in groups) {
                foreach (var item in group.OrderBy(x => x.Pos)) {
                    var factor = item as ConditionFactorClause;
                    if (factor != null) {
                        if (groupKey != factor.Group) {
                            groupKey = factor.Group;
                            builder.Append(")");
                            builder.Append($" {factor.Factor} (");
                            continue;
                        } else {
                            builder.Append($" {factor.Factor} ");
                            continue;
                        }
                    } 
                    var condition = item as ConditionClause;
                    if (condition != null) {
                        var p = $"{_compiler.Slim}{sql.ParamsBindings.Values.Count}";
                        builder.Append($"{_compiler.WrapKey(condition.Column)} {condition.Operator} {p}");
                        sql.ParamsBindings.Add(p, condition.Value);
                    }
                }                
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}
