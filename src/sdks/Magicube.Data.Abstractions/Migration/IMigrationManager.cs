using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Magicube.Data.Abstractions {
	public interface IMigrationManager {
        void BuildTable<T>() where T : Entity;
        void BuildTable(Type type);
        void BuildTable(IDbTable table);
        string GetTableName(Type type);
        bool TableExists(string tableName);
        void SchemaUp();
        void SchemaDown();
    }

    public class NullMigrationManager : IMigrationManager {
        public void BuildTable<T>() where T : Entity {

        }

        public void BuildTable(Type type) {

        }

        public void BuildTable(IDbTable table) {

        }

        public string GetTableName(Type type) {
            var attr = type.GetCustomAttribute<TableAttribute>();
            if (attr != null) return attr.Name;
            return type.Name;
        }

        public void SchemaDown() {

        }

        public void SchemaUp() {

        }

        public bool TableExists(string tableName) {
            return true;
        }
    }
}
