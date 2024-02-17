using Magicube.Core;
using Magicube.Core.Reflection;
using Magicube.Data.Abstractions.SqlBuilder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Magicube.Data.Abstractions.EfDbContext {
    public static class DbContextExtensions {
        public static DynamicEntity Get(this DefaultDbContext dbContext, SqlResult sql) {
            var table = dbContext.SqlQuery(sql);
            DynamicEntity result = null;
            foreach (DataRow row in table.Rows) {
                result = row;
                break;
            }
            return result;
        }

        public static IEnumerable<DynamicEntity> Gets(this DefaultDbContext dbContext, SqlResult sql) {
            var table = dbContext.SqlQuery(sql);
            foreach (DataRow row in table.Rows) {
                yield return row;
            }
        }

        public static DataTable SqlQuery(this DefaultDbContext dbContext, SqlResult sql) {
            DataTable result = null;
            var conn = GetConnection(dbContext);
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = sql.RawSql();
                using (DbDataReader reader = cmd.ExecuteReader()) {
                    result = new DataTable();
                    result.Load(reader);
                    return result;
                }
            }
        }

        public static DataTable GetSchema(this DefaultDbContext dbContext, string tbName) {
            var conn = GetConnection(dbContext);
            return conn.GetSchema(tbName);
        }

        public static void Execute(this DefaultDbContext dbContext, SqlResult sql) {
            var conn = GetConnection(dbContext);
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = sql.RawSql();
                cmd.ExecuteNonQuery();
            }
        }

        public static T Execute<T>(this DefaultDbContext dbContext, SqlResult sql) {
            var conn = GetConnection(dbContext);
            using (var cmd = conn.CreateCommand()) {
                cmd.CommandText = sql.RawSql();
                return cmd.ExecuteScalar().ConvertTo<T>();
            }
        }

        public static IEnumerable<T> SqlQuery<T>(this DefaultDbContext dbContext, SqlResult sql) where T : class, new() {
            DataTable dt = SqlQuery(dbContext, sql);
            return dt.ToEnumerable<T>();
        }

        public static IEnumerable<T> ToEnumerable<T>(this DataTable dt) where T : class, new() {
            var propertyInfos = TypeAccessor.Get<T>().Context.Properties.Select(x => x.Member);
            T[] ts = new T[dt.Rows.Count];
            int i = 0;
            foreach (DataRow row in dt.Rows) {
                T t = New<T>.Instance();
                foreach (var p in propertyInfos) {
                    if (dt.Columns.IndexOf(p.Name) != -1 && row[p.Name] != DBNull.Value)
                        t.SetValue(p, row[p.Name]);
                }
                ts[i] = t;
                i++;
            }
            return ts;
        }

        private static DbConnection GetConnection(DefaultDbContext dbContext) {
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                conn.Open();

            return conn;
        } 
    }
}