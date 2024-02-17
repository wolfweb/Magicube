using Magicube.Data.Abstractions.SqlBuilder.Clauses;
using System;

namespace Magicube.Data.Abstractions.SqlBuilder.Operators {
    public static class WhereOperatorContextExtension {
        public static WhereOperatorContext<TO> WhereEq<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("=", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereGt<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause(">", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereGte<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause(">=", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereLt<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("<", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereLte<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("<=", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereNotEq<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("<>", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereNotIn<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("not in", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> WhereIn<TO>(this WhereOperatorContext<TO> ctx, string column, object value) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                return BuildWhereClause("in", column, value, clause, ctx);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> And<TO>(this WhereOperatorContext<TO> ctx) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                if (clause == null || clause is ConditionFactorClause) throw new DataException("error filter clause");
                return new ConditionFactorClause("and", clause.Group + 1, clause.Pos + 1);
            });
            return ctx;
        }

        public static WhereOperatorContext<TO> Or<TO>(this WhereOperatorContext<TO> ctx) where TO : AbstractOperator {
            AppendConditionClause(ctx, clause => {
                if (clause == null || clause is ConditionFactorClause) throw new DataException("error filter clause");
                return new ConditionFactorClause("or", clause.Group + 1, clause.Pos + 1);
            });
            return ctx;
        }

        public static SqlResult Build<TO>(this WhereOperatorContext<TO> ctx) where TO : AbstractOperator {
            return ctx.Operator.Compile();
        }

        private static ConditionClause BuildWhereClause<TO>(string op, string column, object value, AbstractConditionClause clause, OperatorContext<TO> ctx) where TO : AbstractOperator  {
            if (clause is ConditionClause) {
                ctx.AppendClause(new ConditionFactorClause("and", clause.Group, clause.Pos + 1));
            }

            int group = 0, pos = 0;
            if (clause != null) {
                group = clause.Group; pos = clause.Pos + 1;
            }
            return new ConditionClause(column, value, op, group, pos);
        }

        private static void AppendConditionClause<TO>(WhereOperatorContext<TO> ctx, Func<AbstractConditionClause, AbstractConditionClause> buildClause) where TO : AbstractOperator {
            ctx.AppendClauseBy(x => typeof(AbstractConditionClause).IsAssignableFrom(x.GetType()), data => {
                var clause = data as AbstractConditionClause;
                return buildClause(clause);
            });
        }
    }
}
